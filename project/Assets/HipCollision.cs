using UnityEngine;

public class HipCollision : MonoBehaviour
{
    [SerializeField]
    private PlayerController playerController;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if (playerController.transform.gameObject != collision.gameObject)
            {
                playerController.Damage(1);
            }
        }
        
    }

    private void Update() {
        if (playerController != null) {
            this.transform.parent.position = playerController.transform.position;
            this.transform.parent.localScale = playerController.transform.localScale;
        }
    }
}
