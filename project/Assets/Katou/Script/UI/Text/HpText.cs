using TMPro;
using UnityEngine;

public class HpText : MonoBehaviour
{
    /// <summary>
    /// HP表示用テキスト
    /// </summary>
    private TextMeshProUGUI HpDrawText;

    /// <summary>
    /// HP取得用プレイヤー
    /// </summary>
    [SerializeField, Header("HP取得用プレイヤー")] private PlayerController GetHpPlayer;

    void Start()
    {
        // 取得
        {
            // HP表示用
            HpDrawText = this.GetComponent<TextMeshProUGUI>();

            // フレーム取得用プレイヤー
            if (GetHpPlayer == null)
            {
                GetHpPlayer = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            }
        }
    }
    void Update()
    {
        // 描画
        HPDraw();
    }

    /// <summary>
    /// ヒットポイント描画
    /// </summary>
    private void HPDraw()
    {
        HpDrawText.text = "HP [ " + GetHpPlayer.GetPlayerHp().ToString().PadLeft(2, '0') + " ]";
    }
}
