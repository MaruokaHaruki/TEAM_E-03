using DG.Tweening;
using UnityEngine;

public class CanvasDisplayController : MonoBehaviour
{
    [Header("Display Settings")]
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float scaleInDuration = 0.8f;
    [SerializeField] private Ease fadeEase = Ease.OutQuart;
    [SerializeField] private Ease scaleEase = Ease.OutBack;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();

        // 初期状態設定
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
        
        if (rectTransform != null)
        {
            rectTransform.localScale = Vector3.zero;
        }
    }

    public void ShowCanvas()
    {
        gameObject.SetActive(true);

        // フェードインアニメーション
        if (canvasGroup != null)
        {
            canvasGroup.DOFade(1f, fadeInDuration).SetEase(fadeEase);
        }

        // スケールインアニメーション
        if (rectTransform != null)
        {
            rectTransform.DOScale(Vector3.one, scaleInDuration).SetEase(scaleEase);
        }
    }

    public void HideCanvas()
    {
        // フェードアウトアニメーション
        if (canvasGroup != null)
        {
            canvasGroup.DOFade(0f, fadeInDuration * 0.5f)
                .OnComplete(() => gameObject.SetActive(false));
        }
    }
}
