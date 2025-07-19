using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

[System.Obsolete]
[RequireComponent(typeof(Rigidbody2D))]

public class Player : MonoBehaviour {   
    // プレイヤー設定
    [Header("プレイヤー設定")]
    public string playerID_ = "A";
    public int maxHp_=3;
    public int currentHp_=3;
    public int atk_=1;
    // 移動設定
    [Header("移動設定")]
    public float maxSpeed_ = 5.0f;
    public float acceleration_ = 15.0f;
    public float dragOnStop_ = 5.0f;
    public float dragOnMove_ = 0.0f;

    // ジャンプ・重力設定
    [Header("ジャンプ・重力設定")]
    public float gravity_ = 0.2f;
    public float jumpForce_ = 4.0f;

    // 判定システム
    [Header("判定システム")]
    public GroundCheck groundCheck_;
    public WallCheckFront wallCheckFront_;
    public WallCheckBuck wallCheckBuck_;

    // 自動移動設定
    [Header("自動移動設定")]
    public bool isAutoMode_ = false;
    public float autoMoveSpeed_ = 3.0f;
    public float speedBoostMultiplier_ = 1.5f;
    public float speedBoostDuration_ = 0.5f;

    // 連打ゲージ設定
    [Header("連打ゲージ設定")]
    public float maxComboGauge_ = 100.0f;
    public float comboGaugePerHit_ = 8.0f;
    public float gaugeDrainRate_ = 20.0f;
    public float maxGaugeSpeedMultiplier_ = 5.0f;
    public float minEffectiveGauge_ = 10.0f;
    
    // 連打ゲージ減り速度加速設定
    [Header("連打ゲージ減り速度加速設定")]
    public bool enableGaugeDrainBoost_ = true;
    public float gaugeDrainBoostMultiplier_ = 2.0f;

    // 特殊状態設定
    [Header("無敵状態設定")]
    public float invincibilityDuration_ = 3.0f;
    public float invincibilityColorSpeed_ = 3.0f;  // rainbowSpeed_から名前変更
    public float invincibilitySpeedMultiplier_ = 2.0f;
    public float invincibilityItemTiem = 3.0F;

    // ダメージエフェクト設定
    [Header("ダメージエフェクト設定")]
    public float damageFlashDuration_ = 1.0f;
    public float damageFlashInterval_ = 0.1f;
    public Color damageFlashColor_ = Color.red;

    // スプライト回転設定
    [Header("スプライト回転設定")]
    public bool enableSpriteRotation_ = true;
    public float maxRotationAngle_ = 45.0f;
    public float rotationSensitivity_ = 1.0f;
    public float rotationSmoothness_ = 5.0f;

    // 機能設定
    [Header("機能設定")]
    public bool enableSpeedTransfer_ = true;
    public bool isKeySettingMode_ = false;
    public bool allowMovement_ = false;

    // キー設定
    [Header("キー設定")]
    public KeyCode moveLeftKey_ = KeyCode.A;
    public KeyCode moveRightKey_ = KeyCode.D;
    public KeyCode jumpKey_ = KeyCode.P;

    // 特別ルール設定
    [Header("特別ルール設定")]
    public bool enableDoubleJump_ = false;
    public bool enableStomp_ = false;
    public bool enableReverseJump_ = false;
    public float stompStunDuration_ = 2.0f;
    
    // ノックバック設定
    public float knockBackDuration_ = 0.4f;

    // ダミープレイヤー設定
    [Header("ダミープレイヤー設定")]
    public bool isDummyPlayer_ = false;
    public float dummyMoveInterval_ = 2.0f;
    public float dummyJumpInterval_ = 3.0f;
    public bool dummyAlwaysInvincible_ = false; // デフォルトをfalseに変更
    public bool dummyCanMove_ = false;
    [Header("ダミープレイヤー初期方向設定")]
    [Tooltip("1.0f = 右向き, -1.0f = 左向き")]
    public float dummyInitialDirection_ = 1.0f;

    // コンポーネント参照
    private Animator animator_ = null;
    private Rigidbody2D rigidbody2D_ = null;
    private SpriteRenderer spriteRenderer_ = null;
    private Transform spriteTransform_ = null;

    // 状態フラグ
    public bool isGround_ = false;
    public bool isJumping_ = false;

    // 入力データ
    private Vector2 inputHorizontal_ = Vector2.zero;

    // 壁接触状態
    private bool isHitWallFront_ = false;
    private bool isHitWallBuck_ = false;

    // 自動移動関連
    private float currentDirection_ = 1.0f;
    private bool wasHittingWall_ = false;
    private float speedBoostTimer_ = 0.0f;
    private bool isSpeedBoosted_ = false;

    // 連打ゲージ関連
    private float currentComboGauge_ = 0.0f;
    private bool isGaugeDrainBoosted_ = false;

    // 無敵状態関連
    private bool isInvincible_ = false;
    private float invincibilityTimer_ = 0.0f;
    private Color originalColor_ = Color.white;

    // ダメージエフェクト関連
    private bool isDamageFlashing_ = false;
    private float damageFlashTimer_ = 0.0f;
    private float damageFlashIntervalTimer_ = 0.0f;
    private bool isFlashRed_ = false;

    // キー設定関連
    private string settingTarget_ = "";

    // 特別ルール関連
    public bool hasDoubleJumped_ = false;
    public bool isStunned_ = false;
    public float stunTimer_ = 0.0f;
    public bool shouldReverseOnLanding_ = false;
    
    // ノックバック状態関連
    private bool isKnockedBack_ = false;
    private float knockBackTimer_ = 0.0f;

    // スプライト回転関連
    private float targetRotation_ = 0.0f;
    private float currentRotation_ = 0.0f;

    // ダミープレイヤー関連
    private float dummyMoveTimer_ = 0.0f;
    private float dummyJumpTimer_ = 0.0f;
    private bool dummyMovingRight_ = true;

    private CameraShake cameraShake_;

