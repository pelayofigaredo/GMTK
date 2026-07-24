using UnityEngine;

namespace IO_Scripts.Animation
{
    public class RotationAnimatorSin : MonoBehaviour
    {
        [SerializeField] Vector3 axis;
        [SerializeField] float amplitude = 30;
        [SerializeField] float frequency = 1;
        [SerializeField] float phase = 0;


        Quaternion originalRotation;

        private void Start()
        {
            originalRotation = transform.localRotation;
        }


        void Update()
        {
            float angle = amplitude * Mathf.Sin(frequency * (Time.time + phase));
            transform.localRotation = originalRotation * Quaternion.Euler(axis * angle);
        }
    }
}
