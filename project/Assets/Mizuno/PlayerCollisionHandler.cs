using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // 方向・速度を判定してノックバックを適用
            // 空中でも地上でも同様に処理する
        }
    }

}