    private void Start() {
        // ダミープレイヤーの判定
        if (playerID_.ToUpper() == "D") {
            isDummyPlayer_ = true;
            isAutoMode_ = true;
            allowMovement_ = dummyCanMove_;
            
            // ダミープレイヤーの初期方向を設定
            currentDirection_ = Mathf.Sign(dummyInitialDirection_);
            if (currentDirection_ == 0.0f) currentDirection_ = 1.0f; // 0の場合は右向きに
            dummyMovingRight_ = currentDirection_ > 0;
            
            // 初期スプライトの向きを設定
            if (currentDirection_ > 0) {
                transform.localScale = new Vector3(-1, 1, 1);
            } else {
                transform.localScale = new Vector3(1, 1, 1);
            }
            
            // ダミープレイヤーが常に無敵の場合のみ無敵状態を設定
            if (dummyAlwaysInvincible_) {
                isInvincible_ = true;
                invincibilityTimer_ = float.MaxValue; // 無限に近い値
            }
        }
        
        cameraShake_ = FindObjectOfType<CameraShake>();

        Transform spriteChild = transform.Find("Sprite");
        if (spriteChild != null) {
            spriteTransform_ = spriteChild;
            animator_ = spriteChild.GetComponent<Animator>();
            spriteRenderer_ = spriteChild.GetComponent<SpriteRenderer>();
            
            if (spriteRenderer_ != null) {
                originalColor_ = spriteRenderer_.color;
            }
        }

        rigidbody2D_ = GetComponent<Rigidbody2D>();
        SetDefaultKeys();
    }

    private void Update() {
        // ダミープレイヤーの場合はGameManagerの状態チェックをスキップ
        if (!isDummyPlayer_) {
            UpdateMovementPermission();
        } else {
            allowMovement_ = dummyCanMove_;
            
            // ダミープレイヤーの無敵状態を維持
            if (dummyAlwaysInvincible_) {
                isInvincible_ = true;
                invincibilityTimer_ = float.MaxValue;
            }
        }

        if (isStunned_) {
            stunTimer_ -= Time.deltaTime;
            if (stunTimer_ <= 0.0f) {
                isStunned_ = false;
            }
        }

        if (isKnockedBack_) {
            knockBackTimer_ -= Time.deltaTime;
            if (knockBackTimer_ <= 0f) {
                isKnockedBack_ = false;
            }
        }

        // ダメージ点滅エフェクトの処理
        if (isDamageFlashing_) {
            damageFlashTimer_ -= Time.deltaTime;
            damageFlashIntervalTimer_ -= Time.deltaTime;

            if (damageFlashIntervalTimer_ <= 0.0f) {
                isFlashRed_ = !isFlashRed_;
                damageFlashIntervalTimer_ = damageFlashInterval_;
                UpdateDamageFlashEffect();
            }

            if (damageFlashTimer_ <= 0.0f) {
                isDamageFlashing_ = false;
                isFlashRed_ = false;
                if (spriteRenderer_ != null && !isInvincible_) {
                    spriteRenderer_.color = originalColor_;
                }
            }
        }

        if (isInvincible_) {
            // ダミープレイヤーの場合は無敵タイマーを減らさない
            if (!isDummyPlayer_ || !dummyAlwaysInvincible_) {
                invincibilityTimer_ -= Time.deltaTime;
            }
            
            // 無敵状態の場合はレインボーエフェクトを優先
            if (!isDamageFlashing_) {
                UpdateRainbowEffect();
            }
            
            if (invincibilityTimer_ <= 0.0f && (!isDummyPlayer_ || !dummyAlwaysInvincible_)) {
                isInvincible_ = false;
                if (spriteRenderer_ != null && !isDamageFlashing_) {
                    spriteRenderer_.color = originalColor_;
                }
            }
        }

        // モード切り替え処理
        if (Input.GetKeyDown(KeyCode.Tab)) {
            isAutoMode_ = !isAutoMode_;
        }

        if (Input.GetKeyDown(KeyCode.F3)) {
            enableSpeedTransfer_ = !enableSpeedTransfer_;
        }

        if (Input.GetKeyDown(KeyCode.Alpha6)) {
            Player[] allPlayers = FindObjectsOfType<Player>();
            foreach (Player player in allPlayers) {
                player.currentComboGauge_ = 0.0f;
                player.isSpeedBoosted_ = false;
            }
        }
        
        // キー設定モードの処理
        if (Input.GetKeyDown(KeyCode.F1) && playerID_ == "A") {
            ToggleKeySettingMode();
        }
        else if (Input.GetKeyDown(KeyCode.F2) && playerID_ == "B") {
            ToggleKeySettingMode();
        }

        if (isKeySettingMode_) {
            HandleKeySettingInput();
            return;
        }

        // 移動処理
        if (allowMovement_) {
            if (isDummyPlayer_) {
                if (dummyCanMove_) {
                    DummyMove();
                } else {
                    // 行動しない場合は入力をゼロにしてアニメーション停止
                    inputHorizontal_ = Vector2.zero;
                    isJumping_ = false;
                    
                    if (animator_ != null) {
                        animator_.SetBool("Run", false);
                        animator_.SetBool("Jump", !isGround_);
                    }
                }
            } else if (isAutoMode_) {
                AutoMove();
            }
            else {
                Move();
            }
        }
        else {
            inputHorizontal_ = Vector2.zero;
            isJumping_ = false;
            
            if (animator_ != null) {
                animator_.SetBool("Run", false);
                animator_.SetBool("Jump", !isGround_);
            }
        }

        if (enableSpriteRotation_) {
            UpdateSpriteRotation();
        }

        // Gキーによる連打ゲージ減り速度加速
        if (enableGaugeDrainBoost_ && Input.GetKey(KeyCode.G)) {
            isGaugeDrainBoosted_ = true;
        } else {
            isGaugeDrainBoosted_ = false;
        }

        // 連打ゲージの自然減少
        if (currentComboGauge_ > 0.0f) {
            float drainRate = gaugeDrainRate_;
            
            // Gキーが押されている場合は減り速度を加速
            if (isGaugeDrainBoosted_) {
                drainRate *= gaugeDrainBoostMultiplier_;
            }
            
            currentComboGauge_ -= drainRate * Time.deltaTime;
            currentComboGauge_ = Mathf.Max(0.0f, currentComboGauge_);
        }
    }

