using Unity.VisualScripting;
using UnityEngine;

public class MoveField : MonoBehaviour
{
    /// <summary>移動力</summary>
    [SerializeField, Header("ゲージ変化量")] private float GaugePower = 15.0f;
    /// <summary>右移動フラグ</summary>
    [SerializeField, Header("移動方向フラグ")] private bool RightMoveFlag;
    /// <summary>プレイヤー</summary>
    [SerializeField, Header("プレイヤー")] private Player[] Players;

    // 開始キー
    public KeyCode StartKey = KeyCode.B;

    public PopAnimation SetAnimation;

    /// <summary>位置による自動方向変更を有効にするか</summary>
    [SerializeField, Header("位置による自動方向変更")] private bool AutoDirectionChange = true;

    /// <summary>前回の位置</summary>
    private Vector3 previousPosition;

    /// <summary>現在のキー表示用</summary>
    [SerializeField, Header("現在設定されているキー（表示用）")] private KeyCode currentKey;

    /// <summary>連続でゲージ操作を防ぐためのクールダウン</summary>
    private float cooldownTimer = 0.0f;
    private const float COOLDOWN_DURATION = 0.1f;

    /// <summary>
    /// 外部からStartKeyを変更するためのメソッド
    /// </summary>
    /// <param name="newKey">新しいキー</param>
    public void ChangeStartKey(KeyCode newKey)
    {
        if (StartKey != newKey)
        {
            Debug.Log($"MoveField StartKey changed from {StartKey} to {newKey}");
            StartKey = newKey;
        }
    }

    private void Start()
    {
        // GameManagerからプレイヤーを取得
        if (GameManager.Instance != null)
        {
            Players = new Player[2];
            Players[0] = GameManager.Instance.player1_;
            Players[1] = GameManager.Instance.player2_;
        }
        else
        {
            // GameManagerがない場合はシーン内のプレイヤーを検索
            Players = FindObjectsOfType<Player>();
        }

        // 初期位置を記録
        previousPosition = transform.position;
    }

    private void Update()
    {
        // 現在のキー表示用に更新
        currentKey = StartKey;

        // クールダウンタイマー更新
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        // 位置による自動方向変更
        if (AutoDirectionChange)
        {
            CheckPositionChange();
        }

        // キー入力による動作変更
        if (Input.GetKeyDown(StartKey))
        {
            Debug.Log($"Key {StartKey} pressed! Direction changed to {(!RightMoveFlag ? "Right" : "Left")}");
            RightMoveFlag = !RightMoveFlag;
            SetAnimation.PlayAnimation();
        }
    }

    /// <summary>
    /// 位置変化をチェックして移動方向を自動変更
    /// </summary>
    private void CheckPositionChange()
    {
        Vector3 currentPosition = transform.position;

        // X座標の変化をチェック
        if (Mathf.Abs(currentPosition.x - previousPosition.x) > 0.01f)
        {
            // 右に移動している場合は右方向、左に移動している場合は左方向
            bool newDirection = currentPosition.x > previousPosition.x;

            if (newDirection != RightMoveFlag)
            {
                RightMoveFlag = newDirection;
                SetAnimation.PlayAnimation();
            }
        }

        previousPosition = currentPosition;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // プレイヤーとの接触処理
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null && cooldownTimer <= 0)
            {
                // 連打ゲージ操作を実行
                float moveDirection = RightMoveFlag ? 1.0f : -1.0f;
                player.ApplyMoveFieldEffect(moveDirection, GaugePower);
                
                // クールダウン設定
                cooldownTimer = COOLDOWN_DURATION;
                
                Debug.Log($"MoveField effect applied to {player.playerID_}: Direction={moveDirection}, Power={GaugePower}");
            }
        }

        // キーオブジェクトとの接触でキー変更
        if (collision.gameObject.CompareTag("KeyData"))
        {
            KeyObjectData_Field keyData = collision.GetComponent<KeyObjectData_Field>();
            if (keyData != null)
            {
                ChangeStartKey(keyData.SetKey);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // プレイヤーが矢印内に留まっている間も連続でゲージ操作
        if (collision.gameObject.CompareTag("Player") && cooldownTimer <= 0)
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                float moveDirection = RightMoveFlag ? 1.0f : -1.0f;
                player.ApplyMoveFieldEffect(moveDirection, GaugePower * 0.3f); // Stay時は効果を弱める
                
                cooldownTimer = COOLDOWN_DURATION * 2; // Stay時はクールダウンを長めに
            }
        }
    }
}
