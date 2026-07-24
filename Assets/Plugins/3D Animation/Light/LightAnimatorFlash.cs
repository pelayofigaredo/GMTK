using UnityEngine;
using System.Collections;

namespace IO_Scripts.Animation
{
    public class LightAnimatorFlash : MonoBehaviour
    {
        [SerializeField] Light targetLight;
        [SerializeField] float currentValue = 0;
        [SerializeField] float dimmingSpeed = 0.2f;
        [SerializeField][Range(0, 1)] float flashIncrease = 0.8f;
        [SerializeField] float maxIntensity = 200;
        [SerializeField] float maxIntensityVariance = 20;
        [SerializeField] AnimationCurve animationCurve;

        float currentMaxIntensity;
        bool isActive = false;

        IEnumerator FlashCR()
        {
            while (isActive)
            {
                currentValue -= dimmingSpeed * Time.deltaTime;
                if (currentValue <= 0)
                {
                    currentValue = 0;
                    isActive = false;
                }
                targetLight.intensity = Mathf.Lerp(0, currentMaxIntensity, animationCurve.Evaluate(currentValue));
                yield return new WaitForEndOfFrame();
            }
        }

        [ContextMenu("Flash")]
        public void Flash()
        {
            isActive = true;
            currentMaxIntensity = maxIntensity + UnityEngine.Random.Range(0, maxIntensityVariance);
            currentValue = Mathf.Clamp01(currentValue + flashIncrease);
            StartCoroutine(FlashCR());
        }
    }
}