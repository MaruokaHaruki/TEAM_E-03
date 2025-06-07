using UnityEngine;

[ExecuteInEditMode]
public class WallCheckDelete : MonoBehaviour
{
    /// <summary>
    /// �A�g�t���O�ݒ�p
    /// </summary>
    public bool SetStageCollaborationFlag;
    /// <summary>
    /// �A�g�t���O
    /// </summary>
    private bool StageCollaborationFlag;

    ///�`�F�b�N�p�i���o�[
    [SerializeField, Header("�`�F�b�N�p�i���o�[")] private int CheckObjectTypeNumber = 0;

    /// <summary>
    /// �ݒ�p�X�e�[�W�R���g���[���[
    /// </summary>
    private StageController SetStageController;

    void Start()
    {
        // �A�g�t���O�ݒ�
        StageCollaborationFlag = SetStageCollaborationFlag = true;

        // �ݒ�p�X�e�[�W�R���g���[���[
        SetStageController = this.transform.parent.GetComponent<StageController>();

        // �R���C�_�[��t���ւ���
        BoxCollider deletecollider = this.gameObject.GetComponent<BoxCollider>();
        if (deletecollider != null)
        {
            if (Application.isPlaying)
            {
                Destroy(deletecollider);
            }
            else
            {
                DestroyImmediate(deletecollider);
            }
            this.gameObject.AddComponent<BoxCollider2D>();
        }
    }

    void Update()
    {
        if (CheckObjectTypeNumber <= 0)
        {
            return;
        }

        GameObject checkObject = SetStageController.GetWallObject_Number(CheckObjectTypeNumber);
        if ((checkObject != this.gameObject) && StageCollaborationFlag)
        {
            // �폜
            if (Application.isPlaying)
            {
                Destroy(this.gameObject);
            }
            else
            {
                DestroyImmediate(this.gameObject);
            }
        }
        else
        {
            // �A�g�t���O�ݒ�
            if (StageCollaborationFlag != SetStageCollaborationFlag)
            {
                StageCollaborationFlag = SetStageCollaborationFlag;
                SetStageController.SetCollaborationFlag_Number(CheckObjectTypeNumber, StageCollaborationFlag);
            }
        }
    }

    /// <summary>
    /// �I�u�W�F�N�g�^�C�v�i���o�[
    /// </summary>
    /// <param name="typeNumber">0�ȊO�̃I�u�W�F�N�g�^�C�v�i���o�[</param>
    public void SetCheckData(int typeNumber)
    {
        CheckObjectTypeNumber = typeNumber;
    }
}
