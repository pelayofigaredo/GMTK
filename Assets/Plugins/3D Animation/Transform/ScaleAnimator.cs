using System;
using System.Collections;
using UnityEngine;


namespace IO_Scripts.Animation
{
    ///<summary>
    /// Permite animar la escala de un transform, interpolando entre dos valores.
    /// La animación se puede configurar a traves de time (la duración) y animationCurve (la curva de la animación).
    /// La animacion se lanza con los metodos Grow() y Shrink().
    ///</summary>
    public class ScaleAnimator : MonoBehaviour
    {
        [SerializeField] Vector3 minScale;
        [SerializeField] Vector3 maxScale;
        [SerializeField] bool isAnimating = false;
        [SerializeField] bool isYoyo = false;

        [SerializeField] float animationTime;
        [SerializeField] bool useAnimationCurve = false;
        [SerializeField] AnimationCurve animationCurve;
        [SerializeField] bool startsBig = false;
        [SerializeField] [Range(0,1)]float t;
        float tIncrease;
        bool isIncreasing = true;
        Coroutine animationCoroutine;

        // Start is called before the first frame update
        void Awake()
        {
            tIncrease = 1 / animationTime;
        }

        private void Start()
        {
            if (startsBig)
            {
                transform.localScale = maxScale;
                t = 1;
                isIncreasing = false;
            }
            else
            {
                transform.localScale = minScale;
                t = 0;
                isIncreasing = true;
            }
        }

        IEnumerator AnimationCR()
        {
            isAnimating = true;
            while (isAnimating)
            {
                if (isIncreasing)
                {
                    t += tIncrease * Time.deltaTime;
                    if (t >= 1)
                    {
                        t = 1;
                        GrowCompleted();
                        if (isYoyo)
                        {
                            isIncreasing = false;
                        }
                        else
                        {
                            isAnimating = false;
                        }
                    }
                }
                else
                {
                    t -= tIncrease * Time.deltaTime;
                    if (t <= 0)
                    {
                        t = 0;
                        ShrinkCompleted();
                        if (isYoyo)
                        {
                            isIncreasing = true;
                        }
                        else
                        {
                            isAnimating = false;
                        }
                    }

                }
                UpdateScale();
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
            animationCoroutine = StartCoroutine(AnimationCR());
        }


        [ContextMenu("Grow")]
        public void Grow()
        {
            isIncreasing = true;
            LaunchCoroutine();
        }

        [ContextMenu("Grow Yoyo")]
        public void GrowYoyo()
        {
            isIncreasing = true;
            isYoyo = true;
            LaunchCoroutine();
        }

        [ContextMenu("Shrink")]
        public void Shrink()
        {
            isIncreasing = false;
            LaunchCoroutine();
        }

        [ContextMenu("Shrink Animation")]
        public void ShrinkYoyo()
        {
            isIncreasing = false;
            isYoyo = true;
            LaunchCoroutine();
        }

        [ContextMenu("Stop Animation")]
        public void StopAnimation()
        {
            isAnimating = false;
            isYoyo = false;
            UpdateScale();
        }


        public void SetT(float t)
        {
            this.t = Mathf.Clamp01(t);
            UpdateScale();
        }

        public void SetAnimationTime(float newTime)
        {
            animationTime = newTime;
            tIncrease = 1 / animationTime;
        }

        private void UpdateScale()
        {
            transform.localScale = Vector3.Lerp(minScale, maxScale, (useAnimationCurve) ? animationCurve.Evaluate(t) : t);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                UpdateScale();
            }
        }
#endif

        protected virtual void GrowCompleted() { }
        protected virtual void ShrinkCompleted() { }

        #region Editor

        [ContextMenu("Set current as MIN")]
        void SetCurrentAsMin()
        {
            minScale = transform.localScale;
            t = 0;
        }
        [ContextMenu("Set current as MAX")]
        void SetCurrentAsMax()
        {
            maxScale = transform.localScale;
            t= 1;
        }
        [ContextMenu("Set scale to MIN")]
        void SetScaleToMin()
        {
            t = 0;
            transform.localScale = minScale;
        }
        [ContextMenu("Set scale to MAX")]
        void SetScaleToMax()
        {
            t = 1;
            transform.localScale = maxScale;
        }
        #endregion
    }
}