using UnityEngine;
using DG.Tweening; // DOTweenを使用するために必要

/// <summary>
/// DOTween演出版：キーの長押しでチャージし、離すとパンチを繰り出すギミック。
/// </summary>
public class Push : MonoBehaviour
{
    //================================================
    // 【公開プロパティ】
    //================================================
    /// <summary>
    /// 現在のパンチの強さ（外部から参照可能）
    /// </summary>
    public float CurrentPunchPower { get; private set; }

    //================================================
    // 【状態定義】
    //================================================
    public enum MachineState
    {
        Wait,           // 入力待機中
        Charging,       // チャージ中
        ChargedWait,    // チャージ完了待機中
        Action,         // 発射〜戻りまでの動作中
        Combo,          // コンボ入力中
    }

    //================================================
    // 【インスペクター設定】
    //================================================

    [Header("▼ 起動キー設定")]
    [Tooltip("アクションを起動するキー")]
    [SerializeField] private KeyCode triggerKey_ = KeyCode.Alpha1;

    [Header("▼ コンボ入力設定")]
    [Tooltip("コンボ用キー1")]
    [SerializeField] private KeyCode comboKey1_ = KeyCode.Z;
    [Tooltip("コンボ用キー2")]
    [SerializeField] private KeyCode comboKey2_ = KeyCode.X;
    [Tooltip("コンボでフルチャージになるまでの必要入力回数")]
    [SerializeField] private int maxComboCount_ = 10;
    [Tooltip("コンボ入力の制限時間（秒）")]
    [SerializeField] private float comboTimeLimit_ = 3f;

    [Header("▼ チャージ設定")]
    [Tooltip("最大チャージ時に後退するX方向の距離")]
    [SerializeField] private float maxChargeDistance_ = 3f;

    [Tooltip("フルチャージになるまでの秒数")]
    [SerializeField] private float maxChargeTime_ = 2f;

    [Tooltip("チャージ完了後の最大待機時間（秒）")]
    [SerializeField] private float maxChargedWaitTime_ = 1.0f;

    [Header("▼ プルプル振動（手動実装）")]
    [Tooltip("揺れの速さ")]
    [SerializeField] private float wobbleSpeed_ = 50f;
    [Tooltip("X方向の揺れ幅")]
    [SerializeField] private float wobbleAmountX_ = 0.05f;
    [Tooltip("Y方向の揺れ幅")]
    [SerializeField] private float wobbleAmountY_ = 0.05f;


    [Header("▼ アクション設定")]
    [Tooltip("発射にかかる秒数")]
    [SerializeField] private float shotDuration_ = 0.8f;

    [Tooltip("プレイヤーが取得するパワーの最大値")]
    [SerializeField] private float maxPunchPower_ = 100f;

    [Tooltip("バネの振れ幅（大きいほど大きく揺れる）")]
    [SerializeField] private float elasticAmplitude_ = 1.0f;

    [Tooltip("バネの振動（小さいほど細かく揺れる）")]
    [SerializeField] private float elasticPeriod_ = 0.3f;

    [Tooltip("発射後の停止秒数")]
    [SerializeField] private float pauseDuration_ = 0.5f;

    [Tooltip("元の位置に戻るのにかかる秒数")]
    [SerializeField] private float returnDuration_ = 1.0f;

    [Header("▼ オブジェクト設定")]
    [Tooltip("パンチングに使うコライダー付きのGameObject")]
    [SerializeField] private Collider2D punchingCollider_;

    [Tooltip("伸縮させるバネのTransform")]
    [SerializeField] private Transform springTransform_;

    [Tooltip("バネの固定点（アンカー）のTransform")]
    [SerializeField] private Transform springAnchor_;

    [Tooltip("バネのXスケールにかける補正値")]
    [SerializeField] private float springScaleMultiplier_ = 1.0f;

    [Header("▼ バネの伸縮表現")]
    [Tooltip("バネがX軸で縮んだ時にY軸に膨らむ度合い")]
    [SerializeField] private float springSquashFactor_ = 0.5f;

    [Tooltip("バネのYスケールの最小値（細さの限界）")]
    [SerializeField] private float minSpringScaleY_ = 0.5f;

    [Tooltip("バネのYスケールの最大値（太さの限界）")]
    [SerializeField] private float maxSpringScaleY_ = 1.5f;


