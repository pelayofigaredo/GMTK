using System;
using System.Collections;
using UnityEngine;

/***
 * Pelayo paso por aqui y creo un Singleton para poder pillar referencia al heroe desde cualquier parte
 */

public class GameHandler : MonoBehaviour
{
    #region Singleton
    static GameHandler instance;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
        {
            Debug.LogError("Chavales que ho? " + gameObject);
            Destroy(this);
        }
    }
    public static GameHandler Instance { get => instance; }
    #endregion

    [Header("Configuration")]
    [SerializeField] float roundDuration = 15;

    [Header("Components")]
    [SerializeField] UIHandler uiHandler;
    [SerializeField] EnemySpawner enemySpawner;
    [SerializeField] Hero hero;
    [SerializeField] EndExplosionAnimator endExplosionAnimator;

    public Hero Hero { get => hero; }

    int roundIndex = -1;
    bool isInRound = false;
    float roundTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.2f);
        StartRound();
    }

    // Update is called once per frame
    void Update()
    {
        if(isInRound)
        {
            roundTimer -= Time.deltaTime;
            if (roundTimer <= 0)
            {
                EndRound();
                RoundWon();
            }
            else
            {
                uiHandler.UpdateTimer(roundTimer);
            }
        }
    }

    //Aqui querremos hacer algo de guays 
    public void EnemyDeath(Enemy enemy) {enemySpawner.EnemyDeath(enemy); }

    public void AttackUpdate(IAttacker[] attacks)
    {
        uiHandler.UpdateAttacks(attacks);

    }

    internal void Lose()
    {
        Debug.Log("You lose");
        EndRound();
    }

    #region Round Managment

    void StartRound()
    {
        isInRound = true;
        roundTimer = roundDuration;
        enemySpawner.UpdateValues(roundIndex);
        enemySpawner.ResumeSpawning();
    }

    //La ronda acaba independientemente del resultado
    void EndRound()
    {
        isInRound = false;
        enemySpawner.StopSpawning();
    }

    //El jugador sobrevivio
    void RoundWon()
    {
        if (hero.RemainingPeople >= 2) //Si aún quedan más momias
        {
            StartCoroutine(AdvanceRoundCR());
        }
        else //Si esta era la última
            Victory();
    }

    IEnumerator AdvanceRoundCR()
    {
        enemySpawner.FreezeAllEnemies();
        hero.Freeze(true);

        bool hidden = false;
        bool completed = false;

        Action onHidden = () => hidden = true;
        Action onComplete = () => completed = true;

        endExplosionAnimator.OnHidden += onHidden;
        endExplosionAnimator.OnComplete += onComplete;
        endExplosionAnimator.Play();

        yield return new WaitUntil(() => hidden);
        endExplosionAnimator.OnHidden -= onHidden;

        hero.YouAlwaysLoseOne();
        uiHandler.UpdateAttacks(hero.GetAttacks());

        enemySpawner.KillAllEnemies();

        yield return new WaitUntil(() => completed);
        endExplosionAnimator.OnComplete -= onComplete;

        hero.Freeze(false);
        StartRound();
    }

    private void Victory()
    {
        Debug.Log("You win!");
    }
    #endregion



}
