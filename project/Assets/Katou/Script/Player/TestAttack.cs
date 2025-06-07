using UnityEngine;

public class TestAttack : MonoBehaviour
{
    /// <summary>
    /// �^�_���[�W��
    /// </summary>
    [SerializeField, Header("�^�_���[�W��")] private int ToDamageVolume = 0;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController setPlayer = collision.gameObject.GetComponent<PlayerController>();
        if (setPlayer != null)
        {
            setPlayer.Damage(ToDamageVolume);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log("�����ɓ������Ă���");
    }
}
