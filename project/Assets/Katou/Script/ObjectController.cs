using UnityEngine;

public class ObjectController : MonoBehaviour
{
    /// <summary>
    /// 設定ポジション
    /// </summary>
    [SerializeField, Header("設定ポジション [0、0、0]なら置いてあるポジションに変更される")] private Vector3 SetPosition;

    /// <summary>
    /// 押す数字
    /// </summary>
    [SerializeField, Header("押す数字")] private int StartKeyType;

    /// <summary>
    /// 起動フラグ
    /// </summary>
    [SerializeField] private bool MoveFlag;

    /// <summary>
    /// 移動速度
    /// </summary>
    [SerializeField, Header("移動速度")] private float MoveSpeed;

    [SerializeField] private float EndTime;

    /// <summary>
    /// 起動経過時間
    /// </summary>
    private float ElapsedTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (SetPosition == Vector3.zero)
        {
            SetPosition = this.transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (StartKeyType)
        {
            case 0:
                if (Input.GetKey(KeyCode.Alpha0))
                {
                    StartObjectMove();
                }
                break;
            case 1:
                if (Input.GetKey(KeyCode.Alpha1))
                {
                    StartObjectMove();
                }
                break;
            case 2:
                if (Input.GetKey(KeyCode.Alpha2))
                {
                    StartObjectMove();
                }
                break;
            case 3:
                if (Input.GetKey(KeyCode.Alpha3))
                {
                    StartObjectMove();
                }
                break;
            case 4:
                if (Input.GetKey(KeyCode.Alpha4))
                {
                    StartObjectMove();
                }
                break;
            case 5:
                if (Input.GetKey(KeyCode.Alpha5))
                {
                    StartObjectMove();
                }
                break;
            case 6:
                if (Input.GetKey(KeyCode.Alpha6))
                {
                    StartObjectMove();
                }
                break;
            case 7:
                if (Input.GetKey(KeyCode.Alpha7))
                {
                    StartObjectMove();
                }
                break;
            case 8:
                if (Input.GetKey(KeyCode.Alpha8))
                {
                    StartObjectMove();
                }
                break;
            case 9:
                if (Input.GetKey(KeyCode.Alpha9))
                {
                    StartObjectMove();
                }
                break;
        }

        // 起動中処理
        {
            if (MoveFlag)
            {
                ElapsedTime += Time.deltaTime;

                this.transform.position += new Vector3(0.0f, MoveSpeed * Time.deltaTime, 0.0f);
                if ((SetPosition.y + this.transform.lossyScale.y + 0.1) < this.transform.position.y)
                {
                    this.transform.position = new Vector3(this.transform.position.x, (SetPosition.y + this.transform.lossyScale.y + 0.1f));
                }

                if (ElapsedTime >= EndTime)
                {
                    MoveFlag = false;
                }
            }
            else
            {
                this.transform.position -= Vector3.up * MoveSpeed * Time.deltaTime;

                if (SetPosition.y > this.transform.position.y)
                {
                    this.transform.position = new Vector3(this.transform.position.x, SetPosition.y, this.transform.position.z);
                }
            }
        }
    }

    private void StartObjectMove()
    {
        MoveFlag = true;
        ElapsedTime = 0.0f;
    }
}
