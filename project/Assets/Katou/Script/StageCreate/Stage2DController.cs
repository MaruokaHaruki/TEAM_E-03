using Unity.VisualScripting;
using UnityEngine;

[ExecuteInEditMode]
public class StageController : MonoBehaviour
{
    /// <summary>
    /// 中心からずれているベクトル量
    /// </summary>
    [SerializeField, Header("中心からずれているベクトル量")] private Vector2 CenterShiftVec;

    /// <summary>
    /// ステージの大きさ
    /// </summary>
    [SerializeField, Header("ステージの大きさ")] private Vector2 StageSize = new Vector2(2f, 2f);

    /// <summary>
    /// 厚み
    /// </summary>
    [SerializeField, Header("厚み")] private float Thickness = 3f;

    /// <summary>
    /// ステージ連携フラグ
    /// </summary>
    [SerializeField, Header("ステージ連携フラグ")] private bool StageCollaborationFlag;

    /// <summary>
    /// スタート時一回だけ有効にする
    /// </summary>
    [SerializeField, Header("スタート時一回だけ有効にする")] private bool StartFlag = true;

    /// <summary>
    /// 設定用オブジェクト 設定がなければキューブ
    /// </summary>
    [SerializeField, Header("設定用オブジェクト\n  設定がなければキューブ")] private GameObject SetInstantiateObject;

    [System.Serializable, SerializeField]
    public class WallDatas
    {
        /// <summary>
        /// 壁オブジェクト
        /// </summary>
        [Header("壁オブジェクト")] public GameObject WallObject;

        /// <summary>
        /// 非透過フラグ
        /// </summary>
        [Header("透過フラグ")] public bool InvisibleFlag;

        /// <summary>
        /// メッシュ
        /// </summary>
        [Header("メッシュ")] public MeshRenderer WallMes;

        /// <summary>
        /// 伸ばさない方向
        /// </summary>
        [Header("伸ばさない方向")] public Vector2 NotExtendVec;

        /// <summary>
        /// 連携フラグ
        /// </summary>
        [Header("連携フラグ")] public bool CollaborationFlag;
    }

    /// <summary>地面</summary>
    [SerializeField, Header("地面")] private WallDatas Floor;
    /// <summary>天井</summary>
    [SerializeField, Header("天井")] private WallDatas Ceiling;
    /// <summary>Xプラス方向の壁</summary>
    [SerializeField, Header("Xプラス方向の壁")] private WallDatas Wall_XPlus;
    /// <summary>Xマイナス方向の壁</summary>
    [SerializeField, Header("Xマイナス方向の壁")] private WallDatas Wall_XMinus;


