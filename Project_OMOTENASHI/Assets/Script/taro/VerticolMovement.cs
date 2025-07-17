using UnityEngine;

/// <summary>
/// キーを押している間は上昇し、離すと下降するオブジェクトを制御するスクリプト。
/// </summary>
public class VerticalMovement : MonoBehaviour
{
    [Header("▼ 動作設定")]
    [Tooltip("アクションを起動するキー")]
    [SerializeField] private KeyCode triggerKey = KeyCode.Space;

    [Header("▼ 速度設定")]
    [Tooltip("オブジェクトが上昇するスピード")]
    [SerializeField] private float riseSpeed = 5f;

    [Tooltip("オブジェクトが下降するスピード")]
    [SerializeField] private float fallSpeed = 1f;

    [Header("▼ 移動範囲設定")]
    [Tooltip("オブジェクトが到達できるY座標の最大値")]
    [SerializeField] private float maxHeight = 5f;

    [Tooltip("オブジェクトが到達できるY座標の最小値")]
    [SerializeField] private float minHeight = 0f;

    // プライベート変数
    private Vector3 initialPosition;

    private void Awake()
    {
        // ゲーム開始時の位置を基準とするため、初期位置を保存
        initialPosition = transform.position;
    }

    private void Update()
    {
        // 現在の位置を取得
        Vector3 currentPosition = transform.position;
        float targetY = currentPosition.y;

        // キーが押されているかチェック
        if (Input.GetKey(triggerKey))
        {
            // 上昇処理
            targetY += riseSpeed * Time.deltaTime;
        }
        else
        {
            // 下降処理
            targetY -= fallSpeed * Time.deltaTime;
        }

        // Y座標を最大値と最小値の間に制限する
        // Awakeで取得した初期位置を基準に計算
        float clampedY = Mathf.Clamp(targetY, initialPosition.y + minHeight, initialPosition.y + maxHeight);

        // 新しい位置を設定
        transform.position = new Vector3(currentPosition.x, clampedY, currentPosition.z);
    }
}
