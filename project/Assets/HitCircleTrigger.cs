using UnityEngine;
using System;

public class HitCircleTrigger : MonoBehaviour
{
    public event Action<GameObject> OnHitReceived;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 攻撃してきたプレイヤーに"Player"タグがあるか確認
        if (collision.CompareTag("Player"))
        {
            OnHitReceived.Invoke(collision.gameObject);
        }
    }
}
