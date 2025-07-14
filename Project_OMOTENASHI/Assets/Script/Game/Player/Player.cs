using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete]
[RequireComponent(typeof(Rigidbody2D))]

//=============================================================================
/// プレイヤーキャラクターの移動、ジャンプ、アニメーション制御を統合管理するクラス
/// 2D横スクロールアクションゲーム用のプレイヤーコントローラー
public class Player : MonoBehaviour {   
    ///--------------------------------------------------------------
    ///                      【パブリック変数】
    //========================================
    // 【プレイヤー識別・基本設定】
    [Header("プレイヤー設定")]
    [Tooltip("プレイヤーの識別ID（A, B など）")]
    public string playerID_ = "A";

    [Tooltip("プレイヤーの最大HP値")]
    public int maxHp_ = 100;

    [Tooltip("プレイヤーの現在HP値")]
    public int currentHp_ = 100;

    //========================================
    // 【移動設定】
    [Header("移動設定")]
    [Tooltip("プレイヤーの最大移動速度（単位/秒）")]
    public float maxSpeed_ = 5.0f;

    [Tooltip("移動時の加速力。高いほど素早く最大速度に到達")]
    public float acceleration_ = 15.0f;

    [Tooltip("停止時の抵抗値。高いほど急ブレーキ")]
    public float dragOnStop_ = 5.0f;

    [Tooltip("移動中の抵抗値。通常は0で慣性を保持")]
    public float dragOnMove_ = 0.0f;

    //========================================
    // 【ジャンプ・重力設定】
    [Header("ジャンプ・重力設定")]
    [Tooltip("独自重力の強さ。Unityのデフォルト重力に追加される")]
    public float gravity_ = 0.2f;

    [Tooltip("ジャンプ時に加える瞬間的な力の大きさ")]
    public float jumpForce_ = 4.0f;

    //========================================
    // 【判定システム】
    [Header("判定システム")]
    [Tooltip("地面との接触を判定するGroundCheckコンポーネント")]
    public GroundCheck groundCheck_;

    [Tooltip("前方（進行方向）の壁を検出するコンポーネント")]
    public WallCheckFront wallCheckFront_;

    [Tooltip("後方の壁を検出するコンポーネント")]
    public WallCheckBuck wallCheckBuck_;

    //========================================
    // 【自動移動設定】
    [Header("自動移動設定")]
    [Tooltip("自動移動モードを有効にするかどうか")]
    public bool isAutoMode_ = false;

    [Tooltip("自動移動時の基本速度")]
    public float autoMoveSpeed_ = 3.0f;

    [Tooltip("連打時のスピードアップ倍率")]
    public float speedBoostMultiplier_ = 1.5f;

    [Tooltip("スピードブーストの持続時間")]
    public float speedBoostDuration_ = 0.5f;

    //========================================
    // 【連打ゲージ設定】
    [Header("連打ゲージ設定")]
    [Tooltip("連打ゲージの最大値")]
    public float maxComboGauge_ = 100.0f;

    [Tooltip("1回の連打で増加するゲージ量")]
    public float comboGaugePerHit_ = 8.0f;

    [Tooltip("ゲージの自然減少速度（毎秒）")]
    public float gaugeDrainRate_ = 20.0f;

    [Tooltip("ゲージに応じた最大速度倍率")]
    public float maxGaugeSpeedMultiplier_ = 5.0f;

    [Tooltip("ゲージが効果を発揮する最低値")]
    public float minEffectiveGauge_ = 10.0f;

    //========================================
    // 【特殊状態設定】
    [Header("無敵状態設定")]
    [Tooltip("壁反射後の無敵時間（秒）")]
    public float invincibilityDuration_ = 3.0f;

    [Tooltip("虹色エフェクトの変化速度")]
    public float rainbowSpeed_ = 2.0f;

    [Tooltip("無敵状態中の移動速度倍率")]
    public float invincibilitySpeedMultiplier_ = 2.0f;

    //========================================
    // 【スプライト回転設定】
    [Header("スプライト回転設定")]
    [Tooltip("スプライトの回転を有効にするか")]
    public bool enableSpriteRotation_ = true;

    [Tooltip("回転の最大角度（度）")]
    public float maxRotationAngle_ = 45.0f;

    [Tooltip("回転の感度（高いほど敏感）")]
    public float rotationSensitivity_ = 1.0f;

    [Tooltip("回転の滑らかさ（高いほど滑らか）")]
    public float rotationSmoothness_ = 5.0f;

    //========================================
    // 【機能有効化設定】
    [Header("機能設定")]
    [Tooltip("プレイヤー衝突時の速度交換機能を有効にするか")]
    public bool enableSpeedTransfer_ = true;

    [Tooltip("キー設定変更モード")]
    public bool isKeySettingMode_ = false;

    [Tooltip("プレイヤーの移動を許可するか（ゲーム制御用）")]
    public bool allowMovement_ = false;

    //========================================
    // 【キー設定】
    [Header("キー設定")]
    [Tooltip("移動キー（左）")]
    public KeyCode moveLeftKey_ = KeyCode.A;

    [Tooltip("移動キー（右）")]
    public KeyCode moveRightKey_ = KeyCode.D;

    [Tooltip("ジャンプキー")]
    public KeyCode jumpKey_ = KeyCode.P;
    //========================================
    // 【特別ルール設定】
    [Header("特別ルール設定")]
    [Tooltip("2段ジャンプ機能を有効にするか")]
    public bool enableDoubleJump_ = false;
    
    [Tooltip("踏みつけ機能を有効にするか")]
    public bool enableStomp_ = false;
    
    [Tooltip("反転ジャンプ機能を有効にするか")]
    public bool enableReverseJump_ = false;
    
    [Tooltip("踏みつけスタン時間（秒）")]
    public float stompStunDuration_ = 2.0f;


    ///--------------------------------------------------------------
    ///                      【プライベート変数】
    
    //========================================
    // 【コンポーネント参照】
    private Animator animator_ = null;
    private Rigidbody2D rigidbody2D_ = null;
    private SpriteRenderer spriteRenderer_ = null;
    private Transform spriteTransform_ = null;

    //========================================
    // 【状態フラグ】
    public bool isGround_ = false;
    public bool isJumping_ = false;

