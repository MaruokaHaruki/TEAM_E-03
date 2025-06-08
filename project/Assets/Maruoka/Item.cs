using UnityEngine;

/// <summary>
/// アイテム取得時にプレイヤーへ効果を与える処理
/// </summary>
public class Item : MonoBehaviour {
    public enum ItemType {
        Heal,
        SpeedBoost,
        JumpBoost
    }

    public ItemType itemType; // アイテムの種類
    public float effectValue = 20f; // 効果量

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            PlayerCombat playerCombat = other.GetComponent<PlayerCombat>();
            PlayerController playerController = other.GetComponent<PlayerController>();

            switch (itemType) {
                case ItemType.Heal:
                    if (playerCombat != null) {
                        playerCombat.RestoreHP((int)effectValue);
                    }
                    break;
                case ItemType.SpeedBoost:
                    if (playerController != null) {
                        playerController.ApplySpeedBoost(effectValue, 5f); // 5秒間速度上昇
                    }
                    break;
                case ItemType.JumpBoost:
                    if (playerController != null) {
                        playerController.ApplyJumpBoost(effectValue, 5f); // 5秒間ジャンプ力上昇
                    }
                    break;
            }

            Destroy(gameObject); // 取得後に消す
        }
    }
}
