using UnityEngine;

public class dosun : MonoBehaviour
{
    [Header("移動範囲の設定")]
    [Tooltip("オブジェクトが到達できるY座標の最大値")]
    [SerializeField] private float maxY = 5.0f;

    [Tooltip("オブジェクトが到達できるY座標の最小値")]
    [SerializeField] private float minY = 0.0f;

    [Header("移動速度の設定")]
    [Tooltip("オブジェクトが上下に移動する速度")]
    [SerializeField] private float speed = 3.0f;

    [Header("制御対象のオブジェクト")]
    [Tooltip("右側に配置されているオブジェクト")]
    [SerializeField] private Transform rightObject;

    [Tooltip("左側に配置されているオブジェクト")]
    [SerializeField] private Transform leftObject;

    // オン・オフ状態を管理するフラグ (true: オン状態, false: オフ状態)
    private bool isOn = false;

    /// <summary>
    /// 毎フレーム呼び出される処理：入力チェックとオブジェクト位置更新を行う
    /// </summary>
    void Update()
    {
        // 1. スペースキーがこのフレームで押されたかをチェック
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // isOnフラグを反転する（オンならオフに、オフならオンに切り替える）
            isOn = !isOn;
        }

        // 2. 現在の状態に応じて異なる動作を実行
        if (isOn)
        {
            // オン状態の動作：右が上、左が下に移動
            MoveUp(rightObject);    // 右側のオブジェクトを上に移動
            MoveDown(leftObject);   // 左側のオブジェクトを下に移動
        }
        else
        {
            // オフ状態の動作：右が下、左が上に移動
            MoveDown(rightObject);  // 右側のオブジェクトを下に移動
            MoveUp(leftObject);     // 左側のオブジェクトを上に移動
        }
    }

    /// <summary>
    /// 指定されたオブジェクトを最大Y位置まで滑らかに上向きに移動させる
    /// </summary>
    /// <param name="obj">移動させるオブジェクトのTransform</param>
    private void MoveUp(Transform obj)
    {
        // 目標位置（最大Y座標）に向かって滑らかに移動
        // Time.deltaTimeを掛けることでフレームレートに依存しない一定速度での移動を実現
        float step = speed * Time.deltaTime;
        obj.position = Vector3.MoveTowards(obj.position, new Vector3(obj.position.x, maxY, obj.position.z), step);
    }

    /// <summary>
    /// 指定されたオブジェクトを最小Y位置まで滑らかに下向きに移動させる
    /// </summary>
    /// <param name="obj">移動させるオブジェクトのTransform</param>
    private void MoveDown(Transform obj)
    {
        // 目標位置（最小Y座標）に向かって滑らかに移動
        float step = speed * Time.deltaTime;
        obj.position = Vector3.MoveTowards(obj.position, new Vector3(obj.position.x, minY, obj.position.z), step);
    }
}
