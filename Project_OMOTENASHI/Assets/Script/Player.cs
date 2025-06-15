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
    // 【水平移動パラメータ】
    // プレイヤーの左右移動に関する設定値
    [Header("水平移動設定")]
    [Tooltip("プレイヤーの最大移動速度（単位/秒）")]
    public float maxSpeed_ = 5.0f;

    [Tooltip("移動時の加速力。高いほど素早く最大速度に到達")]
    public float acceleration_ = 15.0f;

    [Tooltip("停止時の抵抗値。高いほど急ブレーキ")]
    public float dragOnStop_ = 5.0f;

    [Tooltip("移動中の抵抗値。通常は0で慣性を保持")]
    public float dragOnMove_ = 0.0f;

    //========================================
    // 【接地判定システム】
    // 地面との接触状態を検出するための参照
    [Header("接地判定")]
    [Tooltip("地面との接触を判定するGroundCheckコンポーネント")]
    public GroundCheck groundCheck_;

    //========================================
    // 【垂直移動パラメータ】
    // ジャンプと重力制御に関する設定値
    [Header("ジャンプ・重力設定")]
    [Tooltip("独自重力の強さ。Unityのデフォルト重力に追加される")]
    public float gravity_ = 0.2f;

    [Tooltip("ジャンプ時に加える瞬間的な力の大きさ")]
    public float jumpForce_ = 4.0f;

    [Tooltip("ジャンプの理論的最大高度（現在未使用）")]
    public float jumpHeight_ = 4.0f;

    //========================================
    // 【壁接触判定システム】
    // 左右の壁との接触を検出し、壁への張り付きを防ぐ
    [Header("壁接触判定")]
    [Tooltip("前方（進行方向）の壁を検出するコンポーネント")]
    public WallCheckFront wallCheckFront_;

    [Tooltip("後方の壁を検出するコンポーネント")]
    public WallCheckBuck wallCheckBuck_;

    //========================================
    // 【プレイヤーステータス】
    // HPや体力などのゲーム内パラメータ
    [Header("プレイヤーステータス")]
    [Tooltip("プレイヤーの最大HP値")]
    public int maxHp_ = 100;

    [Tooltip("プレイヤーの現在HP値")]
    public int currentHp_ = 100;

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
    // 【無敵状態設定】
    [Header("無敵状態設定")]
    [Tooltip("壁反射後の無敵時間（秒）")]
    public float invincibilityDuration_ = 3.0f;

    [Tooltip("虹色エフェクトの変化速度")]
    public float rainbowSpeed_ = 2.0f;

    [Tooltip("無敵状態中の移動速度倍率")]
    public float invincibilitySpeedMultiplier_ = 2.0f;


    ///--------------------------------------------------------------
    ///                      【プライベート変数】
    //========================================
    // 【コンポーネント参照】
    // 必要なUnityコンポーネントへの参照を保持
    private Animator animator_ = null;          // アニメーション制御用
    private Rigidbody2D rigidbody2D_ = null;    // 物理演算制御用
    private SpriteRenderer spriteRenderer_ = null; // スプライト色変更用

    //========================================
    // 【状態フラグ】
    // プレイヤーの現在の行動状態を追跡
    public bool isGround_ = false;             // 地面に接触している状態
    public bool isJumping_ = false;            // ジャンプ動作を実行中

    //========================================
    // 【入力データ】
    // プレイヤーからの入力情報を一時保存
    private Vector2 inputHorizontal_ = Vector2.zero;  // 左右移動入力（-1～1）
    private Vector2 inputVertical_ = Vector2.zero;    // 上下入力（現在未使用）

    //========================================
    // 【壁接触状態】
    // 各方向の壁との接触状況を記録
    private bool isHitWallFront_ = false;      // 前方の壁に接触中
    private bool isHitWallBuck_ = false;       // 後方の壁に接触中

    //========================================
    // 【定数設定】
    //private string groundTag_ = "Ground";       // 地面として認識するオブジェクトのタグ

    //========================================
    // 【自動移動関連】
    private float currentDirection_ = 1.0f;     // 現在の移動方向（1.0f=右、-1.0f=左）
    private bool wasHittingWall_ = false;       // 前フレームで壁に衝突していたか
    private float speedBoostTimer_ = 0.0f;      // スピードブーストの残り時間
    private bool isSpeedBoosted_ = false;       // 現在スピードブースト中か

    //========================================
    // 【無敵状態関連】
    private bool isInvincible_ = false;         // 現在無敵状態か
    private float invincibilityTimer_ = 0.0f;   // 無敵状態の残り時間
    private Color originalColor_ = Color.white; // 元の色を保存用


    ///--------------------------------------------------------------
    ///                      初期化処理
    /// ゲーム開始時の初期化処理
    /// 必要なコンポーネントの取得と初期状態の設定を行う
    private void Start() {
        //========================================
        // アニメーター取得と存在確認
        animator_ = GetComponent<Animator>();
        if (animator_ == null) {
            Debug.LogError("[ERROR] : Animator component not found on Player object. アニメーション制御ができません。");
        }
        //========================================
        // リジッドボディ取得と存在確認
        rigidbody2D_ = GetComponent<Rigidbody2D>();
        if (rigidbody2D_ == null) {
            Debug.LogError("[ERROR] : Rigidbody2D component not found on Player object. 物理演算制御ができません。");
        }
        //========================================
        // スプライトレンダラー取得と存在確認
        spriteRenderer_ = GetComponent<SpriteRenderer>();
        if (spriteRenderer_ == null) {
            Debug.LogError("[ERROR] : SpriteRenderer component not found on Player object. 色変更エフェクトができません。");
        }
        else {
            // 元の色を保存
            originalColor_ = spriteRenderer_.color;
        }
    }


    ///--------------------------------------------------------------
    ///                      メインループ処理
    private void Update() {
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

        //========================================
        // 【移動処理の振り分け】
        if (isAutoMode_) {
            AutoMove();
        }
        else {
            Move();
        }
    }


    ///--------------------------------------------------------------
    ///                      物理演算更新処理
    private void FixedUpdate() {
        //========================================
        // 【環境状態の更新】
        // 周囲の壁や地面との接触状況を最新状態に更新
        isGround_ = groundCheck_.IsGround();                    // 足元の地面接触判定
        isHitWallFront_ = wallCheckFront_.IsHitWallFront();    // 前方壁接触判定
        isHitWallBuck_ = wallCheckBuck_.IsHitWallBuck();       // 後方壁接触判定

        //========================================
        // 【水平移動の物理計算】
        // 現在速度と目標速度の差分から必要な力を算出
        float currentVelocityX = rigidbody2D_.velocity.x;      // 現在のX軸速度を取得
        float targetSpeed = inputHorizontal_.x * maxSpeed_;     // 入力に基づく目標速度を計算
        float speedDiff = targetSpeed - currentVelocityX;      // 速度差分を算出
        float accelRate = acceleration_;                        // 加速度を適用
        float movement = speedDiff * accelRate;                 // 実際に加える力を計算

        //========================================
        // 【壁張り付き防止システム】
        // 壁に向かって移動入力がある場合は移動を制限し、
        // 壁に張り付いて動けなくなる現象を防ぐ
        bool blockFront = isHitWallFront_ && inputHorizontal_.x < 0;  // 前方壁があり左入力時
        bool blockBuck = isHitWallFront_ && inputHorizontal_.x > 0;   // 後方壁があり右入力時

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
    ///                      入力処理・移動制御
    private void Move() {
        //========================================
        // 【入力データの取得】
        // Input Managerで設定された軸から入力値を取得
        float horizontal = Input.GetAxisRaw("Horizontal");     // 水平入力（-1, 0, 1）
        bool jumpPressed = Input.GetButtonDown("Jump");        // ジャンプボタンの押下判定

        //========================================
        // 【入力データの保存】
        // 取得した入力をベクトル形式で保存し、物理演算処理で使用
        inputHorizontal_ = new Vector2(horizontal, 0.0f);      // 水平入力をベクトル化
        inputVertical_ = Vector2.zero;                          // 垂直入力（将来の拡張用）

        //========================================
        // 【ジャンプ入力の処理】
        // 地面に接触している時のみジャンプを許可
        // 空中での多段ジャンプを防止
        if (isGround_) {
            if (jumpPressed) {
                isJumping_ = true;  // ジャンプフラグを立てる（FixedUpdateで実行）
            }
        }
        else {
            isJumping_ = false;     // 空中ではジャンプフラグを無効化
        }

        //========================================
        // 【アニメーション状態の更新】
        // 現在の状態をAnimatorに送信してアニメーション制御
        animator_.SetBool("Jump", !isGround_);  // 空中にいる間はジャンプアニメーション

        //========================================
        // 【キャラクター向きとアニメーションの制御】
        // 移動方向に応じてスプライトを反転し、適切なアニメーションを再生
        if (horizontal > 0) {
            // 右方向への移動
            transform.localScale = new Vector3(-1, 1, 1);       // 右向きに反転
            animator_.SetBool("Run", true);                     // 走行アニメーション開始
        }
        else if (horizontal < 0) {
            // 左方向への移動
            transform.localScale = new Vector3(1, 1, 1);        // 左向き（標準方向）
            animator_.SetBool("Run", true);                     // 走行アニメーション開始
        }
        else {
            // 停止状態
            animator_.SetBool("Run", false);                    // 待機アニメーション
        }
    }

    //---------------------------------------------------------------
    //                      自動移動制御
    // プレイヤーの向いている方に進行｡壁にあたった場合は反転する｡
    // 操作はジャンプは可能だが、移動は連打でスピードが上がるのみ｡
    private void AutoMove() {
        //========================================
        // 【壁衝突判定と方向転換】
        // 前方の壁に衝突した場合、移動方向を反転
        if (isHitWallFront_ && !wasHittingWall_) {
            currentDirection_ *= -1.0f;  // 移動方向を反転
            
            // 壁反射時に無敵状態を付与
            isInvincible_ = true;
            invincibilityTimer_ = invincibilityDuration_;
            Debug.Log($"[AUTO MOVE] : {gameObject.name} が壁に衝突しました。方向を反転し、無敵状態になりました。新しい方向: {(currentDirection_ > 0 ? "右" : "左")}");
        }
        wasHittingWall_ = isHitWallFront_;  // 前フレームの壁衝突状態を保存

        //========================================
        // 【連打によるスピードアップ処理】
        bool moveInputPressed = Input.GetButtonDown("Horizontal") ||
                               Input.GetKeyDown(KeyCode.A) ||
                               Input.GetKeyDown(KeyCode.D) ||
                               Input.GetKeyDown(KeyCode.LeftArrow) ||
                               Input.GetKeyDown(KeyCode.RightArrow);

        if (moveInputPressed) {
            // 移動入力が押された時：スピードブーストを発動
            isSpeedBoosted_ = true;
            speedBoostTimer_ = speedBoostDuration_;
        }

        // スピードブーストタイマーの更新
        if (isSpeedBoosted_) {
            speedBoostTimer_ -= Time.deltaTime;
            if (speedBoostTimer_ <= 0.0f) {
                isSpeedBoosted_ = false;
            }
        }

        //========================================
        // 【ジャンプ入力の処理】
        // 地面に接触している時のみジャンプを許可
        bool jumpPressed = Input.GetButtonDown("Jump");
        if (isGround_ && jumpPressed) {
            isJumping_ = true;
        }
        else if (!isGround_) {
            isJumping_ = false;
        }

        //========================================
        // 【自動移動の実行】
        // 現在の方向に基づいて移動入力を設定
        float currentSpeed = autoMoveSpeed_;
        if (isSpeedBoosted_) {
            currentSpeed *= speedBoostMultiplier_;
        }

        // 無敵状態中は高速移動
        if (isInvincible_) {
            currentSpeed *= invincibilitySpeedMultiplier_;
        }

        inputHorizontal_ = new Vector2(currentDirection_ * currentSpeed / maxSpeed_, 0.0f);
        inputVertical_ = Vector2.zero;

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
        if (collision.gameObject.CompareTag("Player")) {
            Player otherPlayer = collision.gameObject.GetComponent<Player>();
            if (otherPlayer == null || otherPlayer == this) return;

            // 衝突処理の重複を防ぐため、GetInstanceIDが小さい方のオブジェクトは処理をスキップします。
            // これにより、衝突ペアに対して一度だけ判定ロジックが実行されるようになります。
            if (gameObject.GetInstanceID() < otherPlayer.gameObject.GetInstanceID()) {
                return;
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
                    otherPlayer.TakeDamage(20);
                    KnockBack(otherPlayer);
                    ReverseDirection();
                    otherPlayer.ReverseDirection();
                }
                else if (!thisIsInvincible && otherIsInvincible) {
                    // 通常正面 VS 無敵正面：通常正面がダメージ+ノックバック+どちらも反転
                    Debug.Log($"[INFO] {gameObject.name}(通常) と {otherPlayer.gameObject.name}(無敵) が正面衝突。{gameObject.name}にダメージ。");
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
                    Debug.Log($"[INFO] {gameObject.name} が {otherPlayer.gameObject.name} の背後から攻撃。{otherPlayer.gameObject.name} にダメージ。{gameObject.name} は反転。");
                    otherPlayer.TakeDamage(20);
                    KnockBack(otherPlayer);
                } else {
                    // 相手が無敵状態の場合はダメージなし
                    Debug.Log($"[INFO] {gameObject.name} が {otherPlayer.gameObject.name}(無敵) の背後から攻撃したが、ダメージなし。{gameObject.name} は反転。");
                }
                ReverseDirection();
            }
            else if (!otherIsAheadOfMe && amIAheadOfOther) {
                // ケース3: 相手 (otherPlayer) が自分 (this) の背後から攻撃
                if (!thisIsInvincible) {
                    // 自分が通常状態の場合のみダメージを受ける
                    Debug.Log($"[INFO] {otherPlayer.gameObject.name} が {gameObject.name} の背後から攻撃。{gameObject.name} にダメージ。{otherPlayer.gameObject.name} は反転。");
                    TakeDamage(20);
                    Vector2 knockBackDirToThis = (transform.position - otherPlayer.transform.position).normalized;
                    if (knockBackDirToThis == Vector2.zero) knockBackDirToThis = (Random.insideUnitCircle).normalized;
                    rigidbody2D_.AddForce(knockBackDirToThis * 10f, ForceMode2D.Impulse);
                } else {
                    // 自分が無敵状態の場合はダメージなし
                    Debug.Log($"[INFO] {otherPlayer.gameObject.name} が {gameObject.name}(無敵) の背後から攻撃したが、ダメージなし。{otherPlayer.gameObject.name} は反転。");
                }
                otherPlayer.ReverseDirection();
            }
            else {
                Debug.Log($"[INFO] {gameObject.name} と {otherPlayer.gameObject.name} が衝突 (判定外のケース)。otherIsAheadOfMe: {otherIsAheadOfMe}, amIAheadOfOther: {amIAheadOfOther}");
            }
        }
    }


    //---------------------------------------------------------------
    //                      ダメージ処理
    public void TakeDamage(int amount) {
        currentHp_ -= amount;
        Debug.Log($"[DAMAGE] : {gameObject.name} が {amount} ダメージを受けた（残りHP: {currentHp_}）");

        if (currentHp_ <= 0) {
            currentHp_ = 0;
            GameManager.Instance.CurrentGameState = GameManager.GameState.GameOver;

            // 勝者を通知
            if (gameObject.name.Contains("1")) {
                GameManager.Instance.CurrentWinner = GameManager.Winner.Player2;
            }
            else {
                GameManager.Instance.CurrentWinner = GameManager.Winner.Player1;
            }

            Debug.Log($"[GAME OVER] : 勝者は {GameManager.Instance.CurrentWinner}");
        }
    }

    //---------------------------------------------------------------
    //                      ノックバック処理（Player用）
    private void KnockBack(Player target) {
        Vector2 knockDir = (target.transform.position - transform.position).normalized;
        target.rigidbody2D_.AddForce(knockDir * 10f, ForceMode2D.Impulse);
    }

    //---------------------------------------------------------------
    //                      虹色エフェクト更新処理
    /// 無敵状態時にスプライトを虹色に輝かせる
    private void UpdateRainbowEffect() {
        if (spriteRenderer_ == null) return;

        // HSVカラーシステムを使用して虹色を生成
        // Hue（色相）を時間に応じて0～1の範囲で変化させる
        float hue = (Time.time * rainbowSpeed_) % 1.0f;
        
        // HSVからRGBに変換して適用
        Color rainbowColor = Color.HSVToRGB(hue, 1.0f, 1.0f);
        
        // 元の透明度を維持
        rainbowColor.a = originalColor_.a;
        
        spriteRenderer_.color = rainbowColor;
    }

    //---------------------------------------------------------------
    //                      向き反転処理
    public void ReverseDirection() {
        currentDirection_ *= -1.0f;
        transform.localScale = new Vector3(-transform.localScale.x, 1, 1);
    }
}