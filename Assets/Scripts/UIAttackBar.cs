using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIAttackBar : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private RectTransform fill;
    [SerializeField] private TMP_Text label;
    [SerializeField] private Image[] tintImages;

    [Header("Configuration")]
    [SerializeField] private float pixelsPerUnit = 20f;
    [SerializeField] private float minWidth = 0f;
    [SerializeField] private float maxWidth = 400f;
    [SerializeField] private Color tintColor = Color.white;

    [Header("Animación")]
    [SerializeField] private float growDuration = 0.4f;
    [SerializeField] private AnimationCurve growCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);



    public int Target { get; private set; }
    public bool GroupControlled { get; set; }

    public event Action Changed;

    private float _width;
    private Coroutine _widthCo;

    private void Awake()
    {
        _width = fill.sizeDelta.x;
        if (label != null) label.text = Target.ToString();
    }

    private void OnValidate()
    {
        if (Application.isPlaying) return;
        ApplyColor(tintColor);
    }

    public void ApplyColor(Color color)
    {
        tintColor = color;

        for (int i = 0; i < tintImages.Length; i++)
        {
            if (tintImages[i] == null) continue;

            tintImages[i].color = tintColor;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(tintImages[i]);
#endif
        }
    }

    public void Add(int amount = 1)
    {
        if (amount == 0) return;
        SetValue(Target + amount, true);
    }

    public void SetValue(int value, bool animate = true)
    {
        if (value < 0) value = 0;

        Target = value;
        if (label != null) label.text = Target.ToString();

        if (GroupControlled)
        {
            Changed?.Invoke();
            return;
        }

        float w = Mathf.Clamp(Target * pixelsPerUnit, minWidth, maxWidth);
        AnimateWidthTo(w, animate);
    }

    public void AnimateWidthTo(float targetWidth, bool animate = true)
    {
        if (_widthCo != null) StopCoroutine(_widthCo);

        if (!animate || growDuration <= 0f)
        {
            SetWidth(targetWidth);
            return;
        }

        _widthCo = StartCoroutine(WidthCR(targetWidth));
    }

    private IEnumerator WidthCR(float to)
    {
        float from = _width;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / growDuration;
            float eased = growCurve.Evaluate(Mathf.Clamp01(t));
            SetWidth(Mathf.LerpUnclamped(from, to, eased));
            yield return null;
        }

        SetWidth(to);
        _widthCo = null;
    }

    private void SetWidth(float w)
    {
        _width = w;
        var size = fill.sizeDelta;
        size.x = w;
        fill.sizeDelta = size;
    }
}
