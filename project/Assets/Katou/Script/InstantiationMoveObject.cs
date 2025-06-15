using UnityEngine;
using static UnityEditor.PlayerSettings;

public class InstantiationMoveObject : MonoBehaviour
{
    [SerializeField, Header("生成するオブジェクト(ObjectControllerを生成時付ける)")] private GameObject SetInstantiationObject = null;

    /// <summary>生成ポジション</summary>
    private Vector2 InstantiationPos = Vector2.zero;
    /// <summary>目標ポジション</summary>
    private Vector2 TargetPos = Vector2.zero;

    /// <summary>ランダム生成　左上座標</summary>
    private Vector2 RandmInstantiationLeftUpPos = Vector2.zero;
    /// <summary>ランダム生成　右下座標</summary>
    private Vector2 RandmInstantiationRightDownPos = Vector2.zero;

    /// <summary>
    /// 通常生成情報設定
    /// </summary>
    public void SetInstantiationData(Vector2 pos, Vector2 targetPos)
    {
        InstantiationPos = pos;
        TargetPos = targetPos; 
    }

    /// <summary>
    /// 設定後生成
    /// </summary>
    public GameObject InstantiationObject()
    {
        GameObject setObject = Instantiate(SetInstantiationObject);
        setObject.transform.position = InstantiationPos;
        setObject.AddComponent<ObjectController>().SetPositionData(InstantiationPos, TargetPos);

        return setObject;
    }

    /// <summary>
    /// 設定&生成
    /// </summary>
    public GameObject InstantiationObject(Vector2 pos, Vector2 targetPos)
    {
        GameObject setObject = Instantiate(SetInstantiationObject);
        setObject.transform.position = pos;
        setObject.AddComponent<ObjectController>().SetPositionData(pos, targetPos);

        return setObject;
    }

    /// <summary>
    /// ランダム生成情報設定
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
