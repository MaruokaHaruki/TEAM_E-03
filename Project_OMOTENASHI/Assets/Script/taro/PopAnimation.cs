using UnityEngine;
using DG.Tweening; // DOTweenを使用するために必要

/// <summary>
/// オブジェクトをポップにアニメーションさせるスクリプト。
/// </summary>
public class PopAnimation : MonoBehaviour
{
    [Header("▼ アニメーション設定")]
    [Tooltip("予備動作で拡大する倍率")]
    [SerializeField] private float anticipationScaleFactor_ = 1.2f;

    [Tooltip("アニメーション前半にかかる時間（秒）")]
    [SerializeField] private float firstHalfDuration_ = 0.15f;

    [Tooltip("アニメーション後半にかかる時間（秒）")]
    [SerializeField] private float secondHalfDuration_ = 0.4f; // 激しい動きを見せるため少し延長

    [Tooltip("バウンスの振れ幅（大きいほど激しく揺れる）")]
    [SerializeField] private float bounceAmplitude_ = 1.5f;

    [Tooltip("バウンスの振動（小さいほど細かく揺れる）")]
    [SerializeField] private float bouncePeriod_ = 0.3f;


    // プライベート変数
    private Vector3 initialScale_;
    private Sequence currentSequence_;
    private bool isFlipped_ = false; // 現在反転しているかどうかの状態

    private void Awake()
    {
        // このスクリプトがアタッチされたオブジェクトのTransformを対象とする
        Vector3 currentScale = transform.localScale;
        initialScale_ = new Vector3(Mathf.Abs(currentScale.x), currentScale.y, currentScale.z);

        // ゲーム開始時の向きから、初期の反転状態を正しく設定する
        isFlipped_ = currentScale.x < 0;
    }

    private void Update()
    {
        // Xキーが押されたらアニメーションを再生
        if (Input.GetKeyDown(KeyCode.X))
        {
            //PlayAnimation();
        }
    }

    /// <summary>
    /// アニメーションを再生します。
    /// </summary>
    public void PlayAnimation()
    {
        // 既存のアニメーションを停止して、すぐに次を開始する
        currentSequence_?.Kill();

        // 反転状態を切り替える
        isFlipped_ = !isFlipped_;

        // 目標となるXスケールを計算
        float targetX = isFlipped_ ? -initialScale_.x : initialScale_.x;

        // アニメーションシーケンスを作成
        currentSequence_ = DOTween.Sequence();

        // 1. 予備動作：少し大きくなる
        currentSequence_.Append(
            transform.DOScale(initialScale_ * anticipationScaleFactor_, firstHalfDuration_)
                .SetEase(Ease.OutSine)
        );

        // 2. 目標のスケール（Xが反転した状態）にバウンスしながら戻る
        currentSequence_.Append(
            transform.DOScale(new Vector3(targetX, initialScale_.y, initialScale_.z), secondHalfDuration_)
                .SetEase(Ease.OutElastic, bounceAmplitude_, bouncePeriod_) // ★★★ 変更点：より激しいバウンスに変更 ★★★
        );

        // シーケンスの再生設定
        currentSequence_.SetLink(gameObject).Play();
    }
}
