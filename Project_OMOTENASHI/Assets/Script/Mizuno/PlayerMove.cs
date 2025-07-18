using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private float Force;
    [SerializeField]
    private float acc;
    private float muki;
    Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        muki = 1;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        { 
            acc += Force;
        }

        rb.linearVelocity = new Vector2(rb.linearVelocity.x+(speed + acc) * Time.deltaTime*muki, rb.linearVelocity.y);

        acc *= 0.95f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Wall"))
        {
            muki *= -1;

            rb.AddForce(Vector2.right * muki * speed);

        }
    }

}
