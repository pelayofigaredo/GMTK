using UnityEngine;

public class RuneMine : MonoBehaviour
{
    public static GameObject prefabExplosion;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        prefabExplosion = Resources.Load<GameObject>("ExplosionLejana");
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
        Instantiate(prefabExplosion, transform.position, transform.rotation);
    }
}
