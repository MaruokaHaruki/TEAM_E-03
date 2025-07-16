using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[System.Obsolete]
[RequireComponent(typeof(Rigidbody2D))]

public class Player : MonoBehaviour {   
    // プレイヤー設定
    [Header("プレイヤー設定")]
    public string playerID_ = "A";
    public int maxHp_ = 100;
    public int currentHp_ = 100;

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
    public float rainbowSpeed_ = 2.0f;
    public float invincibilitySpeedMultiplier_ = 2.0f;

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

    private CameraShake cameraShake_;

    private void Start() {
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
        UpdateMovementPermission();

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

        if (isInvincible_) {
            invincibilityTimer_ -= Time.deltaTime;
            UpdateRainbowEffect();
            
            if (invincibilityTimer_ <= 0.0f) {
                isInvincible_ = false;
                if (spriteRenderer_ != null) {
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
            if (isAutoMode_) {
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

        // ノックバック中は自動移動を停止
        if (isKnockedBack_) {
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

        if (moveInputPressed && !isInvincible_) {
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

        if (isInvincible_) {
            currentSpeed *= invincibilitySpeedMultiplier_;
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

    private void OnCollisionEnter2D(Collision2D collision) {
        if (GameManager.Instance != null && GameManager.Instance.GetGameState() != GameManager.GameState.Playing) {
            return;
        }

        if (!allowMovement_) {
            return;
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

                    otherPlayer.TakeDamage(20);
                    KnockBack(otherPlayer);
                    ReverseDirection();
                    otherPlayer.ReverseDirection();
                }
                else if (!thisIsInvincible && otherIsInvincible) {
                    if (AudioManager.Instance != null) {
                        AudioManager.Instance.PlaySE("Player_Penguin2Penguin");
                    }

                    TakeDamage(20);
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
                    otherPlayer.TakeDamage(20);
                    KnockBack(otherPlayer);
                }
            }
            else if (!otherIsAheadOfMe && amIAheadOfOther) {
                if (!thisIsInvincible) {
                    TakeDamage(20);
                    Vector2 knockBackDirToThis = (transform.position - otherPlayer.transform.position).normalized;
                    if (knockBackDirToThis == Vector2.zero) knockBackDirToThis = (Random.insideUnitCircle).normalized;
                    rigidbody2D_.AddForce(knockBackDirToThis * 10f, ForceMode2D.Impulse);
                }
            }
        }

        if (collision.gameObject.CompareTag("InvincibleItem")) {
            if (AudioManager.Instance != null) {
                AudioManager.Instance.PlaySE("collision");
            }

            isInvincible_ = true;
            invincibilityTimer_ = 2.0f;
            Destroy(collision.gameObject);
        }
    }

    public void TakeDamage(int amount) {
        if (GameManager.Instance != null && GameManager.Instance.GetGameState() != GameManager.GameState.Playing) {
            return;
        }

        if (!allowMovement_) {
            return;
        }

        if (GameManager.Instance != null) {
            GameManager.Instance.TakeDamage(playerID_, amount);
            currentHp_ = GameManager.Instance.GetPlayerCurrentHp(playerID_);
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

        float hue = (Time.time * rainbowSpeed_) % 1.0f;
        Color rainbowColor = Color.HSVToRGB(hue, 1.0f, 1.0f);
        rainbowColor.a = originalColor_.a;
        spriteRenderer_.color = rainbowColor;
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

        if (isInvincible_) {
            baseSpeed *= invincibilitySpeedMultiplier_;
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

    private void UpdateMovementPermission() {
        if (GameManager.Instance != null) {
            GameManager.GameState currentState = GameManager.Instance.GetGameState();
            bool previousAllowMovement = allowMovement_;
            
            switch (currentState) {
                case GameManager.GameState.Playing:
                    allowMovement_ = true;
                    break;
                case GameManager.GameState.RoundStart:
                case GameManager.GameState.RoundEnd:
                case GameManager.GameState.GameOver:
                case GameManager.GameState.Paused:
                default:
                    allowMovement_ = false;
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
        Debug.Log($"[PLAYER] : {playerID_} の状態をリセット開始");
        
        if (rigidbody2D_ != null) {
            rigidbody2D_.velocity = Vector2.zero;
            rigidbody2D_.angularVelocity = 0f;
            rigidbody2D_.drag = dragOnStop_;
            rigidbody2D_.WakeUp();
            rigidbody2D_.isKinematic = false;
        }

        inputHorizontal_ = Vector2.zero;
        isJumping_ = false;
        currentDirection_ = 1.0f;

        currentComboGauge_ = 0.0f;
        isSpeedBoosted_ = false;
        speedBoostTimer_ = 0.0f;
        isGaugeDrainBoosted_ = false;

        isStunned_ = false;
        stunTimer_ = 0.0f;
        hasDoubleJumped_ = false;
        shouldReverseOnLanding_ = false;

        isInvincible_ = false;
        invincibilityTimer_ = 0.0f;
        if (spriteRenderer_ != null) {
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
            spriteTransform_.localScale = Vector3.one;
        }

        wasHittingWall_ = false;
        allowMovement_ = false;
        
        // 向きを正しく設定
        if (currentDirection_ > 0) {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else {
            transform.localScale = new Vector3(1, 1, 1);
        }
        
        Debug.Log($"[PLAYER] : {playerID_} の状態リセット完了");
    }
    
    public void ForceReactivate() {
        Debug.Log($"[PLAYER] : {playerID_} の強制再有効化開始");
        
        if (rigidbody2D_ != null) {
            rigidbody2D_.velocity = Vector2.zero;
            rigidbody2D_.angularVelocity = 0f;
            rigidbody2D_.drag = dragOnMove_;
            rigidbody2D_.WakeUp();
            rigidbody2D_.isKinematic = false;
            
            if (rigidbody2D_.sharedMaterial != null) {
                rigidbody2D_.sharedMaterial = null;
            }
        }
        
        // 全ての状態フラグをクリア
        isStunned_ = false;
        stunTimer_ = 0.0f;
        isKnockedBack_ = false;
        knockBackTimer_ = 0.0f;
        isGaugeDrainBoosted_ = false;
        
        inputHorizontal_ = Vector2.zero;
        isJumping_ = false;
        
        allowMovement_ = true;
        
        if (isAutoMode_) {
            if (currentDirection_ == 0.0f) {
                currentDirection_ = 1.0f;
            }
        }
        
        Debug.Log($"[PLAYER] : {playerID_} の強制再有効化完了 - allowMovement_: {allowMovement_}");
    }
}