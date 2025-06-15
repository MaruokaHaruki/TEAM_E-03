using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{


    public enum PlayerNumber { PlayerA, PlayerB }

    [SerializeField, Header("�v���C���[�ԍ�")]
    private PlayerNumber playerNumber;

    [SerializeField, Header("�ړ��ݒ�")]
    public float moveSpeed;

    [SerializeField, Header("�W�����v�ݒ�")]
    public float jumpForce;


    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius;

    [SerializeField, Header("�i�s����")]
    private int direction = 1; // 1: �E, -1: ��

    [SerializeField, Header("�������]�̃N�[���_�E������")]
    private float flipCooldownTime = 0.3f;
    private float lastFlipTime = -999f;

    [SerializeField, Header("�U���̓����蔻��")]
    private GameObject HitCircle;

    [SerializeField, Header("������")]
    public float recoilForce = 5f; // �����͂̋���

    [SerializeField, Header("�����ɂ��鎞�ԁi�b�j")]
    private float hitCircleDisableTime = 0.2f;
    private bool isHitCircleDisabled = false;

    [SerializeField, Header("�m�b�N�o�b�N�̋���")]
    private float knockbackDistance = 1.0f;

    [SerializeField, Header("�m�b�N�o�b�N�̍���")]
    private float knockbackUpForce = 2.0f;

    [SerializeField, Header("�h�炷�J����")]
    private CameraShake cameraShake;

    private Rigidbody2D rb;
    private PlayerABInputActions inputActions;

    // ���͏�Ԃ�ێ�����ϐ�
    private bool isPush;
    private bool isJumpPressed;
    private bool isGrounded;

    [SerializeField, Header("���̃X�v���C�g")]
    GameObject sprite;

    //�U���̋���
    public float vibrationStrength = 0.1f;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputActions = new PlayerABInputActions();
    }

    private void OnEnable()
    {
        inputActions.Enable();


        switch (playerNumber)
        {
            case PlayerNumber.PlayerA:
                inputActions.PlayerA.Enable();
                inputActions.PlayerA.Push.performed += ctx => isPush = true;
                inputActions.PlayerA.Jump.performed += ctx => isJumpPressed = true;
                break;

            case PlayerNumber.PlayerB:
                inputActions.PlayerB.Enable();
                inputActions.PlayerB.Push.performed += ctx => isPush = true;
                inputActions.PlayerB.Jump.performed += ctx => isJumpPressed = true;
                break;
        }
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }


    private void FixedUpdate()
    {
        // �ړ�����
        if (isPush)
        {
            Vector2 moveVec = new Vector2(moveSpeed * direction, 0);
            rb.AddForce(moveVec);
            isPush = false;

        }

        // �W�����v����
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isJumpPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        isJumpPressed = false;


    }


    private void OnCollisionEnter2D(Collision2D col)
    {
        //Debug.Log($"{gameObject.name} hit {col.gameObject.name}");
        if (col.gameObject.CompareTag("Wall") || col.gameObject.CompareTag("Player"))
        {
            

            // �������]
            direction *= -1;


            if (Time.time - lastFlipTime < flipCooldownTime) return; // �N�[���_�E�����͉������Ȃ�
            lastFlipTime = Time.time;

            // �i�s�����̃X�v���C�g�𔽓]
            Vector3 localScale = sprite.transform.localScale;
            localScale.x = Mathf.Abs(localScale.x) * direction;
            sprite.transform.localScale = localScale;

            // �m�b�N�o�b�N�����i�v���C���[�Ƃ̂݁j
            if (col.gameObject.CompareTag("Player"))
            {
                Knockback();
            }
            // �J�����U������
            if (cameraShake != null)
            {

                cameraShake.Shake(0.2f, 0.3f); // �b�A����
                StartCoroutine(Vib());

            }

            // ���G���ԏ���
            if (!isHitCircleDisabled)
            {
                StartCoroutine(TemporarilyDisableHitCircle());
            }
        }

    }

    // HitCircle���ꎞ�I�ɖ���������R���[�`��
    private System.Collections.IEnumerator TemporarilyDisableHitCircle()
    {
        isHitCircleDisabled = true;

        var collider = HitCircle.GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
            yield return new WaitForSeconds(hitCircleDisableTime);
            collider.enabled = true;
        }

        isHitCircleDisabled = false;
    }

    private void Knockback()
    {
        Vector2 knockbackDirection = new Vector2(direction, 1).normalized;
        Vector2 destination = rb.position + knockbackDirection * knockbackDistance;

        // �n�`��Raycast�Ŋm�F���ĕǂ��Ȃ��Ƃ������ړ�
        RaycastHit2D hit = Physics2D.Raycast(rb.position, knockbackDirection, knockbackDistance, groundLayer);
        if (!hit.collider)
        {
            // �m�b�N�o�b�N�����ɒ��ڈړ�
            rb.MovePosition(destination);
        }
        else
        {
            // �ړ��ł��Ȃ��̂ŁA����������ɔ�����������
            rb.AddForce(new Vector2(0, knockbackUpForce), ForceMode2D.Impulse);
        }
    }

    private System.Collections.IEnumerator Vib()
    {
        if (Gamepad.current != null)
        {
            float[] strengths = { 0.8f, 0.5f, 0.3f, 0.1f };
            float[] durations = { 0.02f, 0.02f, 0.02f, 0.02f };

            for (int i = 0; i < strengths.Length; i++)
            {
                Gamepad.current.SetMotorSpeeds(strengths[i], strengths[i]);
                yield return new WaitForSeconds(durations[i]);
            }

            Gamepad.current.SetMotorSpeeds(0, 0);
        }
    }
}