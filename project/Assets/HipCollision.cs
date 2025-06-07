using UnityEngine;

public class HipCollision : MonoBehaviour
{
    [SerializeField]
    private PlayerController playerController;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if (this.transform.parent.gameObject != collision.gameObject)
            {
                playerController.Damage(1);
            }
        }
    }
}
