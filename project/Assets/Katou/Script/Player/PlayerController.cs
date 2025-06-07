using Unity.Collections;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// float 座標計算 誤差
    /// </summary>
    private const float FloatPosError = 1.0f;

    /// <summary>
    /// プレイヤーHP
    /// </summary>
    [SerializeField, Header("HP")] private int PlayerHp;

    /// <summary>
    /// プレイヤーリジットボディ
    /// </summary>
    private Rigidbody2D PlayerRigidbody;

    /// <summary>
    /// プレイヤー移動速度 直接触れる予定なし
    /// </summary>
    [SerializeField, Header("プレイヤースピード"), Min(0.01f)] private float PlayerSpeed = 1.0f;
    /// <summary>
    /// 速度設定用
    /// </summary>
    private float UnsignedSpeed
    {
        get { return PlayerSpeed; }
        set { PlayerSpeed = Mathf.Abs(value); }
    }

    /// <summary>
    /// リジットボディ移動フラグ
    /// </summary>
    [SerializeField, Header("リジットボディ移動フラグ")] private bool RigidbodyMoveFlag = true;

    /// <summary>
    /// 重力無効化フラグ
    /// </summary>
    [SerializeField, Header("重力無効化フラグ")] private bool GravityDeactivationFlag = true;

    /// <summary>ジャンプフラグ</summary>
    [SerializeField, Header("ジャンプフラグ")] private bool JumpFlag;
    /// <summary>最大高度</summary>
    [SerializeField, Header("最大高度")] private float MaxHeight;
    /// <summary>最低高度</summary>
    [SerializeField, Header("最低高度")] private float MinHeight;
    /// <summary>ジャンプ移動速度</summary>
    [SerializeField, Header("ジャンプ移動速度")] private float JumpMoveSpeed;
    /// <summary>ジャンプ速度　秒単位</summary>
    [SerializeField, Header("ジャンプ速度　秒単位")] private float JumpSecondSpeed;
    /// <summary>ジャンプ処理秒数 確認用</summary>
    private float CheckJumpSecond;
    /// <summary>ジャンプアップ時速度</summary>
    [SerializeField, Header("ジャンプアップ最大時速度")] private float JumpUpMaxSpeed;
    /// <summary>ジャンプダウン時速度</summary>
    [SerializeField, Header("ジャンプダウン最大時速度")] private float JumpDownMaxSpeed;
    ///// <summary>ジャンプアップ割合</summary>
    //[SerializeField, Header("ジャンプ実行割合")] private float JumpExecutionRatio;
    /// <summary>ジャンプ重力</summary>
    [SerializeField, Header("ジャンプ重力")] private float JumpGravity;


    /// <summary>
    /// フレーム数保存用
    /// </summary>
    [SerializeField, Header("フレーム数保存用")] private int FrameNumber;
    /// <summary>
    /// フレーム数
    /// </summary>
    public int Frame
    {
        get { return FrameNumber; }
        set { FrameNumber = value; }
    }

    void Start()
    {
        // 取得
        {
            PlayerRigidbody = this.GetComponent<Rigidbody2D>();
        }

        // 初期化
        {
            // フレーム数
            //FrameNumber = 0;

            // HP
            PlayerHp = 10;

            // ジャンプ関係
            {
                // ジャンプフラグ
                JumpFlag = false;

                // 最大上昇速度
                JumpUpMaxSpeed = 2f;

                // 最大降下速度
                JumpDownMaxSpeed = -2f;

                // 重力
                JumpGravity = 0.2f;
            }
        }
    }

    void Update()
    {
        // 重力無効化操作
        SetGravityDeactivation();

        if (Input.GetKeyDown(KeyCode.J))
        {
            SetJump();
        }
    }

    void FixedUpdate()
    {
        // 移動
        float moveVec = Input.GetAxisRaw("Horizontal");
        MoveXVec(moveVec);

        // ジャンプ
        JumpProcess();
    }

    /// <summary>
    /// 移動　左
    /// </summary>
    public void MoveLeft(float moveSpeed)
    {
        MoveXVec(-Mathf.Abs(moveSpeed));
    }

    /// <summary>
    /// 移動　右
    /// </summary>
    public void MoveRight(float moveSpeed)
    {
        MoveXVec(Mathf.Abs(moveSpeed));
    }

    /// <summary>
    /// 移動　X軸
    /// </summary>
    public void MoveXVec(float moveSpeed)
    {
        if (RigidbodyMoveFlag)
        {
            PlayerRigidbody.linearVelocity = new Vector3(moveSpeed * UnsignedSpeed, PlayerRigidbody.linearVelocityY);
        }
        else
        {
            this.transform.position += (Vector3.right * moveSpeed * UnsignedSpeed);
        }
        this.transform.localScale = new Vector3(Mathf.Sign(moveSpeed) * Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y, this.transform.localScale.z);
    }

    /// <summary>
    /// リジットボディ移動フラグ　設定
    /// </summary>
    public void SetRigidbodyMoveFlag(bool rigidbodyMoveFlag)
    {
        RigidbodyMoveFlag = rigidbodyMoveFlag;
    }

    /// <summary>
    /// ジャンプ速度設定
    /// </summary>
    public void SetJumpSpeed(float upSpeed, float downSpeed)
    {
        JumpUpMaxSpeed = upSpeed;
        JumpDownMaxSpeed = downSpeed;
    }

    /// <summary>
    /// ジャンプ開始
    /// </summary>
    /// <param name="jumpHeight">ジャンプの到達点</param>
    public void SetJump(/*float jumpHeight*/)
    {
        if (JumpFlag == true)
        {
            return;
        }
        JumpFlag = true;
        GravityDeactivationFlag = true;
        JumpSecondSpeed = JumpUpMaxSpeed;
        //MaxHeight = jumpHeight;
        MinHeight = this.transform.position.y;
        CheckJumpSecond = 0f;
    }

    /// <summary>
    /// ジャンプ処理
    /// </summary>
    private void JumpProcess()
    {
        if (JumpFlag)
        {
            // ジャンプ中処理
            {
                JumpMoveSpeed = (JumpSecondSpeed * 1/*秒*/) - (JumpGravity * 1/*秒数*/);
                JumpSecondSpeed -= (JumpGravity * 1/*秒数*/);

                // 降下速度上限設定
                if (JumpSecondSpeed < JumpDownMaxSpeed)
                {
                    JumpSecondSpeed = JumpDownMaxSpeed;
                }
                //if (Mathf.Sign(JumpMoveSpeed) == 1.0f)
                //{
                //}
                //else
                //{
                //}
            }

            // ジャンプ
            this.transform.position += (Vector3.up * JumpMoveSpeed);

            // ジャンプ終了判定 /*地面に当たった時になるかも*/
            if ((Mathf.Sign(JumpMoveSpeed) == -1f) && ((MinHeight + FloatPosError) >= this.transform.position.y))
            {
                Debug.Log(1000);
                JumpFlag = false;
                GravityDeactivationFlag = false;
            }
        }
    }

    /// <summary>
    /// 重力無効化操作
    /// </summary>
    private void SetGravityDeactivation()
    {
        if (GravityDeactivationFlag)
        {
            PlayerRigidbody.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
            //PlayerRigidbody.useFullKinematicContacts = true;
/*            PlayerRigidbody.linearVelocityY = 0;
            if (PlayerRigidbody.bodyType != RigidbodyType2D.Kinematic)
            {
                PlayerRigidbody.bodyType = RigidbodyType2D.Kinematic;
            }*/
        }
        else
        {
            PlayerRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            //PlayerRigidbody.useFullKinematicContacts = false;
            /*            PlayerRigidbody.linearVelocityY = test;
                        if (PlayerRigidbody.bodyType == RigidbodyType2D.Kinematic)
                        {
                            PlayerRigidbody.bodyType = RigidbodyType2D.Dynamic;
                        }*/
        }
    }

    /// <summary>
    /// ダメージ
    /// </summary>
    public void Damage(int damage)
    {
        PlayerHp -= damage;

        HpCheck();
    }

    /// <summary>
    /// HP確認
    /// </summary>
    private void HpCheck()
    {
        if (PlayerHp <= 0)
        {
            PlayerHp = 0;
            DeathProcess();
        }
    }

    /// <summary>
    /// 死亡処理
    /// </summary>
    private void DeathProcess()
    {
        Debug.Log("死亡");
    }
    
    //*--------*      設定 : 取得      *--------*//

    /// <summary>プレイヤーHP設定</summary>
    public void SetPlayerHp(int hp) {PlayerHp = hp; }
    /// <summary>プレイヤーHP取得</summary>
    public int GetPlayerHp() { return PlayerHp; }

    //*--------*                       *--------*//
}
