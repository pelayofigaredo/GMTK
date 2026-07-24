using System.Collections.Generic;
using UnityEngine;

public class UIAttackBarGroup : MonoBehaviour
{
    [SerializeField] private List<UIAttackBar> bars = new List<UIAttackBar>();
    [SerializeField] private float totalWidth = 600f;
    [SerializeField] private bool animate = true;

    private void Awake()
    {
        for (int i = 0; i < bars.Count; i++)
        {
            bars[i].GroupControlled = true;
            bars[i].Changed += OnBarChanged;
        }
        Apply(false);
    }

    public void Add(int index, int amount)
    {
        bars[index].Add(amount);
    }

    public void SetValue(int index, int value)
    {
        bars[index].SetValue(value);
    }

    public void SetValue(int value)
    {
        for (int i = 0; i < bars.Count; i++)
        {
            bars[i].SetValue(value);
        }
    }

    private void OnBarChanged() => Apply(animate);

    private void Apply(bool anim)
    {
        int sum = 0;
        for (int i = 0; i < bars.Count; i++)
            sum += Mathf.Max(0, bars[i].Target);

        for (int i = 0; i < bars.Count; i++)
        {
            float share = sum > 0 ? (float)Mathf.Max(0, bars[i].Target) / sum : 0f;
            bars[i].AnimateWidthTo(share * totalWidth, anim);
        }
    }

    internal void ApplyColor(int index, Color valu)
    {
        bars[index].ApplyColor(valu);
    }
}
