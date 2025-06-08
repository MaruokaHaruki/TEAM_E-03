using UnityEngine;

public class PlayerController_01_02 : MonoBehaviour
{
    /// <summary>
    /// �ړ�����
    /// </summary>
    private Vector2 MoveVec;

    /// <summary>
    /// ��ɉe������x�N�g��
    /// </summary>
    [SerializeField, Header("��ɉe������x�N�g��")] private Vector2 AlwaysVec;

    /// <summary>
    /// �������Ƃ��̃x�N�g��
    /// </summary>
    [SerializeField] private Vector2 KeyVec;
    [SerializeField, Header("KeyVec���Z�b�g�p")] private Vector2 SetKeyVec;
    [SerializeField, Header("KeyVec�����p")] private float KeyPlusAmount;
    [SerializeField, Header("KeyVec�����p")] private float KeyMinusAmount;
    [SerializeField, Header("KeyVec�ő�ړ���")] private float KeyMaxAmount;

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
            // �ʏ�F
        }
        else
        {
            // ���G
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
    /// �ړ������ݒ�
    /// </summary>
    /// <param name="vecFlag">�������t���O</param>
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
    /// �ړ������擾
    /// </summary>
    public float GetMoveDirection()
    {
        return Mathf.Sign(AlwaysVec.x);
    }

    /// <summary>
    /// �ړ��ʎ擾
    /// </summary>
    /// <returns></returns>
    public Vector2 GetMoveVec()
    {
        return MoveVec;
    }
}
