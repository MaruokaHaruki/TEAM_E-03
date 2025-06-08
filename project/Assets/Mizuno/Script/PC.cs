using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PC : MonoBehaviour
{
    public int HP=10;
    public float acceleration = 20f;
    public float maxSpeed = 50f;
    public float minSpeed = 0.5f;
    public KeyCode moveKey = KeyCode.RightArrow;
    public int direction = 1; // 1: 右, -1: 左
    public string opponentTag = "Player"; // 衝突する相手のタグ

    public KeyCode jumpKey = KeyCode.Space;
    public float jumpForce = 300f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    private bool isGrounded;


    private Rigidbody2D rb;
    private Vector2 lastVelocity;
    private Vector2 currentVelocity;

    public int maxHP = 3;
    private int currentHP;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHP=maxHP;
    }

    private void Update()
    {
        //if (Input.GetKey(moveKey))
        //{
        //    rb.linearVelocity += new Vector2(direction * acceleration * Time.deltaTime, 0);

        //    if (Mathf.Abs(rb.linearVelocity.x) > maxSpeed)
        //    {
        //        rb.linearVelocity = new Vector2(Mathf.Sign(rb.linearVelocity.x) * maxSpeed, rb.linearVelocity.y);
        //    }

        //    if (Mathf.Abs(rb.linearVelocity.x) < minSpeed)
        //    {
        //        rb.linearVelocity = new Vector2(Mathf.Sign(rb.linearVelocity.x) * minSpeed, rb.linearVelocity.y);
        //    }
        //}

        //連打で加速
        if (Input.GetKeyDown(KeyCode.W))
        {
            rb.linearVelocity += new Vector2(direction * acceleration * Time.deltaTime, 0);

            // Debug.Log(lastVelocity);
            if (Mathf.Abs(rb.linearVelocity.x) > maxSpeed)
            {
                rb.linearVelocity = new Vector2(Mathf.Sign(rb.linearVelocity.x) * maxSpeed, rb.linearVelocity.y);
            }

            if (Mathf.Abs(rb.linearVelocity.x) < minSpeed)
            {
                rb.linearVelocity = new Vector2(Mathf.Sign(rb.linearVelocity.x) * minSpeed, rb.linearVelocity.y);
            }
        }

        // ジャンプ処理
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded && Input.GetKeyDown(jumpKey))
        {
            rb.AddForce(Vector2.up * jumpForce);
        }
        lastVelocity = rb.linearVelocity*0.9f;

        // 左右反転
        if (rb.linearVelocity.x > 0.01f)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (rb.linearVelocity.x < -0.01f)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Reflect();
        }
        else if (collision.gameObject.CompareTag(opponentTag))
        {
            // 相手プレイヤーのスクリプトを取得
            PC opponent = collision.gameObject.GetComponent<PC>();
            if (opponent != null)
            {
                Vector2 selfDir = lastVelocity.normalized;
                Vector2 opponentDir = new Vector2(opponent.direction, 0).normalized;

                float dot = Vector2.Dot(selfDir, opponentDir);

                if (dot < -0.5f)
                {
                    Debug.Log($"{gameObject.name} hit {collision.gameObject.name} from behind → DEAL DAMAGE!");
                    // ここでダメージ処理を呼ぶ（相手に）
                    opponent.TakeDamage(1);
                }
                else
                {
                    Debug.Log($"{gameObject.name} hit {collision.gameObject.name} from front → NO DAMAGE");
                }
            }

            Reflect();
        }
    }

    // 反射処理をまとめたメソッド
    private void Reflect()
    {
        float speed = lastVelocity.magnitude;
        Vector2 reflectDir = Vector2.Reflect(lastVelocity.normalized, Vector2.right);
        rb.linearVelocity = reflectDir * speed;
        direction *= -1;
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;
       // Debug.Log($"{gameObject.name} took {amount} damage! HP = {currentHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
       // Debug.Log($"{gameObject.name} has been defeated!");
        gameObject.SetActive(false); // とりあえず消す
    }
}
