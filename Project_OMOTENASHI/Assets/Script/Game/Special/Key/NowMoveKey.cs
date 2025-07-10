using UnityEngine;

public class NowMoveKey : MonoBehaviour
{
    // シングルトン
    public static NowMoveKey Instance { get; private set; }

    // プレイヤーAキー
    public KeyCode JumpPlayerAKey;
    public KeyCode AccelerationPlayerAKey;

    // プレイヤーBキー
    public KeyCode JumpPlayerBKey;
    public KeyCode AccelerationPlayerBKey;

    private void Awake()
    {
        // シングルトンの設定
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
