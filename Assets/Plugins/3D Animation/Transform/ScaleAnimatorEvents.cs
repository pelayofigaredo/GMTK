using UnityEngine;
using UnityEngine.Events;

namespace IO_Scripts.Animation
{
    public class ScaleAnimatorEvents : ScaleAnimator
    {
        [Header("Events")]
        public UnityEvent OnGrowComplete;
        public UnityEvent OnShrinkComplete;

        protected override void GrowCompleted()   { OnGrowComplete?.Invoke();   }
        protected override void ShrinkCompleted() { OnShrinkComplete?.Invoke(); }
    }
}
