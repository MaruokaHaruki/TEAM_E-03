using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour
{
    [SerializeField, Header("被ダメージ当たり判定")]
    private GameObject HitCircle;

    private void Start()
    {
        // 攻撃される判定を有効化
        HitCircle.GetComponent<PolygonCollider2D>().enabled = true;
    }

    // HitCircleに入った相手を検出するための中継クラスにアクセス
    private void OnEnable()
    {
        if (HitCircle != null)
        {
            var trigger = HitCircle.GetComponent<HitCircleTrigger>();
            if (trigger != null)
            {
                trigger.OnHitReceived += OnHitReceived;
            }
        }
    }

    private void OnDisable()
    {
        if (HitCircle != null)
        {
            var trigger = HitCircle.GetComponent<HitCircleTrigger>();
            if (trigger != null)
            {
                trigger.OnHitReceived -= OnHitReceived;
            }
        }
    }

    // 他プレイヤーが自分のHitCircleに入ったとき呼ばれる
    private void OnHitReceived(GameObject attacker)
    {
        if (attacker != this.gameObject)
        {
            Debug.Log($"{gameObject.name} は {attacker.name} に攻撃された！");
            // ここで自分がダメージを受ける処理を書く
            GetComponent<Player>().TakeDamage(1);
        }
    }
}
