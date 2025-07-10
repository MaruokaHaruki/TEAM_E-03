using Unity.VisualScripting;
using UnityEngine;

public class MoveField : MonoBehaviour
{
    /// <summary>加える力</summary>
    [SerializeField, Header("加える力")] private float MovePower;
    /// <summary>右移動方向フラグ</summary>
    [SerializeField, Header("移動方向フラグ")] private bool RightMoveFlag;
    /// <summary>プレイヤー</summary>
    [SerializeField, Header("プレイヤー")] private Rigidbody2D[] PlayerRigidBody;

    private void Update()
    {
        // test
        if (Input.GetKeyDown(KeyCode.B))
        {
            RightMoveFlag = !RightMoveFlag;
            this.transform.localScale = new Vector3(-this.transform.localScale.x, this.transform.localScale.y, this.transform.localScale.z);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player")
        {
            return;
        }

        foreach (Rigidbody2D player in PlayerRigidBody)
        {
            if (collision.gameObject == player.gameObject)
            {
                    player.AddForceX(MovePower * (RightMoveFlag ? 1.0f : -1.0f), ForceMode2D.Impulse);
            }
        }
    }
}
