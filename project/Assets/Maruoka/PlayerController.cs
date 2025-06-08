using UnityEngine;

public class PlayerController : MonoBehaviour {
    // プレイヤーの移動速度とジャンプ力
    public float moveSpeed = 5.0f;
    public float jumpForce = 10.0f;

    // 地面判定用
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb_;
    private bool isGrounded_;
    private bool isFacingRight_ = true;

    private void Awake() {
        rb_ = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb_.linearVelocity = new Vector2(moveInput * moveSpeed, rb_.linearVelocity.y);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded_) {
            rb_.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        // 向き反転
        if ((moveInput > 0 && !isFacingRight_) || (moveInput < 0 && isFacingRight_)) {
            Flip();
        }

        // 地面チェック
        isGrounded_ = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
    }

    // 向きを反転
    private void Flip() {
        isFacingRight_ = !isFacingRight_;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }


    private float speedBoostMultiplier_ = 1f;
    private float jumpBoostMultiplier_ = 1f;

    public void ApplySpeedBoost(float boostMultiplier, float duration) {
        StopCoroutine(nameof(SpeedBoostCoroutine));
        StartCoroutine(SpeedBoostCoroutine(boostMultiplier, duration));
    }

    private IEnumerator SpeedBoostCoroutine(float multiplier, float duration) {
        speedBoostMultiplier_ = multiplier;
        yield return new WaitForSeconds(duration);
        speedBoostMultiplier_ = 1f;
    }

    public void ApplyJumpBoost(float boostMultiplier, float duration) {
        StopCoroutine(nameof(JumpBoostCoroutine));
        StartCoroutine(JumpBoostCoroutine(boostMultiplier, duration));
    }

    private IEnumerator JumpBoostCoroutine(float multiplier, float duration) {
        jumpBoostMultiplier_ = multiplier;
        yield return new WaitForSeconds(duration);
        jumpBoostMultiplier_ = 1f;
    }

}
