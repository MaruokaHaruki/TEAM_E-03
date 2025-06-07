using TMPro;
using UnityEngine;

public class FrameText : MonoBehaviour
{
    /// <summary>
    /// �t���[���`��p�e�L�X�g
    /// </summary>
    private TextMeshProUGUI FrameDrawText;

    /// <summary>
    /// �t���[�����擾�p�v���C���[
    /// </summary>
    [SerializeField, Header("�t���[�����擾�p�v���C���[")] private PlayerController GetFramePlayer;

    void Start()
    {
        // �擾
        {
            // �t���[���`��p�e�L�X�g
            FrameDrawText = this.gameObject.GetComponent<TextMeshProUGUI>();

            // �t���[���擾�p�v���C���[
            if (GetFramePlayer == null)
            {
                GetFramePlayer = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            }
        }

        // �`��
        FrameNumberDraw();
    }

    void Update()
    {
        // �`��
        FrameNumberDraw();
    }

    /// <summary>
    /// �t���[�����`��
    /// </summary>
    private void FrameNumberDraw()
    {
        FrameDrawText.text = "FPS [ " + GetFramePlayer.Frame.ToString().PadLeft(2, '0') + " ]";
    }
}
