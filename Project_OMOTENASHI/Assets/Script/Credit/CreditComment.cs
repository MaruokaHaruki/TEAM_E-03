using UnityEngine;
using TMPro;

public class CreditComment : MonoBehaviour {
    [Header("生存時間（秒）")]
    public float lifetime = 5f;

    [Header("フェード時間（秒）")]
    public float fadeDuration = 1.5f;

    private float timer = 0f;
    private TextMeshPro text_;
    private Color originalColor_;
    private bool isFading = false;

    void Start() {
        text_ = GetComponent<TextMeshPro>();
        if (text_ != null) {
            originalColor_ = text_.color;
        }
    }

    void Update() {
        timer += Time.deltaTime;

        // フェードアウト開始
        if (timer >= lifetime && !isFading) {
            isFading = true;
            StartCoroutine(FadeAndDestroy());
        }
    }

    private System.Collections.IEnumerator FadeAndDestroy() {
        float elapsed = 0f;

        while (elapsed < fadeDuration) {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);

            if (text_ != null) {
                Color c = originalColor_;
                c.a = alpha;
                text_.color = c;
            }

            yield return null;
        }

        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            Destroy(gameObject);
        }
    }
}
