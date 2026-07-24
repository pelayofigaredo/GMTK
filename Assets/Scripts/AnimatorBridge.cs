using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorBridge : MonoBehaviour
{
   Animator animator;

    public Animator Animator { get => animator;}

    public event System.Action OnAttack;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }
   
    public void Attack() => OnAttack?.Invoke();
}
