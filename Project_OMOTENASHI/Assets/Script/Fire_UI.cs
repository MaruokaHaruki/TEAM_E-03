using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class Fire_UI : MonoBehaviour
{
    [SerializeField]
    Image fire_image_;

    [Header("プレイヤー")]   //Inspectorでプレイヤー選択してください。
    public PlayerList player;
    private Player instplayer;  // 選択したプレイヤーを持たせる。

    [Header("ゲージ色設定")]
    public Color lowColor = Color.red;
    public Color midColor = Color.yellow;
    public Color highColor = Color.green;


    public enum PlayerList
    {
        Player1,Player2
    }

    private void Start()
    {

        if (player == PlayerList.Player1)
        {

            instplayer = GameManager.Instance.player1_;
        }
        else if (player == PlayerList.Player2)
        {
            instplayer = GameManager.Instance.player2_;
        }
    }

    private void Update()
    {

        float value = GetValue();

        fire_image_.DOFillAmount(value, 0.1f);

        Color col = GetColorByValue(value);
        fire_image_.DOColor(col, 0.1f);
    }

    private float GetValue()
    {
        //0-100%の値を取得
        float spd = instplayer.GetGaugePercentage() * 0.01f;

        //ゲージの値を0-1に変換
        return  Mathf.Clamp(spd,0.0f,1.0f);

         
    }
    private Color GetColorByValue(float val)
    {
        if (val < 0.25f)
        {
            // 赤 → 黄
            float t = Mathf.InverseLerp(0.0f, 0.3f, val);
            return Color.Lerp(lowColor, midColor, t);
        }
        else if (val < 0.6f)
        {
            // 黄 → 緑
            float t = Mathf.InverseLerp(0.3f, 0.7f, val);
            return Color.Lerp(midColor, highColor, t);
        }
        else
        {
            // 緑に固定
            float t = Mathf.InverseLerp(0.7f, 1.0f, val);
            return Color.Lerp(highColor, highColor, t); 
        }
    }

}
