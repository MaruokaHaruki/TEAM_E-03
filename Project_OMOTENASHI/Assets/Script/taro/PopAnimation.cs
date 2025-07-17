using UnityEngine;
using DG.Tweening; // DOTweenの名前空間をインポート

/// <summary>
/// ポップアニメーションを制御するクラス
/// </summary>
public class PopAnimation : MonoBehaviour
{
    [Header("ポップアニメーション設定")]
    [Tooltip("アニメーション開始時のスケール倍率")]
    [SerializeField] private float anticipationScaleFactor_ = 1.2f;

    [Tooltip("アニメーション前半の持続時間")]
    [SerializeField] private float firstHalfDuration_ = 0.15f;

    [Tooltip("アニメーション後半の持続時間")]
    [SerializeField] private float secondHalfDuration_ = 0.4f; // アニメーション後半の持続時間

    [Tooltip("バウンスアニメーションの振幅")]
    [SerializeField] private float bounceAmplitude_ = 1.5f;

    [Tooltip("バウンスアニメーションの周期")]
    [SerializeField] private float bouncePeriod_ = 0.3f;


    // 初期スケール
    private Vector3 initialScale_;
    private Sequence currentSequence_;
    private bool isFlipped_ = false; // スプライトの向きが反転しているかどうか

    private void Awake()
    {
        // 現在のスケールを取得
        Vector3 currentScale = transform.localScale;
        initialScale_ = new Vector3(Mathf.Abs(currentScale.x), currentScale.y, currentScale.z);

        // スプライトの向きが反転しているかどうかを判定
        isFlipped_ = currentScale.x < 0;
    }

    private void Update()
    {
        // Xキーが押されたらアニメーションを再生
        if (Input.GetKeyDown(KeyCode.X))
        {
            PlayAnimation();
        }
    }

    /// <summary>
    /// �A�j���[�V�������Đ����܂��B
    /// </summary>
    public void PlayAnimation()
    {
        // �����̃A�j���[�V�������~���āA�����Ɏ����J�n����
        currentSequence_?.Kill();

        // ���]��Ԃ�؂�ւ���
        isFlipped_ = !isFlipped_;

        // �ڕW�ƂȂ�X�X�P�[�����v�Z
        float targetX = isFlipped_ ? -initialScale_.x : initialScale_.x;

        // �A�j���[�V�����V�[�P���X���쐬
        currentSequence_ = DOTween.Sequence();

        // 1. �\������F�����傫���Ȃ�
        currentSequence_.Append(
            transform.DOScale(initialScale_ * anticipationScaleFactor_, firstHalfDuration_)
                .SetEase(Ease.OutSine)
        );

        // 2. �ڕW�̃X�P�[���iX�����]������ԁj�Ƀo�E���X���Ȃ���߂�
        currentSequence_.Append(
            transform.DOScale(new Vector3(targetX, initialScale_.y, initialScale_.z), secondHalfDuration_)
                .SetEase(Ease.OutElastic, bounceAmplitude_, bouncePeriod_) // ������ �ύX�_�F��范�����o�E���X�ɕύX ������
        );

        // �V�[�P���X�̍Đ��ݒ�
        currentSequence_.SetLink(gameObject).Play();
    }
}
