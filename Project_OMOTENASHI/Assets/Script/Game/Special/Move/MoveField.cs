using Unity.VisualScripting;
using UnityEngine;

public class MoveField : MonoBehaviour
{
    /// <summary>�������</summary>
    [SerializeField, Header("�������")] private float MovePower;
    /// <summary>�E�ړ������t���O</summary>
    [SerializeField, Header("�ړ������t���O")] private bool RightMoveFlag;
    /// <summary>�v���C���[</summary>
    [SerializeField, Header("�v���C���[")] private Rigidbody2D[] PlayerRigidBody;

    // �J�n�L�[
    public KeyCode StartKey = KeyCode.B;

    private void Start()
    {
        PlayerRigidBody = new Rigidbody2D[2];
        PlayerRigidBody[0] = GameManager.Instance.player1_.GetComponent<Rigidbody2D>();
        PlayerRigidBody[1] = GameManager.Instance.player2_.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(StartKey))
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
