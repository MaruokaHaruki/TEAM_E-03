using UnityEngine;

public class TestAttack_01_02 : MonoBehaviour
{
    [SerializeField] private PlayerController_01_02 Player;

    [SerializeField] private bool HipDamageFlag;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Player != null)
        {
            this.transform.parent.position = Player.transform.position;
            this.transform.parent.gameObject.transform.localScale = Player.gameObject.transform.localScale;
        }
        else
        {
            Destroy(this.transform.parent.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (collision.gameObject != Player.gameObject)
            {
                if (collision.gameObject.GetComponent<PlayerController_01_02>().GetMoveDirection() == Player.GetMoveDirection())
                {
                    if (HipDamageFlag)
                    {
                        Player.Hp = 0;
                        Destroy(this.transform.parent.gameObject);
                    }
                }
                else
                {
                    Player.transform.position += (Vector3.left * Player.GetMoveVec().x);
                }
            }
        }
    }
}
