using UnityEngine;
using UnityEngine.AI;

public class EnemyPersecutor : Enemy
{
    [Header("Persecutor")]
    [Tooltip("Radio del offset aleatorio alrededor del jugador.")]
    [SerializeField] float minOffset = 2f;
    [SerializeField] float maxOffset = 6f;
    [Tooltip("Cada cuánto recalcula el punto objetivo (además de al llegar a él).")]
    [SerializeField] float recalcInterval = 0.4f;
    [Tooltip("A qué distancia del punto objetivo se considera que ha llegado.")]
    [SerializeField] float arrivalThreshold = 0.5f;
    [Tooltip("Distancia al jugador a partir de la cual carga directamente.")]
    [SerializeField] float chargeDistance = 5f;
    [Tooltip("Velocidad del agente durante la carga (0 = no tocar la velocidad).")]
    [SerializeField] float chargeSpeed = 0f;

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
                if (distToPlayer <= chargeDistance) { EnterCharge(); break; }

                recalcTimer -= Time.deltaTime;
                float distToTarget = Vector3.Distance(transform.position, targetPosition);
                if (recalcTimer <= 0f || distToTarget <= arrivalThreshold)
                    PickNewTarget();
                break;

            case State.Charging:
                SetDestination(player.position);        // embiste al jugador directo
                chargeTimer -= Time.deltaTime;
                if (chargeTimer <= 0f || distToPlayer <= chargeHitDistance)
                    EnterCooldown();
                break;

            case State.Cooldown:
                cooldownTimer -= Time.deltaTime;         // quieto, sin recalcular nada
                if (cooldownTimer <= 0f)
                    EnterPursuit();
                break;
        }
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
        agent.speed = chargeSpeed;
    }

    void EnterCooldown()
    {
        state = State.Cooldown;
        cooldownTimer = cooldownDuration;
        if (chargeSpeed > 0f) agent.speed = baseSpeed;
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
        Gizmos.DrawWireSphere(transform.position, chargeDistance);

        // anillo de offset alrededor del jugador (min y max)
        Gizmos.color = new Color(0.3f, 0.6f, 1f);
        Gizmos.DrawWireSphere(player.position, minOffset);
        Gizmos.DrawWireSphere(player.position, maxOffset);
    }
#endif

    #endregion
}
