using System.Collections;
using UnityEngine;

public class RoundDirectionController: MonoBehaviour
{
    [System.Serializable]
    public class UIElement
    {
        public CanvasGroup canvasGroup;
        public float delay; // この要素の表示までの待機時間
    }

    public UIElement[] elements;
    public float fadeDuration = 1f;

    void Awake()
    {
        // 初期状態では全て非表示
        foreach (var element in elements)
        {
            element.canvasGroup.alpha = 0f;
        }
    }

   public  void RoundDirectonStart()
    {
        

        StartCoroutine(FadeInSequence());
    }

   public void RoundDirectonEnd()
    {
        foreach (var element in elements)
        {
            element.canvasGroup.alpha = 0f; // 終わったら全て非表示
        }

    }

    IEnumerator FadeInSequence()
    {
        foreach (var element in elements)
        {
            yield return new WaitForSeconds(element.delay);
            StartCoroutine(FadeIn(element.canvasGroup));
        }
    }

    IEnumerator FadeIn(CanvasGroup canvasGroup)
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }
}
