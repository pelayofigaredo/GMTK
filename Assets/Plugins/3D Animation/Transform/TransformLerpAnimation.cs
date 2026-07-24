using System.Collections;
using UnityEngine;
namespace IO_Scripts.Animation
{
    public class TransformLerpAnimation : MonoBehaviour
    {
        [SerializeField] Transform targetTransform;
        [SerializeField] Transform aTransform;
        [SerializeField] Transform bTransform;
        [SerializeField] float animationTime = 2;


        [ContextMenu("Go to A")]
        public void GoToA()
        {
            StopAllCoroutines();
            StartCoroutine(LerpCR(bTransform,aTransform));
        }

        [ContextMenu("Go to B")]
        public void GoToB()
        {
            StopAllCoroutines();
            StartCoroutine(LerpCR(aTransform, bTransform));
        }

        IEnumerator LerpCR(Transform orign, Transform destination)
        {
            float t = 0;
            float step = 1 / animationTime;
            while (t < 1)
            {
                t += step * Time.deltaTime;
                if (t > 1) t = 1;
                LerpTransform(targetTransform, orign, destination, t);
                yield return null;
            }
        }

       static void LerpTransform(Transform target, Transform a, Transform b, float t)
        {
            target.SetPositionAndRotation(
                Vector3.Lerp(a.position, b.position, t),
                Quaternion.Slerp(a.rotation, b.rotation, t)
                );
            target.localScale = Vector3.Lerp(a.localScale, b.localScale, t);
        }
    }
}
