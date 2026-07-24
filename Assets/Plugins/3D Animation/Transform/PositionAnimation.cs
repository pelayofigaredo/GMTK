using System.Collections;
using UnityEngine;

namespace IO_Scripts.Animation
{
    public class PositionAnimation : MonoBehaviour
{
        [SerializeField] SpaceMode mode = SpaceMode.World;
        [SerializeField] bool isAnimating = false;
        [SerializeField] Vector3 initialPosition;
        [SerializeField] Vector3 finalPosition;
        [SerializeField] float animationTime = 2;
        [SerializeField] bool overrideScenePosition = false;
        [SerializeField] bool useAnimationCurve = false;
        [SerializeField] bool isYoyo = false;
        [SerializeField] AnimationCurve animationCurve;
        [SerializeField]
        [Range(0, 1)] float t = 0;
        float step;
        bool goingToDestination = false;
        Coroutine animationCoroutine;

        private void Awake()
        {
            step = 1 / animationTime;
            if(!overrideScenePosition)
            {
                switch (mode)
                {
                    case SpaceMode.World:
                        initialPosition = transform.position;
                        break;
                    case SpaceMode.Local:
                        initialPosition = transform.localPosition;
                        break;
                }
            }
        }

        IEnumerator AnimationCR()
        {
            while (isAnimating)
            {
                if (goingToDestination)
                {
                    t += step * Time.deltaTime;
                    if (t >= 1)
                    {
                        t = 1;
                        if (isYoyo)
                            goingToDestination = false; // If yo-yo, switch direction
                        else
                            isAnimating = false; // If not yo-yo, stop animating
                    }
                }
                else
                {
                    t -= step * Time.deltaTime;
                    if (t <= 0)
                    {
                        t = 0;
                        if (isYoyo)
                            goingToDestination = true; // If yo-yo, switch direction
                        else
                            goingToDestination = false; // If not yo-yo, stop animating
                    }
                }
                UpdatePosition();
                yield return new WaitForEndOfFrame();
            }
            animationCoroutine = null; 
        }

        void LaunchAnimationCR()
        {
           if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
            }
            isAnimating = true;
            animationCoroutine = StartCoroutine(AnimationCR());
        }

        [ContextMenu("Go")]
        public void Go()
        {
            step = 1 / animationTime;
            goingToDestination = true;
            LaunchAnimationCR();
        }

        [ContextMenu("Go Yoyo")]
        public void GoYoyo()
        {
            step = 1 / animationTime;
            goingToDestination = true;
            isYoyo = true;
            LaunchAnimationCR();
        }

        public void Go(Vector3 destination)
        {
            this.finalPosition = destination;
            t = 0;
            Go();
        }

        [ContextMenu("Return")]
        public void Return()
        {
            step = 1 / animationTime;
            goingToDestination = false;
            LaunchAnimationCR();
        }

        [ContextMenu("Return Yoyo")]
        public void ReturnYoyo()
        {
            step = 1 / animationTime;
            goingToDestination = false;
            isYoyo = true;
            LaunchAnimationCR();
        }

        public void SetTime(float time)
        {
            this.animationTime = time;
            step = 1 / time;
        }

        public void SetT(float t)
        {
            this.t = Mathf.Clamp01(t);
            UpdatePosition();
        }

        void UpdatePosition()
        {
            float evaluatedT = useAnimationCurve ? animationCurve.Evaluate(t) : t;
            switch(mode)
            {
                case SpaceMode.World:
                    transform.position = Vector3.Lerp(initialPosition, finalPosition, evaluatedT);
                    break;
                case SpaceMode.Local:
                    transform.localPosition = Vector3.Lerp(initialPosition, finalPosition, evaluatedT);
                    break;
            }
        }


#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Vector3 origin = Vector3.zero;
            Vector3 destination = Vector3.zero;

            switch (mode)
            {
                case SpaceMode.World:
                    origin = initialPosition;
                    destination = finalPosition;
                    break;

                case SpaceMode.Local:
                    origin = (transform.parent != null) ? transform.parent.TransformPoint(initialPosition) : initialPosition;
                    destination = (transform.parent != null) ? transform.parent.TransformPoint(finalPosition) : finalPosition;
                    break;
            }

            UnityEngine.Gizmos.DrawLine(origin, destination);
        }

        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                UpdatePosition();
            }
        }

#endif

        [System.Serializable]
        enum SpaceMode
        {
            World,
            Local
        }
    }
}
