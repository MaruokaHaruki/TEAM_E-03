using UnityEngine;

public class TestAttack_01_02 : MonoBehaviour
{
    [SerializeField] private PlayerController_01_02 Player;
    [SerializeField] private bool HipDamageFlag;
    [SerializeField, Header("正面衝突時の自身の反動距離")] private float selfKnockbackDistance = 0.1f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
// ...existing code...
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Player == null) return; // Playerがnullなら何もしない

        if (collision.gameObject.tag == "Player")
        {
            if (collision.gameObject != Player.gameObject) // 自分自身との衝突は無視
            {
                PlayerController_01_02 otherPlayerController = collision.gameObject.GetComponent<PlayerController_01_02>();
                if (otherPlayerController == null) return; // 相手のコントローラーがない場合は無視

                // 相手と自分が異なる方向を向いている場合（正面衝突に近い）
                if (otherPlayerController.GetMoveDirection() != Player.GetMoveDirection())
                {
                    // 攻撃者（Player）が少し後退する
                    Vector3 knockbackDirection = (Player.transform.position - collision.transform.position).normalized;
                    // Y軸の動きを無視したい場合は knockbackDirection.y = 0; knockbackDirection.Normalize(); とする
                    Player.transform.position += knockbackDirection * selfKnockbackDistance;
                }
                // 相手と自分が同じ方向を向いている場合（追突）
                else
                {
                    if (HipDamageFlag)
                    {
                        // 追突した場合、相手プレイヤーにダメージを与える
                        otherPlayerController.Hp = 0; 
                        // 攻撃判定オブジェクトを消すかどうかはゲームの仕様による
                        // Destroy(this.transform.parent.gameObject); 
                    }
                }
            }
        }
    }
}