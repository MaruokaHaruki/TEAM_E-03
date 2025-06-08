using UnityEngine;

public class PlayerController_01_02 : MonoBehaviour
{
    /// <summary>
    /// 移動方向
    /// </summary>
    private Vector2 MoveVec;

    /// <summary>
    /// 常に影響するベクトル
    /// </summary>
    [SerializeField, Header("常に影響するベクトル")] private Vector2 AlwaysVec;

    /// <summary>
    /// 押したときのベクトル
    /// </summary>
    [SerializeField] private Vector2 KeyVec;
    [SerializeField, Header("KeyVecリセット用")] private Vector2 SetKeyVec;
    [SerializeField, Header("KeyVec増加用")] private float KeyPlusAmount;
    [SerializeField, Header("KeyVec減少用")] private float KeyMinusAmount;
    [SerializeField, Header("KeyVec最大移動量")] private float KeyMaxAmount;

    [SerializeField] private bool KeyKFlag;

    public int Hp;

    float time;

    [SerializeField] private float CheckTime;

    [SerializeField] private Material IdeMaterial;
    [SerializeField] private Material InvincibleMaterial;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Hp = 1;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time < CheckTime)
        {
            // 通常色
        }
        else
        {
            // 無敵
        }

        MoveVec = Vector2.zero;

        MoveVec += AlwaysVec;

        if (KeyKFlag)
        {
            if (Input.GetKey(KeyCode.K))
            {
                MoveVec += KeyVec;
                KeyVec.x += KeyPlusAmount;
                if (Mathf.Abs(KeyMaxAmount) < Mathf.Abs(KeyVec.x))
                {
                    if (KeyMaxAmount != 0)
                    {
                        KeyVec.x = (Mathf.Abs(KeyMaxAmount) * Mathf.Sign(KeyVec.x));
                    }
                }
            }
            else
            {
                if (KeyVec.x > SetKeyVec.x)
                {
                    KeyVec.x -= KeyMinusAmount;
                }
                else
                {
                    KeyVec = SetKeyVec;
                }
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.D))
            {
                MoveVec += KeyVec;
                KeyVec.x += KeyPlusAmount;
                if (Mathf.Abs(KeyMaxAmount) < Mathf.Abs(KeyVec.x))
                {
                    if (KeyMaxAmount != 0)
                    {
                        KeyVec.x = (Mathf.Abs(KeyMaxAmount) * Mathf.Sign(KeyVec.x));
                    }
                }
            }
            else
            {
                if (KeyVec.x > SetKeyVec.x)
                {
                    KeyVec.x -= KeyMinusAmount;
                }
                else
                {
                    KeyVec = SetKeyVec;
                }
            }
        }

        this.transform.position += (Vector3)MoveVec * Time.deltaTime;

        if (Hp <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            time = 0;
            SetMoveVecs(collision.transform.position.x > this.transform.position.x);
            //if ()
            //{
            //    MoveVec.x = Mathf.Abs(MoveVec.x); 
            //    this.transform.localScale = new Vector3(Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y, this.transform.localScale.z);
            //}
            //else
            //{
            //    MoveVec.x = -Mathf.Abs(MoveVec.x);
            //    this.transform.localScale = new Vector3(-Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y, this.transform.localScale.z);
            //}
        }

        if (collision.gameObject.tag == "Player")
        {
            if (time < CheckTime)
            {
                Destroy(collision.gameObject);
            }

            if (collision.transform.position.x < this.transform.position.x)
            {
                SetMoveVecs(false);

                this.transform.position += new Vector3(0.1f, 0f);
            }
            else
            {
                SetMoveVecs(true);

                this.transform.position += new Vector3(-0.1f, 0f);
            }
        }
    }

    /// <summary>
    /// 移動方向設定
    /// </summary>
    /// <param name="vecFlag">左方向フラグ</param>
    private void SetMoveVecs(bool leftVecFlag)
    {
        if (!leftVecFlag)
        {

            AlwaysVec.x = Mathf.Abs(AlwaysVec.x);
            KeyVec.x = Mathf.Abs(KeyVec.x);
            SetKeyVec.x = Mathf.Abs(SetKeyVec.x);
            KeyPlusAmount = Mathf.Abs(KeyPlusAmount);
            KeyMinusAmount = -Mathf.Abs(KeyMinusAmount);

            this.transform.localScale = new Vector3(Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y, this.transform.localScale.z);
        }
        else
        {
            AlwaysVec.x = -Mathf.Abs(AlwaysVec.x);
            KeyVec.x = -Mathf.Abs(KeyVec.x);
            SetKeyVec.x = -Mathf.Abs(SetKeyVec.x);
            KeyPlusAmount = -Mathf.Abs(KeyPlusAmount);
            KeyMinusAmount = Mathf.Abs(KeyMinusAmount);

            this.transform.localScale = new Vector3(-Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y, this.transform.localScale.z);
        }
    }

    /// <summary>
    /// 移動方向取得
    /// </summary>
    public float GetMoveDirection()
    {
        return Mathf.Sign(AlwaysVec.x);
    }

    /// <summary>
    /// 移動量取得
    /// </summary>
    /// <returns></returns>
    public Vector2 GetMoveVec()
    {
        return MoveVec;
    }
}
