using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField, Header("移動設定")]
    public float moveSpeed;

    [SerializeField, Header("ジャンプ設定")]
    public float jumpForce;


    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius;

    [SerializeField, Header("進行方向")]
    private int direction = 1; // 1: 右, -1: 左

    [SerializeField, Header("攻撃の当たり判定")]
    private GameObject HitCircle;

    [SerializeField, Header("反動力")]
    public float recoilForce = 5f; // 反動力の強さ

    [SerializeField, Header("無効にする時間（秒）")]
    private float hitCircleDisableTime = 0.2f;
    private bool isHitCircleDisabled = false;

    [SerializeField, Header("ノックバックの距離")]
    private float knockbackDistance = 1.0f;

    [SerializeField, Header("ノックバックの高さ")]
    private float knockbackUpForce = 2.0f;
   
    [SerializeField, Header("揺らすカメラ")]
    private CameraShake cameraShake;

    private Rigidbody2D rb;
    private PlayerInputActions inputActions;
    
    // 入力状態を保持する変数
    private bool isPush;
    private bool isJumpPressed;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();


        inputActions.Player.Push.performed += ctx => isPush = true;

        inputActions.Player.Jump.performed += ctx => isJumpPressed = true;
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    private void FixedUpdate()
    {
        // 移動処理
        if (isPush)
        {
            Vector2 moveVec = new Vector2(moveSpeed * direction, 0);
            rb.AddForce(moveVec);
            isPush = false;

        }

        // ジャンプ処理
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isJumpPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        isJumpPressed = false;


    }
    /// /デバッグ用
    //private void OnDrawGizmosSelected()
    //{
    //    if (groundCheck != null)
    //    {
    //        Gizmos.color = Color.green;
    //        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    //    }
    //}

    private void UpdateHitCirclePosition()
    {
        // 進行方向と逆方向にHitCircleを配置
        float offsetX = Mathf.Abs(HitCircle.transform.localPosition.x); 
        HitCircle.transform.localPosition = new Vector3(-direction * offsetX, HitCircle.transform.localPosition.y, HitCircle.transform.localPosition.z);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Wall") || col.gameObject.CompareTag("Player"))
        {
            // 方向反転
            direction *= -1;

            // HitCircleの位置反転
            UpdateHitCirclePosition();

            // ノックバック処理（プレイヤーとのみ）
            if (col.gameObject.CompareTag("Player"))
            {
                Knockback();
            }

            // カメラ振動処理
            if (cameraShake != null)
            {
                cameraShake.Shake(0.15f, 0.1f); // 秒、強さ
            }

            // 無敵時間処理
            if (!isHitCircleDisabled)
            {
                StartCoroutine(TemporarilyDisableHitCircle());
            }
        }
    }

    // HitCircleを一時的に無効化するコルーチン
    private System.Collections.IEnumerator TemporarilyDisableHitCircle()
    {
        isHitCircleDisabled = true;

        var collider = HitCircle.GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
            yield return new WaitForSeconds(hitCircleDisableTime);
            collider.enabled = true;
        }

        isHitCircleDisabled = false;
    }

    private void Knockback()
    {
        Vector2 knockbackDirection = new Vector2(direction, 1).normalized;
        Vector2 destination = rb.position + knockbackDirection * knockbackDistance;

        // 地形をRaycastで確認して壁がないときだけ移動
        RaycastHit2D hit = Physics2D.Raycast(rb.position, knockbackDirection, knockbackDistance, groundLayer);
        if (!hit.collider)
        {
            // ノックバック方向に直接移動
            rb.MovePosition(destination);
        }
        else
        {
            // 移動できないので、上方向だけに反動をかける
            rb.AddForce(new Vector2(0, knockbackUpForce), ForceMode2D.Impulse);
        }
    }

}