using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("ひっとぽいんと")]
    public int hitPoint; // プレイヤー体力

    [Header("点滅設定")]
    public float flashDuration = 0.1f;     // 点滅1回の時間
    public int flashCount = 3;             // 点滅の回数

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isFlashing = false;
    private bool isInvincible = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    // プレイヤーが攻撃を受けたときの処理
    public void TakeDamage(int damage)
    {
        // 点滅中はダメージを無視
        if (isInvincible)
        {
            Debug.Log($"{gameObject.name} は無敵中のためダメージ無効！");
            return;
        }


        hitPoint -= damage;
        Debug.Log($"{gameObject.name} は {damage} のダメージを受けた！残り体力: {hitPoint}");

        if (!isFlashing)
            StartCoroutine(FlashRed());

        if (hitPoint <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} は死亡しました！");
        // プレイヤーの死亡処理を書く
        Destroy(gameObject);
    }

    // 点滅処理
    private System.Collections.IEnumerator FlashRed()
    {
        isFlashing = true;
        isInvincible = true;

        for (int i = 0; i < flashCount; i++)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.red;
            }

            yield return new WaitForSeconds(flashDuration);

            if (spriteRenderer != null)
            {
                spriteRenderer.color = originalColor;
            }

            yield return new WaitForSeconds(flashDuration);
        }

        isFlashing = false;
        isInvincible = false;
    }
}