    //================================================
    // 【プライベート変数】
    //================================================
    private MachineState currentState_ = MachineState.Wait;
    private Vector3 basePos_;                   // コライダーの初期ローカル位置
    private Vector3 chargedPos_;                // チャージ完了時の位置
    private Sequence actionSeq_;                // 発射〜戻りまでのシーケンス
    private float currentChargeTime_ = 0f;      // 現在のチャージ経過時間
    private float currentChargeDistance_ = 0f;  // 現在の後退距離
    private float chargedWaitTimer_ = 0f;       // チャージ完了後の待機タイマー
    private SpriteRenderer spriteRenderer_;     // スプライトレンダラー
    private Color baseColor_;                   // 初期のスプライト色
    private float baseSpringSpriteWidth_;       // バネ画像の初期幅
    private float baseSpringScaleY_;            // バネの初期Yスケール
    private float baseSpringDistance_;          // バネの初期距離

    // コンボ関連の変数
    private int currentComboCount_ = 0;         // 現在のコンボ数
    private float comboTimer_ = 0f;             // コンボ制限時間のタイマー

    //================================================
    // 【Unity ライフサイクル】
    //================================================

    private void Awake()
    {
        if (punchingCollider_ != null)
        {
            basePos_ = punchingCollider_.transform.localPosition;
            spriteRenderer_ = punchingCollider_.GetComponent<SpriteRenderer>();
            if (spriteRenderer_ != null)
            {
                baseColor_ = spriteRenderer_.color;
            }
            else
            {
                Debug.LogWarning("[PUSH] : punchingCollider_ に SpriteRenderer が見つかりません。色の変更は行われません。", this);
            }
        }
        else
        {
            Debug.LogWarning("[PUSH] : punchingCollider_ が設定されていません", this);
        }

        // バネの初期設定
        if (springTransform_ != null && springAnchor_ != null)
        {
            // スケール計算の基準となる、スプライトの元々の幅を取得
            var springRenderer = springTransform_.GetComponent<SpriteRenderer>();
            if (springRenderer != null && springRenderer.sprite != null)
            {
                baseSpringSpriteWidth_ = springRenderer.sprite.bounds.size.x;
            }
            else
            {
                baseSpringSpriteWidth_ = 1f; // SpriteRendererがなければ1を基準とする
                Debug.LogWarning("[PUSH] : springTransform_ に SpriteRenderer または Sprite が見つかりません。スケール計算の基準が1になります。", this);
            }

            // Yスケールと距離の初期値を取得
            baseSpringScaleY_ = springTransform_.localScale.y;
            baseSpringDistance_ = Vector3.Distance(springAnchor_.position, punchingCollider_.transform.position);
        }

        SetState(MachineState.Wait);
    }

    private void Update()
    {
        switch (currentState_)
        {
            case MachineState.Wait:
                if (Input.GetKeyDown(triggerKey_))
                {
                    StartCharge();
                }
                else if (Input.GetKeyDown(comboKey1_) || Input.GetKeyDown(comboKey2_))
                {
                    StartCombo();
                }
                break;

            case MachineState.Combo:
                // コンボ入力の検出
                if (Input.GetKeyDown(comboKey1_) || Input.GetKeyDown(comboKey2_))
                {
                    AddComboInput();
                }

                // コンボ制限時間の管理
                comboTimer_ += Time.deltaTime;
                if (comboTimer_ >= comboTimeLimit_)
                {
                    // 時間切れ - 即座に発射
                    ReleaseCharge();
                }
                break;

            case MachineState.Charging:
                // チャージ時間を加算
                currentChargeTime_ += Time.deltaTime;

                // チャージ率を計算 (0.0 ~ 1.0)
                float chargeRatio = Mathf.Clamp01(currentChargeTime_ / maxChargeTime_);

                // Ease.OutQuadを再現: t * (2 - t)
                float easedRatio = chargeRatio * (2 - chargeRatio);

                // 現在の後退距離と位置を更新
                currentChargeDistance_ = maxChargeDistance_ * easedRatio;
                punchingCollider_.transform.localPosition = new Vector3(basePos_.x - currentChargeDistance_, basePos_.y, basePos_.z);

                // キーを離したら発射
                if (Input.GetKeyUp(triggerKey_))
                {
                    ReleaseCharge();
                }
                // チャージが完了したら即座に発射
                else if (currentChargeTime_ >= maxChargeTime_)
                {
                    ReleaseCharge();
                }
                break;

            case MachineState.ChargedWait:
                // 手動でプルプルさせる
                WobbleObject();

                chargedWaitTimer_ += Time.deltaTime;
                if (Input.GetKeyUp(triggerKey_) || chargedWaitTimer_ >= maxChargedWaitTime_)
                {
                    ReleaseCharge();
                }
                break;

            case MachineState.Action:
                // アクション中は入力を受け付けない
                break;
        }

        // 毎フレーム、バネの見た目を更新する
        UpdateSpring();
    }

