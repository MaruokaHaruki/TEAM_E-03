using Unity.VisualScripting;
using UnityEngine;

[ExecuteInEditMode]
public class StageController : MonoBehaviour
{
    /// <summary>
    /// ���S���炸��Ă���x�N�g����
    /// </summary>
    [SerializeField, Header("���S���炸��Ă���x�N�g����")] private Vector2 CenterShiftVec;

    /// <summary>
    /// �X�e�[�W�̑傫��
    /// </summary>
    [SerializeField, Header("�X�e�[�W�̑傫��")] private Vector2 StageSize = new Vector2(2f, 2f);

    /// <summary>
    /// ����
    /// </summary>
    [SerializeField, Header("����")] private float Thickness = 3f;

    /// <summary>
    /// �X�e�[�W�A�g�t���O
    /// </summary>
    [SerializeField, Header("�X�e�[�W�A�g�t���O")] private bool StageCollaborationFlag;

    /// <summary>
    /// �X�^�[�g����񂾂��L���ɂ���
    /// </summary>
    [SerializeField, Header("�X�^�[�g����񂾂��L���ɂ���")] private bool StartFlag = true;

    /// <summary>
    /// �ݒ�p�I�u�W�F�N�g �ݒ肪�Ȃ���΃L���[�u
    /// </summary>
    [SerializeField, Header("�ݒ�p�I�u�W�F�N�g\n  �ݒ肪�Ȃ���΃L���[�u")] private GameObject SetInstantiateObject;

    [System.Serializable, SerializeField]
    public class WallDatas
    {
        /// <summary>
        /// �ǃI�u�W�F�N�g
        /// </summary>
        [Header("�ǃI�u�W�F�N�g")] public GameObject WallObject;

        /// <summary>
        /// �񓧉߃t���O
        /// </summary>
        [Header("���߃t���O")] public bool InvisibleFlag;

        /// <summary>
        /// ���b�V��
        /// </summary>
        [Header("���b�V��")] public MeshRenderer WallMes;

        /// <summary>
        /// �L�΂��Ȃ�����
        /// </summary>
        [Header("�L�΂��Ȃ�����")] public Vector2 NotExtendVec;

        /// <summary>
        /// �A�g�t���O
        /// </summary>
        [Header("�A�g�t���O")] public bool CollaborationFlag;
    }

    /// <summary>�n��</summary>
    [SerializeField, Header("�n��")] private WallDatas Floor;
    /// <summary>�V��</summary>
    [SerializeField, Header("�V��")] private WallDatas Ceiling;
    /// <summary>X�v���X�����̕�</summary>
    [SerializeField, Header("X�v���X�����̕�")] private WallDatas Wall_XPlus;
    /// <summary>X�}�C�i�X�����̕�</summary>
    [SerializeField, Header("X�}�C�i�X�����̕�")] private WallDatas Wall_XMinus;


    /// <summary>
    /// �A�^�b�`��
    /// </summary>
    void OnValidate()
    {
        // �f�[�^�I�u�W�F�ɂԂ�����
        Debug.Log("�A�^�b�`");

        if (Application.isPlaying)
        {
            Debug.Log("�Đ���");
            StageCollaborationFlag = true;
            return;
        }

#if !UNITY_EDITOR
        StageCollaborationFlag = true;
        return;
#endif
        if (!StartFlag)
        {
            return;
        }
        StartFlag = false;

        // �X�e�[�W�A�g�t���O
        StageCollaborationFlag = true;

        // �n��
        if (Floor == null)
        {
            Floor = SetOnValidateWallObujectProcess(Vector2.up, 1, "Floor");
        }
        // �V��
        if (Ceiling == null)
        {
            Ceiling = SetOnValidateWallObujectProcess(Vector2.down, 2, "Ceiling");
        }
        // X�v���X
        if (Wall_XPlus == null)
        {
            Wall_XPlus = SetOnValidateWallObujectProcess(Vector2.right, 3, "Wall (XPlus)");
        }
        // X�}�C�i�X
        if (Wall_XMinus == null)
        {
            Wall_XMinus = SetOnValidateWallObujectProcess(Vector2.left, 4, "Wall (XMinus)");
        }
    }