    //========================================
    // 【入力データ】
    private Vector2 inputHorizontal_ = Vector2.zero;
    //private Vector2 inputVertical_ = Vector2.zero;

    //========================================
    // 【壁接触状態】
    private bool isHitWallFront_ = false;
    private bool isHitWallBuck_ = false;

    //========================================
    // 【自動移動関連】
    private float currentDirection_ = 1.0f;
    private bool wasHittingWall_ = false;
    private float speedBoostTimer_ = 0.0f;
    private bool isSpeedBoosted_ = false;

    //========================================
    // 【連打ゲージ関連】
    private float currentComboGauge_ = 0.0f;
    //private float lastInputTime_ = 0.0f;

    //========================================
    // 【無敵状態関連】
    private bool isInvincible_ = false;
    private float invincibilityTimer_ = 0.0f;
    private Color originalColor_ = Color.white;

    //========================================
    // 【キー設定関連】
    private KeyCode pendingKey_ = KeyCode.None;
    private string settingTarget_ = "";

    //========================================
    // 【特別ルール関連】
    public bool hasDoubleJumped_ = false;
    public bool isStunned_ = false;
    public float stunTimer_ = 0.0f;
    public bool shouldReverseOnLanding_ = false;

    //========================================
    // 【スプライト回転関連】
    private float targetRotation_ = 0.0f;
    private float currentRotation_ = 0.0f;

    //NOTE: カメラの揺れを制御するためのコンポーネント
    private CameraShake cameraShake_;


