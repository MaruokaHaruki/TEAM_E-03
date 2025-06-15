using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour
{
    [SerializeField, Header("��_���[�W�����蔻��")]
    private GameObject HitCircle;

    private void Start()
    {
        // �U������锻���L����
        HitCircle.GetComponent<PolygonCollider2D>().enabled = true;
    }

    // HitCircle�ɓ�������������o���邽�߂̒��p�N���X�ɃA�N�Z�X
    private void OnEnable()
    {
        if (HitCircle != null)
        {
            var trigger = HitCircle.GetComponent<HitCircleTrigger>();
            if (trigger != null)
            {
                trigger.OnHitReceived += OnHitReceived;
            }
        }
    }

    private void OnDisable()
    {
        if (HitCircle != null)
        {
            var trigger = HitCircle.GetComponent<HitCircleTrigger>();
            if (trigger != null)
            {
                trigger.OnHitReceived -= OnHitReceived;
            }
        }
    }

    // ���v���C���[��������HitCircle�ɓ������Ƃ��Ă΂��
    private void OnHitReceived(GameObject attacker)
    {
        if (attacker != this.gameObject)
        {
            Debug.Log($"{gameObject.name} �� {attacker.name} �ɍU�����ꂽ�I");
            // �����Ŏ������_���[�W���󂯂鏈��������
            GetComponent<Player>().TakeDamage(1);
        }
    }
}
