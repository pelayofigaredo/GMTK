using UnityEngine;

public class RuneMine : MonoBehaviour
{
    public static GameObject prefabExplosion;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        prefabExplosion = Resources.Load<GameObject>("ExplosionMina");
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnCollisionEnter(Collision collision)
    {
        Instantiate(prefabExplosion, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
