using TMPro;
using UnityEngine;

public class FrameText : MonoBehaviour
{
    /// <summary>
    /// フレーム描画用テキスト
    /// </summary>
    private TextMeshProUGUI FrameDrawText;

    /// <summary>
    /// フレーム数取得用プレイヤー
    /// </summary>
    [SerializeField, Header("フレーム数取得用プレイヤー")] private PlayerController GetFramePlayer;

    void Start()
    {
        // 取得
        {
            // フレーム描画用テキスト
            FrameDrawText = this.gameObject.GetComponent<TextMeshProUGUI>();

            // フレーム取得用プレイヤー
            if (GetFramePlayer == null)
            {
                GetFramePlayer = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            }
        }

        // 描画
        FrameNumberDraw();
    }

    void Update()
    {
        // 描画
        FrameNumberDraw();
    }

    /// <summary>
    /// フレーム数描画
    /// </summary>
    private void FrameNumberDraw()
    {
        FrameDrawText.text = "FPS [ " + GetFramePlayer.Frame.ToString().PadLeft(2, '0') + " ]";
    }
}
