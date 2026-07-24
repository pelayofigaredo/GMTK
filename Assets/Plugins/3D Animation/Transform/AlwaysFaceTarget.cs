using UnityEngine;

namespace IO_Scripts.Animation
{
    ///<summary>
    /// Garantiza que el transform siempre mire hacia el target, entendiendo el eje local Z como su parte frontal.
    ///</summary>
    public class AlwaysFaceTarget : MonoBehaviour
    {
        [SerializeField] Transform target;

        public Transform Target { get => target; set => target = value; }

        void Update()
        {
            transform.LookAt(target);
        }

        private void OnValidate()
        {
            transform.LookAt(target);
        }
    }

}
