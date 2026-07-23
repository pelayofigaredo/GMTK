using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public int damage = 10;
    public List<Enemy> enemiesDamaged = new List<Enemy>();
    public float duration = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        duration -= Time.deltaTime;
        if (duration < 0)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && !enemiesDamaged.Contains(enemy))
        {
            enemy.ReceiveDamage(damage);
            enemiesDamaged.Add(enemy);
        }
    }
}
