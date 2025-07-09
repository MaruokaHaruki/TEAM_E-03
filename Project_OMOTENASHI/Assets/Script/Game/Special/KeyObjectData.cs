using UnityEngine;

public class KeyObjectData : MonoBehaviour
{
    /// <summary>設定キー</summary>
    public KeyCode SetKey;
    /// <summary>ヒット領域サイズ</summary>
    public Vector2 HitFieldSize = new Vector2(1.0f, 2.0f);
    /// <summary>ヒット領域ポジション</summary>
    public Vector2 HitFieldPos = new Vector2(0.0f, 0.0f);

    // 右と左のポジション
    public Vector2 GetLeftAndRightPos()
    {
        Vector2 set = new Vector2(HitFieldPos.x - HitFieldSize.x, HitFieldPos.x + HitFieldSize.x);
        return set;
    }
}
