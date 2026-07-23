using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public class Enemy : MonoBehaviour
{
    [Header("Configuration")]
    public int life = 30;

    Vector3 destination;

    [Header("Components")]
    [SerializeField] protected NavMeshAgent agent;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<Hero>() != null)
        {
            Destroy(collision.collider.gameObject);
            GameObject.FindAnyObjectByType<GameHandler>().Lose();
        }
    }

    public void ReceiveDamage(int damage)
    {
        life -= damage;
        if (life < 1)
        {
            GameHandler.Instance.EnemyDeath(this);
            Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    protected void SetDestination(Vector3 destination)
    {
        this.destination = destination;
        agent.destination = destination;
    }
}