    /// <summary>
    /// チャージ動作を開始する
    /// </summary>
    private void StartCharge()
    {
        SetState(MachineState.Charging);
    }

    /// <summary>
    /// コンボ入力を開始する
    /// </summary>
    private void StartCombo()
    {
        SetState(MachineState.Combo);
        currentComboCount_ = 1; // 最初の入力をカウント
        comboTimer_ = 0f;
        UpdateComboCharge();
    }

    /// <summary>
    /// コンボ入力を追加する
    /// </summary>
    private void AddComboInput()
    {
        currentComboCount_++;
        UpdateComboCharge();

        // 最大コンボ数に達したら即座に発射
        if (currentComboCount_ >= maxComboCount_)
        {
            ReleaseCharge();
        }
    }

    /// <summary>
    /// コンボ数に応じてチャージ状態を更新する
    /// </summary>
    private void UpdateComboCharge()
    {
        // コンボ率を計算 (0.0 ~ 1.0)
        float comboRatio = Mathf.Clamp01((float)currentComboCount_ / maxComboCount_);

        // Ease.OutQuadを再現: t * (2 - t)
        float easedRatio = comboRatio * (2 - comboRatio);

        // 現在の後退距離と位置を更新
        currentChargeDistance_ = maxChargeDistance_ * easedRatio;
        punchingCollider_.transform.localPosition = new Vector3(basePos_.x - currentChargeDistance_, basePos_.y, basePos_.z);

        Debug.Log($"[COMBO] : {currentComboCount_}/{maxComboCount_} (距離: {currentChargeDistance_:F2})");
    }

    /// <summary>
    /// コンボ状態からチャージ完了待機状態に移行する
    /// </summary>
    private void TransitionToChargedWait()
    {
        SetState(MachineState.ChargedWait);
        // 揺れの基準となる位置を保存
        chargedPos_ = punchingCollider_.transform.localPosition;
        chargedWaitTimer_ = 0f;
    }

    /// <summary>
    /// オブジェクトをサイン波で揺らす
    /// </summary>
    private void WobbleObject()
    {
        // 時間を元にサイン波を生成。異なる周波数でXとYを揺らすと、より自然に見える
        float offsetX = Mathf.Sin(Time.time * wobbleSpeed_) * wobbleAmountX_;
        float offsetY = Mathf.Sin(Time.time * wobbleSpeed_ * 1.2f) * wobbleAmountY_; // Yは少し速く

        // チャージ完了時の位置を基準に揺らす
        punchingCollider_.transform.localPosition = new Vector3(
            chargedPos_.x + offsetX,
            chargedPos_.y + offsetY,
            chargedPos_.z
        );
    }

    /// <summary>
    /// チャージを解放し、アクションを開始する
    /// </summary>
    private void ReleaseCharge()
    {
        if (currentState_ != MachineState.Charging && currentState_ != MachineState.ChargedWait && currentState_ != MachineState.Combo) return;

        // 揺れを止めて位置を補正
        punchingCollider_.transform.localPosition = new Vector3(basePos_.x - currentChargeDistance_, basePos_.y, basePos_.z);

        // ★★★ 変更点：パンチパワーを計算して設定 ★★★
        float chargeRatio = (maxChargeDistance_ > 0) ? Mathf.Clamp01(currentChargeDistance_ / maxChargeDistance_) : 0;
        CurrentPunchPower = maxPunchPower_ * chargeRatio;

        SetState(MachineState.Action);

        float shotDistance = currentChargeDistance_ * 2;
        float targetX = basePos_.x + shotDistance;

        PlayActionSequence(targetX);
    }

