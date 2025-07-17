using UnityEngine;

public class CreditComment : MonoBehaviour {
    // プレイヤーとぶつかったら自分を消す
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            Destroy(gameObject);
        }
    }
}
