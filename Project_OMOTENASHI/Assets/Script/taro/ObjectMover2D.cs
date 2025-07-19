using UnityEngine;
using DG.Tweening; // DOTween�̃A�Z�b�g��g�p���邽�߂ɕK�v�ł�

public class ObjectMover2D : MonoBehaviour
{
    [Header("�A�j���[�V�����ݒ�")]
    [Tooltip("On��Ԃ̎���Y���W��ǂꂾ�����炷��")]
    [SerializeField]
    private float yOffset = 2f;

    [Tooltip("�A�j���[�V�����̎��ԁi�b�j")]
    [SerializeField]
    private float duration = 0.4f;

    [Tooltip("�A�j���[�V�����̕ω��̎d���i�s����߂����������j")]
    [SerializeField]
    private Ease ease = Ease.OutBack; // �s���Ɩ߂�ŋ��ʂ�Ease��g�p

    private Vector3 startPosition;       // �A�j���[�V�����O�̏������W
    private bool isStateOn = false;      // ���݂̏�� (true: On, false: Off)

    void Awake()
    {
        // �N�����̃��[���h���W��������W�Ƃ��ĕۑ�
        startPosition = transform.position;
    }

    void Update()
    {
        // Space�L�[�������ꂽ�u�Ԃ���o������
        if (Input.GetKeyDown(KeyCode.G))
        {
            // ��Ԃ�؂�ւ��郁�\�b�h��Ăяo��
            ToggleState();
        }
    }

    /// <summary>
    /// �I�u�W�F�N�g�̏�Ԃ�؂�ւ��܂��B
    /// </summary>
    public void ToggleState()
    {
        // ���d�v�F�����̃A�j���[�V��������̏�Œ�~������
        // ����ɂ��A�A�j���[�V�����̓r���ł�X���[�Y�ɔ��]�ł���
        transform.DOKill();

        // ���݂̏�Ԃ𔽓]������
        isStateOn = !isStateOn;

        // On��ԂȂ�ڕW�ʒu��v�Z���AOff��ԂȂ珉���ʒu�ɖ߂�
        Vector3 targetPosition = isStateOn ? startPosition + new Vector3(0, yOffset, 0) : startPosition;

        // �v�Z�����ڕW�ʒu�փA�j���[�V������J�n
        transform.DOMove(targetPosition, duration).SetEase(ease);
    }
}