using System.Collections;
using UnityEngine;
namespace IO_Scripts.Animation
{
    /// <summary>
    /// Animates the rotation of a GameObject towards a target rotation over a specified duration.
    /// Supports both local and world rotation modes, and can use a custom animation curve for interpolation.
    /// 
    /// Usage:
    /// - Set the target rotation in <see cref="destinationEuler"/> (in Euler angles).
    /// - Call <see cref="Go"/> to animate towards the target, or <see cref="Return"/> to animate back.
    /// - Use <see cref="GoTo"/> to animate to an arbitrary rotation.
    /// - Use <see cref="GoYoyo"/>, <see cref="ReturnYoyo"/>, or <see cref="GoToYoYO"/> for yo-yo (ping-pong) style animation.
    /// - The animation can be customized with <see cref="animationTime"/> and <see cref="curve"/>.
    /// 
    /// Fields:
    /// - <see cref="mode"/>: Determines if rotation is applied in local or world space.
    /// - <see cref="destinationEuler"/>: The target rotation in Euler angles.
    /// - <see cref="animationTime"/>: Duration of the rotation animation.
    /// - <see cref="useCurve"/>: Whether to use the animation curve for interpolation.
    /// - <see cref="curve"/>: The animation curve used for interpolation.
    /// - <see cref="isYoyo"/>: Enables yo-yo (ping-pong) animation mode.
    /// 
    /// Methods:
    /// - <see cref="Go"/>: Starts animating towards the target rotation.
    /// - <see cref="GoYoyo"/>: Starts animating towards the target rotation in yo-yo mode (ping-pong).
    /// - <see cref="Return"/>: Animates back to the original rotation.
    /// - <see cref="ReturnYoyo"/>: Animates back to the original rotation in yo-yo mode (ping-pong).
    /// - <see cref="GoTo"/>: Animates to a specified rotation (Euler angles).
    /// - <see cref="GoToYoYO"/>: Animates to a specified rotation (Quaternion) in yo-yo mode.
    /// - <see cref="SetTime"/>: Sets the animation duration.
    /// - <see cref="SetT"/>: Sets the current interpolation value and updates the rotation.
    /// </summary>
    public class RotationToTargetAnimator : MonoBehaviour
    {
        [SerializeField] Mode mode = Mode.local;
        [SerializeField] Vector3 destinationEuler;
        [SerializeField] float animationTime = 1;
        [SerializeField] bool useCurve;
        [SerializeField] AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);

        [SerializeField] bool isAnimating = false;
        [SerializeField] bool goingToDestination = false;
        [SerializeField] bool  isYoyo = false; 
        
        float step;
        float t = 0;

        Quaternion origin;
        Quaternion destination;

        Coroutine animationCoroutine;

        private void Awake()
        {
            origin = (mode == Mode.local) ? transform.localRotation : transform.rotation;
            step = 1 / animationTime;
        }

        void LaunchCoroutine()
        {
            if(animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
            }
            isAnimating = true;
            animationCoroutine = StartCoroutine(AnimationCR());
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
                UpdateRotation();
                yield return new WaitForEndOfFrame();
            }
            animationCoroutine = null;
        }

#if UNITY_EDITOR
        [ContextMenu("Go")]
        void GoEditor() {Go(false);}

        [ContextMenu("Go Yoyo")]
        void GoYoyoEditor(){GoYoyo(false);}

        [ContextMenu("Return")]
        void ReturnEditor(){Return(false);}

        [ContextMenu("Return Yoyo")]
        void ReturnYoyoEditor(){ReturnYoyo(false);}
#endif


        [ContextMenu("Go")]
        public void Go(bool resetT = false)
        {
            origin = transform.rotation;
            destination = Quaternion.Euler(destinationEuler);
            goingToDestination = true;
            if (resetT)
                t = 0;

            LaunchCoroutine();
        }

        [ContextMenu("Go Yoyo")]
        public void GoYoyo(bool resetT = false)
        {
            origin = transform.rotation;
            destination = Quaternion.Euler(destinationEuler);
            goingToDestination = true;
            isYoyo = true;
            if (resetT)
                t = 0;

            LaunchCoroutine();
        }

        [ContextMenu("Return")]
        public void Return(bool resetT = false)
        {
            origin = transform.rotation;
            destination = Quaternion.Euler(destinationEuler);
            goingToDestination = false;
            if (resetT)
                t = 0;

            LaunchCoroutine();
        }

        [ContextMenu("Return Yoyo")]
        public void ReturnYoyo(bool resetT = false)
        {
            origin = transform.rotation;
            destination = Quaternion.Euler(destinationEuler);
            goingToDestination = false;
            isYoyo = true;
            if (resetT)
                t = 0;

            LaunchCoroutine();
        }

        public void GoTo(Vector3 target)
        {
            origin = transform.rotation;
            destination = Quaternion.Euler(target);
            goingToDestination = true;
            t = 0;
            LaunchCoroutine();
        }

        public void GoToYoYO(Quaternion target)
        {
            origin = transform.rotation;
            destination = target;
            goingToDestination = true;
            isYoyo = true;
            t = 0;
            LaunchCoroutine();
        }

        public void SetTime(float time)
        {
            this.animationTime = time;
            step = 1 / time;
        }

        public void SetT(float t)
        {
            this.t = Mathf.Clamp01(t);
            UpdateRotation();
        }

        private void UpdateRotation()
        {
            Quaternion newRotation = Quaternion.Lerp(origin, destination, (useCurve) ? curve.Evaluate(t) : t);
            if (mode == Mode.local)
                transform.localRotation = newRotation;
            else
                transform.rotation = newRotation;
        }

        [System.Serializable]
        enum Mode
        {
            local,
            world
        }
    }
}