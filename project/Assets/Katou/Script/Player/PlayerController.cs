using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.PlayerSettings;
using UnityEngine.UIElements;
using UnityEngine.Playables;

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// float 座標計算 誤差
    /// </summary>
    private const float FloatPosError = 1.0f;

    /// <summary>
    /// プレイヤーHP
    /// </summary>
    [SerializeField, Header("HP")] private int PlayerHp;

    /// <summary>
    /// プレイヤーリジッドボディ
    /// </summary>
    private Rigidbody2D PlayerRigidbody;

    /// <summary>
    /// プレイヤー移動速度 負の値を設定不可
    /// </summary>
    [SerializeField, Header("プレイヤースピード"), Min(0.01f)] private float PlayerSpeed = 1.0f;
    /// <summary>
    /// 絶対値設定用
    /// </summary>
    private float UnsignedSpeed
    {
        get { return PlayerSpeed; }
        set { PlayerSpeed = Mathf.Abs(value); }
    }

    /// <summary>
    /// リジッドボディ移動フラグ
    /// </summary>
    [SerializeField, Header("リジッドボディ移動フラグ")] private bool RigidbodyMoveFlag = true;

    /// <summary>
    /// 重力無効化フラグ
    /// </summary>
    [SerializeField, Header("重力無効化フラグ")] private bool GravityDeactivationFlag = true;

    /// <summary>ジャンプフラグ</summary>
    [SerializeField, Header("ジャンプフラグ")] private bool JumpFlag;
    /// <summary>ジャンプ移動速度</summary>
    [SerializeField, Header("ジャンプ移動速度")] private float JumpMoveSpeed;
    /// <summary>ジャンプ速度 毎秒</summary>
    [SerializeField, Header("ジャンプ速度 毎秒")] private float JumpSecondSpeed;
    /// <summary>ジャンプアップ最大瞬時速度</summary>
    [SerializeField, Header("ジャンプアップ最大瞬時速度")] private float JumpUpMaxSpeed;
    /// <summary>ジャンプダウン最大瞬時速度</summary>
    [SerializeField, Header("ジャンプダウン最大瞬時速度")] private float JumpDownMaxSpeed;
    /// <summary>ジャンプ重力</summary>
    [SerializeField, Header("ジャンプ重力")] private float JumpGravity;

    /// <summary>
    /// フレーム番号保存用
    /// </summary>
    [SerializeField, Header("フレーム番号保存用")] private int FrameNumber;

    [SerializeField, Header("ステート")] private PlayerState playerState;

    // Input System関連
    private PlayerInputActions inputActions_;
    private Vector2 lastInput_;          // 前フレームのスティック入力
    private float accumulatedRotation_;  // 累積回転量（StickRotationToFPSと同じ仕組み）
    private int rotationDirection_;      // 回転方向（1: 右回転, -1: 左回転, 0: 停止）

    [Header("スティック回転移動設定")]
    [SerializeField] private float minSpeed_ = 0.5f;          // 最小移動速度
    [SerializeField] private float maxSpeed_ = 10f;           // 最大移動速度
    [SerializeField] private float rotationToSpeedFactor_ = 0.8f; // 回転角→速度変換倍率
    [SerializeField, Range(0f, 1f)] private float decayRate_ = 0.9f; // 減衰率
    [SerializeField] private float minInputThreshold_ = 0.3f; // 最小入力閾値
    /// <summary>
    /// 右移動フラグ
    /// </summary>
    [SerializeField, Header("右移動フラグ")] private bool RightMoveFlag;

    public int Frame
    {
        get { return FrameNumber; }
        set { FrameNumber = value; }
    }

    void Awake()
    {
        inputActions_ = new PlayerInputActions();
    }

    void OnEnable()
    {
        inputActions_.Gameplay.Enable();
        
        // ジャンプ入力のコールバック設定
        inputActions_.Gameplay.Jump.performed += OnJumpPerformed;
    }

    void OnDisable()
    {
        inputActions_.Gameplay.Jump.performed -= OnJumpPerformed;
        inputActions_.Gameplay.Disable();
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        SetJump();
    }

    void Start()
    {
        // 取得
        {
            PlayerRigidbody = this.GetComponent<Rigidbody2D>();
        }

        // 初期化
        {
            PlayerHp = 10;
            JumpFlag = false;
            accumulatedRotation_ = 0f;
            rotationDirection_ = 0;
        }
    }

    void Update()
    {
        // スティック回転による移動処理
        HandleStickRotationMovement();

        // ジャンプ処理
        JumpProcess();

        // 重力無効化設定
        SetGravityDeactivation();
    }

    /// <summary>
    /// スティック回転による移動処理（StickRotationToFPSと同じロジック）
    /// </summary>
    private void HandleStickRotationMovement()
    {
        Vector2 stickInput = inputActions_.Gameplay.Move.ReadValue<Vector2>();

        // スティックの回転を計算（2つのベクトルの角度差）
        if (stickInput.magnitude > minInputThreshold_ && lastInput_.magnitude > minInputThreshold_)
        {
            float angle = Vector2.SignedAngle(lastInput_, stickInput);
            
            // 回転方向を記録（符号付きで方向を保持）
            if (Mathf.Abs(angle) > 0.1f) // 最小回転閾値
            {
                rotationDirection_ = (int)Mathf.Sign(angle);
                accumulatedRotation_ += Mathf.Abs(angle); // 絶対値で積算（StickRotationToFPSと同じ）
            }
        }
        else
        {
            // スティック入力がない場合は方向をリセット
            rotationDirection_ = 0;
            // 累積回転量も素早く減衰
            accumulatedRotation_ *= 0.8f;
        }

        lastInput_ = stickInput;

        // 回転量に応じた移動速度の計算（StickRotationToFPSのFPS計算と同じ仕組み）
        float currentSpeed = Mathf.Clamp(minSpeed_ + accumulatedRotation_ * rotationToSpeedFactor_, minSpeed_, maxSpeed_);
        
        // 方向を考慮した最終移動速度（符号を反転して右回転で右に移動するように修正）
        float finalMoveSpeed = -currentSpeed * rotationDirection_;

        // 移動処理（回転量が十分にあり、かつ現在回転している場合のみ移動）
        if (Mathf.Abs(finalMoveSpeed) > 0.01f && rotationDirection_ != 0 && accumulatedRotation_ > 1f)
        {
            playerState.SetState(PlayerState.State.Run);
            if (false)
            {
                MoveXVec(finalMoveSpeed);
            }
            else if (RightMoveFlag)
            {
                MoveRight(finalMoveSpeed);
            }
            else
            {
                MoveLeft(finalMoveSpeed);
            }
        }
        else
        {
            if (!JumpFlag) // ジャンプ中でなければIdleに
            {
                playerState.SetState(PlayerState.State.Idle);
            }
        }

        // 回転量を減衰（徐々に戻る）
        accumulatedRotation_ *= decayRate_;
        
        // 累積回転量が小さくなったら方向もリセット
        if (accumulatedRotation_ < 0.5f)
        {
            rotationDirection_ = 0;
        }
        
        // デバッグ用（StickRotationToFPSのFPS表示のように）
        Debug.Log($"回転量: {accumulatedRotation_:F1}, 速度: {currentSpeed:F1}, 方向: {rotationDirection_}");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            RightMoveFlag = !RightMoveFlag;
        }
    }

    void FixedUpdate()
    {
       
    }

    /// <summary>
    /// 移動 左
    /// </summary>
    public void MoveLeft(float moveSpeed)
    {
        MoveXVec(-Mathf.Abs(moveSpeed));
    }

    /// <summary>
    /// 移動 右
    /// </summary>
    public void MoveRight(float moveSpeed)
    {
        MoveXVec(Mathf.Abs(moveSpeed));
    }

    /// <summary>
    /// 移動 X軸
    /// </summary>
    public void MoveXVec(float moveSpeed)
    {
        if (RigidbodyMoveFlag)
        {
            PlayerRigidbody.linearVelocity = new Vector3(moveSpeed, PlayerRigidbody.linearVelocityY);
        }
        else
        {
            this.transform.position += (Vector3.right * moveSpeed * Time.deltaTime);
        }
        
        // キャラクターの向きを設定
        if (Mathf.Abs(moveSpeed) > 0.01f)
        {
            this.transform.localScale = new Vector3(Mathf.Sign(moveSpeed) * Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y, this.transform.localScale.z);
        }
    }

    /// <summary>
    /// リジッドボディ移動フラグ 設定
    /// </summary>
    public void SetRigidbodyMoveFlag(bool rigidbodyMoveFlag)
    {
        RigidbodyMoveFlag = rigidbodyMoveFlag;
    }

    /// <summary>
    /// ジャンプ速度設定
    /// </summary>
    public void SetJumpSpeed(float upSpeed, float downSpeed)
    {
        JumpUpMaxSpeed = upSpeed;
        JumpDownMaxSpeed = downSpeed;
    }

    /// <summary>
    /// ジャンプ開始
    /// </summary>
    public void SetJump()
    {
        if (JumpFlag == true)
        {
            return;
        }
        playerState.SetState(PlayerState.State.Jump);
        JumpFlag = true;
        GravityDeactivationFlag = true;
        JumpSecondSpeed = JumpUpMaxSpeed;
    }

    /// <summary>
    /// ジャンプ処理
    /// </summary>
    private void JumpProcess()
    {
        if (JumpFlag)
        {
            // ジャンプ物理演算
            {
                JumpMoveSpeed = (JumpSecondSpeed * Time.deltaTime) - (JumpGravity * Time.deltaTime);
                JumpSecondSpeed -= (JumpGravity * Time.deltaTime);

                // 下降速度上限設定
                if (JumpSecondSpeed < JumpDownMaxSpeed)
                {
                    JumpSecondSpeed = JumpDownMaxSpeed;
                }
            }

            // ジャンプ
            this.transform.position += (Vector3.up * JumpMoveSpeed);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Stage")
        {
            JumpEndCheckObject(collision.gameObject);
        }
    }

    /// <summary>
    /// 取得したオブジェクトが自分の下にある場合ジャンプを終わらせる
    /// </summary>
    public void JumpEndCheckObject(GameObject checkObject)
    {
        if ((GetObjectSize(checkObject, 1) > GetObjectSize(this.gameObject, -1)) &&
            (GetObjectSize(checkObject, -1) < GetObjectSize(this.gameObject, 1)))
        {
            JumpEnd();
        }
    }

    private float GetObjectSize(GameObject getObject, float direction)
    {
        return ((getObject.transform.lossyScale.x * 0.5f * direction) + getObject.transform.position.x);
    }

    

    /// <summary>
    /// ジャンプ終了
    /// </summary>
    public void JumpEnd()
    {
        JumpFlag = false;
        GravityDeactivationFlag = false;
        playerState.SetState(PlayerState.State.Idle);
    }

    /// <summary>
    /// 重力無効化設定
    /// </summary>
    private void SetGravityDeactivation()
    {
        if (GravityDeactivationFlag)
        {
            PlayerRigidbody.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        }
        else
        {
            PlayerRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    /// <summary>
    /// ダメージ
    /// </summary>
    public void Damage(int damage)
    {
        PlayerHp -= damage;
        HpCheck();
    }

    /// <summary>
    /// HP確認
    /// </summary>
    private void HpCheck()
    {
        if (PlayerHp <= 0)
        {
            PlayerHp = 0;
            DeathProcess();
        }
    }

    /// <summary>
    /// 死亡処理
    /// </summary>
    private void DeathProcess()
    {
        Debug.Log("[" +this.gameObject.name + "]の負け");
    }
    
    //*--------*      設定 : 取得      *--------*//

    /// <summary>プレイヤーHP設定</summary>
    public void SetPlayerHp(int hp) {PlayerHp = hp; }
    /// <summary>プレイヤーHP取得</summary>
    public int GetPlayerHp() { return PlayerHp; }

    //*--------*                       *--------*//

    void OnDestroy()
    {
        inputActions_?.Dispose();
    }
}