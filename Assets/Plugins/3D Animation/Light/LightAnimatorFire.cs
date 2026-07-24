using UnityEngine;

namespace IO_Scripts.Animation
{
    public class LightAnimatorFire : MonoBehaviour
    {
        [SerializeField] Light target;
        [SerializeField] float minValue = 1;
        [SerializeField] float maxValue = 2;
        [SerializeField] bool affectRange = true;
        [SerializeField] float minRange = 1;
        [SerializeField] float maxRange = 10;
        [SerializeField] float noiseSeed = 1;
        [SerializeField] float noiseFrequency = 1;

        void Update()
        {
            float noiseValue = Mathf.PerlinNoise1D((Time.time * noiseFrequency) + noiseSeed);
            target.intensity = Mathf.Lerp(minValue, maxValue, noiseValue);
            if (affectRange)
            {
                target.range = Mathf.Lerp(minRange, maxRange, noiseValue);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (target == null)
            {
                target = GetComponent<Light>();
            }
        }
#endif
    }
}
