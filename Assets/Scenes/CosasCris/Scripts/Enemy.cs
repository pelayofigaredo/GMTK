using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int life = 30;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
            Destroy(gameObject);
    }
}