    private void FixedUpdate() {
        bool wasGrounded = isGround_;
        isGround_ = groundCheck_.IsGround();
        
        bool wasHittingWallFront = isHitWallFront_;
        isHitWallFront_ = wallCheckFront_.IsHitWallFront();
        isHitWallBuck_ = wallCheckBuck_.IsHitWallBuck();
        // 着地音の再生と震え効果
        if (!wasGrounded && isGround_) {
            if (AudioManager.Instance != null) {
                AudioManager.Instance.PlaySE("Player_Landing");
            }
            // 着地時の震え効果
            if (spriteTransform_ != null) {
                spriteTransform_.DOShakeScale(0.2f, 1.0f, 5).OnComplete(() => {
                    spriteTransform_.localScale = Vector3.one;
                });
            }
        }

        // ジャンプ中の壁衝突判定
        if (!isGround_ && isHitWallFront_ && !wasHittingWallFront) {
            // ジャンプ中の壁衝突時の震え効果
            if (spriteTransform_ != null) {
                spriteTransform_.DOShakeScale(0.2f, 1.0f, 5).OnComplete(() => {
                    spriteTransform_.localScale = Vector3.one;
                });
            }
        }

        // 反転ジャンプの処理
        if (enableReverseJump_ && !wasGrounded && isGround_ && shouldReverseOnLanding_) {
            ReverseDirection();
            shouldReverseOnLanding_ = false;
        }

        if (isGround_ && !wasGrounded) {
            hasDoubleJumped_ = false;
        }

        // ノックバック中は物理的な移動処理のみスキップ
        if (isKnockedBack_) {
            // 独自重力システムは継続
            rigidbody2D_.AddForce(Vector2.down * gravity_, ForceMode2D.Force);
            return;
        }

        // 水平移動の物理計算
        float currentVelocityX = rigidbody2D_.velocity.x;
        float targetSpeed = inputHorizontal_.x * maxSpeed_;
        float speedDiff = targetSpeed - currentVelocityX;
        float movement = speedDiff * acceleration_;

        // 壁張り付き防止
        bool blockFront = isHitWallFront_ && inputHorizontal_.x < 0;
        bool blockBuck = isHitWallFront_ && inputHorizontal_.x > 0;

        if (!blockFront && !blockBuck) {
            rigidbody2D_.AddForce(Vector2.right * movement, ForceMode2D.Force);
        }

        // 動的抵抗制御
        if (Mathf.Abs(inputHorizontal_.x) < 0.01f) {
            rigidbody2D_.drag = dragOnStop_;
        }
        else {
            rigidbody2D_.drag = dragOnMove_;
        }

        // ジャンプ力の適用
        if (isJumping_) {
            if (AudioManager.Instance != null) {
                AudioManager.Instance.PlaySE("Player_Jump");
            }
            
            if (hasDoubleJumped_ && !isGround_) {
                rigidbody2D_.velocity = new Vector2(rigidbody2D_.velocity.x, 0f);
            }
            
            rigidbody2D_.AddForce(Vector2.up * jumpForce_, ForceMode2D.Impulse);
            isJumping_ = false;
        }

        // 独自重力システム
        rigidbody2D_.AddForce(Vector2.down * gravity_, ForceMode2D.Force);
    }

    private void UpdateSpriteRotation() {
        if (spriteTransform_ == null || rigidbody2D_ == null) return;

        Vector2 velocity = rigidbody2D_.velocity;

        if (velocity.magnitude > 0.5f) {
            float velocityAngle = Mathf.Atan2(velocity.y, Mathf.Abs(velocity.x)) * Mathf.Rad2Deg;
            velocityAngle = Mathf.Clamp(velocityAngle, -maxRotationAngle_, maxRotationAngle_);
            targetRotation_ = velocityAngle * rotationSensitivity_;
        }
        else {
            targetRotation_ = 0.0f;
        }

        currentRotation_ = Mathf.Lerp(currentRotation_, targetRotation_, rotationSmoothness_ * Time.deltaTime);
        spriteTransform_.localRotation = Quaternion.Euler(0, 0, -currentRotation_);
    }

    private void Move() {
        if (GameManager.Instance != null && GameManager.Instance.GetGameState() != GameManager.GameState.Playing) {
            inputHorizontal_ = Vector2.zero;
            isJumping_ = false;
            return;
        }

        if (!allowMovement_) {
            inputHorizontal_ = Vector2.zero;
            isJumping_ = false;
            return;
        }

        float horizontal = 0.0f;
        if (Input.GetKey(moveRightKey_)) {
            horizontal += 1.0f;
        }
        if (Input.GetKey(moveLeftKey_)) {
            horizontal -= 1.0f;
        }
        
        bool jumpPressed = Input.GetKeyDown(jumpKey_);

        inputHorizontal_ = new Vector2(horizontal, 0.0f);

        if (!isStunned_) {
            if (isGround_) {
                if (jumpPressed) {
                    isJumping_ = true;
                    
                    if (enableReverseJump_) {
                        shouldReverseOnLanding_ = true;
                    }
                }
            }
            else {
                if (enableDoubleJump_ && jumpPressed && !hasDoubleJumped_) {
                    isJumping_ = true;
                    hasDoubleJumped_ = true;
                }
            }
        }
        else {
            isJumping_ = false;
        }

        animator_.SetBool("Jump", !isGround_);

        if (horizontal > 0) {
            transform.localScale = new Vector3(-1, 1, 1);
            animator_.SetBool("Run", true);
        }
        else if (horizontal < 0) {
            transform.localScale = new Vector3(1, 1, 1);
            animator_.SetBool("Run", true);
        }
        else {
            animator_.SetBool("Run", false);
        }
    }

