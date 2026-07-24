using System.Collections;
using UnityEngine;

namespace IO_Scripts.Animation
{
    ///<summary>
    /// Permite animar la rotacion de un transform con un movimiento constante.
    ///</summary>
    public class RotationAnimatorContinuous : MonoBehaviour
    {
        [SerializeField] Transform target;
        [SerializeField] Vector3 axis;
        [SerializeField]
        [Tooltip("Si esta activado, el eje de rotacion se escoge aleatoriamente entre -Axis y Axis")]
        bool randomAxis;

        [SerializeField] Space space = Space.World;
        [SerializeField] float speed;
        [SerializeField] bool isOn = true;

        Quaternion originalRotation;
        Coroutine animationCoroutine;
        public float Speed { get => speed; set => speed = value; }
        public bool IsPlaying { get => isOn; }

        private void Awake()
        {
            if (target == null)
                target = transform;

            originalRotation = transform.rotation;
            if (randomAxis)
                axis = new Vector3(UnityEngine.Random.Range(-axis.x, axis.x), UnityEngine.Random.Range(-axis.y, axis.y), UnityEngine.Random.Range(-axis.z, axis.z));
        }

        private void Update()
        {
            if (isOn)
            {
                target.Rotate(axis * speed * Time.deltaTime, space);
            }
        }

        IEnumerator AnimationCR()
        {
            while (isOn)
            {
                target.Rotate(axis * speed * Time.deltaTime, space);
                yield return new WaitForEndOfFrame();
            }
        }

        void LaunchCoroutine()
        {
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
            }
            animationCoroutine = StartCoroutine(AnimationCR());
        }

        public void Play(bool inverted = false) 
        { 
            isOn = true;
            if (inverted)
            {
                speed = -1 * speed;  
            }
            LaunchCoroutine();
        }
        public void Stop() { isOn = false; }
        public void Toggle() { isOn = !isOn; }
        public void ResetRotation() { target.rotation = originalRotation; }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                if (target == null)
                    target = transform;
            }
        }
#endif
    }
}

