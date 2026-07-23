using UnityEngine;

public class Missile : MonoBehaviour
{
    public float speed = 10f;
    public static GameObject prefabExplosion;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        prefabExplosion = Resources.Load<GameObject>("ExplosionLejana");
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(transform.forward.normalized * speed * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
        Instantiate(prefabExplosion, transform.position, transform.rotation);
    }


}
