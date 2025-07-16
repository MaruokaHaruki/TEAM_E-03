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

    // キー変更オブジェクト  KeyChangeObjects
    [SerializeField] private GameObject []KeyObjects;

    public KeyCode JumpKey(string playerType)
    {
        if (playerType == "A")
        {
            return JumpPlayerAKey;
        }
        else if (playerType == "B")
        {
            return JumpPlayerBKey;
        }

        return KeyCode.None;
    }

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

    public void SetKeyObjects(bool activeFlag, KeyCode jumpPlayerAKey, KeyCode accelerationPlayerAKey, KeyCode jumpPlayerBKey, KeyCode accelerationPlayerBKey)
    {
        for (int i = 0; i < KeyObjects.Length; i++)
        {
            KeyObjects[i].SetActive(activeFlag);
        }

        JumpPlayerAKey = jumpPlayerAKey;
        AccelerationPlayerAKey = accelerationPlayerAKey;
        JumpPlayerBKey = jumpPlayerBKey;
        AccelerationPlayerBKey = accelerationPlayerBKey;
    }
}
