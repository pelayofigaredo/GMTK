using System.Collections;
using UnityEngine;

namespace IO_Scripts.Animation
{
    public class FollowWobble : MonoBehaviour
    {
        [SerializeField] bool active = true;
        [SerializeField] Transform target;
        [SerializeField] float speed = 1;
        [SerializeField] float elasticity = 2f; 

        private void Awake()
        {
            if (target == null)
            {
                active = false;
            }
        }

        IEnumerator FollowCR()
        {
            while (active)
            {
                Vector3 newPosition = Vector3.Lerp(transform.position, target.position, Time.deltaTime * speed);
                newPosition = Vector3.Lerp(transform.position, newPosition, elasticity);
                transform.position = newPosition;
                yield return new WaitForEndOfFrame();
            }
        }

        public void SetTarget(Transform target, bool activate = true)
        {
            this.target = target;
            SetActive(activate);
        }

        public void SetActive(bool active)
        {
            this.active = active;
            if (active)
            {
                StartCoroutine(FollowCR());
            }
        }

        public void Toggle()
        {
            active = !active;
            if (active)
            {
                StartCoroutine(FollowCR());
            }
        }
    }
}