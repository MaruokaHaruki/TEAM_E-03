using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField, Header("�ړ��ݒ�")]
    public float moveSpeed;

    [SerializeField, Header("�W�����v�ݒ�")]
    public float jumpForce;


    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius;

    [SerializeField, Header("�i�s����")]
    private int direction = 1; // 1: �E, -1: ��

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
    private PlayerInputActions inputActions;
    
    // ���͏�Ԃ�ێ�����ϐ�
    private bool isPush;
    private bool isJumpPressed;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();


        inputActions.Player.Push.performed += ctx => isPush = true;

        inputActions.Player.Jump.performed += ctx => isJumpPressed = true;
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
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
    /// /�f�o�b�O�p
    //private void OnDrawGizmosSelected()
    //{
    //    if (groundCheck != null)
    //    {
    //        Gizmos.color = Color.green;
    //        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    //    }
    //}

    private void UpdateHitCirclePosition()
    {
        // �i�s�����Ƌt������HitCircle��z�u
        float offsetX = Mathf.Abs(HitCircle.transform.localPosition.x); 
        HitCircle.transform.localPosition = new Vector3(-direction * offsetX, HitCircle.transform.localPosition.y, HitCircle.transform.localPosition.z);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Wall") || col.gameObject.CompareTag("Player"))
        {
            // �������]
            direction *= -1;

            // HitCircle�̈ʒu���]
            UpdateHitCirclePosition();

            // �m�b�N�o�b�N�����i�v���C���[�Ƃ̂݁j
            if (col.gameObject.CompareTag("Player"))
            {
                Knockback();
            }

            // �J�����U������
            if (cameraShake != null)
            {
                cameraShake.Shake(0.15f, 0.1f); // �b�A����
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

}