    ///--------------------------------------------------------------
    ///                      初期化処理
    /// ゲーム開始時の初期化処理
    /// 必要なコンポーネントの取得と初期状態の設定を行う
    private void Start()
    {

        // NOTE: ここではCameraShakeコンポーネントを取得している
        // CameraShakeコンポーネントを取得
        cameraShake_ = FindObjectOfType<CameraShake>();

        //========================================
        // 子オブジェクト「Sprite」からアニメーター取得と存在確認
        Transform spriteChild = transform.Find("Sprite");
        if (spriteChild != null)
        {
            spriteTransform_ = spriteChild;
            animator_ = spriteChild.GetComponent<Animator>();
            if (animator_ == null)
            {
                Debug.LogError("[ERROR] : Animator component not found on Sprite child object. アニメーション制御ができません。");
            }

            //========================================
            // 子オブジェクト「Sprite」からスプライトレンダラー取得と存在確認
            spriteRenderer_ = spriteChild.GetComponent<SpriteRenderer>();
            if (spriteRenderer_ == null)
            {
                Debug.LogError("[ERROR] : SpriteRenderer component not found on Sprite child object. 色変更エフェクトができません。");
            }
            else
            {
                // 元の色を保存
                originalColor_ = spriteRenderer_.color;
            }
        }
        else
        {
            Debug.LogError("[ERROR] : 'Sprite' child object not found. 描画系コンポーネントを取得できません。");
        }

        //========================================
        // リジッドボディ取得と存在確認
        rigidbody2D_ = GetComponent<Rigidbody2D>();
        if (rigidbody2D_ == null)
        {
            Debug.LogError("[ERROR] : Rigidbody2D component not found on Player object. 物理演算制御ができません。");
        }

        //========================================
        // プレイヤー別デフォルトキー設定
        SetDefaultKeys();
    }
    ///--------------------------------------------------------------
    ///                      メインループ処理
    private void Update() {
        //========================================
        // 【ゲーム状態による移動制御チェック】
        UpdateMovementPermission();

        //========================================
        // 【スタン状態タイマーの更新】
        if (isStunned_) {
            stunTimer_ -= Time.deltaTime;
            if (stunTimer_ <= 0.0f) {
                isStunned_ = false;
                Debug.Log($"[STUN] : {gameObject.name} のスタンが回復しました");
            }
        }

        //========================================
        // 【無敵状態タイマーの更新】
        if (isInvincible_) {
            invincibilityTimer_ -= Time.deltaTime;
            
            // 虹色エフェクトの更新
            UpdateRainbowEffect();
            
            if (invincibilityTimer_ <= 0.0f) {
                isInvincible_ = false;
                // 元の色に戻す
                if (spriteRenderer_ != null) {
                    spriteRenderer_.color = originalColor_;
                }
                Debug.Log($"[INVINCIBILITY] : {gameObject.name} の無敵状態が終了しました");
            }
        }

        //========================================
        // 【モード切り替え処理】
        // Tabキーで通常操作と自動移動を切り替え
        if (Input.GetKeyDown(KeyCode.Tab)) {
            isAutoMode_ = !isAutoMode_;
            Debug.Log($"[MODE CHANGE] : {(isAutoMode_ ? "自動移動モード" : "通常操作モード")}に切り替わりました");
        }

        // F3キーで速度交換機能のON/OFFを切り替え
        if (Input.GetKeyDown(KeyCode.F3)) {
            enableSpeedTransfer_ = !enableSpeedTransfer_;
            Debug.Log($"[SPEED TRANSFER TOGGLE] : 速度交換機能が {(enableSpeedTransfer_ ? "有効" : "無効")} になりました");
        }

        // 6キーで全員の連打ゲージを0にする
        if (Input.GetKeyDown(KeyCode.Alpha6)) {
            Player[] allPlayers = FindObjectsOfType<Player>();
            foreach (Player player in allPlayers) {
                player.currentComboGauge_ = 0.0f;
                player.isSpeedBoosted_ = false;
            }
            Debug.Log("[RESET SPEED] : 全プレイヤーの速度がリセットされました");
        }

        //========================================
        // 【キー設定モードの処理】
        if (Input.GetKeyDown(KeyCode.F1) && playerID_ == "A") {
            ToggleKeySettingMode();
        }
        else if (Input.GetKeyDown(KeyCode.F2) && playerID_ == "B") {
            ToggleKeySettingMode();
        }

        // キー設定モード中の処理
        if (isKeySettingMode_) {
            HandleKeySettingInput();
            return; // キー設定モード中は他の処理をスキップ
        }

        //========================================
        // 【移動処理の振り分け】
        if (allowMovement_) {
            if (isAutoMode_) {
                AutoMove();
            }
            else {
                Move();
            }
        }
        else {
            // 移動が許可されていない場合は入力をリセット
            inputHorizontal_ = Vector2.zero;
            isJumping_ = false;
            
            // アニメーションを停止状態に
            if (animator_ != null) {
                animator_.SetBool("Run", false);
                animator_.SetBool("Jump", !isGround_);
            }
        }

        //========================================
        // 【スプライト回転の更新】
        if (enableSpriteRotation_) {
            UpdateSpriteRotation();
        }

        //========================================
        // 【連打ゲージの自然減少】
        if (currentComboGauge_ > 0.0f) {
            currentComboGauge_ -= gaugeDrainRate_ * Time.deltaTime;
            currentComboGauge_ = Mathf.Max(0.0f, currentComboGauge_);
        }
    }
    ///--------------------------------------------------------------
    ///                      物理演算更新処理
    private void FixedUpdate() {
        //========================================
        // 【環境状態の更新】
        // 周囲の壁や地面との接触状況を最新状態に更新
        bool wasGrounded = isGround_;  // 前フレームの地面接触状態を保存
        isGround_ = groundCheck_.IsGround();
        isHitWallFront_ = wallCheckFront_.IsHitWallFront();
        isHitWallBuck_ = wallCheckBuck_.IsHitWallBuck();

        //========================================
        // 【着地音の再生】
        // 空中から地面に着地した瞬間に着地音を再生
        if (!wasGrounded && isGround_) {
            if (AudioManager.Instance != null) {
                AudioManager.Instance.PlaySE("Player_Landing");
            }
        }

        //========================================
        // 【反転ジャンプの処理】
        // 空中から着地した瞬間に反転ジャンプが有効なら方向転換
        if (enableReverseJump_ && !wasGrounded && isGround_ && shouldReverseOnLanding_) {
            ReverseDirection();
            shouldReverseOnLanding_ = false;
            Debug.Log($"[REVERSE JUMP] : {gameObject.name} が着地時に方向反転しました");
        }

        // 地面に接触したら2段ジャンプフラグをリセット
        if (isGround_ && !wasGrounded) {
            hasDoubleJumped_ = false;
        }

        //========================================
        // 【水平移動の物理計算】
        // 現在速度と目標速度の差分から必要な力を算出
        float currentVelocityX = rigidbody2D_.velocity.x;
        float targetSpeed = inputHorizontal_.x * maxSpeed_;
        float speedDiff = targetSpeed - currentVelocityX;
        float accelRate = acceleration_;
        float movement = speedDiff * accelRate;

        //========================================
        // 【壁張り付き防止システム】
        // 壁に向かって移動入力がある場合は移動を制限し、
        // 壁に張り付いて動けなくなる現象を防ぐ
        bool blockFront = isHitWallFront_ && inputHorizontal_.x < 0;
        bool blockBuck = isHitWallFront_ && inputHorizontal_.x > 0;

        if (!blockFront && !blockBuck) {
            // 壁による移動制限がない場合のみ移動力を適用
            rigidbody2D_.AddForce(Vector2.right * movement, ForceMode2D.Force);
        }

        //========================================
        // 【動的抵抗制御】
        // 入力状況に応じてドラッグ値を切り替え、
        // 精密な移動制御と自然な停止動作を実現
        if (Mathf.Abs(inputHorizontal_.x) < 0.01f) {
            // 入力がない場合：急停止用の高い抵抗を適用
            rigidbody2D_.drag = dragOnStop_;
        }
        else {
            // 移動入力中：慣性を活かすため低い抵抗を適用
            rigidbody2D_.drag = dragOnMove_;
        }

        //========================================
        // 【ジャンプ力の適用】
        // ジャンプフラグが立っている場合、瞬間的な上向きの力を加える
        // フラグは即座にリセットして連続ジャンプを防ぐ
        if (isJumping_) {
            // ジャンプ音を再生
            if (AudioManager.Instance != null) {
                AudioManager.Instance.PlaySE("Player_Jump");
            }
            
            // 2段ジャンプの場合はY軸速度を一度リセット
            if (hasDoubleJumped_ && !isGround_) {
                // Y軸速度を0にリセットしてから新しいジャンプ力を適用
                rigidbody2D_.velocity = new Vector2(rigidbody2D_.velocity.x, 0f);
            }
            
            rigidbody2D_.AddForce(Vector2.up * jumpForce_, ForceMode2D.Impulse);
            isJumping_ = false;  // ジャンプ実行後は即座にフラグをリセット
        }

        //========================================
        // 【独自重力システム】
        // Unityの標準重力に加えて、ゲーム専用の重力を追加適用
        // より細かい落下制御を可能にする
        rigidbody2D_.AddForce(Vector2.down * gravity_, ForceMode2D.Force);
    }

    ///--------------------------------------------------------------
    ///                      スプライト回転制御
    /// 速度ベクトルに基づいてスプライトの向きを更新
    private void UpdateSpriteRotation() {
        if (spriteTransform_ == null || rigidbody2D_ == null) return;

        // 現在の速度ベクトルを取得
        Vector2 velocity = rigidbody2D_.velocity;

        // 速度が十分にある場合のみ回転を適用
        if (velocity.magnitude > 0.5f) {
            // 速度ベクトルから角度を計算（ラジアンから度に変換）
            float velocityAngle = Mathf.Atan2(velocity.y, Mathf.Abs(velocity.x)) * Mathf.Rad2Deg;
            
            // 角度を制限範囲内にクランプ
            velocityAngle = Mathf.Clamp(velocityAngle, -maxRotationAngle_, maxRotationAngle_);
            
            // 感度を適用
            targetRotation_ = velocityAngle * rotationSensitivity_;
        }
        else {
            // 速度が小さい場合は水平に戻す
            targetRotation_ = 0.0f;
        }

        // 滑らかに回転を補間
        currentRotation_ = Mathf.Lerp(currentRotation_, targetRotation_, rotationSmoothness_ * Time.deltaTime);

        // スプライトに回転を適用（Z軸回転のみ）
        spriteTransform_.localRotation = Quaternion.Euler(0, 0, -currentRotation_);
    }

