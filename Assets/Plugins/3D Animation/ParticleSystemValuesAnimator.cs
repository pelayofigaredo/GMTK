using System.Collections;
using UnityEngine;
namespace IO_Scripts.Animation
{
    public class ParticleSystemValuesAnimator : MonoBehaviour
    {
        [SerializeField] ParticleSystem target;
        [SerializeField] ParticleSystemValues initialValues;
        [SerializeField] ParticleSystemValues finalValues;
        [SerializeField] bool updateConstant;
        [SerializeField]
        [Range(0, 1)] float t;

        bool isPlaying = false;
        float direction = 1.0f;
        Coroutine animationCoroutine;

#if UNITY_EDITOR
        void Update()
        {
            if (updateConstant)
            {
                UpdateValues();
            }
        }
#endif

        IEnumerator AnimationCR()
        {
            while (isPlaying)
            {
                t = Mathf.Clamp01(t + (direction * Time.deltaTime));
                UpdateValues();
                if (t == 0.0f || t == 1.0f)
                {
                    isPlaying = false;
                }
                yield return new WaitForEndOfFrame();
            }
            animationCoroutine = null;
        }

       void LaunchCoroutine() 
        {
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
            }
            isPlaying = true;
            animationCoroutine = StartCoroutine(AnimationCR());
        }

        public void Play()
        {
            direction = 1.0f;
            LaunchCoroutine();
        }

        public void PlayReverse()
        {
            direction = -1.0f;
            LaunchCoroutine();
        }

        [ContextMenu("Update values")]
        void UpdateValues()
        {
            ParticleSystem.MainModule main = target.main;
            main.startSpeed = new ParticleSystem.MinMaxCurve(Mathf.Lerp(initialValues.StartSpeed.constantMin, finalValues.StartSpeed.constantMin, t), Mathf.Lerp(initialValues.StartSpeed.constantMax, finalValues.StartSpeed.constantMax, t));

            ParticleSystem.EmissionModule emission = target.emission;
            emission.rateOverTime = Mathf.Lerp(initialValues.EmissionRate, finalValues.EmissionRate, t);
        }

#if UNITY_EDITOR

        [ContextMenu("Set current as iniitial")]
        private void SetCurrentAsInitial()
        {
            if (target != null)
            {
                initialValues.GetValues(target);
            }
        }

        [ContextMenu("Set current as final")]
        private void SetCurrentAsFinal()
        {
            if (target != null)
            {
                finalValues.GetValues(target);
            }
        }

#endif

        [System.Serializable]
        class ParticleSystemValues
        {
            public float EmissionRate;
            public ParticleSystem.MinMaxCurve StartSpeed;

            public void SetValues(ParticleSystem target)
            {
                ParticleSystem.MainModule main = target.main;
                main.startSpeed = StartSpeed;

                ParticleSystem.EmissionModule emission = target.emission;
                emission.rateOverTime = EmissionRate;
            }

            public void GetValues(ParticleSystem target)
            {
                ParticleSystem.MainModule main = target.main;
                StartSpeed = main.startSpeed;

                ParticleSystem.EmissionModule emission = target.emission;
                EmissionRate = emission.rateOverTime.constant;
            }
        }
    }
}