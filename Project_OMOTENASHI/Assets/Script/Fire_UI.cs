using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Fire_UI : MonoBehaviour
{
    [SerializeField]
    Image fire_image_;

    [Header("プレイヤー")]   //Inspectorでプレイヤー選択してください。
    public PlayerList player;
    private Player instplayer;  // 選択したプレイヤーを持たせる。
    
    
    //スライダー用の変数(0-1)
    private float value;

    public enum PlayerList
    {
        Player1,Player2
    }

    private void Start()
    {
        value = 0;

        if (player == PlayerList.Player1)
        {
            instplayer = GameManager.Instance.player1_;
        }
        if (player == PlayerList.Player2)
        {
            instplayer = GameManager.Instance.player2_;
        }
    }

    private void Update()
    {
        fire_image_.DOFillAmount(GetValue(), 0.1f);
    }

    private float GetValue()
    {
        float spd = instplayer.GetGaugePercentage() * 0.01f;

        value = Mathf.Clamp(spd,0.0f,1.0f);

        return value; 
    }
}