    ///--------------------------------------------------------------
    ///                      入力処理・移動制御
    private void Move() {
        //========================================
        // 【入力データの取得】
        // プレイヤー別のキー設定を使用
        float horizontal = 0.0f;
        if (Input.GetKey(moveRightKey_)) {
            horizontal += 1.0f;
        }
        if (Input.GetKey(moveLeftKey_)) {
            horizontal -= 1.0f;
        }
        
        bool jumpPressed = Input.GetKeyDown(jumpKey_);

        //========================================
        // 【入力データの保存】
        inputHorizontal_ = new Vector2(horizontal, 0.0f);
        //inputVertical_ = Vector2.zero;

        //========================================
        // 【ジャンプ入力の処理】
        if (!isStunned_) {
            if (isGround_) {
                if (jumpPressed) {
                    isJumping_ = true;
                    
                    // 反転ジャンプが有効な場合は着地時反転フラグを立てる
                    if (enableReverseJump_) {
                        shouldReverseOnLanding_ = true;
                    }
                }
            }
            else {
                // 2段ジャンプの処理
                if (enableDoubleJump_ && jumpPressed && !hasDoubleJumped_) {
                    isJumping_ = true;
                    hasDoubleJumped_ = true;
                    Debug.Log($"[DOUBLE JUMP] : {gameObject.name} が2段ジャンプを実行しました");
                }
            }
        }
        else {
            isJumping_ = false; // スタン中はジャンプ不可
        }

        //========================================
        // 【アニメーション状態の更新】
        animator_.SetBool("Jump", !isGround_);

        //========================================
        // 【キャラクター向きとアニメーションの制御】
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
    }    //---------------------------------------------------------------
    //                      自動移動制御
    private void AutoMove() {
        //========================================
        // 【壁衝突判定と方向転換】
        // 前方の壁に衝突した場合、移動方向を反転
        if (isHitWallFront_ && !wasHittingWall_) {
            currentDirection_ *= -1.0f;  // 移動方向を反転
            
            // プレイヤーと壁の衝突音を再生
            if (AudioManager.Instance != null) {
                AudioManager.Instance.PlaySE("Player_Penguin2Wall");
            }
            
            // 壁反射時に無敵状態を付与
            isInvincible_ = true;
            invincibilityTimer_ = invincibilityDuration_;
            Debug.Log($"[AUTO MOVE] : {gameObject.name} が壁に衝突しました。方向を反転し、無敵状態になりました。新しい方向: {(currentDirection_ > 0 ? "右" : "左")}");
        }
        wasHittingWall_ = isHitWallFront_;  // 前フレームの壁衝突状態を保存

        //========================================
        // 【連打によるゲージ蓄積処理】
        bool moveInputPressed = Input.GetKeyDown(moveLeftKey_) ||
                               Input.GetKeyDown(moveRightKey_);

        // 無敵状態中は連打ゲージ蓄積を無効化
        if (moveInputPressed && !isInvincible_) {
            // 連打音を再生
            if (AudioManager.Instance != null) {
                AudioManager.Instance.PlaySE("Player_Barrage");
            }
            
            // 連打ゲージを増加
            currentComboGauge_ += comboGaugePerHit_;
            currentComboGauge_ = Mathf.Min(maxComboGauge_, currentComboGauge_);
            //lastInputTime_ = Time.time;
            
            Debug.Log($"[COMBO GAUGE] : Player {playerID_} - ゲージ: {currentComboGauge_:F1}/{maxComboGauge_} ({GetGaugePercentage():F1}%)");
        }
        else if (moveInputPressed && isInvincible_) {
            Debug.Log($"[COMBO GAUGE] : Player {playerID_} - 無敵状態中のため連打ゲージ蓄積が無効化されました");
        }

        // スピードブーストタイマーの更新（従来の一時的なブースト）
        if (isSpeedBoosted_) {
            speedBoostTimer_ -= Time.deltaTime;
            if (speedBoostTimer_ <= 0.0f) {
                isSpeedBoosted_ = false;
            }
        }

        //========================================
        // 【ジャンプ入力の処理】
        // 地面に接触している時のみジャンプを許可
        bool jumpPressed = Input.GetKeyDown(jumpKey_);
        if (!isStunned_) {
            if (isGround_ && jumpPressed) {
                isJumping_ = true;
                
                // 反転ジャンプが有効な場合は着地時反転フラグを立てる
                if (enableReverseJump_) {
                    shouldReverseOnLanding_ = true;
                }
            }
            else if (!isGround_ && enableDoubleJump_ && jumpPressed && !hasDoubleJumped_) {
                isJumping_ = true;
                hasDoubleJumped_ = true;
                Debug.Log($"[DOUBLE JUMP] : {gameObject.name} が2段ジャンプを実行しました");
            }
        }
        else {
            isJumping_ = false; // スタン中はジャンプ不可
        }

        //========================================
        // 【自動移動の実行】
        // 現在の方向に基づいて移動入力を設定
        float currentSpeed = autoMoveSpeed_;
        
        // 従来のスピードブースト
        if (isSpeedBoosted_) {
            currentSpeed *= speedBoostMultiplier_;
        }

        // 無敵状態中は規定の高速移動のみ適用、連打ゲージは無効
        if (isInvincible_) {
            currentSpeed *= invincibilitySpeedMultiplier_;
        }
        else {
            // 通常状態時のみ連打ゲージによるスピードアップを適用
            float gaugeSpeedMultiplier = GetGaugeSpeedMultiplier();
            currentSpeed *= gaugeSpeedMultiplier;
        }

        inputHorizontal_ = new Vector2(currentDirection_ * currentSpeed / maxSpeed_, 0.0f);
        //inputVertical_ = Vector2.zero;

        //========================================
        // 【アニメーション状態の更新】
        animator_.SetBool("Jump", !isGround_);
        animator_.SetBool("Run", true);  // 自動移動中は常に走行アニメーション

        //========================================
        // 【キャラクター向きの制御】
        // 移動方向に応じてスプライトを反転
        if (currentDirection_ > 0) {
            // 右方向への移動
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else {
            // 左方向への移動
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
    //---------------------------------------------------------------
    //                      衝突判定処理
    private void OnCollisionEnter2D(Collision2D collision) {
        // 踏みつけ判定
        if (enableStomp_ && collision.gameObject.CompareTag("Player"))
        {
            Player otherPlayer = collision.gameObject.GetComponent<Player>();
            if (otherPlayer == null || otherPlayer == this) return;

            // 自分が相手より上にいて、下向きの速度を持っている場合は踏みつけ
            bool isAbove = transform.position.y > otherPlayer.transform.position.y + 0.5f;
            bool isMovingDown = rigidbody2D_.velocity.y < -1.0f;

            if (isAbove && isMovingDown && !otherPlayer.isStunned_)
            {
                // 踏みつけ成功
                otherPlayer.ApplyStun(stompStunDuration_);

                // 踏みつけた側は跳ね返る
                rigidbody2D_.velocity = new Vector2(rigidbody2D_.velocity.x, jumpForce_ * 0.7f);

                // 踏みつけ音を再生
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySE("Player_Step");
                }

                Debug.Log($"[STOMP] : {gameObject.name} が {otherPlayer.gameObject.name} を踏みつけました");
                return; // 踏みつけ成功時は通常の衝突処理をスキップ
            }
        }

        if (collision.gameObject.CompareTag("Player")) {
            Player otherPlayer = collision.gameObject.GetComponent<Player>();
            if (otherPlayer == null || otherPlayer == this) return;

            // 衝突処理の重複を防ぐため、GetInstanceIDが小さい方のオブジェクトは処理をスキップします。
            // これにより、衝突ペアに対して一度だけ判定ロジックが実行されるようになります。
            if (gameObject.GetInstanceID() < otherPlayer.gameObject.GetInstanceID()) {
                return;
            }

            // プレイヤー同士の衝突音を再生部分を削除（個別に処理するため）
            if (collision.gameObject.CompareTag("Player")) {
                //Player otherPlayer = collision.gameObject.GetComponent<Player>();
                if (otherPlayer == null || otherPlayer == this) return;

                // 衝突処理の重複を防ぐため、GetInstanceIDが小さい方のオブジェクトは処理をスキップします。
                // これにより、衝突ペアに対して一度だけ判定ロジックが実行されるようになります。
                if (gameObject.GetInstanceID() < otherPlayer.gameObject.GetInstanceID()) {
                    return;
                }

                // 速度交換ロジック
                if (isAutoMode_ && otherPlayer.isAutoMode_ && enableSpeedTransfer_ && otherPlayer.enableSpeedTransfer_) {
                    float mySpeed = GetCurrentEffectiveSpeed();
                    float otherSpeed = otherPlayer.GetCurrentEffectiveSpeed();

                    // 速度差が十分にある場合のみ速度交換を実行
                    if (Mathf.Abs(mySpeed - otherSpeed) > 0.5f) {
                        // お互いの速度を一時保存
                        float tempMySpeed = mySpeed;
                        float tempOtherSpeed = otherSpeed;

                        // お互いの速度を入れ替え
                        AdjustGaugeToAchieveSpeed(tempOtherSpeed);
                        otherPlayer.AdjustGaugeToAchieveSpeed(tempMySpeed);

                        Debug.Log($"[SPEED TRANSFER] : {gameObject.name}(速度:{tempMySpeed:F2}) <-> {otherPlayer.gameObject.name}(速度:{tempOtherSpeed:F2}) 速度を交換しました");
                    }
                }

                // 無敵状態の判定
                bool thisIsInvincible = isInvincible_;
                bool otherIsInvincible = otherPlayer.isInvincible_;

                // 自分(this)から見た相手(otherPlayer)の相対X位置
                float relativeXToOther = otherPlayer.transform.position.x - transform.position.x;
                // 自分の進行方向の符号 (+1 or -1)
                float myDirSign = Mathf.Sign(currentDirection_);
                // 相手が自分の進行方向の正面にいるか
                bool otherIsAheadOfMe = Mathf.Sign(relativeXToOther) == myDirSign;

                // 相手(otherPlayer)から見た自分(this)の相対X位置
                float relativeXToMe = transform.position.x - otherPlayer.transform.position.x; // = -relativeXToOther
                                                                                               // 相手の進行方向の符号 (+1 or -1)
                float otherDirSign = Mathf.Sign(otherPlayer.currentDirection_);
                // 自分が相手の進行方向の正面にいるか
                bool amIAheadOfOther = Mathf.Sign(relativeXToMe) == otherDirSign;

                if (otherIsAheadOfMe && amIAheadOfOther) {
                    // ケース1: 正面衝突の各パターン
                    if (thisIsInvincible && otherIsInvincible) {
                        // 無敵正面 VS 無敵正面：お互いノーダメージでノックバック+反射
                        Debug.Log($"[INFO] {gameObject.name}(無敵) と {otherPlayer.gameObject.name}(無敵) が正面衝突。両者無敵のためノーダメージ、ノックバック+反転。");

                        // 正面衝突音を再生
                        if (AudioManager.Instance != null) {
                            AudioManager.Instance.PlaySE("Player_Penguin2Penguin");
                        }

                        ReverseDirection();
                        otherPlayer.ReverseDirection();

                        // 両者にノックバックを適用
                        Vector2 knockBackDirToMe = (transform.position - otherPlayer.transform.position).normalized;
                        if (knockBackDirToMe == Vector2.zero) knockBackDirToMe = (Random.insideUnitCircle).normalized;
                        rigidbody2D_.AddForce(knockBackDirToMe * 5f, ForceMode2D.Impulse);

                        Vector2 knockBackDirToOther = (otherPlayer.transform.position - transform.position).normalized;
                        if (knockBackDirToOther == Vector2.zero) knockBackDirToOther = (Random.insideUnitCircle).normalized;
                        otherPlayer.rigidbody2D_.AddForce(knockBackDirToOther * 5f, ForceMode2D.Impulse);
                    }
                    else if (thisIsInvincible && !otherIsInvincible) {
                        // 無敵正面 VS 通常正面：通常正面がダメージ+ノックバック+どちらも反転
                        Debug.Log($"[INFO] {gameObject.name}(無敵) と {otherPlayer.gameObject.name}(通常) が正面衝突。{otherPlayer.gameObject.name}にダメージ。");

                        // 正面衝突音を再生
                        if (AudioManager.Instance != null) {
                            AudioManager.Instance.PlaySE("Player_Penguin2Penguin");
                        }

                        otherPlayer.TakeDamage(20);
                        KnockBack(otherPlayer);
                        ReverseDirection();
                        otherPlayer.ReverseDirection();
                    }
                    else if (!thisIsInvincible && otherIsInvincible) {
                        // 通常正面 VS 無敵正面：通常正面がダメージ+ノックバック+どちらも反転
                        Debug.Log($"[INFO] {gameObject.name}(通常) と {otherPlayer.gameObject.name}(無敵) が正面衝突。{gameObject.name}にダメージ。");

                        // 正面衝突音を再生
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
                        // 通常状態同士の正面衝突：今まで通り
                        Debug.Log($"[INFO] {gameObject.name} と {otherPlayer.gameObject.name} が正面衝突。両者反転、ノックバック。");

                        // 正面衝突音を再生
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
                    // ケース2: 自分 (this) が相手 (otherPlayer) の背後から攻撃
                    if (!otherIsInvincible) {
                        // 相手が通常状態の場合のみダメージを与える
                        Debug.Log($"[INFO] {gameObject.name} が {otherPlayer.gameObject.name} の背後から攻撃。{otherPlayer.gameObject.name} にダメージ。");

                        // 背後衝突音を再生
                        if (AudioManager.Instance != null) {
                            AudioManager.Instance.PlaySE("Player_Penguin2Hip");
                        }

                        otherPlayer.TakeDamage(20);
                        KnockBack(otherPlayer);
                    }
                    else {
                        // 相手が無敵状態の場合はダメージなし
                        Debug.Log($"[INFO] {gameObject.name} が {otherPlayer.gameObject.name}(無敵) の背後から攻撃したが、ダメージなし。");

                        // 背後衝突音を再生
                        if (AudioManager.Instance != null) {
                            AudioManager.Instance.PlaySE("Player_Penguin2Hip");
                        }
                    }
                    // 背後攻撃時は反転しない（追突の自然な見た目を保持）
                }
                else if (!otherIsAheadOfMe && amIAheadOfOther) {
                    // ケース3: 相手 (otherPlayer) が自分 (this) の背後から攻撃
                    if (!thisIsInvincible) {
                        // 自分が通常状態の場合のみダメージを受ける
                        Debug.Log($"[INFO] {otherPlayer.gameObject.name} が {gameObject.name} の背後から攻撃。{gameObject.name} にダメージ。");

                        // 背後衝突音を再生
                        if (AudioManager.Instance != null) {
                            AudioManager.Instance.PlaySE("Player_Penguin2Hip");
                        }

                        TakeDamage(20);
                        Vector2 knockBackDirToThis = (transform.position - otherPlayer.transform.position).normalized;
                        if (knockBackDirToThis == Vector2.zero) knockBackDirToThis = (Random.insideUnitCircle).normalized;
                        rigidbody2D_.AddForce(knockBackDirToThis * 10f, ForceMode2D.Impulse);
                    }
                    else {
                        // 自分が無敵状態の場合はダメージなし
                        Debug.Log($"[INFO] {otherPlayer.gameObject.name} が {gameObject.name}(無敵) の背後から攻撃したが、ダメージなし。");

                        // 背後衝突音を再生
                        if (AudioManager.Instance != null) {
                            AudioManager.Instance.PlaySE("Player_Penguin2Hip");
                        }
                    }
                    // 背後攻撃時は反転しない（追突の自然な見た目を保持）
                }
                else {
                    Debug.Log($"[INFO] {gameObject.name} と {otherPlayer.gameObject.name} が衝突 (判定外のケース)。otherIsAheadOfMe: {otherIsAheadOfMe}, amIAheadOfOther: {amIAheadOfOther}");
                }
            }

            if (collision.gameObject.CompareTag("InvincibleItem")) {
                // アイテム取得SEを再生
                if (AudioManager.Instance != null) {
                    AudioManager.Instance.PlaySE("collision");
                }

                isInvincible_ = true;
                invincibilityTimer_ = 2.0f;
                Destroy(collision.gameObject);
            }
        }
    }


    //---------------------------------------------------------------
    //                      ダメージ処理
    public void TakeDamage(int amount) {
        // GameManagerに処理を委譲
        if (GameManager.Instance != null) {
            GameManager.Instance.TakeDamage(playerID_, amount);
            
            // 自身のHPも同期して更新
            currentHp_ = GameManager.Instance.GetPlayerCurrentHp(playerID_);
        }
        else {
            // GameManagerが存在しない場合の従来処理
            currentHp_ -= amount;
            Debug.Log($"[DAMAGE] : {gameObject.name} が {amount} ダメージを受けた（残りHP: {currentHp_}）");

            if (currentHp_ <= 0) {
                currentHp_ = 0;
                Debug.Log($"[GAME OVER] : {gameObject.name} が敗北しました");
            }
        }
    }

    //---------------------------------------------------------------
    //                      ノックバック処理（Player用）
    private void KnockBack(Player target) {
        Vector2 knockDir = (target.transform.position - transform.position).normalized;
        target.rigidbody2D_.AddForce(knockDir * 10f, ForceMode2D.Impulse);
    }

    //--------------------------------------------------------------
    //                      虹色エフェクト更新処理
    /// 無敵状態時にスプライトを虹色に輝かせる
    private void UpdateRainbowEffect() {
        if (spriteRenderer_ == null) return;

        // HSVカラーシステムを使用して虹色を生成
        // Hue（色相）を時間に応じて0-～1の範囲で変化させる
        float hue = (Time.time * rainbowSpeed_) % 1.0f;
        
        // HSVからRGBに変換して適用
        Color rainbowColor = Color.HSVToRGB(hue, 1.0f, 1.0f);
        
        // 元の透明度を維持
        rainbowColor.a = originalColor_.a;
        
        spriteRenderer_.color = rainbowColor;
    }

    //---------------------------------------------------------------
    //                      プレイヤー別デフォルトキー設定
    /// プレイヤーIDに基づいてデフォルトのキー設定を適用
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
            default:
                Debug.LogWarning($"[KEY SETTING] : 未知のプレイヤーID '{playerID_}'。デフォルト設定を使用します。");
                break;
        }
        Debug.Log($"[KEY SETTING] : Player {playerID_} - 左:{moveLeftKey_}, 右:{moveRightKey_}, ジャンプ:{jumpKey_}");
    }

    //---------------------------------------------------------------
    //                      キー設定モード切り替え
    /// キー設定モードのオン/オフを切り替え
    private void ToggleKeySettingMode() {
        isKeySettingMode_ = !isKeySettingMode_;
        if (isKeySettingMode_) {
            Debug.Log($"[KEY SETTING] : Player {playerID_} がキー設定モードに入りました");
            Debug.Log("1: 左移動キー設定, 2: 右移動キー設定, 3: ジャンプキー設定, ESC: 終了");
        }
        else {
            Debug.Log($"[KEY SETTING] : Player {playerID_} がキー設定モードを終了しました");
            settingTarget_ = "";
            pendingKey_ = KeyCode.None;
        }
    }

    //---------------------------------------------------------------
    //                      キー設定入力処理
    /// キー設定モード中の入力処理
    private void HandleKeySettingInput() {
        // ESCキーで設定モード終了
        if (Input.GetKeyDown(KeyCode.Escape)) {
            ToggleKeySettingMode();
            return;
        }

        // 設定対象が決まっていない場合、対象選択を処理
        if (string.IsNullOrEmpty(settingTarget_)) {
            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                settingTarget_ = "moveLeft";
                Debug.Log($"[KEY SETTING] : Player {playerID_} - 左移動キーを設定してください（現在: {moveLeftKey_}）");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2)) {
                settingTarget_ = "moveRight";
                Debug.Log($"[KEY SETTING] : Player {playerID_} - 右移動キーを設定してください（現在: {moveRightKey_}）");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3)) {
                settingTarget_ = "jump";
                Debug.Log($"[KEY SETTING] : Player {playerID_} - ジャンプキーを設定してください（現在: {jumpKey_}）");
            }
        }
        // 設定対象が決まっている場合、キー入力を待機
        else {
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode))) {
                if (Input.GetKeyDown(key) && key != KeyCode.Escape) {
                    SetKey(settingTarget_, key);
                    settingTarget_ = "";
                    Debug.Log("1: 左移動キー設定, 2: 右移動キー設定, 3: ジャンプキー設定, ESC: 終了");
                    break;
                }
            }
        }
    }

    //---------------------------------------------------------------
    //                      キー設定適用
    /// 指定されたキーを設定対象に適用
    private void SetKey(string target, KeyCode key) {
        switch (target) {
            case "moveLeft":
                moveLeftKey_ = key;
                Debug.Log($"[KEY SETTING] : Player {playerID_} の左移動キーを {key} に設定しました");
                break;
            case "moveRight":
                moveRightKey_ = key;
                Debug.Log($"[KEY SETTING] : Player {playerID_} の右移動キーを {key} に設定しました");
                break;
            case "jump":
                jumpKey_ = key;
                Debug.Log($"[KEY SETTING] : Player {playerID_} のジャンプキーを {key} に設定しました");
                break;
        }
    }

    //---------------------------------------------------------------
    //                      向き反転処理
    public void ReverseDirection() {
        currentDirection_ *= -1.0f;
        transform.localScale = new Vector3(-transform.localScale.x, 1, 1);
    }

    //---------------------------------------------------------------
    //                      連打ゲージ関連メソッド
    /// 現在のゲージ量に基づく速度倍率を計算（イーズイン効果付き）
    private float GetGaugeSpeedMultiplier() {
        if (currentComboGauge_ < minEffectiveGauge_) {
            return 1.0f; // ゲージが最低値未満の場合は通常速度
        }

        // ゲージの割合を計算（0.0～1.0）
        float gaugeRatio = (currentComboGauge_ - minEffectiveGauge_) / (maxComboGauge_ - minEffectiveGauge_);
        gaugeRatio = Mathf.Clamp01(gaugeRatio);

        // イーズイン効果を適用（2乗カーブで緩やかに始まり急激に上昇）
        float easedRatio = gaugeRatio * gaugeRatio;

        // 1.0から最大倍率まで補間
        return Mathf.Lerp(1.0f, maxGaugeSpeedMultiplier_, easedRatio);
    }

    /// 現在のゲージ量をパーセンテージで取得
    public float GetGaugePercentage() {
        return (currentComboGauge_ / maxComboGauge_) * 100.0f;
    }
    //→UIで欲しいのでprivateからpublicへ

    /// 現在のゲージレベルを取得（5段階）
    private int GetGaugeLevel() {
        float percentage = GetGaugePercentage();
        if (percentage >= 80.0f) return 5;
        if (percentage >= 60.0f) return 4;
        if (percentage >= 40.0f) return 3;
        if (percentage >= 20.0f) return 2;
        if (percentage >= minEffectiveGauge_ / maxComboGauge_ * 100.0f) return 1;
        return 0;
    }

    /// 現在の有効な移動速度を取得
    public float GetCurrentEffectiveSpeed() {
        float baseSpeed = autoMoveSpeed_;
        
        // スピードブーストの適用
        if (isSpeedBoosted_) {
            baseSpeed *= speedBoostMultiplier_;
        }

        // 無敵状態中の速度倍率
        if (isInvincible_) {
            baseSpeed *= invincibilitySpeedMultiplier_;
        }

        // 連打ゲージによる速度倍率（無敵状態中は適用しない）
        if (!isInvincible_) {
            float gaugeMultiplier = GetGaugeSpeedMultiplier();
            baseSpeed *= gaugeMultiplier;
        }

        return baseSpeed;
    }

    /// 指定された目標速度に到達するように連打ゲージを調整
    public void AdjustGaugeToAchieveSpeed(float targetSpeed) {
        // 基本速度を計算（スピードブーストと無敵状態を除く）
        float baseSpeed = autoMoveSpeed_;
        
        // 目標とする速度倍率を計算
        float targetMultiplier = targetSpeed / baseSpeed;
        
        // 速度倍率に基づいて連打ゲージを設定
        SetComboGaugeForTargetSpeedMultiplier(targetMultiplier);
        
        Debug.Log($"[GAUGE ADJUST] : {gameObject.name} - 目標速度:{targetSpeed:F2} のためゲージを {currentComboGauge_:F1} に調整");
    }

    /// 目標とする速度倍率に基づいて連打ゲージを設定
    private void SetComboGaugeForTargetSpeedMultiplier(float targetMultiplier) {
        if (targetMultiplier <= 1.0f) {
            currentComboGauge_ = 0.0f;
            return;
        }

        // イーズイン効果を考慮した逆算処理
        // targetMultiplier = Lerp(1.0, maxGaugeSpeedMultiplier_, easedRatio)
        // easedRatio = (targetMultiplier - 1.0) / (maxGaugeSpeedMultiplier_ - 1.0)
        float easedRatio = (targetMultiplier - 1.0f) / (maxGaugeSpeedMultiplier_ - 1.0f);
        easedRatio = Mathf.Clamp01(easedRatio);
        
        // イーズイン効果の逆関数を適用（平方根でeasedRatioから元のratioを復元）
        float requiredRatio = Mathf.Sqrt(easedRatio);
        
        // 実際のゲージ値を計算
        float requiredGauge = minEffectiveGauge_ + (requiredRatio * (maxComboGauge_ - minEffectiveGauge_));
        currentComboGauge_ = Mathf.Min(requiredGauge, maxComboGauge_);
    }

    /// ゲージ情報をデバッグ表示用に取得
    public string GetGaugeDebugInfo() {
        return $"Player {playerID_} - Gauge: {currentComboGauge_:F1}/{maxComboGauge_} ({GetGaugePercentage():F1}%) Level: {GetGaugeLevel()} Speed: x{GetGaugeSpeedMultiplier():F2}";
    }

    //---------------------------------------------------------------
    //                      踏みつけスタン処理
    /// 踏みつけによるスタン状態を適用
    public void ApplyStun(float stunDuration) {
        isStunned_ = true;
        stunTimer_ = stunDuration;
        Debug.Log($"[STUN] : {gameObject.name} が {stunDuration}秒間スタンしました");
    }

    ///--------------------------------------------------------------
    ///                      移動許可状態更新
    /// GameManagerの状態に基づいてプレイヤーの移動許可を更新
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
            
            // 移動許可状態が変更された場合のログ出力とリアクティベーション
            if (previousAllowMovement != allowMovement_) {
                Debug.Log($"[MOVEMENT PERMISSION] : {gameObject.name} の移動許可が {previousAllowMovement} -> {allowMovement_} に変更されました (GameState: {currentState})");
                
                // Playing状態になった時は緊急回復処理を実行
                if (allowMovement_ && currentState == GameManager.GameState.Playing) {
                    ForceReactivate();
                }
            }
        }
        else {
            allowMovement_ = false;
        }
    }

    ///--------------------------------------------------------------
    ///                      プレイヤー状態リセット
    /// ラウンド開始時にプレイヤーの状態を初期化
    public void ResetPlayerState() {
        // 物理状態をリセット
        if (rigidbody2D_ != null) {
            rigidbody2D_.velocity = Vector2.zero;
            rigidbody2D_.angularVelocity = 0f;
            rigidbody2D_.drag = dragOnStop_; // ドラッグ値も初期化
            rigidbody2D_.WakeUp(); // Rigidbody2Dを強制的に起動
        }

        // 移動関連の状態をリセット
        inputHorizontal_ = Vector2.zero;
        isJumping_ = false;
        currentDirection_ = 1.0f;

        // 連打ゲージをリセット
        currentComboGauge_ = 0.0f;
        isSpeedBoosted_ = false;
        speedBoostTimer_ = 0.0f;

        // 特殊状態をリセット
        isStunned_ = false;
        stunTimer_ = 0.0f;
        hasDoubleJumped_ = false;
        shouldReverseOnLanding_ = false;

        // 無敵状態をリセット
        isInvincible_ = false;
        invincibilityTimer_ = 0.0f;
        if (spriteRenderer_ != null) {
            spriteRenderer_.color = originalColor_;
        }

        // アニメーション状態をリセット
        if (animator_ != null) {
            animator_.SetBool("Run", false);
            animator_.SetBool("Jump", false);
        }

        // スプライト回転をリセット
        targetRotation_ = 0.0f;
        currentRotation_ = 0.0f;
        if (spriteTransform_ != null) {
            spriteTransform_.localRotation = Quaternion.identity;
        }

        // 壁衝突状態をリセット
        wasHittingWall_ = false;
        
        // 移動許可状態を一時的にリセット（カウントダウン終了後に有効化される）
        allowMovement_ = false;

        Debug.Log($"[PLAYER RESET] : {gameObject.name} の状態がリセットされました");
    }
    
    ///--------------------------------------------------------------
    ///                      緊急回復処理
    /// プレイヤーが完全に動かなくなった場合の緊急回復処理
    public void ForceReactivate() {
        // 物理状態を強制的にリセット
        if (rigidbody2D_ != null) {
            rigidbody2D_.velocity = Vector2.zero;
            rigidbody2D_.angularVelocity = 0f;
            rigidbody2D_.drag = dragOnMove_;
            rigidbody2D_.WakeUp(); // Rigidbody2Dを強制的に起動
            
            // 物理マテリアルも確認
            if (rigidbody2D_.sharedMaterial != null) {
                rigidbody2D_.sharedMaterial = null; // 一時的にマテリアルを削除
            }
        }
        
        // 全ての阻害要因をリセット
        isStunned_ = false;
        stunTimer_ = 0.0f;
        
        // 入力状態をクリア
        inputHorizontal_ = Vector2.zero;
        isJumping_ = false;
        
        // 移動許可を強制的に有効化（既に有効でも確実に設定）
        allowMovement_ = true;
        
        // 自動移動モードの場合、方向を確実に設定
        if (isAutoMode_) {
            if (currentDirection_ == 0.0f) {
                currentDirection_ = 1.0f; // デフォルトで右方向
            }
        }
        
        Debug.Log($"[FORCE REACTIVATE] : {gameObject.name} が緊急回復処理を実行しました (allowMovement_: {allowMovement_}, currentDirection_: {currentDirection_})");
    }
}