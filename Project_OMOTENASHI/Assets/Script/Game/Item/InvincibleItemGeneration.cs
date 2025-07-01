using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class InvincibleItemGeneration : MonoBehaviour
{
    /// <summary>生成アイテム</summary>
    [SerializeField] private GameObject InvincibleItem;

    /// <summary>右反応キー</summary>
    [SerializeField] protected KeyCode LeftKey = KeyCode.Alpha1;
    /// <summary>左反応キー</summary>
    [SerializeField] protected KeyCode RightKey = KeyCode.Alpha0;

    /// <summary>左オブジェ</summary>
    [SerializeField] private GameObject LeftObj;
    /// <summary>右オブジェ</summary>
    [SerializeField] private GameObject RightObj;

    /// <summary>設定アングル</summary>
    [SerializeField, Range(0.0f, 180.0f)] private float SetAngle;

    /// <summary>生成割合</summary>
    [SerializeField, Range(0.01f, 1.0f)] private float PopItemRate = 0.1f;

    /// <summary>開く速度</summary>
    [SerializeField] private float MoveSpeed = 10.0f;

    /// <summary>アイテム生成フラグ</summary>
    private bool PopItemFlag = false;

    /// <summary>複数生成阻止確認用</summary>
    private GameObject CheckObject = null;
    
    /// <summary>アイテム落とし確認用</summary>
    private BoxCollider2D CheckObjectBoxCollider;


    void Awake()
    {
        // 左・右オブジェ取得
        if (LeftObj == null)
        {
            LeftObj = this.transform.Find("LeftObj").gameObject;
        }
        if (RightObj == null)
        {
            RightObj = this.transform.Find("RightObj").gameObject;
        }
    }

    private void OnEnable()
    {
        PopItemFlag = false;
        LeftObj.transform.rotation = Quaternion.identity;
        RightObj.transform.rotation = Quaternion.identity;
    }

    void Update()
    {
        bool keyPushFlag = true;

        if (Input.GetKey(LeftKey) || PopItemFlag)
        {
            LeftObj.transform.eulerAngles = new Vector3(0.0f, 0.0f, GetNextAngle(-SetAngle, LeftObj.transform.eulerAngles.z, MoveSpeed));
        }
        else
        {
            LeftObj.transform.eulerAngles = new Vector3(0.0f, 0.0f, GetNextAngle(0.0f, LeftObj.transform.eulerAngles.z, MoveSpeed));
            keyPushFlag = false;
        }

        if (Input.GetKey(RightKey) || PopItemFlag)
        {
            RightObj.transform.eulerAngles = new Vector3(0.0f, 0.0f, GetNextAngle(SetAngle, RightObj.transform.eulerAngles.z, MoveSpeed));
        }
        else
        {
            RightObj.transform.eulerAngles = new Vector3(0.0f, 0.0f, GetNextAngle(0.0f, RightObj.transform.eulerAngles.z, MoveSpeed));
            keyPushFlag = false;
        }


        if (keyPushFlag)
        {
            // 無敵アイテム生成フラグオン
            PopItemFlag = true;
        }

        if ((CheckObject == null) && !PopItemFlag)
        {
            CheckObject = Instantiate(InvincibleItem, this.transform.position + new Vector3(0.0f, -0.5f, 0.0f), Quaternion.identity);
        }

        // 確認→落とす
        CheckPop();
    }

    /// <summary>条件を満たしている場合アイテム落とす</summary>
    private void CheckPop()
    {
        if (PopItemFlag)
        {
            if (((GetCanonicalAngle(LeftObj.transform.eulerAngles.z) / -SetAngle) >= PopItemRate) &&
                ((GetCanonicalAngle(RightObj.transform.eulerAngles.z) / SetAngle) >= PopItemRate))
            {
                // 生成
                if (CheckObject != null)
                {
                    CheckObjectBoxCollider = CheckObject.AddComponent<BoxCollider2D>();
                    Rigidbody2D setRigidbody = CheckObject.GetComponent<Rigidbody2D>();
                    if (setRigidbody != null)
                    {
                        setRigidbody.gravityScale = 1.0f;
                    }
                }
                PopItemFlag = false;
            }
            else
            {
                if (CheckObjectBoxCollider != null)
                {
                    PopItemFlag = false;
                }
            }
        }
    }

    /// <summary>次のアングルを取得</summary>
    private float GetNextAngle(float setAngle, float nowAngle, float speed)
    {

        float deffAngle = setAngle - GetCanonicalAngle(nowAngle);

        if (deffAngle == 0.0f)
        {
            return setAngle;
        }

        if (deffAngle < 0.0f)
        {
            deffAngle += speed;
            if (deffAngle > 0.0f)
            {
                deffAngle = 0.0f;
            }
        }
        else
        {
            deffAngle -= speed;
            if (deffAngle < 0.0f)
            {
                deffAngle = 0.0f;
            }
        }

        return setAngle - deffAngle;
    }

    /// <summary>一定範囲内にする</summary>
    private float GetCanonicalAngle(float angle)
    {
        if (angle > 180.0f)
        {
            angle -= 360.0f;
        }
        else if (angle < -180.0f)
        {
            angle += 360.0f;
        }

        return angle;
    }

    /// <summary>このオブジェクトの有無を設定</summary>
    internal void SetActiveFlag(bool activeFlag)
    {
        if (CheckObject != null)
        {
            Destroy(CheckObject);
        }

        this.gameObject.SetActive(activeFlag);
    }
}
