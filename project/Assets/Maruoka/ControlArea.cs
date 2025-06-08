using UnityEngine;

public class ControlArea : MonoBehaviour {
    public float requiredTime = 10.0f;

    private float playerAStayTime_ = 0;
    private float playerBStayTime_ = 0;

    private void OnTriggerStay2D(Collider2D collision) {
        Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
        if (rb == null || rb.linearVelocity.magnitude > 0.5f) return;

        if (collision.name == "PlayerA") {
            playerAStayTime_ += Time.deltaTime;
            if (playerAStayTime_ >= requiredTime)
                GameManager.Instance.GameOver(collision.gameObject);
        }
        else if (collision.name == "PlayerB") {
            playerBStayTime_ += Time.deltaTime;
            if (playerBStayTime_ >= requiredTime)
                GameManager.Instance.GameOver(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.name == "PlayerA") playerAStayTime_ = 0;
        if (collision.name == "PlayerB") playerBStayTime_ = 0;
    }
}
