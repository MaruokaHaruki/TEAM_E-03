using UnityEngine;
using DG.Tweening;

/// <summary>
/// パンチ用オブジェクト（Collider）を前方にビューッと押し出し、
/// 一定時間停止後に元の位置へ戻す。タグとコライダー状態を状態に応じて変更。
/// </summary>
public class Push : MonoBehaviour
{
    ///--------------------------------------------------------------
    /// 【列挙型：ステート定義】

    public enum MachineState
    {
        Wait,
        Shot,
        Charge
    }

    ///--------------------------------------------------------------
    /// 【インスペクター設定】

    [Header("押し出す距離")]
    [Tooltip("パンチ動作で押し出すX方向の距離（＋右／－左）")]
    [SerializeField] private float offsetX_ = 12f;

    [Header("パンチ動作の時間設定")]
    [Tooltip("ビューッと伸びる秒数")]
    [SerializeField] private float outDuration_ = 0.3f;

    [Tooltip("停止する秒数")]
    [SerializeField] private float pauseDuration_ = 1f;

    [Tooltip("戻るのにかかる秒数")]
    [SerializeField] private float returnDuration_ = 2f;

    [Header("パンチ用コライダー")]
    [Tooltip("パンチングに使うコライダー付きのGameObject")]
    [SerializeField] private Collider2D punchingCollider_;

    ///--------------------------------------------------------------
    /// 【プライベート変数】

    private Sequence seq_;                   // DOTweenの再生シーケンス
    private Vector3 basePos_;               // コライダーの初期ローカル位置
    private MachineState currentState_ = MachineState.Wait;

    ///--------------------------------------------------------------
    /// 【Unity ライフサイクル】

    private void Awake()
    {
        if (punchingCollider_ != null)
        {
            basePos_ = punchingCollider_.transform.localPosition;
        }
        else
        {
            Debug.LogWarning("[PUSH] : punchingCollider_ が設定されていません");
        }
    }

    private void Update()
    {
        //========================================
        // 入力チェック（連打防止）
        if (seq_ != null && seq_.IsActive() && seq_.IsPlaying()) return;

        // 1キーでパンチ動作を開始
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            PlayOneShot();
        }
    }

    ///--------------------------------------------------------------
    /// 【パンチ動作シーケンス】

    /// <summary>
    /// Shot → Wait → Charge の順でコライダーを動かす
    /// </summary>
    private void PlayOneShot()
    {
        seq_?.Kill();

        seq_ = DOTween.Sequence()

            // Shot: ビューッと発射
            .AppendCallback(() => SetState(MachineState.Shot))
            .Append(punchingCollider_.transform.DOLocalMoveX(basePos_.x + offsetX_, outDuration_)
                .SetEase(Ease.OutBounce))

            // Wait: 停止
            .AppendCallback(() => SetState(MachineState.Wait))
            .AppendInterval(pauseDuration_)

            // Charge: ゆっくり戻る
            .AppendCallback(() => SetState(MachineState.Charge))
            .Append(punchingCollider_.transform.DOLocalMoveX(basePos_.x, returnDuration_))

            // 完了処理
            .SetLink(gameObject)
            .OnComplete(() =>
            {
                punchingCollider_.transform.localPosition = basePos_; // 誤差補正
                currentState_ = MachineState.Wait;
            });
    }

    ///--------------------------------------------------------------
    /// 【状態・見た目制御】

    /// <summary>
    /// 現在の状態に応じてタグとコライダーを切り替える
    /// </summary>
    private void SetState(MachineState state)
    {
        currentState_ = state;

        switch (state)
        {
            case MachineState.Shot:
            case MachineState.Wait:
                SetTag("Punching");
                SetCollider(true);
                break;

            case MachineState.Charge:
                SetTag("Ground");
                SetCollider(true);  // Charge中でもColliderを有効にしておく
                break;
        }

        Debug.Log($"[MODE CHANGE] : 状態遷移 → {state}");
    }

    /// <summary>
    /// オブジェクトのタグを切り替える
    /// </summary>
    private void SetTag(string tagName)
    {
        if (punchingCollider_ != null && punchingCollider_.gameObject.tag != tagName)
        {
            punchingCollider_.gameObject.tag = tagName;
        }
    }

    /// <summary>
    /// コライダーを有効／無効にする
    /// </summary>
    private void SetCollider(bool enabled)
    {
        if (punchingCollider_ != null)
        {
            punchingCollider_.enabled = enabled;
        }
    }
}
