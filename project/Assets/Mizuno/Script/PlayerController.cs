using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{


    public enum PlayerNumber { PlayerA, PlayerB }

    [SerializeField, Header("プレイヤー番号")]
    private PlayerNumber playerNumber;

    [SerializeField, Header("移動設定")]
    public float moveSpeed;

    [SerializeField, Header("ジャンプ設定")]
    public float jumpForce;


    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius;

    [SerializeField, Header("進行方向")]
    private int direction = 1; // 1: 右, -1: 左

    [SerializeField, Header("方向反転のクールダウン時間")]
    private float flipCooldownTime = 0.3f;
    private float lastFlipTime = -999f;

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
    private PlayerABInputActions inputActions;

    // 入力状態を保持する変数
    private bool isPush;
    private bool isJumpPressed;
    private bool isGrounded;

    [SerializeField, Header("牛のスプライト")]
    GameObject sprite;

    //振動の強さ
    public float vibrationStrength = 0.1f;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputActions = new PlayerABInputActions();
    }

    private void OnEnable()
    {
        inputActions.Enable();


        switch (playerNumber)
        {
            case PlayerNumber.PlayerA:
                inputActions.PlayerA.Enable();
                inputActions.PlayerA.Push.performed += ctx => isPush = true;
                inputActions.PlayerA.Jump.performed += ctx => isJumpPressed = true;
                break;

            case PlayerNumber.PlayerB:
                inputActions.PlayerB.Enable();
                inputActions.PlayerB.Push.performed += ctx => isPush = true;
                inputActions.PlayerB.Jump.performed += ctx => isJumpPressed = true;
                break;
        }
    }

    private void OnDisable()
    {
        inputActions.Disable();
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


    private void OnCollisionEnter2D(Collision2D col)
    {
        //Debug.Log($"{gameObject.name} hit {col.gameObject.name}");
        if (col.gameObject.CompareTag("Wall") || col.gameObject.CompareTag("Player"))
        {
            

            // 方向反転
            direction *= -1;


            if (Time.time - lastFlipTime < flipCooldownTime) return; // クールダウン中は何もしない
            lastFlipTime = Time.time;

            // 進行方向のスプライトを反転
            Vector3 localScale = sprite.transform.localScale;
            localScale.x = Mathf.Abs(localScale.x) * direction;
            sprite.transform.localScale = localScale;

            // ノックバック処理（プレイヤーとのみ）
            if (col.gameObject.CompareTag("Player"))
            {
                Knockback();
            }
            // カメラ振動処理
            if (cameraShake != null)
            {

                cameraShake.Shake(0.2f, 0.3f); // 秒、強さ
                StartCoroutine(Vib());

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

    private System.Collections.IEnumerator Vib()
    {
        if (Gamepad.current != null)
        {
            float[] strengths = { 0.8f, 0.5f, 0.3f, 0.1f };
            float[] durations = { 0.02f, 0.02f, 0.02f, 0.02f };

            for (int i = 0; i < strengths.Length; i++)
            {
                Gamepad.current.SetMotorSpeeds(strengths[i], strengths[i]);
                yield return new WaitForSeconds(durations[i]);
            }

            Gamepad.current.SetMotorSpeeds(0, 0);
        }
    }
}