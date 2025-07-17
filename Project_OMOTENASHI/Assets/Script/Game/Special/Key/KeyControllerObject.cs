using UnityEngine;

public class KeyControllerObject : MonoBehaviour
{
    /// <summary>変更するキー</summary>
    [SerializeField, Header("変更するキー")] private CHANGE_KEY_TYPE ChangeKeyType;

    /// <summary>変更確認用オブジェクト</summary>
    [SerializeField, Header("変更確認用オブジェクト")] private GameObject[] ChangeCheckObject;

    /// <summary>変更するキー選択用enum</summary>
    enum CHANGE_KEY_TYPE
    {
        PLAYER_A_JUMP = 0,
        PLAYER_A_ACCELERATION,
        PLAYER_B_JUMP,
        PLAYER_B_ACCELERATION,

        // 移動方向反転キー設定
        SET_MOVE_FIELD_CHANGE_KEY,
    }

    /// <summary>開始時ポジション</summary>
    private Vector3 StartPosition;

    // 挟まれた時有効化
    [SerializeField] private GameObject GroundObject;

    /// <summary>
    /// 挟まれているか判定するフラグ(左)
    /// </summary>
    [SerializeField] private HitCount LeftHitFlag;
    /// <summary>
    /// 挟まれているか判定するフラグ(右
    /// </summary>
    [SerializeField] private HitCount RightHitFlag;

    /// <summary>
    /// リジットボディ
    /// </summary>
    private Rigidbody2D KeyRigidbody2D;
    /// <summary>
    /// 上方向への力
    /// </summary>
    [SerializeField] private float UpPower;

    [SerializeField] private MoveField MoveHieldChangeKey;

    private void Awake()
    {
        StartPosition = Vector3.zero;
    }

    private void OnEnable()
    {
        SetStartPosition();
    }

    private void Start()
    {
        StartPosition = this.transform.position;
        if (StartPosition == Vector3.zero)
        {
            StartPosition.z += 0.1f;
        }

        if (GroundObject == null)
        {
            GroundObject = Instantiate(this.gameObject);
            Destroy(GroundObject.GetComponent<KeyControllerObject>());
            GroundObject.tag = "Ground";
        }

        KeyRigidbody2D = this.gameObject.GetComponent<Rigidbody2D>();

        GameObject[] set =  GameObject.FindGameObjectsWithTag("KeyData");
        ChangeCheckObject = new GameObject[set.Length];
        for (int i = 0; i < set.Length; i++)
        {
            ChangeCheckObject[i] = set[i] ;
        }

        SetStartPosition();
    }
    public void SetStartPosition()
    {
        if (StartPosition != Vector3.zero)
        {
            this.transform.position = StartPosition;
            LeftHitFlag.MyHitCount = 0;
            RightHitFlag.MyHitCount = 0;
            GroundObject.SetActive(false);
        }
    }

    private void Update()
    {
        KeyCode setKey = KeyCode.None;
        float hitRange = 0.0f;

        Vector2 playerLeftAndRightPos = new Vector2(this.transform.position.x - (this.transform.localScale.x * 0.5f), this.transform.position.x +(this.transform.localScale.x * 0.5f));

        for (int i = 0; i < ChangeCheckObject.Length; i++)
        {
            if (HitCheck(this.transform.position, this.transform.localScale, ChangeCheckObject[i].transform.position, ChangeCheckObject[i].transform.localScale))
            {
                float objectHitRange = 0.0f;

                if (playerLeftAndRightPos.x < (ChangeCheckObject[i].transform.position.x - (ChangeCheckObject[i].transform.localScale.x * 0.5f)))
                {
                    objectHitRange = Mathf.Abs((ChangeCheckObject[i].transform.position.x - (ChangeCheckObject[i].transform.localScale.x * 0.5f)) - playerLeftAndRightPos.y);
                }
                else if (playerLeftAndRightPos.y > (ChangeCheckObject[i].transform.position.x + (ChangeCheckObject[i].transform.localScale.x * 0.5f)))
                {
                    objectHitRange = Mathf.Abs(playerLeftAndRightPos.x - (ChangeCheckObject[i].transform.position.x + (ChangeCheckObject[i].transform.localScale.x * 0.5f)));
                }
                else
                {
                    objectHitRange = Mathf.Abs(playerLeftAndRightPos.x - playerLeftAndRightPos.y);
                }

                if (objectHitRange > hitRange)
                {
                    setKey = ChangeCheckObject[i].GetComponent<KeyObjectData_Field>().SetKey;
                }
            }
        }

        if (setKey != KeyCode.None)
        {
            switch (ChangeKeyType)
            {
                case CHANGE_KEY_TYPE.PLAYER_A_JUMP:
                    NowMoveKey.Instance.JumpPlayerAKey = setKey;
                    break;

                case CHANGE_KEY_TYPE.PLAYER_A_ACCELERATION:
                    NowMoveKey.Instance.AccelerationPlayerAKey = setKey;
                    break;

                case CHANGE_KEY_TYPE.PLAYER_B_JUMP:
                    NowMoveKey.Instance.JumpPlayerBKey = setKey;
                    break;

                case CHANGE_KEY_TYPE.PLAYER_B_ACCELERATION:
                    NowMoveKey.Instance.AccelerationPlayerBKey = setKey;
                    break;

                case CHANGE_KEY_TYPE.SET_MOVE_FIELD_CHANGE_KEY:
                    if (MoveHieldChangeKey != null)
                    {
                        MoveHieldChangeKey.StartKey = setKey;
                    }
                    break;
            }
        }

        {
            LeftHitFlag.transform.parent.localPosition = Vector3.zero;
            RightHitFlag.transform.parent.localPosition = Vector3.zero;
        }

        if ((LeftHitFlag.MyHitCount > 0) && (RightHitFlag.MyHitCount > 0))
        {
            if (!GroundObject.activeSelf)
            {
                GroundObject.SetActive(true);
            }

            GroundObject.transform.position = this.transform.position;
        }
        else
        {
            if (GroundObject.activeSelf)
            {
                GroundObject.SetActive(false);
            }
        }
    }

    private bool HitCheck(Vector2 srcPos, Vector2 srcSize, Vector2 dstPos, Vector2 dstSize)
    {
        bool hitFlag = true;

        // X軸
        if (((srcPos.x - (srcSize.x * 0.5f)) >= (dstPos.x + (dstSize.x * 0.5f))) || ((srcPos.x + (srcSize.x * 0.5f)) <= (dstPos.x - (dstSize.x * 0.5f))))
        {
            hitFlag = false;
        }

        // Y軸
        if (hitFlag && (((srcPos.y - (srcSize.y * 0.5f)) >= (dstPos.y + (dstSize.y * 0.5f))) || ((srcPos.y + (srcSize.y * 0.5f)) <= (dstPos.y - (dstSize.y * 0.5f)))))
        {
            hitFlag = false;
        }

        return hitFlag;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if ((this.transform.position.y - (this.transform.localScale.y * 0.5f)) >= (collision.transform.position.y + 0.2f/*プレイヤーのサイズを2で割って誤差で0.1f減らした値*/))
            {
                KeyRigidbody2D.AddForceY(UpPower, ForceMode2D.Impulse);
            }
        }
    }
}