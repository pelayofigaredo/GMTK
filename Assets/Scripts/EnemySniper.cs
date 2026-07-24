using UnityEngine;
using UnityEngine.AI;

public class EnemySniper : Enemy
{
    public static  GameObject prefab;
    [Header("Sniper")]
    [Tooltip("Radio del offset aleatorio alrededor del jugador.")]
    [SerializeField] float minOffset = 2f;
    [SerializeField] float maxOffset = 6f;
    [Tooltip("Cada cuánto recalcula el punto objetivo (además de al llegar a él).")]
    [SerializeField] float recalcInterval = 0.4f;
    [Tooltip("A qué distancia del punto objetivo se considera que ha llegado.")]
    [SerializeField] float arrivalThreshold = 0.5f;
    [Tooltip("Distancia al jugador a partir de la cual dispara.")]
    [SerializeField] float shootDistance = 5f;

    [Header("Charge / Cooldown")]
    [Tooltip("Duración máxima de la embestida antes de agotarse.")]
    [SerializeField] float chargeDuration = 1.5f;
    [Tooltip("Distancia al jugador que da la carga por 'acertada' y la termina antes.")]
    [SerializeField] float chargeHitDistance = 1f;
    [Tooltip("Tiempo que se queda quieto tras cargar.")]
    [SerializeField] float cooldownDuration = 1.5f;

    enum State { Pursuing, Charging, Cooldown }
    State state = State.Pursuing;

    Transform player;
    Vector3 targetPosition;
    float recalcTimer;
    float chargeTimer;
    float cooldownTimer;
    float baseSpeed;


    void Start()
    {
        prefab = Resources.Load<GameObject>("DarkMissile");
        player = GameHandler.Instance.Hero.transform;
        baseSpeed = agent.speed;
        PickNewTarget();
    }

    void Update()
    {
        if (player == null || frozen) return;

        float distToPlayer = Vector3.Distance(transform.position, player.position);

        switch (state)
        {
            case State.Pursuing:
                if (distToPlayer <= shootDistance) { EnterCharge(); break; }

                recalcTimer -= Time.deltaTime;
                float distToTarget = Vector3.Distance(transform.position, targetPosition);
                if (recalcTimer <= 0f || distToTarget <= arrivalThreshold)
                    PickNewTarget();
                break;

            case State.Charging:
                SetDestination(player.position);        // prepara el disparo
                chargeTimer -= Time.deltaTime;
                if (chargeTimer <= 0f)
                {
                    Shoot();
                    EnterCooldown();
                }
                break;

            case State.Cooldown:
                cooldownTimer -= Time.deltaTime;         // quieto, sin recalcular nada
                if (cooldownTimer <= 0f)
                    EnterPursuit();
                break;
        }
    }

    private void Shoot()
    {
        GameObject missile = GameObject.Instantiate(prefab, transform.position, Quaternion.identity);
        missile.transform.forward = transform.forward;
    }

    void PickNewTarget()
    {
        // punto aleatorio en un anillo (minOffset..maxOffset) alrededor del jugador, en XZ
        Vector2 r = Random.insideUnitCircle.normalized * Random.Range(minOffset, maxOffset);
        Vector3 candidate = player.position + new Vector3(r.x, 0f, r.y);

        if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, maxOffset, NavMesh.AllAreas))
            candidate = hit.position;

        targetPosition = candidate;
        SetDestination(targetPosition);
        recalcTimer = recalcInterval;
    }

    void EnterCharge()
    {
        state = State.Charging;
        chargeTimer = chargeDuration;
        agent.speed = 0;
    }

    void EnterCooldown()
    {
        state = State.Cooldown;
        cooldownTimer = cooldownDuration;
        if (agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
    }


    void EnterPursuit()
    {
        state = State.Pursuing;
        agent.speed = baseSpeed;
        PickNewTarget();      
    }

    #region Editor 
#if UNITY_EDITOR

    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying || player == null) return;

        // línea y distancia real al jugador
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, player.position);

        // radio de carga (verde persiguiendo, rojo cargando, gris en cooldown)
        Gizmos.color = state == State.Charging ? Color.red
                     : state == State.Cooldown ? Color.gray
                     : Color.green;
        Gizmos.DrawWireSphere(transform.position, shootDistance);

        // anillo de offset alrededor del jugador (min y max)
        Gizmos.color = new Color(0.3f, 0.6f, 1f);
        Gizmos.DrawWireSphere(player.position, minOffset);
        Gizmos.DrawWireSphere(player.position, maxOffset);
    }
#endif

    #endregion
}
