using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public static class SliderShake
{
    public static void Shake(this Slider slider)
    {
        RectTransform rt = slider.GetComponent<RectTransform>();
        if (rt == null) return;

        DOTween.Kill(rt);
        rt.DOShakeScale(0.15f, 0.25f, 10, 90f);
    }
}