    /// <summary>
    /// �ύX��
    /// </summary>
    void Update()
    {
        Debug.Log("�X�V");

        if (StageCollaborationFlag)
        {
            // �n��
            SetUpdateWallObujectProcess(Floor);
            // �V��
            SetUpdateWallObujectProcess(Ceiling);
            //X�v���X
            SetUpdateWallObujectProcess(Wall_XPlus);
            //X�}�C�i�X
            SetUpdateWallObujectProcess(Wall_XMinus);
        }
    }

    /// <summary>
    /// �Ǎۂ̈ړ��擾
    /// </summary>
    /// <param name="vec">����</param>
    /// <param name="StageSize">�X�e�[�W�T�C�Y</param>
    /// <param name="scale">�傫��</param>
    /// <returns></returns>
    private Vector3 SetByTheWallPosition(Vector3 vec, float StageSize, float scale)
    {
        return (vec * ((StageSize * 0.5f) - (scale * 0.5f)));
    }

    /// <summary>
    /// �f�^�b�`��
    /// </summary>
    void OnDestroy()
    {
        Debug.Log("�f�^�b�`");
        if (StageCollaborationFlag)
        {
            // �n��
            SetOnDestroyWallObujectProcess(Floor);
            // �V��
            SetOnDestroyWallObujectProcess(Ceiling);
            // X�v���X
            SetOnDestroyWallObujectProcess(Wall_XPlus);
            // X�}�C�i�X
            SetOnDestroyWallObujectProcess(Wall_XMinus);
        }
    }

    /// <summary>
    /// �A�^�b�`���E�H�[���f�[�^��ݒ�
    /// </summary>
    /// <param name="wallObject">�E�H�[���f�[�^�I�u�W�F�N�g</param>
    /// <param name="notExtendVec">�L�΂��Ȃ�����</param>
    private WallDatas SetOnValidateWallObujectProcess(Vector2 notExtendVec, int setWallObjectNumber, string name)
    {
        WallDatas setWallData = new WallDatas();

        if (setWallData.WallObject == null)
        {
            // Wall���q�I�u�W�F�N�g�Ƃ��Đ���
            if (SetInstantiateObject != null)
            {
                setWallData.WallObject = Instantiate(SetInstantiateObject);
            }
            else
            {
                setWallData.WallObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            }
            setWallData.WallObject.transform.parent = this.transform;
            setWallData.WallObject.transform.localPosition = Vector3.zero;

            // ���O
            setWallData.WallObject.name = name;

            // �`�F�b�N�p�X�N���v�g�ݒ�
            setWallData.WallObject.AddComponent<WallCheckDelete>().SetCheckData(setWallObjectNumber);

            // ���߃t���O�𖳌���
            setWallData.InvisibleFlag = true;

            // ���b�V���擾
            setWallData.WallMes = setWallData.WallObject.GetComponent<MeshRenderer>();

            // �L�΂��Ȃ������ݒ�
            setWallData.NotExtendVec = notExtendVec;

            // �A�g�t���O
            setWallData.CollaborationFlag = true;

            // �ʒu�@�傫�� ���̑��ݒ�
            SetSize_Position_Etc(setWallData);
        }

        return setWallData;
    }

    /// <summary>
    /// �X�V������
    /// </summary>
    /// <param name="wallObject"></param>
    private void SetUpdateWallObujectProcess(WallDatas wallObject)
    {
        if (wallObject.CollaborationFlag)
        {
            // �ʒu �傫���@���̑��ݒ�
            SetSize_Position_Etc(wallObject);
        }
    }

