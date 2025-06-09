using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("�Ђ��Ƃۂ����")]
    public int hitPoint; // �v���C���[�̗�

    [Header("�_�Őݒ�")]
    public float flashDuration = 0.1f;     // �_��1��̎���
    public int flashCount = 3;             // �_�ł̉�

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isFlashing = false;
    private bool isInvincible = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    // �v���C���[���U�����󂯂��Ƃ��̏���
    public void TakeDamage(int damage)
    {
        // �_�Œ��̓_���[�W�𖳎�
        if (isInvincible)
        {
            Debug.Log($"{gameObject.name} �͖��G���̂��߃_���[�W�����I");
            return;
        }


        hitPoint -= damage;
        Debug.Log($"{gameObject.name} �� {damage} �̃_���[�W���󂯂��I�c��̗�: {hitPoint}");

        if (!isFlashing)
            StartCoroutine(FlashRed());

        if (hitPoint <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} �͎��S���܂����I");
        // �v���C���[�̎��S����������
        Destroy(gameObject);
    }

    // �_�ŏ���
    private System.Collections.IEnumerator FlashRed()
    {
        isFlashing = true;
        isInvincible = true;

        for (int i = 0; i < flashCount; i++)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.red;
            }

            yield return new WaitForSeconds(flashDuration);

            if (spriteRenderer != null)
            {
                spriteRenderer.color = originalColor;
            }

            yield return new WaitForSeconds(flashDuration);
        }

        isFlashing = false;
        isInvincible = false;
    }
}
