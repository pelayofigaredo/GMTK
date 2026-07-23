using UnityEngine;

public class Fading : MonoBehaviour
{
    public float timeElapsed = 0f;
    public float duration = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed > duration)
            Destroy(gameObject);
    }
}
