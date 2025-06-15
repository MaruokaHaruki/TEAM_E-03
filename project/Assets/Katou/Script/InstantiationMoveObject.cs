using UnityEngine;
using static UnityEditor.PlayerSettings;

public class InstantiationMoveObject : MonoBehaviour
{
    [SerializeField, Header("��������I�u�W�F�N�g(ObjectController�𐶐����t����)")] private GameObject SetInstantiationObject = null;

    /// <summary>�����|�W�V����</summary>
    private Vector2 InstantiationPos = Vector2.zero;
    /// <summary>�ڕW�|�W�V����</summary>
    private Vector2 TargetPos = Vector2.zero;

    /// <summary>�����_�������@������W</summary>
    private Vector2 RandmInstantiationLeftUpPos = Vector2.zero;
    /// <summary>�����_�������@�E�����W</summary>
    private Vector2 RandmInstantiationRightDownPos = Vector2.zero;

    /// <summary>
    /// �ʏ퐶�����ݒ�
    /// </summary>
    public void SetInstantiationData(Vector2 pos, Vector2 targetPos)
    {
        InstantiationPos = pos;
        TargetPos = targetPos; 
    }

    /// <summary>
    /// �ݒ�㐶��
    /// </summary>
    public GameObject InstantiationObject()
    {
        GameObject setObject = Instantiate(SetInstantiationObject);
        setObject.transform.position = InstantiationPos;
        setObject.AddComponent<ObjectController>().SetPositionData(InstantiationPos, TargetPos);

        return setObject;
    }

    /// <summary>
    /// �ݒ�&����
    /// </summary>
    public GameObject InstantiationObject(Vector2 pos, Vector2 targetPos)
    {
        GameObject setObject = Instantiate(SetInstantiationObject);
        setObject.transform.position = pos;
        setObject.AddComponent<ObjectController>().SetPositionData(pos, targetPos);

        return setObject;
    }

    /// <summary>
    /// �����_���������ݒ�
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="targetPos"></param>
    /// <param name="leftUpPos"></param>
    /// <param name="rightDownPos"></param>
    public void SetRandmInstantiationData(Vector2 targetPos, Vector2 leftUpPos, Vector2 rightDownPos)
    {
        TargetPos = targetPos;
        RandmInstantiationLeftUpPos = leftUpPos;
        RandmInstantiationRightDownPos = rightDownPos;
    }

    public void RandmInstantiationObject()
    {
    }

    public void RandmInstantiationObject(Vector2 tes)
    {
    }
}
