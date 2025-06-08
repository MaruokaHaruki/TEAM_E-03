using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("�ړ��ݒ�")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 300f;
    [Header("�W�����v�ݒ�")]
    public float jumpForce = 8f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    private Rigidbody2D rb;
    private PlayerInputActions inputActions;
    private Vector2 moveInput;
    private Vector2 rotateInput;
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

        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += _ => moveInput = Vector2.zero;

        inputActions.Player.Rotate.performed += ctx => rotateInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Rotate.canceled += _ => rotateInput = Vector2.zero;

        inputActions.Player.Jump.performed += ctx => isJumpPressed = true;
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    private void FixedUpdate()
    {
        // �����ړ�
        Vector2 velocity = transform.right * moveInput.x * moveSpeed;
        rb.linearVelocity= new Vector2(velocity.x, rb.linearVelocity.y);

        // ��]�i���E�X�e�B�b�N��������͂Ŋp�x�𑀍삷��Ȃǁj
        if (rotateInput.x != 0)
        {
            float rotation = -rotateInput.x * rotationSpeed * Time.fixedDeltaTime;
            transform.Rotate(Vector3.forward, rotation);
        }

        // �W�����v����
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isJumpPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        isJumpPressed = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}