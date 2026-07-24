using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.Rendering.DebugUI;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] Enemy[] enemyPrefabs;

    [Header("Configuration")]
    [SerializeField] float baseMinInterval = 1.5f;
    [SerializeField] float baseMaxInterval = 3f;
    [SerializeField] int baseSpawnCount = 3;
    [SerializeField] int baseMaxAlive = 10;

    [Header("Level Update")]
    [SerializeField] float intervalDecreaseByLevel = 0.2f;
    [SerializeField] int spawnCountIncreaseByLevel = 1;
    [SerializeField] int maxAliveIncreaseByLevel = 5;

    [Header("Spawn Points")]
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] float spawnRadius = 15f;
    [SerializeField] float navSampleRadius = 3f;

    float timer;
    int spawnedCount;
    int maxAlive;
    int spawnCount;
    bool active;

    float minInterval;
    float maxInterval;

    List<Enemy> aliveEnemies = new List<Enemy>();

    void Start()
    {
        minInterval = baseMinInterval;
        maxInterval = baseMaxInterval;
        maxAlive = baseMaxAlive;
        spawnCount = baseSpawnCount;
    }

    void Update()
    {
        if (!active)
            return;

        if (aliveEnemies.Count >= maxAlive) return;

        timer -= Time.deltaTime;
        if (timer > 0f) return;

        SpawnSeveral(spawnCount);
        timer = Random.Range(minInterval, maxInterval);
    }

    public void UpdateValues(int currentLevel)
    {
        minInterval = baseMinInterval - (intervalDecreaseByLevel * currentLevel);
        maxInterval = baseMaxInterval - (intervalDecreaseByLevel * currentLevel);
        maxAlive = baseMaxAlive + (maxAliveIncreaseByLevel * currentLevel);
        spawnCount = baseSpawnCount + (spawnCountIncreaseByLevel * currentLevel);
    }

    public void FreezeAllEnemies()
    {
        for (int i = aliveEnemies.Count - 1; i >= 0; i--)
        {
            aliveEnemies[i].Freeze();
        }
    }

    public void KillAllEnemies()
    {
        for (int i = aliveEnemies.Count - 1; i >= 0; i--)
        {
            aliveEnemies[i].Kill();
        }
    }

    public void EnemyDeath(Enemy enemy)
    {
        aliveEnemies.Remove(enemy);
    }

    void SpawnSeveral(int spawnCount)
    {
        for (int i = 0; i < spawnCount; i++)
            SpawnOne();
    }

    void SpawnOne()
    {
        if (!TryGetSpawnPosition(out Vector3 pos)) return;

        Enemy prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        Enemy enemy = Instantiate(prefab, pos, Quaternion.identity);
        aliveEnemies.Add(enemy);
        spawnedCount++;
    }

    bool TryGetSpawnPosition(out Vector3 result)
    {
        Vector3 candidate = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
        Vector2 r = Random.insideUnitCircle * spawnRadius;
        candidate += new Vector3(r.x, 0f, r.y);

        if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, navSampleRadius, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = candidate;
        return false;
    }

    public void StopSpawning() { active = false; }
    public void ResumeSpawning()
    {
        active = true;
    }

#if UNITY_EDITOR

    #region Editor

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            foreach (var p in spawnPoints)
                if (p != null) Gizmos.DrawWireSphere(p.position, spawnRadius);
        }
    }

    #endregion
#endif
}