using System.Collections;
using UnityEngine;

namespace IO_Scripts.Animation
{
    public class RotationAnimationSingleAxis : MonoBehaviour
    {
        public enum Axis { X, Y, Z }
        [SerializeField] Axis axis = Axis.Y;
        [SerializeField] float time = 1;

        Coroutine rotationCoroutine;
        float initialAngle;
        float targetAngle;

        public void Rotate(float amount)
        {
            if (rotationCoroutine != null)
                StopCoroutine(rotationCoroutine);

            targetAngle = initialAngle + amount;
            initialAngle = GetCurrentAngle();
            rotationCoroutine = StartCoroutine(RotateCR());
        }

        [ContextMenu("Rotate 90")]
        public void Rotate90()
        {
            Rotate(90f);
        }

        [ContextMenu("Rotate -90")]
        public void RotateM90()
        {
            Rotate(-90f);
        }



        private IEnumerator RotateCR()
        {
            float t = 0f;
            float step = 1f / time;
            while (t < 1f)
            {
                t += Time.deltaTime * step;
                float angle = Mathf.Lerp(initialAngle, targetAngle, t);
                SetAngle(angle);
                yield return null;
            }
            SetAngle(targetAngle); // Ensure we end exactly at the target angle
            rotationCoroutine = null;
        }

        float GetCurrentAngle()
        {
            switch (axis)
            {
                case Axis.X:
                    return transform.localEulerAngles.x;
                case Axis.Y:
                    return transform.localEulerAngles.y;
                case Axis.Z:
                    return transform.localEulerAngles.z;
                default:
                    return 0f;
            }
        }

        float SetAngle(float angle)
        {
            Vector3 euler = transform.localEulerAngles;
            switch (axis)
            {
                case Axis.X:
                    euler.x = angle;
                    break;
                case Axis.Y:
                    euler.y = angle;
                    break;
                case Axis.Z:
                    euler.z = angle;
                    break;
            }
            transform.localEulerAngles = euler;
            return angle;
        }
    }
}
