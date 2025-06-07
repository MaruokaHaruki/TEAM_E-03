using TMPro;
using UnityEngine;

public class HpText : MonoBehaviour
{
    /// <summary>
    /// HP�\���p�e�L�X�g
    /// </summary>
    private TextMeshProUGUI HpDrawText;

    /// <summary>
    /// HP�擾�p�v���C���[
    /// </summary>
    [SerializeField, Header("HP�擾�p�v���C���[")] private PlayerController GetHpPlayer;

    void Start()
    {
        // �擾
        {
            // HP�\���p
            HpDrawText = this.GetComponent<TextMeshProUGUI>();

            // �t���[���擾�p�v���C���[
            if (GetHpPlayer == null)
            {
                GetHpPlayer = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            }
        }
    }
    void Update()
    {
        // �`��
        HPDraw();
    }

    /// <summary>
    /// �q�b�g�|�C���g�`��
    /// </summary>
    private void HPDraw()
    {
        HpDrawText.text = "HP [ " + GetHpPlayer.GetPlayerHp().ToString().PadLeft(2, '0') + " ]";
    }
}
