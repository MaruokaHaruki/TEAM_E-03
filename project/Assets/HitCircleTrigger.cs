using UnityEngine;
using System;

public class HitCircleTrigger : MonoBehaviour
{
    public event Action<GameObject> OnHitReceived;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �U�����Ă����v���C���[��"Player"�^�O�����邩�m�F
        if (collision.CompareTag("Player"))
        {
            OnHitReceived.Invoke(collision.gameObject);
        }
    }
}