    /// <summary>
    /// �f�^�b�`������ 
    /// </summary>
    /// <param name="wallObject"></param>
    private void SetOnDestroyWallObujectProcess(WallDatas wallObject)
    {
        if (!wallObject.CollaborationFlag)
        {
            // �q�I�u�W�F�N�g����O��
            wallObject.WallObject.transform.SetParent(null);
        }
        else
        {
            // �폜
            if (Application.isPlaying)
            {
                Destroy(wallObject.WallObject);
            }
            else
            {
                DestroyImmediate(wallObject.WallObject);
            }
        }
    }

    /// <summary>
    /// �T�C�Y�@���W�@���̑��ݒ�
    /// </summary>
    /// <param name="wallDataObject">�ݒ蕨</param>
    private void SetSize_Position_Etc(WallDatas wallDataObject)
    {
        // ���W�ݒ�
        wallDataObject.WallObject.transform.localPosition = (new Vector2(SetLocalPosOne(wallDataObject.NotExtendVec.x, StageSize.x),
                                                                         SetLocalPosOne(wallDataObject.NotExtendVec.y, StageSize.y)) + CenterShiftVec);

        // �傫���ݒ�
        wallDataObject.WallObject.transform.localScale = new Vector3(SetLocalScaleOne(wallDataObject.NotExtendVec.x, StageSize.x),
                                                                     SetLocalScaleOne(wallDataObject.NotExtendVec.y, StageSize.y),
                                                                     0.1f);

        // ���ߐݒ�
        wallDataObject.WallMes.enabled = wallDataObject.InvisibleFlag;
    }

    /// <summary>
    /// 1���W�ݒ�p�֐�
    /// </summary>
    /// <param name="notExtendVolume">�L�΂��Ȃ�����</param>
    /// <param name="stageSizeVolume">�X�e�[�W�T�C�Y</param>
    /// <returns>1���W</returns>
    float SetLocalPosOne(float notExtendVolume, float stageSizeVolume)
    {
        return ((notExtendVolume != 0.0f) ? (((stageSizeVolume * 0.5f) * -notExtendVolume) + ((Thickness * 0.5f) * -notExtendVolume)) : 0.0f);
    }

    /// <summary>
    /// 1�T�C�Y�ݒ�p�֐�
    /// </summary>
    /// <param name="notExtendVolume">�L�΂��Ȃ�����</param>
    /// <param name="stageSizeVolume">�X�e�[�W�T�C�Y</param>
    /// <returns>1�T�C�Y</returns>
    float SetLocalScaleOne(float notExtendVolume, float stageSizeVolume)
    {
        return Thickness + ((notExtendVolume != 0.0f) ? 0.0f : (stageSizeVolume + Thickness));
    }

    /// <summary>
    /// �����ɉ������E�H�[���f�[�^��Ԃ�
    /// </summary>
    /// <param name="number">�Q�ƃi���o�[</param>
    /// <returns></returns>
    private WallDatas GetWallDatas_Number(int number)
    {
        switch (number)
        {
            case 1:
                return Floor;

            case 2:
                return Ceiling;

            case 3:
                return Wall_XPlus;

            case 4:
                return Wall_XMinus;
        }
        
        return null;
    }

    /// <summary>
    /// �����ɉ������I�u�W�F�N�g��Ԃ�
    /// </summary>
    /// <param name="number">�Q�ƃi���o�[</param>
    /// <returns>�E�H�[���I�u�W�F�N�g</returns>
    public GameObject GetWallObject_Number(int number)
    {
        WallDatas setData = GetWallDatas_Number(number);
        if (setData != null)
        {
            return setData.WallObject;
        }

        return null;
    }

    /// <summary>
    /// �����ɉ������I�u�W�F�N�g�̘A�g�t���O�ݒ�
    /// </summary>
    /// <param name="number">�Q�ƃi���o�[</param>
    /// <param name="setFlag">�ݒ�t���O</param>
    public void SetCollaborationFlag_Number(int number, bool setFlag)
    {
        WallDatas setData = GetWallDatas_Number(number);
        if (setData != null)
        {
            setData.CollaborationFlag = setFlag;
        }
    }
}