    /// <summary>
    /// アタッチ時
    /// </summary>
    void OnValidate()
    {
        // データオブジェにぶち込む
        Debug.Log("アタッチ");

        if (Application.isPlaying)
        {
            Debug.Log("再生中");
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

        // ステージ連携フラグ
        StageCollaborationFlag = true;

        // 地面
        if (Floor == null)
        {
            Floor = SetOnValidateWallObujectProcess(Vector2.up, 1, "Floor");
        }
        // 天井
        if (Ceiling == null)
        {
            Ceiling = SetOnValidateWallObujectProcess(Vector2.down, 2, "Ceiling");
        }
        // Xプラス
        if (Wall_XPlus == null)
        {
            Wall_XPlus = SetOnValidateWallObujectProcess(Vector2.right, 3, "Wall (XPlus)");
        }
        // Xマイナス
        if (Wall_XMinus == null)
        {
            Wall_XMinus = SetOnValidateWallObujectProcess(Vector2.left, 4, "Wall (XMinus)");
        }
    }

    /// <summary>
    /// 変更時
    /// </summary>
    void Update()
    {
        Debug.Log("更新");

        if (StageCollaborationFlag)
        {
            // 地面
            SetUpdateWallObujectProcess(Floor);
            // 天井
            SetUpdateWallObujectProcess(Ceiling);
            //Xプラス
            SetUpdateWallObujectProcess(Wall_XPlus);
            //Xマイナス
            SetUpdateWallObujectProcess(Wall_XMinus);
        }
    }

    /// <summary>
    /// 壁際の移動取得
    /// </summary>
    /// <param name="vec">方向</param>
    /// <param name="StageSize">ステージサイズ</param>
    /// <param name="scale">大きさ</param>
    /// <returns></returns>
    private Vector3 SetByTheWallPosition(Vector3 vec, float StageSize, float scale)
    {
        return (vec * ((StageSize * 0.5f) - (scale * 0.5f)));
    }

    /// <summary>
    /// デタッチ時
    /// </summary>
    void OnDestroy()
    {
        Debug.Log("デタッチ");
        if (StageCollaborationFlag)
        {
            // 地面
            SetOnDestroyWallObujectProcess(Floor);
            // 天井
            SetOnDestroyWallObujectProcess(Ceiling);
            // Xプラス
            SetOnDestroyWallObujectProcess(Wall_XPlus);
            // Xマイナス
            SetOnDestroyWallObujectProcess(Wall_XMinus);
        }
    }

    /// <summary>
    /// アタッチ時ウォールデータ一つ設定
    /// </summary>
    /// <param name="wallObject">ウォールデータオブジェクト</param>
    /// <param name="notExtendVec">伸ばさない方向</param>
    private WallDatas SetOnValidateWallObujectProcess(Vector2 notExtendVec, int setWallObjectNumber, string name)
    {
        WallDatas setWallData = new WallDatas();

        if (setWallData.WallObject == null)
        {
            // Wallを子オブジェクトとして生成
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

            // 名前
            setWallData.WallObject.name = name;

            // チェック用スクリプト設定
            setWallData.WallObject.AddComponent<WallCheckDelete>().SetCheckData(setWallObjectNumber);

            // 透過フラグを無効か
            setWallData.InvisibleFlag = true;

            // メッシュ取得
            setWallData.WallMes = setWallData.WallObject.GetComponent<MeshRenderer>();

            // 伸ばさない方向設定
            setWallData.NotExtendVec = notExtendVec;

            // 連携フラグ
            setWallData.CollaborationFlag = true;

            // 位置　大きさ その他設定
            SetSize_Position_Etc(setWallData);
        }

        return setWallData;
    }

    /// <summary>
    /// 更新時処理
    /// </summary>
    /// <param name="wallObject"></param>
    private void SetUpdateWallObujectProcess(WallDatas wallObject)
    {
        if (wallObject.CollaborationFlag)
        {
            // 位置 大きさ　その他設定
            SetSize_Position_Etc(wallObject);
        }
    }

    /// <summary>
    /// デタッチ時処理 
    /// </summary>
    /// <param name="wallObject"></param>
    private void SetOnDestroyWallObujectProcess(WallDatas wallObject)
    {
        if (!wallObject.CollaborationFlag)
        {
            // 子オブジェクトから外す
            wallObject.WallObject.transform.SetParent(null);
        }
        else
        {
            // 削除
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
    /// サイズ　座標　その他設定
    /// </summary>
    /// <param name="wallDataObject">設定物</param>
    private void SetSize_Position_Etc(WallDatas wallDataObject)
    {
        // 座標設定
        wallDataObject.WallObject.transform.localPosition = (new Vector2(SetLocalPosOne(wallDataObject.NotExtendVec.x, StageSize.x),
                                                                         SetLocalPosOne(wallDataObject.NotExtendVec.y, StageSize.y)) + CenterShiftVec);

        // 大きさ設定
        wallDataObject.WallObject.transform.localScale = new Vector3(SetLocalScaleOne(wallDataObject.NotExtendVec.x, StageSize.x),
                                                                     SetLocalScaleOne(wallDataObject.NotExtendVec.y, StageSize.y),
                                                                     0.1f);

        // 透過設定
        wallDataObject.WallMes.enabled = wallDataObject.InvisibleFlag;
    }

    /// <summary>
    /// 1座標設定用関数
    /// </summary>
    /// <param name="notExtendVolume">伸ばさない方向</param>
    /// <param name="stageSizeVolume">ステージサイズ</param>
    /// <returns>1座標</returns>
    float SetLocalPosOne(float notExtendVolume, float stageSizeVolume)
    {
        return ((notExtendVolume != 0.0f) ? (((stageSizeVolume * 0.5f) * -notExtendVolume) + ((Thickness * 0.5f) * -notExtendVolume)) : 0.0f);
    }

    /// <summary>
    /// 1サイズ設定用関数
    /// </summary>
    /// <param name="notExtendVolume">伸ばさない方向</param>
    /// <param name="stageSizeVolume">ステージサイズ</param>
    /// <returns>1サイズ</returns>
    float SetLocalScaleOne(float notExtendVolume, float stageSizeVolume)
    {
        return Thickness + ((notExtendVolume != 0.0f) ? 0.0f : (stageSizeVolume + Thickness));
    }

    /// <summary>
    /// 数字に応じたウォールデータを返す
    /// </summary>
    /// <param name="number">参照ナンバー</param>
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
    /// 数字に応じたオブジェクトを返す
    /// </summary>
    /// <param name="number">参照ナンバー</param>
    /// <returns>ウォールオブジェクト</returns>
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
    /// 数字に応じたオブジェクトの連携フラグ設定
    /// </summary>
    /// <param name="number">参照ナンバー</param>
    /// <param name="setFlag">設定フラグ</param>
    public void SetCollaborationFlag_Number(int number, bool setFlag)
    {
        WallDatas setData = GetWallDatas_Number(number);
        if (setData != null)
        {
            setData.CollaborationFlag = setFlag;
        }
    }
}