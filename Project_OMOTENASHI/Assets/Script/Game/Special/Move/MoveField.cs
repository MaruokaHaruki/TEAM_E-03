using Unity.VisualScripting;
using UnityEngine;

public class MoveField : MonoBehaviour {
    /// <summary>移動力</summary>
    [SerializeField, Header("移動力")] private float MovePower;
    /// <summary>右移動フラグ</summary>
    [SerializeField, Header("移動方向フラグ")] private bool RightMoveFlag;
    /// <summary>プレイヤー</summary>
    [SerializeField, Header("プレイヤー")] private Rigidbody2D[] PlayerRigidBody;

    // 開始キー
    public KeyCode StartKey = KeyCode.B;

    public PopAnimation SetAnimation;

    /// <summary>位置による自動方向変更を有効にするか</summary>
    [SerializeField, Header("位置による自動方向変更")] private bool AutoDirectionChange = true;

    /// <summary>前回の位置</summary>
    private Vector3 previousPosition;

    /// <summary>現在のキーを表示用</summary>
    [SerializeField, Header("現在設定されているキー（表示用）")] private KeyCode currentKey;

    /// <summary>
    /// 外部からStartKeyを変更するためのメソッド
    /// </summary>
    /// <param name="newKey">新しいキー</param>
    public void ChangeStartKey(KeyCode newKey) {
        if (StartKey != newKey) {
            Debug.Log($"MoveField StartKey changed from {StartKey} to {newKey}");
            StartKey = newKey;
        }
    }

    private void Start() {
        PlayerRigidBody = new Rigidbody2D[2];
        PlayerRigidBody[0] = GameManager.Instance.player1_.GetComponent<Rigidbody2D>();
        PlayerRigidBody[1] = GameManager.Instance.player2_.GetComponent<Rigidbody2D>();

        // 初期位置を記録
        previousPosition = transform.position;
    }

    private void Update() {
        // 現在のキーを表示用に更新
        currentKey = StartKey;

        // 位置による自動方向変更
        if (AutoDirectionChange) {
            CheckPositionChange();
        }

        // キー入力による手動変更
        if (Input.GetKeyDown(StartKey)) {
            Debug.Log($"Key {StartKey} pressed! Direction changed to {(!RightMoveFlag ? "Right" : "Left")}");
            RightMoveFlag = !RightMoveFlag;
            SetAnimation.PlayAnimation();
        }
    }

    /// <summary>
    /// 位置変化をチェックして移動方向を自動変更
    /// </summary>
    private void CheckPositionChange() {
        Vector3 currentPosition = transform.position;

        // X座標の変化をチェック
        if (Mathf.Abs(currentPosition.x - previousPosition.x) > 0.01f) {
            // 右に移動している場合は右向き、左に移動している場合は左向き
            bool newDirection = currentPosition.x > previousPosition.x;

            if (newDirection != RightMoveFlag) {
                RightMoveFlag = newDirection;
                SetAnimation.PlayAnimation();
            }
        }

        previousPosition = currentPosition;
    }

    private void OnTriggerStay2D(Collider2D collision) {

    }

    private void OnTriggerEnter2D(Collider2D collision) {
        // キーオブジェクトとの接触でキー変更
        if (collision.gameObject.tag == "KeyData") {
            KeyObjectData_Field keyData = collision.GetComponent<KeyObjectData_Field>();
            if (keyData != null) {
                ChangeStartKey(keyData.SetKey);
            }
        }


        if (collision.gameObject.tag != "Player") {
            return;
        }

        foreach (Rigidbody2D player in PlayerRigidBody) {
            if (collision.gameObject == player.gameObject) {
                player.AddForceX(MovePower * (RightMoveFlag ? 1.0f : -1.0f), ForceMode2D.Impulse);
            }
        }
    }
}
