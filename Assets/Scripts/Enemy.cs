using System;
using UnityEngine;
using UnityEngine.AI;


public class Enemy : MonoBehaviour
{
    [Header("Configuration")]
    public int life = 30;

    protected bool frozen = false;
    Vector3 destination;

    [Header("Components")]
    [SerializeField] protected NavMeshAgent agent;

    protected void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<Hero>() != null)
        {
            Debug.Log("No puedo caer más bajo");
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

    internal void Kill()
    {
        ReceiveDamage(life);
    }

    internal void Freeze()
    {
        frozen = true;
        agent.isStopped=true;
    }
}