    private void AutoMove() {
        // ダミープレイヤーの場合はGameManagerの状態チェックをスキップ
        if (!isDummyPlayer_ && GameManager.Instance != null && GameManager.Instance.GetGameState() != GameManager.GameState.Playing) {
            inputHorizontal_ = Vector2.zero;
            isJumping_ = false;
            return;
        }

        if (!allowMovement_) {
            inputHorizontal_ = Vector2.zero;
            isJumping_ = false;
            return;
        }

        // 壁衝突判定と方向転換
        if (isHitWallFront_ && !wasHittingWall_) {
            currentDirection_ *= -1.0f;
            
            if (AudioManager.Instance != null) {
                AudioManager.Instance.PlaySE("Player_Penguin2Wall");
            }
            
            // 壁衝突時の震え効果
            if (spriteTransform_ != null) {
                spriteTransform_.DOShakeScale(0.2f, 1.0f, 5).OnComplete(() => {
                    spriteTransform_.localScale = Vector3.one;
                });
            }
            
            isInvincible_ = true;
            invincibilityTimer_ = invincibilityDuration_;
        }
        wasHittingWall_ = isHitWallFront_;

        // 連打によるゲージ蓄積処理
        bool moveInputPressed = Input.GetKeyDown(moveLeftKey_) ||
                               Input.GetKeyDown(moveRightKey_);

        if (moveInputPressed) {
            if (AudioManager.Instance != null) {
                AudioManager.Instance.PlaySE("Player_Barrage");
            }
            
            currentComboGauge_ += comboGaugePerHit_;
            currentComboGauge_ = Mathf.Min(maxComboGauge_, currentComboGauge_);
        }

        if (isSpeedBoosted_) {
            speedBoostTimer_ -= Time.deltaTime;
            if (speedBoostTimer_ <= 0.0f) {
                isSpeedBoosted_ = false;
            }
        }

        // ジャンプ入力の処理
        bool jumpPressed = Input.GetKeyDown(jumpKey_);
        if (!isStunned_) {
            if (isGround_ && jumpPressed) {
                isJumping_ = true;
                
                if (enableReverseJump_) {
                    shouldReverseOnLanding_ = true;
                }
            }
            else if (!isGround_ && enableDoubleJump_ && jumpPressed && !hasDoubleJumped_) {
                isJumping_ = true;
                hasDoubleJumped_ = true;
            }
        }
        else {
            isJumping_ = false;
        }

        // 自動移動の実行
        float currentSpeed = autoMoveSpeed_;
        
        if (isSpeedBoosted_) {
            currentSpeed *= speedBoostMultiplier_;
        }
        else {
            float gaugeSpeedMultiplier = GetGaugeSpeedMultiplier();
            currentSpeed *= gaugeSpeedMultiplier;
        }

        inputHorizontal_ = new Vector2(currentDirection_ * currentSpeed / maxSpeed_, 0.0f);

        animator_.SetBool("Jump", !isGround_);
        animator_.SetBool("Run", true);

        if (currentDirection_ > 0) {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    /// <summary>
    /// ダミープレイヤー専用の移動処理
    /// </summary>
    private void DummyMove() {
        // 行動しない設定の場合は何もしない
        if (!dummyCanMove_) {
            inputHorizontal_ = Vector2.zero;
            isJumping_ = false;
            return;
        }

        if (!allowMovement_) {
            inputHorizontal_ = Vector2.zero;
            isJumping_ = false;
            return;
        }

        // タイマー更新
        dummyMoveTimer_ += Time.deltaTime;
        dummyJumpTimer_ += Time.deltaTime;

        // 移動方向変更
        if (dummyMoveTimer_ >= dummyMoveInterval_) {
            dummyMovingRight_ = !dummyMovingRight_;
            dummyMoveTimer_ = 0.0f;
            currentDirection_ = dummyMovingRight_ ? 1.0f : -1.0f;
        }

        // 壁衝突時の方向転換
        if (isHitWallFront_ && !wasHittingWall_) {
            currentDirection_ *= -1.0f;
            dummyMovingRight_ = currentDirection_ > 0;
            dummyMoveTimer_ = 0.0f;
        }
        wasHittingWall_ = isHitWallFront_;

        // ジャンプ処理
        if (dummyJumpTimer_ >= dummyJumpInterval_ && isGround_) {
            isJumping_ = true;
            dummyJumpTimer_ = 0.0f;
        }

        // 自動移動の実行
        float currentSpeed = autoMoveSpeed_;
        inputHorizontal_ = new Vector2(currentDirection_ * currentSpeed / maxSpeed_, 0.0f);

        // アニメーション更新
        if (animator_ != null) {
            animator_.SetBool("Jump", !isGround_);
            animator_.SetBool("Run", true);
        }

        // スプライトの向き更新
        if (currentDirection_ > 0) {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        // ダミープレイヤーの場合はGameManagerの状態チェックをスキップ
        if (!isDummyPlayer_ && GameManager.Instance != null && GameManager.Instance.GetGameState() != GameManager.GameState.Playing) {
            return;
        }

        if (!allowMovement_) {
            return;
        }
        
        if (collision.gameObject.CompareTag("Damage"))
        {
            TakeDamage(atk_);
            ReverseDirection();
        }

        // NOTE:修正予定
        if (collision.gameObject.CompareTag("Punching"))
        {
            var push = collision.gameObject.GetComponentInParent<Push>();
            float punchPower = (push != null) ? push.CurrentPunchPower : 0f;

            // 向きをプレイヤー同士の相対位置で決める
            Vector2 punchOrigin = collision.transform.position;
            Vector2 myPosition = transform.position;
            
            Vector2 punchDir = (myPosition - punchOrigin).normalized; // パンチ元 → 自分へのベクトル（≒攻撃方向）
            Vector2 knockDir = (punchDir + Vector2.up * 0.5f).normalized; // 上方向を加えて「斜め後ろ上」に

            rigidbody2D_.velocity = Vector2.zero; // 現在の速度をリセット
            rigidbody2D_.AddForce(knockDir * punchPower, ForceMode2D.Impulse);

            // ノックバック状態にする
            isKnockedBack_ = true;
            knockBackTimer_ = knockBackDuration_;
            return;
        }

        // 踴みつけ判定
        if (enableStomp_ && collision.gameObject.CompareTag("Player"))
        {
            Player otherPlayer = collision.gameObject.GetComponent<Player>();
            if (otherPlayer == null || otherPlayer == this) return;

            bool isAbove = transform.position.y > otherPlayer.transform.position.y + 0.5f;
            bool isMovingDown = rigidbody2D_.velocity.y < -1.0f;

            if (isAbove && isMovingDown && !otherPlayer.isStunned_)
            {
                otherPlayer.ApplyStun(stompStunDuration_);
                rigidbody2D_.velocity = new Vector2(rigidbody2D_.velocity.x, jumpForce_ * 0.7f);

                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySE("Player_Step");
                }

                return;
            }
        }

        if (collision.gameObject.CompareTag("Player")) {
            Player otherPlayer = collision.gameObject.GetComponent<Player>();
            if (otherPlayer == null || otherPlayer == this) return;

            // プレイヤー衝突時の震え効果
            if (spriteTransform_ != null) {
                spriteTransform_.DOShakeScale(0.2f, 1.0f, 5).OnComplete(() => {
                    spriteTransform_.localScale = Vector3.one;
                });
            }
            if (otherPlayer.spriteTransform_ != null) {
                otherPlayer.spriteTransform_.DOShakeScale(0.2f, 1.0f, 5).OnComplete(() => {
                    otherPlayer.spriteTransform_.localScale = Vector3.one;
                });
            }

            if (gameObject.GetInstanceID() < otherPlayer.gameObject.GetInstanceID()) {
                return;
            }

            // 速度交換ロジック
            if (isAutoMode_ && otherPlayer.isAutoMode_ && enableSpeedTransfer_ && otherPlayer.enableSpeedTransfer_) {
                float mySpeed = GetCurrentEffectiveSpeed();
                float otherSpeed = otherPlayer.GetCurrentEffectiveSpeed();

                if (Mathf.Abs(mySpeed - otherSpeed) > 0.5f) {
                    float tempMySpeed = mySpeed;
                    float tempOtherSpeed = otherSpeed;

                    AdjustGaugeToAchieveSpeed(tempOtherSpeed);
                    otherPlayer.AdjustGaugeToAchieveSpeed(tempMySpeed);
                }
            }

            bool thisIsInvincible = isInvincible_;
            bool otherIsInvincible = otherPlayer.isInvincible_;

            float relativeXToOther = otherPlayer.transform.position.x - transform.position.x;
            float myDirSign = Mathf.Sign(currentDirection_);
            bool otherIsAheadOfMe = Mathf.Sign(relativeXToOther) == myDirSign;

            float relativeXToMe = transform.position.x - otherPlayer.transform.position.x;
            float otherDirSign = Mathf.Sign(otherPlayer.currentDirection_);
            bool amIAheadOfOther = Mathf.Sign(relativeXToMe) == otherDirSign;

            // 後ろから当たった場合の判定
            bool iHitOtherFromBehind = !otherIsAheadOfMe && !amIAheadOfOther && 
                                     Mathf.Sign(relativeXToOther) != myDirSign;
            bool otherHitMeFromBehind = !otherIsAheadOfMe && !amIAheadOfOther && 
                                       Mathf.Sign(relativeXToMe) != otherDirSign;

            if (otherIsAheadOfMe && amIAheadOfOther) {
                if (thisIsInvincible && otherIsInvincible) {
                    if (AudioManager.Instance != null) {
                        AudioManager.Instance.PlaySE("Player_Penguin2Penguin");
                    }

                    ReverseDirection();
                    otherPlayer.ReverseDirection();

                    Vector2 knockBackDirToMe = (transform.position - otherPlayer.transform.position).normalized;
                    if (knockBackDirToMe == Vector2.zero) knockBackDirToMe = (Random.insideUnitCircle).normalized;
                    rigidbody2D_.AddForce(knockBackDirToMe * 5f, ForceMode2D.Impulse);

                    Vector2 knockBackDirToOther = (otherPlayer.transform.position - transform.position).normalized;
                    if (knockBackDirToOther == Vector2.zero) knockBackDirToOther = (Random.insideUnitCircle).normalized;
                    otherPlayer.rigidbody2D_.AddForce(knockBackDirToOther * 5f, ForceMode2D.Impulse);
                }
                else if (thisIsInvincible && !otherIsInvincible) {
                    if (AudioManager.Instance != null) {
                        AudioManager.Instance.PlaySE("Player_Penguin2Penguin");
                    }

                    otherPlayer.TakeDamage(atk_);
                    KnockBack(otherPlayer);
                    ReverseDirection();
                    otherPlayer.ReverseDirection();
                }
                else if (!thisIsInvincible && otherIsInvincible) {
                    if (AudioManager.Instance != null) {
                        AudioManager.Instance.PlaySE("Player_Penguin2Penguin");
                    }

                    TakeDamage(atk_);
                    Vector2 knockBackDirToThis = (transform.position - otherPlayer.transform.position).normalized;
                    if (knockBackDirToThis == Vector2.zero) knockBackDirToThis = (Random.insideUnitCircle).normalized;
                    rigidbody2D_.AddForce(knockBackDirToThis * 10f, ForceMode2D.Impulse);
                    ReverseDirection();
                    otherPlayer.ReverseDirection();
                }
                else {
                    if (AudioManager.Instance != null) {
                        AudioManager.Instance.PlaySE("Player_Penguin2Penguin");
                    }

                    ReverseDirection();
                    otherPlayer.ReverseDirection();

                    Vector2 knockBackDirToMe = (transform.position - otherPlayer.transform.position).normalized;
                    if (knockBackDirToMe == Vector2.zero) knockBackDirToMe = (Random.insideUnitCircle).normalized;
                    rigidbody2D_.AddForce(knockBackDirToMe * 5f, ForceMode2D.Impulse);

                    Vector2 knockBackDirToOther = (otherPlayer.transform.position - transform.position).normalized;
                    if (knockBackDirToOther == Vector2.zero) knockBackDirToOther = (Random.insideUnitCircle).normalized;
                    otherPlayer.rigidbody2D_.AddForce(knockBackDirToOther * 5f, ForceMode2D.Impulse);
                }
            }
            else if (otherIsAheadOfMe && !amIAheadOfOther) {
                if (!otherIsInvincible) {
                    otherPlayer.TakeDamage(atk_);
                    KnockBack(otherPlayer);
                }
                ReverseDirection();
            }
            else if (!otherIsAheadOfMe && amIAheadOfOther) {
                if (!thisIsInvincible) {
                    TakeDamage(atk_);
                    Vector2 knockBackDirToThis = (transform.position - otherPlayer.transform.position).normalized;
                    if (knockBackDirToThis == Vector2.zero) knockBackDirToThis = (Random.insideUnitCircle).normalized;
                    rigidbody2D_.AddForce(knockBackDirToThis * 10f, ForceMode2D.Impulse);
                }
                ReverseDirection();
            }
            else if (iHitOtherFromBehind || otherHitMeFromBehind) {
                // 後ろから当たった場合は両方反転
                if (AudioManager.Instance != null) {
                    AudioManager.Instance.PlaySE("Player_Penguin2Penguin");
                }

                ReverseDirection();
                otherPlayer.ReverseDirection();

                // 軽いノックバック
                Vector2 knockBackDirToMe = (transform.position - otherPlayer.transform.position).normalized;
                if (knockBackDirToMe == Vector2.zero) knockBackDirToMe = (Random.insideUnitCircle).normalized;
                rigidbody2D_.AddForce(knockBackDirToMe * 3f, ForceMode2D.Impulse);

                Vector2 knockBackDirToOther = (otherPlayer.transform.position - transform.position).normalized;
                if (knockBackDirToOther == Vector2.zero) knockBackDirToOther = (Random.insideUnitCircle).normalized;
                otherPlayer.rigidbody2D_.AddForce(knockBackDirToOther * 3f, ForceMode2D.Impulse);
            }
        }

        if (collision.gameObject.CompareTag("InvincibleItem")) {
            if (AudioManager.Instance != null) {
                AudioManager.Instance.PlaySE("collision");
            }

            isInvincible_ = true;
            invincibilityTimer_ = invincibilityItemTiem;
            Destroy(collision.gameObject);
        }
    }

    public void TakeDamage(int amount) {
        // ダミープレイヤーの場合
        if (isDummyPlayer_) {
            // 常時無敵設定がtrueの場合のみダメージを無効化
            if (dummyAlwaysInvincible_) {
                return;
            }
            
            // ダミープレイヤーはダメージを受けたら即座に消える
            if (AudioManager.Instance != null) {
                AudioManager.Instance.PlaySE("Player_Defeat"); // 撃破音があれば再生
            }
            
            // カメラシェイクエフェクト
            if (cameraShake_ != null) {
                cameraShake_.ShakeCamera(CameraShake.ShakeType.Light);
            }
            
            // GameObjectを破棄
            Destroy(gameObject);
            return;
        }

        // 通常プレイヤーの処理
        if (GameManager.Instance != null && GameManager.Instance.GetGameState() != GameManager.GameState.Playing) {
            return;
        }

        if (!allowMovement_) {
            return;
        }

        // ダメージ点滅エフェクトを開始
        StartDamageFlash();
        
        // ダメージ処理
        if (GameManager.Instance != null) {
            GameManager.Instance.TakeDamage(playerID_, amount);
            currentHp_ = GameManager.Instance.GetPlayerCurrentHp(playerID_);
            if (currentHp_ > 0) {
                cameraShake_.ShakeCamera(CameraShake.ShakeType.Light);
            }
        }
        else {
            currentHp_ -= amount;
            if (currentHp_ <= 0) {
                currentHp_ = 0;
            }
        }
    }

    private void KnockBack(Player target) {
        Vector2 knockDir = (target.transform.position - transform.position).normalized;
        target.rigidbody2D_.AddForce(knockDir * 10f, ForceMode2D.Impulse);
    }

    private void UpdateRainbowEffect() {
        if (spriteRenderer_ == null) return;

        // プレイヤーIDに基づいて暖色系・寒色系の色変化を作成
        float time = Time.time * invincibilityColorSpeed_;
        Color invincibleColor;

        if (playerID_.ToUpper() == "A") {
            // プレイヤーA: 暖色系（赤→オレンジ→黄色→ピンク）
            float hue = 0.0f + (Mathf.Sin(time) * 0.15f); // 赤を基準に±15度の範囲
            float saturation = 0.8f + (Mathf.Sin(time * 1.5f) * 0.2f); // 明度を変化させて光る効果
            float brightness = 0.9f + (Mathf.Sin(time * 2.0f) * 0.1f); // 高い明度で光って見える
            
            invincibleColor = Color.HSVToRGB(hue, saturation, brightness);
        }
        else if (playerID_.ToUpper() == "D") {
            // ダミープレイヤー: 緑系（緑→黄緑→エメラルドグリーン）
            float hue = 0.3f + (Mathf.Sin(time) * 0.1f); // 緑を基準に±10度の範囲
            float saturation = 0.7f + (Mathf.Sin(time * 1.2f) * 0.2f);
            float brightness = 0.8f + (Mathf.Sin(time * 1.8f) * 0.1f);
            
            invincibleColor = Color.HSVToRGB(hue, saturation, brightness);
        }
        else {
            // プレイヤーB: 寒色系（青→水色→紫→青緑）
            float hue = 0.6f + (Mathf.Sin(time) * 0.15f); // 青を基準に±15度の範囲
            float saturation = 0.8f + (Mathf.Sin(time * 1.5f) * 0.2f); // 明度を変化させて光る効果
            float brightness = 0.9f + (Mathf.Sin(time * 2.0f) * 0.1f); // 高い明度で光って見える
            
            invincibleColor = Color.HSVToRGB(hue, saturation, brightness);
        }

        invincibleColor.a = originalColor_.a;
        spriteRenderer_.color = invincibleColor;
    }

    private void SetDefaultKeys() {
        switch (playerID_.ToUpper()) {
            case "A":
                moveLeftKey_ = KeyCode.A;
                moveRightKey_ = KeyCode.D;
                jumpKey_ = KeyCode.W;
                break;
            case "B":
                moveLeftKey_ = KeyCode.J;
                moveRightKey_ = KeyCode.L;
                jumpKey_ = KeyCode.I;
                break;
            case "D":
                // ダミープレイヤーはキー入力を使わない
                moveLeftKey_ = KeyCode.None;
                moveRightKey_ = KeyCode.None;
                jumpKey_ = KeyCode.None;
                break;
        }
    }

    private void ToggleKeySettingMode() {
        isKeySettingMode_ = !isKeySettingMode_;
        if (!isKeySettingMode_) {
            settingTarget_ = "";
        }
    }

    private void HandleKeySettingInput() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            ToggleKeySettingMode();
            return;
        }

        if (string.IsNullOrEmpty(settingTarget_)) {
            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                settingTarget_ = "moveLeft";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2)) {
                settingTarget_ = "moveRight";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3)) {
                settingTarget_ = "jump";
            }
        }
        else {
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode))) {
                if (Input.GetKeyDown(key) && key != KeyCode.Escape) {
                    SetKey(settingTarget_, key);
                    settingTarget_ = "";
                    break;
                }
            }
        }
    }

    private void SetKey(string target, KeyCode key) {
        switch (target) {
            case "moveLeft":
                moveLeftKey_ = key;
                break;
            case "moveRight":
                moveRightKey_ = key;
                break;
            case "jump":
                jumpKey_ = key;
                break;
        }
    }

    public void ReverseDirection() {
        currentDirection_ *= -1.0f;
        transform.localScale = new Vector3(-transform.localScale.x, 1, 1);
    }

    private float GetGaugeSpeedMultiplier() {
        if (currentComboGauge_ < minEffectiveGauge_) {
            return 1.0f;
        }

        float gaugeRatio = (currentComboGauge_ - minEffectiveGauge_) / (maxComboGauge_ - minEffectiveGauge_);
        gaugeRatio = Mathf.Clamp01(gaugeRatio);
        float easedRatio = gaugeRatio * gaugeRatio;

        return Mathf.Lerp(1.0f, maxGaugeSpeedMultiplier_, easedRatio);
    }

    public float GetGaugePercentage() {
        return (currentComboGauge_ / maxComboGauge_) * 100.0f;
    }

    private int GetGaugeLevel() {
        float percentage = GetGaugePercentage();
        if (percentage >= 80.0f) return 5;
        if (percentage >= 60.0f) return 4;
        if (percentage >= 40.0f) return 3;
        if (percentage >= 20.0f) return 2;
        if (percentage >= minEffectiveGauge_ / maxComboGauge_ * 100.0f) return 1;
        return 0;
    }

    public float GetCurrentEffectiveSpeed() {
        float baseSpeed = autoMoveSpeed_;
        
        if (isSpeedBoosted_) {
            baseSpeed *= speedBoostMultiplier_;
        }

        if (!isInvincible_) {
            float gaugeMultiplier = GetGaugeSpeedMultiplier();
            baseSpeed *= gaugeMultiplier;
        }

        return baseSpeed;
    }

    public void AdjustGaugeToAchieveSpeed(float targetSpeed) {
        float baseSpeed = autoMoveSpeed_;
        float targetMultiplier = targetSpeed / baseSpeed;
        SetComboGaugeForTargetSpeedMultiplier(targetMultiplier);
    }

    private void SetComboGaugeForTargetSpeedMultiplier(float targetMultiplier) {
        if (targetMultiplier <= 1.0f) {
            currentComboGauge_ = 0.0f;
            return;
        }

        float easedRatio = (targetMultiplier - 1.0f) / (maxGaugeSpeedMultiplier_ - 1.0f);
        easedRatio = Mathf.Clamp01(easedRatio);
        float requiredRatio = Mathf.Sqrt(easedRatio);
        float requiredGauge = minEffectiveGauge_ + (requiredRatio * (maxComboGauge_ - minEffectiveGauge_));
        currentComboGauge_ = Mathf.Min(requiredGauge, maxComboGauge_);
    }

    public string GetGaugeDebugInfo() {
        return $"Player {playerID_} - Gauge: {currentComboGauge_:F1}/{maxComboGauge_} ({GetGaugePercentage():F1}%) Level: {GetGaugeLevel()} Speed: x{GetGaugeSpeedMultiplier():F2}";
    }

    public void ApplyStun(float stunDuration) {
        isStunned_ = true;
        stunTimer_ = stunDuration;
    }

    /// <summary>
    /// MoveFieldからの連打ゲージ操作
    /// </summary>
    /// <param name="moveFieldDirection">MoveFieldの移動方向（1.0f=右、-1.0f=左）</param>
    /// <param name="gaugePower">ゲージ変化量</param>
    public void ApplyMoveFieldEffect(float moveFieldDirection, float gaugePower) {
        if (!allowMovement_ || !isAutoMode_) return;

        // プレイヤーの現在の移動方向とMoveFieldの方向を比較
        bool directionsMatch = (currentDirection_ > 0 && moveFieldDirection > 0) || 
                              (currentDirection_ < 0 && moveFieldDirection < 0);

        if (directionsMatch) {
            // 方向が一致する場合：ゲージ増加
            currentComboGauge_ += gaugePower;
            currentComboGauge_ = Mathf.Min(maxComboGauge_, currentComboGauge_);
            
            if (AudioManager.Instance != null) {
                AudioManager.Instance.PlaySE("Player_Barrage");
            }
        }
        else {
            // 方向が逆の場合：ゲージ減少
            currentComboGauge_ -= gaugePower * 0.5f; // 減少量は増加量の半分
            currentComboGauge_ = Mathf.Max(0.0f, currentComboGauge_);
            
            if (AudioManager.Instance != null) {
                AudioManager.Instance.PlaySE("Player_MoveField_Opposite");
            }
        }

        // MoveField効果時の視覚的フィードバック
        PlayShakeEffect();
    }

    private void PlayShakeEffect() {
        if (spriteTransform_ != null) {
            spriteTransform_.DOShakeScale(0.2f, 1.0f, 5).OnComplete(() => {
                spriteTransform_.localScale = Vector3.one;
            });
        }
    }

    private void UpdateMovementPermission() {
        // ダミープレイヤーは常に移動可能
        if (isDummyPlayer_) {
            allowMovement_ = true;
            return;
        }

        if (GameManager.Instance != null) {
            GameManager.GameState currentState = GameManager.Instance.GetGameState();
            bool previousAllowMovement = allowMovement_;
           
            switch (currentState) {
                case GameManager.GameState.Playing:
                    allowMovement_ = true;
                    break;
                case GameManager.GameState.RoundStart:
                    allowMovement_ = false;

                    break;
                case GameManager.GameState.RoundEnd:
                case GameManager.GameState.GameOver:
                case GameManager.GameState.Paused:
                default:
                    break;
            }
            
            if (previousAllowMovement != allowMovement_) {
                if (allowMovement_ && currentState == GameManager.GameState.Playing) {
                    ForceReactivate();
                }
            }
        }
        else {
            allowMovement_ = false;
        }
    }

    public void ResetPlayerState() {
        if (rigidbody2D_ != null) {
            rigidbody2D_.velocity = Vector2.zero;
            rigidbody2D_.angularVelocity = 0f;
            rigidbody2D_.drag = dragOnStop_;
            rigidbody2D_.WakeUp();
        }

        inputHorizontal_ = Vector2.zero;
        isJumping_ = false;
        
        // ダミープレイヤーの場合は初期方向を使用
        if (isDummyPlayer_) {
            currentDirection_ = Mathf.Sign(dummyInitialDirection_);
            if (currentDirection_ == 0.0f) currentDirection_ = 1.0f;
            dummyMovingRight_ = currentDirection_ > 0;
            
            // スプライトの向きも初期設定に戻す
            if (currentDirection_ > 0) {
                transform.localScale = new Vector3(-1, 1, 1);
            } else {
                transform.localScale = new Vector3(1, 1, 1);
            }
        } else {
            currentDirection_ = 1.0f;
        }

        currentComboGauge_ = 0.0f;
        isSpeedBoosted_ = false;
        speedBoostTimer_ = 0.0f;

        isStunned_ = false;
        stunTimer_ = 0.0f;
        hasDoubleJumped_ = false;
        shouldReverseOnLanding_ = false;

        // ダミープレイヤーの無敵状態は維持
        if (!isDummyPlayer_ || !dummyAlwaysInvincible_) {
            isInvincible_ = false;
            invincibilityTimer_ = 0.0f;
        }
        
        // ダメージエフェクトをリセット
        isDamageFlashing_ = false;
        damageFlashTimer_ = 0.0f;
        isFlashRed_ = false;
        
        if (spriteRenderer_ != null && (!isDummyPlayer_ || !dummyAlwaysInvincible_)) {
            spriteRenderer_.color = originalColor_;
        }

        // ノックバック状態をリセット
        isKnockedBack_ = false;
        knockBackTimer_ = 0.0f;

        if (animator_ != null) {
            animator_.SetBool("Run", false);
            animator_.SetBool("Jump", false);
        }

        targetRotation_ = 0.0f;
        currentRotation_ = 0.0f;
        if (spriteTransform_ != null) {
            spriteTransform_.localRotation = Quaternion.identity;
        }

        wasHittingWall_ = false;
        
        // ダミープレイヤーは移動許可の設定を維持
        if (!isDummyPlayer_) {
            allowMovement_ = false;
        } else {
            allowMovement_ = dummyCanMove_;
        }
    }
    
    public void ForceReactivate() {
        if (rigidbody2D_ != null) {
            rigidbody2D_.velocity = Vector2.zero;
            rigidbody2D_.angularVelocity = 0f;
            rigidbody2D_.drag = dragOnMove_;
            rigidbody2D_.WakeUp();
            
            if (rigidbody2D_.sharedMaterial != null) {
                rigidbody2D_.sharedMaterial = null;
            }
        }
        
        isStunned_ = false;
        stunTimer_ = 0.0f;
        
        inputHorizontal_ = Vector2.zero;
        isJumping_ = false;
        
        allowMovement_ = true;
        
        if (isAutoMode_) {
            if (currentDirection_ == 0.0f) {
                currentDirection_ = 1.0f;
            }
        }
    }

    /// <summary>
    /// ダメージ点滅エフェクトを開始
    /// </summary>
    private void StartDamageFlash() {
        isDamageFlashing_ = true;
        damageFlashTimer_ = damageFlashDuration_;
        damageFlashIntervalTimer_ = 0.0f;
        isFlashRed_ = true;
        UpdateDamageFlashEffect();
    }

    /// <summary>
    /// ダメージ点滅エフェクトの色を更新
    /// </summary>
    private void UpdateDamageFlashEffect() {
        if (spriteRenderer_ == null) return;

        if (isFlashRed_) {
            Color flashColor = damageFlashColor_;
            flashColor.a = originalColor_.a;
            spriteRenderer_.color = flashColor;
        }
        else {
            spriteRenderer_.color = originalColor_;
        }
    }

    public int GetMaxHP()
    {
        return maxHp_;
    }


}