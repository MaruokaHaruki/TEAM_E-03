using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // �����E���x�𔻒肵�ăm�b�N�o�b�N��K�p
            // �󒆂ł��n��ł����l�ɏ�������
        }
    }

}
