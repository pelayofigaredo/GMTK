using IO_Scripts.MaterialAnimation;
using System.Collections;
using UnityEngine;

public class EndExplosionAnimator : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private CharacterHandler characterHandler;
    [SerializeField] private Transform explosion;
    [SerializeField] MaterialAnimator[] materialAnimators;


    [Header("Posición")]
    [SerializeField] private Vector3 minScale = Vector3.zero;
    [SerializeField] private Vector3 maxScale = Vector3.one;
    [SerializeField] private Vector3 offset = Vector3.zero;


    [Header("Curva de animación")]
    [SerializeField] private AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Parámetro t (0..1)")]
    [Range(0f, 1f)]
    [SerializeField] private float t = 0f;

    [Header("Runtime")]
    [Min(0f)]
    [SerializeField] private float duration = 1f;
    [Min(0f)]
    [SerializeField] private float initialAnimationWait = 1f;

    public event System.Action OnComplete;
    public event System.Action OnHidden;

    public float T
    {
        get => t;
        set { t = Mathf.Clamp01(value); Apply(); }
    }

    public bool IsPlaying { get; private set; }

    private Coroutine _playRoutine;

    private void Start()
    {
        explosion.gameObject.SetActive(false);
    }

    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            Apply();
        }
    }

    [ContextMenu("Play")]
    public void Play()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("[CurveScaler] Play() solo tiene efecto en runtime.", this);
            return;
        }

        foreach (var animator in materialAnimators)
        {
            animator.SetT(0);
            animator.Play();
        }

        if (_playRoutine != null) StopCoroutine(_playRoutine);
        _playRoutine = StartCoroutine(PlayRoutine());
    }

    public void Stop()
    {
        if (_playRoutine != null)
        {
            StopCoroutine(_playRoutine);
            _playRoutine = null;
        }
        IsPlaying = false;
    }

    private IEnumerator PlayRoutine()
    {
        explosion.position = characterHandler.transform.position + offset;

        IsPlaying = true;

        characterHandler.Die();
        yield return new WaitForSeconds(initialAnimationWait);
        explosion.gameObject.SetActive(true);

        bool launchHidden = false;

        if (duration <= 0f)
        {
            T = 1f;
        }
        else
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                if (!launchHidden)
                {
                    if (elapsed > duration / 2)
                    {
                        launchHidden = true;
                        OnHidden();
                    }
                }
                elapsed += Time.deltaTime;
                T = Mathf.Clamp01(elapsed / duration);
                Apply();
                yield return null;
            }
            T = 1f;
        }

        IsPlaying = false;
        _playRoutine = null;

        explosion.gameObject.SetActive(false);
        foreach (var animator in materialAnimators)
        {
            animator.SetT(0);
        }

        OnComplete?.Invoke();
    }

    private void Apply()
    {
        float eased = curve.Evaluate(t);
        explosion.localScale = Vector3.LerpUnclamped(minScale, maxScale, eased);
    }
}
