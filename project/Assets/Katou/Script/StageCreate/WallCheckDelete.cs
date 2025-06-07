using UnityEngine;

[ExecuteInEditMode]
public class WallCheckDelete : MonoBehaviour
{
    /// <summary>
    /// 連携フラグ設定用
    /// </summary>
    public bool SetStageCollaborationFlag;
    /// <summary>
    /// 連携フラグ
    /// </summary>
    private bool StageCollaborationFlag;

    ///チェック用ナンバー
    [SerializeField, Header("チェック用ナンバー")] private int CheckObjectTypeNumber = 0;

    /// <summary>
    /// 設定用ステージコントローラー
    /// </summary>
    private StageController SetStageController;

    void Start()
    {
        // 連携フラグ設定
        StageCollaborationFlag = SetStageCollaborationFlag = true;

        // 設定用ステージコントローラー
        SetStageController = this.transform.parent.GetComponent<StageController>();

        // コライダーを付け替える
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
            // 削除
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
            // 連携フラグ設定
            if (StageCollaborationFlag != SetStageCollaborationFlag)
            {
                StageCollaborationFlag = SetStageCollaborationFlag;
                SetStageController.SetCollaborationFlag_Number(CheckObjectTypeNumber, StageCollaborationFlag);
            }
        }
    }

    /// <summary>
    /// オブジェクトタイプナンバー
    /// </summary>
    /// <param name="typeNumber">0以外のオブジェクトタイプナンバー</param>
    public void SetCheckData(int typeNumber)
    {
        CheckObjectTypeNumber = typeNumber;
    }
}
