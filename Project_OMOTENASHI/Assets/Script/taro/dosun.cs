using UnityEngine;

public class dosun : MonoBehaviour
{
    [Header("移動範囲 (子オブジェクトで共有)")]
    [Tooltip("オブジェクトが到達できるY座標の最大値")]
    [SerializeField] private float maxY = 5.0f;

    [Tooltip("オブジェクトが到達できるY座標の最小値")]
    [SerializeField] private float minY = 0.0f;

    [Header("移動速度 (子オブジェクトで共有)")]
    [Tooltip("オブジェクトが移動する速さ")]
    [SerializeField] private float speed = 3.0f;

    [Header("操作する子オブジェクト")]
    [Tooltip("右側に配置するオブジェクト")]
    [SerializeField] private Transform rightObject;

    [Tooltip("左側に配置するオブジェクト")]
    [SerializeField] private Transform leftObject;

    // ON/OFFの状態を管理するフラグ (true: ON, false: OFF)
    private bool isOn = false;

    /// <summary>
    /// 毎フレーム呼び出される更新処理
    /// </summary>
    void Update()
    {
        // 1. Gキーが押された瞬間を検知
        if (Input.GetKeyDown(KeyCode.G))
        {
            // isOnフラグを反転させる (trueならfalseに、falseならtrueに)
            isOn = !isOn;
        }

        // 2. 現在の状態に応じて処理を分岐
        if (isOn)
        {
            // ONの時の処理
            MoveUp(rightObject);    // 右オブジェクトを上に移動
            MoveDown(leftObject);   // 左オブジェクトを下に移動
        }
        else
        {
            // OFFの時の処理
            MoveDown(rightObject);  // 右オブジェクトを下に移動
            MoveUp(leftObject);     // 左オブジェクトを上に移動
        }
    }

    /// <summary>
    /// 指定されたオブジェクトを上に移動させる
    /// </summary>
    /// <param name="obj">移動させるオブジェクトのTransform</param>
    private void MoveUp(Transform obj)
    {
        // 目的地（最大Y座標）に向かって移動
        // Vector3.up は (0, 1, 0) の方向ベクトル
        // Time.deltaTimeを掛けることで、フレームレートに依存しない滑らかな移動にする
        float step = speed * Time.deltaTime;
        obj.position = Vector3.MoveTowards(obj.position, new Vector3(obj.position.x, maxY, obj.position.z), step);
    }

    /// <summary>
    /// 指定されたオブジェクトを下に移動させる
    /// </summary>
    /// <param name="obj">移動させるオブジェクトのTransform</param>
    private void MoveDown(Transform obj)
    {
        // 目的地（最小Y座標）に向かって移動
        float step = speed * Time.deltaTime;
        obj.position = Vector3.MoveTowards(obj.position, new Vector3(obj.position.x, minY, obj.position.z), step);
    }
}
