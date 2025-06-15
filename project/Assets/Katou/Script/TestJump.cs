using System.Security.Cryptography.X509Certificates;
using UnityEngine;

/* タスク
 * ジャンプ
 * 追う側切り替えフラグ
 */
public class TestJump : MonoBehaviour {
    /// <summary>ジャンプ数</summary>
    [SerializeField] private int JumpCount;
    [SerializeField, Header("最大ジャンプ数")] private int MaxJumpCount = 1;
    /// <summary>j</summary>
    [SerializeField] private float DownMoveSpeed;
    /// <summary> W     v   x @ b P  </summary>
    [SerializeField] private float JumpSecondSpeed;
    /// <summary> W     v     b    m F p</summary>
    [SerializeField] private float CheckJumpSecond;
    /// <summary> W     v A b v     x</summary>
    [SerializeField, Header(" W     v A b v ő厞   x")] private float JumpPower;
    /// <summary> W     v _ E       x</summary>
    [SerializeField, Header(" W     v _ E   ő厞   x")] private float DownMaxSpeed;
    /// <summary> W     v d  </summary>
    [SerializeField, Header(" W     v d  ")] private float GravityPower;
    private float NowGravityPower;

    /// <summary>
    /// 地面の上にいるフラグ
    /// </summary>
    private bool OnGround;

    [SerializeField, Header("ジャンプキー")] private KeyCode MoveKey;
    [SerializeField] private bool KeyMFlag;

    [SerializeField] private bool RigidbodyMoveFlag = true;
    [SerializeField] private Rigidbody2D MyRigidbody2D;

    void Start() {

        // 取得
        {
            if (MyRigidbody2D == null)
            {
                MyRigidbody2D = this.gameObject.GetComponent<Rigidbody2D>();
            }
        }

        // 設定
        {
            // リジットボディ重力削除
            if (RigidbodyMoveFlag)
            {// 仮置き
                JumpPower = 500.0f;
            }
            else
            {
                MyRigidbody2D.gravityScale = 0;
            }

            OnGround = false;

            NowGravityPower = 0;
            JumpCount = 0;

            if (MoveKey == KeyCode.None)
            {
                if (KeyMFlag)
                {
                    MoveKey = KeyCode.M;
                }
                else
                {
                    MoveKey = KeyCode.C;
                }
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(MoveKey))
        {
            StartJump();
        }
    }

    private void FixedUpdate()
    {
        // 移動初期化
        DownMoveSpeed = 0;

        if (!RigidbodyMoveFlag)
        {
            // ジャンプ
            JumpProcess();

            if (!GetGroundFlag())
            {
                NowGravityPower += GravityPower * Time.deltaTime;
                // 最低速度設定
                if (NowGravityPower < -Mathf.Abs(DownMaxSpeed))
                {
                    NowGravityPower = -Mathf.Abs(DownMaxSpeed);
                }


                DownMoveSpeed -= NowGravityPower;


                // y軸処理
                this.transform.position += (Vector3.up * DownMoveSpeed/* * Time.deltaTime*/);
            }

            OnGround = false;
        }
    }

    /// <summary>
    /// ジャンプスタート
    /// </summary>
    public void StartJump() {
        if (JumpCount >= MaxJumpCount) {
            return;
        }
        JumpCount += 1;
        if (RigidbodyMoveFlag)
        {
            MyRigidbody2D.AddForce(new Vector2(0.0f, JumpPower));
        }
    }



    /// <summary>
    /// ジャンプ処理
    /// </summary>
    public void JumpProcess() {
        if (JumpCount > 0) {
            DownMoveSpeed += JumpPower;

        }
    }


    private void OnCollisionStay2D(Collision2D collision) {
        if (OnGroundCheckObject(collision.gameObject) /*&& (collision.gameObject.tag != "Wall")*/) {
            OnGround = true;
            NowGravityPower = 0;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        // ジャンプチェック
        if (OnGroundCheckObject(collision.gameObject) /*&& (collision.gameObject.tag != "Wall")*/) {
            JumpEnd();
        }
    }




    /// <summary>
    /// 取得したオブジェクトが自分の下にある場合true
    /// </summary>
    public bool OnGroundCheckObject(GameObject checkObject) {
        if (((GetObjectSize(checkObject, 1) - 0.5f) > GetObjectSize(this.gameObject, -1)) &&
            ((GetObjectSize(checkObject, -1) + 0.5f) < GetObjectSize(this.gameObject, 1)) &&
            (checkObject.transform.position.y < this.transform.position.y)) {
            return true;
        }
        return false;
    }

    private float GetObjectSize(GameObject getObject, float direction) {
        return ((getObject.transform.lossyScale.x * 0.5f * direction) + getObject.transform.position.x);
    }

    /// <summary>
    /// ジャンプ終了
    /// </summary>
    public void JumpEnd() {
        JumpCount = 0;
    }

    private bool GetGroundFlag() {
        return OnGround && (JumpCount == 0);
    }
}