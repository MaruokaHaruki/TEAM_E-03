using DG.Tweening;
using TMPro;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    /// <summary>
    /// 数字表示用テキスト
    /// </summary>
    [SerializeField, Header("数字表示用テキスト")] private TextMeshProUGUI NumberText;
    /// <summary>
    /// 数字表示用テキストのポジション
    /// </summary>
    [SerializeField, Header("数字表示用テキストのポジション")] private RectTransform NumberTextPos;
    /// <summary>
    /// ポジション設定用倍率
    /// </summary>
    [SerializeField, Header("ポジション設定用倍率")] private Vector2 SetNumberTextRatio;

    /// <summary>
    /// テキスト位置修正用カメラ
    /// </summary>
    [SerializeField, Header("修正用カメラ")] private Camera MainCamera;

    /// <summary>
    /// 設定ポジション
    /// </summary>
    [SerializeField, Header("設定ポジション [0,0,0]なら置いてるポジションに変更される")] private Vector2 SetPosition;
    [SerializeField, Header("目標ポジション [0,0,0]なら設定ポジションのサイズ分の座標になる")] private Vector2 TargetPosition;

    /// <summary>
    /// 開始番号
    /// </summary>
    [SerializeField, Header("開始番号")] private int StartKeyType;
    private int SetNumber = -1;
    private KeyCode SetKeyCode = KeyCode.None;

    /// <summary>
    /// 移動フラグ
    /// </summary>
    [SerializeField, Header("移動フラグ")] private bool MoveFlag;

    /// <summary>
    /// 移動速度
    /// </summary>
    [SerializeField, Header("移動速度")] private float MoveSpeed;

    /// <summary>
    /// まっすぐ移動フラグ
    /// </summary>
    [SerializeField] private bool StraightMoveFlag = true;

    [SerializeField] private float EndTime;

    [SerializeField] private float SwayTime;
    [SerializeField] private Vector3 SwayVec;
    [SerializeField] private Vector3 StartSwayVec;

    [SerializeField] private bool ThreeDimensionalFlag = false;

    /// <summary>
    /// 経過時間
    /// </summary>
    private float ElapsedTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 取得
        {
            if (MainCamera == null)
            {
                MainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
            }

            if (NumberTextPos == null)
            {
                if (NumberText != null)
                {
                    NumberTextPos = NumberText.gameObject.GetComponent<RectTransform>();
                }
            }
            else if (NumberText == null)
            {
                NumberText = NumberTextPos.gameObject.GetComponent<TextMeshProUGUI>();
            }
        }

        // 設定
        {
            if (SetPosition == Vector2.zero)
            {
                SetPosition = this.transform.position;
            }
            if (TargetPosition == Vector2.zero)
            {
                TargetPosition = new Vector2(SetPosition.x, SetPosition.y + this.transform.localScale.y);
            }

            if (MoveSpeed == 0.0f)
            {
                MoveSpeed = 1.0f;
            }

            SetNumberTextRatio.x = 0.1f;
            SetNumberTextRatio.y = 0.1f;

            if (EndTime == 0.0f)
            {
                EndTime = 1.0f;
            }
            if (SwayTime == 0.0f)
            {
                SwayTime = EndTime;
            }

            SetNumber = -1;
            if (SetKeyCode == KeyCode.None)
            {
                SetKeyCode = KeyCode.Alpha0;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 描画
        NumberText.text = StartKeyType.ToString();
        // テキスト配置調整用
        if (ThreeDimensionalFlag)
        {
            Vector2 setVec =  this.transform.position - MainCamera.transform.position;
            NumberTextPos.anchoredPosition = new Vector2(-setVec.x * SetNumberTextRatio.x, -setVec.y * SetNumberTextRatio.y);
        }

        if (SetNumber != StartKeyType)
        {
            switch (StartKeyType)
            {
                case 0:
                    SetKeyCode = KeyCode.Alpha0;
                    break;
                case 1:
                    SetKeyCode = KeyCode.Alpha1;
                    break;
                case 2:
                    SetKeyCode = KeyCode.Alpha2;
                    break;
                case 3:
                    SetKeyCode = KeyCode.Alpha3;
                    break;
                case 4:
                    SetKeyCode = KeyCode.Alpha4;
                    break;
                case 5:
                    SetKeyCode = KeyCode.Alpha5;
                    break;
                case 6:
                    SetKeyCode = KeyCode.Alpha6;
                    break;
                case 7:
                    SetKeyCode = KeyCode.Alpha7;
                    break;
                case 8:
                    SetKeyCode = KeyCode.Alpha8;
                    break;
                case 9:
                    SetKeyCode = KeyCode.Alpha9;
                    break;
            }
        }
        if (Input.GetKey(SetKeyCode))
        {
            StartObjectMove();
        }
    }

    private void FixedUpdate()
    {
        // 移動処理
        if (MoveFlag)
        {
            ElapsedTime += Time.deltaTime;

            //this.transform.position += new Vector3(MoveSpeed * Time.deltaTime * (TargetPosition.x == 0.0f ? 0.0f : Mathf.Sign(TargetPosition.x)),
            //                                       MoveSpeed * Time.deltaTime * (TargetPosition.y == 0.0f ? 0.0f : Mathf.Sign(TargetPosition.y)), 0.0f);
            //if ((SetPosition.y + this.transform.lossyScale.y + 0.1) < this.transform.position.y)
            //{
            //    this.transform.position = new Vector3(this.transform.position.x, (SetPosition.y + this.transform.lossyScale.y + 0.1f));
            //}

            //TargetMove(TargetPosition);

            if (ElapsedTime >= EndTime)
            {
                MoveFlag = false;
                SwayVec = StartSwayVec;
                // 移動
            }
            /*else if (ElapsedTime >= SwayTime)
            {
                this.transform.position += SwayVec;
                SwayVec = -SwayVec * 1.1f;
            }*/
        }
        else
        {
            //TargetMove(SetPosition);
            /*this.transform.position -= (((Vector3.up) * MoveSpeed * Time.deltaTime) + (((Vector3.up) * MoveSpeed * Time.deltaTime)));

            if (SetPosition.y > this.transform.position.y)
            {
                this.transform.position = new Vector3(this.transform.position.x, SetPosition.y, this.transform.position.z);
            }*/
        }
    }

    private void StartObjectMove()
    {
        MoveFlag = true;
        ElapsedTime = 0.0f;
    }


    public void SetPositionData(Vector2 pos, Vector2 targetPos)
    {
        SetPosition = pos;
        TargetPosition = targetPos;
    }

    private Vector2 GetTargetVec(Vector2 pos, Vector2 targetPos)
    {
        return (targetPos - pos);
    }

    private void TargetMove(Vector2 targetPos)
    {
        // 移動量取得
        Vector2 setMoveVec = GetTargetVec(this.transform.position, targetPos);
        Vector3 setVec = new Vector3((setMoveVec.x == 0) ? 0 : Mathf.Sign(setMoveVec.x), (setMoveVec.y == 0) ? 0 : Mathf.Sign(setMoveVec.y), 0.0f);

        // 移動
        {
            if (StraightMoveFlag)
            {
                //*
                float denominator = Mathf.Abs(setMoveVec.x) + Mathf.Abs(setMoveVec.y);
                if (denominator != 0.0f)
                {
                    this.transform.position += new Vector3((setMoveVec.x / denominator) * MoveSpeed * Time.deltaTime,
                                                           (setMoveVec.y / denominator) * MoveSpeed * Time.deltaTime, 0.0f);//*/
                }
            }
            else
            {
                this.transform.position += ((Vector3)setVec * MoveSpeed * Time.deltaTime);
            }

            setMoveVec = GetTargetVec(this.transform.position, targetPos);

            if (setVec.x != ((setMoveVec.x == 0) ? 0 : Mathf.Sign(setMoveVec.x)))
            {
                this.transform.position = new Vector3(targetPos.x, this.transform.position.y, this.transform.position.z);
            }
            
            if (setVec.y != ((setMoveVec.y == 0) ? 0 : Mathf.Sign(setMoveVec.y)))
            {
                this.transform.position = new Vector3(this.transform.position.x, targetPos.y, this.transform.position.z);
            }
        }
    }
}