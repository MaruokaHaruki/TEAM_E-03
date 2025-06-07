using Unity.Collections;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// float ���W�v�Z �덷
    /// </summary>
    private const float FloatPosError = 1.0f;

    /// <summary>
    /// �v���C���[HP
    /// </summary>
    [SerializeField, Header("HP")] private int PlayerHp;

    /// <summary>
    /// �v���C���[���W�b�g�{�f�B
    /// </summary>
    private Rigidbody2D PlayerRigidbody;

    /// <summary>
    /// �v���C���[�ړ����x ���ڐG���\��Ȃ�
    /// </summary>
    [SerializeField, Header("�v���C���[�X�s�[�h"), Min(0.01f)] private float PlayerSpeed = 1.0f;
    /// <summary>
    /// ���x�ݒ�p
    /// </summary>
    private float UnsignedSpeed
    {
        get { return PlayerSpeed; }
        set { PlayerSpeed = Mathf.Abs(value); }
    }

    /// <summary>
    /// ���W�b�g�{�f�B�ړ��t���O
    /// </summary>
    [SerializeField, Header("���W�b�g�{�f�B�ړ��t���O")] private bool RigidbodyMoveFlag = true;

    /// <summary>
    /// �d�͖������t���O
    /// </summary>
    [SerializeField, Header("�d�͖������t���O")] private bool GravityDeactivationFlag = true;

    /// <summary>�W�����v�t���O</summary>
    [SerializeField, Header("�W�����v�t���O")] private bool JumpFlag;
    /// <summary>�ő卂�x</summary>
    [SerializeField, Header("�ő卂�x")] private float MaxHeight;
    /// <summary>�Œፂ�x</summary>
    [SerializeField, Header("�Œፂ�x")] private float MinHeight;
    /// <summary>�W�����v�ړ����x</summary>
    [SerializeField, Header("�W�����v�ړ����x")] private float JumpMoveSpeed;
    /// <summary>�W�����v���x�@�b�P��</summary>
    [SerializeField, Header("�W�����v���x�@�b�P��")] private float JumpSecondSpeed;
    /// <summary>�W�����v�����b�� �m�F�p</summary>
    private float CheckJumpSecond;
    /// <summary>�W�����v�A�b�v�����x</summary>
    [SerializeField, Header("�W�����v�A�b�v�ő厞���x")] private float JumpUpMaxSpeed;
    /// <summary>�W�����v�_�E�������x</summary>
    [SerializeField, Header("�W�����v�_�E���ő厞���x")] private float JumpDownMaxSpeed;
    ///// <summary>�W�����v�A�b�v����</summary>
    //[SerializeField, Header("�W�����v���s����")] private float JumpExecutionRatio;
    /// <summary>�W�����v�d��</summary>
    [SerializeField, Header("�W�����v�d��")] private float JumpGravity;


    /// <summary>
    /// �t���[�����ۑ��p
    /// </summary>
    [SerializeField, Header("�t���[�����ۑ��p")] private int FrameNumber;
    /// <summary>
    /// �t���[����
    /// </summary>
    public int Frame
    {
        get { return FrameNumber; }
        set { FrameNumber = value; }
    }

    void Start()
    {
        // �擾
        {
            PlayerRigidbody = this.GetComponent<Rigidbody2D>();
        }

        // ������
        {
            // �t���[����
            //FrameNumber = 0;

            // HP
            PlayerHp = 10;

            // �W�����v�֌W
            {
                // �W�����v�t���O
                JumpFlag = false;

                // �ő�㏸���x
                JumpUpMaxSpeed = 2f;

                // �ő�~�����x
                JumpDownMaxSpeed = -2f;

                // �d��
                JumpGravity = 0.2f;
            }
        }
    }

    void Update()
    {
        // �d�͖���������
        SetGravityDeactivation();

        if (Input.GetKeyDown(KeyCode.J))
        {
            SetJump();
        }
    }

    void FixedUpdate()
    {
        // �ړ�
        float moveVec = Input.GetAxisRaw("Horizontal");
        MoveXVec(moveVec);

        // �W�����v
        JumpProcess();
    }

    /// <summary>
    /// �ړ��@��
    /// </summary>
    public void MoveLeft(float moveSpeed)
    {
        MoveXVec(-Mathf.Abs(moveSpeed));
    }

    /// <summary>
    /// �ړ��@�E
    /// </summary>
    public void MoveRight(float moveSpeed)
    {
        MoveXVec(Mathf.Abs(moveSpeed));
    }

    /// <summary>
    /// �ړ��@X��
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
    /// ���W�b�g�{�f�B�ړ��t���O�@�ݒ�
    /// </summary>
    public void SetRigidbodyMoveFlag(bool rigidbodyMoveFlag)
    {
        RigidbodyMoveFlag = rigidbodyMoveFlag;
    }

    /// <summary>
    /// �W�����v���x�ݒ�
    /// </summary>
    public void SetJumpSpeed(float upSpeed, float downSpeed)
    {
        JumpUpMaxSpeed = upSpeed;
        JumpDownMaxSpeed = downSpeed;
    }

    /// <summary>
    /// �W�����v�J�n
    /// </summary>
    /// <param name="jumpHeight">�W�����v�̓��B�_</param>
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
    /// �W�����v����
    /// </summary>
    private void JumpProcess()
    {
        if (JumpFlag)
        {
            // �W�����v������
            {
                JumpMoveSpeed = (JumpSecondSpeed * 1/*�b*/) - (JumpGravity * 1/*�b��*/);
                JumpSecondSpeed -= (JumpGravity * 1/*�b��*/);

                // �~�����x����ݒ�
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

            // �W�����v
            this.transform.position += (Vector3.up * JumpMoveSpeed);

            // �W�����v�I������ /*�n�ʂɓ����������ɂȂ邩��*/
            if ((Mathf.Sign(JumpMoveSpeed) == -1f) && ((MinHeight + FloatPosError) >= this.transform.position.y))
            {
                Debug.Log(1000);
                JumpFlag = false;
                GravityDeactivationFlag = false;
            }
        }
    }

    /// <summary>
    /// �d�͖���������
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
    /// �_���[�W
    /// </summary>
    public void Damage(int damage)
    {
        PlayerHp -= damage;

        HpCheck();
    }

    /// <summary>
    /// HP�m�F
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
    /// ���S����
    /// </summary>
    private void DeathProcess()
    {
        Debug.Log("���S");
    }
    
    //*--------*      �ݒ� : �擾      *--------*//

    /// <summary>�v���C���[HP�ݒ�</summary>
    public void SetPlayerHp(int hp) {PlayerHp = hp; }
    /// <summary>�v���C���[HP�擾</summary>
    public int GetPlayerHp() { return PlayerHp; }

    //*--------*                       *--------*//
}
