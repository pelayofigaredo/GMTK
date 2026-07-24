using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace IO_Scripts.Animation
{
    /// <summary>
    /// Animates a Light's intensity and/or color over time, using linear interpolation or AnimationCurves.
    /// Supports one-shot and ping-pong animation, and can trigger events when the animation completes.
    /// </summary>
    public class LightAnimator : MonoBehaviour
    {
        // Reference to the Light component to animate
        [SerializeField] Light target;

        [SerializeField] float animationTime = 4; // Total duration of the animation in seconds
        [SerializeField] bool launchEventOnEnd = false; // If true, triggers an event when the animation ends

        //Intensity
        [SerializeField] bool useIntensity = true; // If true, animates the light's intensity
        [SerializeField] float minValue = 1; // Minimum intensity value
        [SerializeField] float maxValue = 2; // Maximum intensity value
        [SerializeField] bool intensityUsesAnimationCurve = true; // If true, uses the curve for intensity interpolation
        [SerializeField] AnimationCurve intensityAnimationCurve; // Animation curve for intensity

        //Color
        [SerializeField] bool useColor = false; // If true, animates the light's color
        [SerializeField] Color colorA = new Color(1, 1, 1, 1); // Start color
        [SerializeField] Color colorB = new Color(1, 0.5f, 0.5f, 1); // End color
        [SerializeField] bool colorUsesAnimationCurve = false; // If true, uses the curve for color interpolation
        [SerializeField] AnimationCurve colorAnimationCurve; // Animation curve for color

        public bool eventOnlyOnce = true; // If true, the event is triggered only once per animation
        public UnityEvent OnAnimationCompleted; // Event invoked when the animation completes

        bool canLaunchEvent = true; // Controls if the event can be triggered

        float curretnT; // Normalized progress of the animation (0 to 1)
        float step; // Animation progress speed per second

        bool isAnimating = false; // True if the animation is currently running
        bool isIncreasing = false; // True if animating forward, false if backward
        bool pingPong = false; // If true, animation reverses direction at each end

        Coroutine animationCoroutine; // Reference to the running coroutine


        void Awake()
        {
            if (useIntensity)
                curretnT = Mathf.InverseLerp(minValue, maxValue, target.intensity);
            SetT(curretnT);
        }

        IEnumerator AnimationCR()
        {
            while (isAnimating)
            {
                curretnT += (isIncreasing) ? (step * Time.deltaTime) : (-step * Time.deltaTime);
                curretnT = Mathf.Clamp01(curretnT);

                // Handle animation end or direction reversal
                if (curretnT == 0 || curretnT == 1)
                {
                    if (pingPong)
                        isIncreasing = !isIncreasing;
                    else
                        isAnimating = false;

                    // Trigger event if configured
                    if (launchEventOnEnd && canLaunchEvent)
                    {
                        OnAnimationCompleted?.Invoke();
                        if (eventOnlyOnce)
                            canLaunchEvent = false;
                    }
                }
                SetT(curretnT);
                yield return new WaitForEndOfFrame();
            }

            animationCoroutine = null; // Clear the coroutine reference when done
        }

        /// <summary>
        /// Applies the normalized progress value to the Light's intensity and/or color.
        /// </summary>
        /// <param name="t">Normalized animation progress (0 to 1)</param>
        void SetT(float t)
        {
            if (useIntensity)
            {
                float intensityEvaluated = intensityUsesAnimationCurve ? intensityAnimationCurve.Evaluate(t) : t;
                target.intensity = Mathf.Lerp(minValue, maxValue, intensityEvaluated);
            }
            if (useColor)
            {
                float colorEvaluated = colorUsesAnimationCurve ? colorAnimationCurve.Evaluate(t) : t;
                target.color = Color.Lerp(colorA, colorB, colorEvaluated);
            }
        }


        /// <summary>
        /// Starts the animation from the beginning to the end.
        /// </summary>
        [ContextMenu("Play")]
        public void Play()
        {
            step = 1 / animationTime;
            isAnimating = true;
            isIncreasing = true;
            LaunchAnimationCR();
        }

        /// <summary>
        /// Starts the animation in ping-pong mode (back and forth).
        /// </summary>
        [ContextMenu("PlayPingPong")]
        public void PlayPingPong()
        {
            step = 1 / animationTime;
            pingPong = true;
            isAnimating = true;
            isIncreasing = true;
            LaunchAnimationCR();
        }

        /// <summary>
        /// Starts the animation in reverse (from end to start).
        /// </summary>
        [ContextMenu("Reverse")]
        public void Reverse()
        {
            step = 1 / animationTime;
            isAnimating = true;
            isIncreasing = false;
            LaunchAnimationCR();
        }

        /// <summary>
        /// Starts the animation in reverse ping-pong mode.
        /// </summary>
        [ContextMenu("ReversePingPong")]
        public void ReversePingPong()
        {
            step = 1 / animationTime;
            pingPong = true;
            isAnimating = true;
            isIncreasing = false;
            LaunchAnimationCR();
        }

        /// <summary>
        /// Stops the animation and disables ping-pong mode.
        /// </summary>
        [ContextMenu("Stop")]
        public void Stop()
        {
            isAnimating = false;
            pingPong = false;
        }

        void LaunchAnimationCR()
        {
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
            }
            animationCoroutine = StartCoroutine(AnimationCR());
        }

#if UNITY_EDITOR
        /// <summary>
        /// Validates and corrects values in the editor.
        /// </summary>
        private void OnValidate()
        {
            if (target == null) target = GetComponent<Light>();

            // Ensure animation time is not negative
            if (animationTime < 0)
            {
                animationTime = 0;
            }
        }
#endif
    }
}
