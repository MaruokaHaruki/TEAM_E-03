using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// プレイヤーのジャンプ機能を管理するクラス
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerJump : MonoBehaviour
{
    [Header("ジャンプ設定")]
    [SerializeField] private float jumpForce_ = 10f;        // ジャンプの力
    [SerializeField] private float jumpCooldown_ = 0.2f;    // ジャンプのクールダウン時間
    [SerializeField] private int maxJumpCount_ = 2;         // 最大ジャンプ回数（多段ジャンプ）
    
    [Header("接地判定")]
    [SerializeField] private Transform groundCheckPoint_;   // 接地判定用の位置
    [SerializeField] private float groundCheckRadius_ = 0.3f; // 接地判定の半径
    [SerializeField] private LayerMask groundLayerMask_ = 1; // 地面のレイヤーマスク
    
    [Header("デバッグ表示用")]
    [SerializeField] private TMPro.TextMeshProUGUI jumpInfoText_; // ジャンプ情報表示用テキスト
    
    // コンポーネント参照
    private Rigidbody rb_;
    private PlayerInputActions inputActions_;
    
    // ジャンプ関連の状態
    private bool isGrounded_;
    private int currentJumpCount_;
    private float lastJumpTime_;
    private bool jumpInputPressed_;
    
    void Awake()
    {
        // コンポーネント取得
        rb_ = GetComponent<Rigidbody>();
        
        // Input Actions初期化
        inputActions_ = new PlayerInputActions();
        inputActions_.Gameplay.Jump.performed += OnJumpInput;
        inputActions_.Gameplay.Jump.canceled += OnJumpInputCanceled;
    }
    
    void OnEnable()
    {
        inputActions_.Gameplay.Enable();
    }
    
    void OnDisable()
    {
        inputActions_.Gameplay.Disable();
    }
    
    void OnDestroy()
    {
        inputActions_?.Dispose();
    }
    
    void Update()
    {
        CheckGrounded();
        UpdateJumpInfo();
        
        // ジャンプ処理
        if (jumpInputPressed_ && CanJump())
        {
            PerformJump();
            jumpInputPressed_ = false;
        }
    }
    
    /// <summary>
    /// 接地判定を行う
    /// </summary>
    private void CheckGrounded()
    {
        Vector3 checkPosition = groundCheckPoint_ != null ? groundCheckPoint_.position : transform.position;
        isGrounded_ = Physics.CheckSphere(checkPosition, groundCheckRadius_, groundLayerMask_);
        
        // 接地している場合、ジャンプ回数をリセット
        if (isGrounded_)
        {
            currentJumpCount_ = 0;
        }
    }
    
    /// <summary>
    /// ジャンプが可能かどうかを判定
    /// </summary>
    private bool CanJump()
    {
        // クールダウン中はジャンプ不可
        if (Time.time - lastJumpTime_ < jumpCooldown_)
            return false;
        
        // 最大ジャンプ回数に達している場合はジャンプ不可
        if (currentJumpCount_ >= maxJumpCount_)
            return false;
        
        return true;
    }
    
    /// <summary>
    /// ジャンプを実行
    /// </summary>
    private void PerformJump()
    {
        // 上向きの速度をリセットしてからジャンプ力を適用
        Vector3 velocity = rb_.linearVelocity;
        velocity.y = 0f;
        rb_.linearVelocity = velocity;
        
        // ジャンプ力を適用
        rb_.AddForce(Vector3.up * jumpForce_, ForceMode.Impulse);
        
        // ジャンプ状態を更新
        currentJumpCount_++;
        lastJumpTime_ = Time.time;
        
        Debug.Log($"ジャンプ実行! 回数: {currentJumpCount_}/{maxJumpCount_}");
    }
    
    /// <summary>
    /// ジャンプ入力時のコールバック
    /// </summary>
    private void OnJumpInput(InputAction.CallbackContext context)
    {
        jumpInputPressed_ = true;
    }
    
    /// <summary>
    /// ジャンプ入力終了時のコールバック
    /// </summary>
    private void OnJumpInputCanceled(InputAction.CallbackContext context)
    {
        // 必要に応じて実装（可変ジャンプなど）
    }
    
    /// <summary>
    /// ジャンプ情報のUI更新
    /// </summary>
    private void UpdateJumpInfo()
    {
        if (jumpInfoText_ != null)
        {
            string groundStatus = isGrounded_ ? "接地中" : "空中";
            string jumpStatus = $"ジャンプ回数: {currentJumpCount_}/{maxJumpCount_}";
            string cooldownStatus = "";
            
            float remainingCooldown = jumpCooldown_ - (Time.time - lastJumpTime_);
            if (remainingCooldown > 0)
            {
                cooldownStatus = $"クールダウン: {remainingCooldown:F1}秒";
            }
            
            jumpInfoText_.text = $"{groundStatus}\n{jumpStatus}\n{cooldownStatus}";
        }
    }
    
    /// <summary>
    /// 強制的にジャンプ回数をリセット
    /// </summary>
    public void ResetJumpCount()
    {
        currentJumpCount_ = 0;
    }
    
    /// <summary>
    /// 現在のジャンプ回数を取得
    /// </summary>
    public int GetCurrentJumpCount()
    {
        return currentJumpCount_;
    }
    
    /// <summary>
    /// 接地状態を取得
    /// </summary>
    public bool IsGrounded()
    {
        return isGrounded_;
    }
    
    // デバッグ用：接地判定の可視化
    void OnDrawGizmosSelected()
    {
        if (groundCheckPoint_ != null)
        {
            Gizmos.color = isGrounded_ ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheckPoint_.position, groundCheckRadius_);
        }
        else
        {
            Gizmos.color = isGrounded_ ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position, groundCheckRadius_);
        }
    }
}