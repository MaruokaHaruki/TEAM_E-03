using UnityEngine;

public class KeyObjectData_Field : MonoBehaviour
{
    /// <summary>設定キー</summary>
    public KeyCode SetKey;
    /// <summary>ヒット領域サイズ</summary>
    [Header("判定はこれで取っているオブジェクトサイズなどは関係ない")] public Vector2 HitFieldSize = new Vector2(1.0f, 2.0f);
    /// <summary>ヒット領域ポジション</summary>
    [Header("XYZがすべて0なら現在座標を基準にに高さを調整して(サイズが影響)設定する")] public Vector3 HitFieldPos = Vector3.zero;

    private void Start()
    {
        if (HitFieldPos == Vector3.zero)
        {
            HitFieldPos = new Vector3(this.transform.position.x, this.transform.position.y + ((HitFieldSize.y - this.transform.lossyScale.y) * 0.5f));
        }
    }

    // 右と左のポジション
    public Vector2 GetLeftAndRightPos()
    {
        Vector2 set = new Vector2(HitFieldPos.x - HitFieldSize.x, HitFieldPos.x + HitFieldSize.x);
        return set;
    }
}