    /// <summary>
    /// 発射から戻りまでの一連のシーケンスを再生する
    /// </summary>
    /// <param name="targetX">発射の目標X座標</param>
    private void PlayActionSequence(float targetX)
    {
        actionSeq_?.Kill();

        actionSeq_ = DOTween.Sequence()
            .AppendCallback(() => SetTag("Punching"))
            .Append(
                punchingCollider_.transform
                    .DOLocalMoveX(targetX, shotDuration_)
                    .SetEase(Ease.OutElastic, elasticAmplitude_, elasticPeriod_)
            )
            .Join(
                punchingCollider_.transform
                    .DOPunchScale(new Vector3(0, -0.2f, 0), shotDuration_ * 0.2f, 1, 0.5f)
            )
            .AppendInterval(pauseDuration_)
            .AppendCallback(() => SetTag("Ground"))
            .Append(
                punchingCollider_.transform
                    .DOLocalMoveX(basePos_.x, returnDuration_)
                    .SetEase(Ease.InOutSine)
                    .OnStart(() => {
                        if (spriteRenderer_ != null)
                        {
                            spriteRenderer_.DOFade(0.4f, 0.2f);
                        }
                        // 半透明時にコライダーをオフにする
                        if (punchingCollider_ != null)
                        {
                            punchingCollider_.enabled = false;
                        }
                    })
            )
            .SetLink(gameObject)
            .OnComplete(() =>
            {
                punchingCollider_.transform.localPosition = basePos_;
                if (spriteRenderer_ != null)
                {
                    spriteRenderer_.color = baseColor_;
                }
                // 動作完了時にコライダーをオンに戻す
                if (punchingCollider_ != null)
                {
                    punchingCollider_.enabled = true;
                }

                // ★★★ 変更点：パンチパワーをリセット ★★★
                CurrentPunchPower = 0f;

                SetState(MachineState.Wait);
            });
    }

    //================================================
    // 【状態・見た目制御】
    //================================================

    private void SetState(MachineState state)
    {
        // 新しい状態に遷移する前の初期化処理
        if (state == MachineState.Wait || state == MachineState.Charging)
        {
            currentChargeTime_ = 0f;
            currentChargeDistance_ = 0f;
        }
        else if (state == MachineState.Combo)
        {
            currentComboCount_ = 0;
            comboTimer_ = 0f;
            currentChargeDistance_ = 0f;
        }
        else if (state == MachineState.ChargedWait)
        {
            chargedWaitTimer_ = 0f;
            // 揺れの基準となる位置を保存
            chargedPos_ = punchingCollider_.transform.localPosition;
        }

        currentState_ = state;
        Debug.Log($"[MODE CHANGE] : 状態遷移 → {state}");
    }

    private void SetTag(string tagName)
    {
        if (punchingCollider_ != null && punchingCollider_.gameObject.tag != tagName)
        {
            punchingCollider_.gameObject.tag = tagName;
        }
    }

    /// <summary>
    /// バネの見た目（位置、回転、スケール）を更新する
    /// </summary>
    private void UpdateSpring()
    {
        // 必要なオブジェクトが設定されていなければ何もしない
        if (springTransform_ == null || springAnchor_ == null || punchingCollider_ == null)
        {
            return;
        }

        // 2点間のベクトルと距離を計算
        Vector3 anchorPos = springAnchor_.position;
        Vector3 blockPos = punchingCollider_.transform.position;
        Vector3 direction = blockPos - anchorPos;
        float distance = direction.magnitude;

        // 1. 位置を2点の中間に設定
        springTransform_.position = (anchorPos + blockPos) / 2f;

        // 2. 回転を2点間を結ぶ方向に向ける
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        springTransform_.rotation = Quaternion.Euler(0, 0, angle);

        // 3. スケールを距離に合わせて変更
        float newScaleX = 1f;
        if (baseSpringSpriteWidth_ > 0.001f)
        {
            newScaleX = (distance / baseSpringSpriteWidth_) * springScaleMultiplier_;
        }

        // YスケールをXスケールの逆数に連動させて、伸縮感を出す
        float newScaleY = baseSpringScaleY_;
        if (baseSpringDistance_ > 0.001f) // ゼロ除算を避ける
        {
            float distanceRatio = distance / baseSpringDistance_;
            // (1 - distanceRatio) がプラスなら太く、マイナスなら細くなる
            newScaleY = baseSpringScaleY_ * (1 + (1 - distanceRatio) * springSquashFactor_);
        }

        // 計算したYスケールを最小値と最大値の間に制限する
        newScaleY = Mathf.Clamp(newScaleY, minSpringScaleY_, maxSpringScaleY_);

        springTransform_.localScale = new Vector3(newScaleX, newScaleY, springTransform_.localScale.z);
    }

    private void OnDestroy()
    {
        // オブジェクト破棄時にすべてのTweenを確実に停止する
        actionSeq_?.Kill();
    }
}
