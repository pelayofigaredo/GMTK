using System.Collections;
using UnityEngine;

namespace IO_Scripts.Animation
{
    ///<summary>
    /// Permite animar la rotacion de un objeto mediante saltos entre valores aleatorios en un rango dado.
    /// Util para animar elementos que vibran más rapido de lo que la tasa de fotogramas podria mostrar.
    ///</summary>
    public class RandomRotationAnimation : MonoBehaviour
    {
        public Vector2 XChange = new Vector2(0, 0);
        public Vector2 YChange = new Vector2(0, 0);
        public Vector2 ZChange = new Vector2(0, 0);

        Quaternion originalRotation;
        Coroutine animationCoroutine;

        void Start()
        {
            originalRotation = transform.rotation;
        }

        IEnumerator AnimationCR()
        {
            while (true)
            {
                Randomice();
                yield return new WaitForEndOfFrame();
            }
        }

        public void Randomice()
        {
            float x = UnityEngine.Random.Range(XChange.x, XChange.y);
            float y = UnityEngine.Random.Range(YChange.x, YChange.y);
            float z = UnityEngine.Random.Range(ZChange.x, ZChange.y);
            transform.rotation = originalRotation * Quaternion.Euler(x,y,z);
        }

        public void Restart()
        {
            transform.rotation = originalRotation;
        }

        [ContextMenu("Play Animation")]
        public void PlayAnimation()
        {
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
            }
            animationCoroutine = StartCoroutine(AnimationCR());
        }

        [ContextMenu("Stop Animation")]
        public void StopAnimation()
        {
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
                animationCoroutine = null;
            }
            transform.rotation = originalRotation; // Reset rotation when stopping
        }
    }
}