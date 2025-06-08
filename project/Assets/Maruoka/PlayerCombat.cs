using UnityEngine;

public class PlayerCombat : MonoBehaviour {
    public int maxHp = 100;
    public float knockbackForce = 10.0f;
    private int currentHp_;
    private Rigidbody2D rb_;

    private void Start() {
        rb_ = GetComponent<Rigidbody2D>();
        currentHp_ = maxHp;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (!collision.gameObject.CompareTag("Player")) return;

        Rigidbody2D enemyRb = collision.gameObject.GetComponent<Rigidbody2D>();
        PlayerCombat enemyCombat = collision.gameObject.GetComponent<PlayerCombat>();

        // 速度差の計算
        float selfSpeed = rb_.linearVelocity.magnitude;
        float enemySpeed = enemyRb.linearVelocity.magnitude;

        Vector2 contactNormal = collision.GetContact(0).normal;
        bool isBackAttack = Vector2.Dot(rb_.linearVelocity.normalized, contactNormal) > 0.5f;

        if (isBackAttack) {
            enemyCombat.TakeDamage(20, rb_.linearVelocity.normalized * knockbackForce);
        }
        else if (selfSpeed > enemySpeed) {
            enemyCombat.TakeDamage(15, rb_.linearVelocity.normalized * knockbackForce);
        }
    }

    public void TakeDamage(int damage, Vector2 knockback) {
        currentHp_ -= damage;
        rb_.AddForce(-knockback, ForceMode2D.Impulse);

        if (currentHp_ <= 0) {
            GameManager.Instance.GameOver(gameObject);
        }
    }

    public int GetCurrentHp() {
        return currentHp_;
    }

    /// <summary>
    /// HPを回復する
    /// </summary>
    public void RestoreHP(int amount) {
        currentHp_ = Mathf.Min(currentHp_ + amount, maxHp_);
        Debug.Log($"{gameObject.name} は {amount} 回復！ 現在HP: {currentHp_}");
    }
}
