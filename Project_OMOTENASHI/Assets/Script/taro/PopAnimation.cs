using UnityEngine;
using DG.Tweening; // DOTween��g�p���邽�߂ɕK�v

/// <summary>
/// �I�u�W�F�N�g��|�b�v�ɃA�j���[�V����������X�N���v�g�B
/// </summary>
public class PopAnimation : MonoBehaviour
{
    [Header("�� �A�j���[�V�����ݒ�")]
    [Tooltip("�������Ŋg�傷��{��")]
    [SerializeField] private float anticipationScaleFactor_ = 1.2f;

    [Tooltip("�A�j���[�V�����O���ɂ����鎞�ԁi�b�j")]
    [SerializeField] private float firstHalfDuration_ = 0.15f;

    [Tooltip("�A�j���[�V�����㔼�ɂ����鎞�ԁi�b�j")]
    [SerializeField] private float secondHalfDuration_ = 0.4f; // ����������������邽�ߏ�������

    [Tooltip("�o�E���X�̐U�ꕝ�i�傫���قǌ������h���j")]
    [SerializeField] private float bounceAmplitude_ = 1.5f;

    [Tooltip("�o�E���X�̐U���i�������قǍׂ����h���j")]
    [SerializeField] private float bouncePeriod_ = 0.3f;


    // �v���C�x�[�g�ϐ�
    private Vector3 initialScale_;
    private Sequence currentSequence_;
    private bool isFlipped_ = false; // ���ݔ��]���Ă��邩�ǂ����̏��

    private void Awake()
    {
        // ���̃X�N���v�g���A�^�b�`���ꂽ�I�u�W�F�N�g��Transform��ΏۂƂ���
        Vector3 currentScale = transform.localScale;
        initialScale_ = new Vector3(Mathf.Abs(currentScale.x), currentScale.y, currentScale.z);

        // �Q�[���J�n���̌�������A�����̔��]��Ԃ𐳂����ݒ肷��
        isFlipped_ = currentScale.x < 0;
    }

    private void Update()
    {
        // X�L�[�������ꂽ��A�j���[�V������Đ�
        if (Input.GetKeyDown(KeyCode.X))
        {
            //PlayAnimation();
        }
    }

    /// <summary>
    /// �A�j���[�V������Đ����܂��B
    /// </summary>
    public void PlayAnimation()
    {
        // �����̃A�j���[�V�������~���āA�����Ɏ���J�n����
        currentSequence_?.Kill();

        // ���]��Ԃ�؂�ւ���
        isFlipped_ = !isFlipped_;

        // �ڕW�ƂȂ�X�X�P�[����v�Z
        float targetX = isFlipped_ ? -initialScale_.x : initialScale_.x;

        // �A�j���[�V�����V�[�P���X��쐬
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
