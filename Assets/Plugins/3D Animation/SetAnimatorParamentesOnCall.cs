using UnityEngine;

namespace IO_Scripts.Animation
{
    public class SetAnimatorParamentesOnCall : MonoBehaviour
    {
        [SerializeField] AnimatorParameters[] parameters;
        [ContextMenu("Set")]
        public void Set()
        {
            foreach (AnimatorParameters parameter in parameters)
            {
                parameter.Set();
            }
        }
    }

    [System.Serializable]
    public class AnimatorParameters
    {
        public Animator Animator;
        public string Name;
        public AnimatorControllerParameterType ParameterType;
        public float FloatValue;
        public bool BoolValue;
        public int IntValue;

        public void Set()
        {
            switch (ParameterType)
            {
                case AnimatorControllerParameterType.Float:
                    Animator.SetFloat(Name, FloatValue);
                    break;
                case AnimatorControllerParameterType.Int:
                    Animator.SetInteger(Name, IntValue);
                    break;
                case AnimatorControllerParameterType.Bool:
                    Animator.SetBool(Name, BoolValue);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    Animator.SetTrigger(Name);
                    break;
            }
        }
    }
}