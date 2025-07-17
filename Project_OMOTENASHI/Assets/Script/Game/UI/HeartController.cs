using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Fire_UI;

public class HeartController : MonoBehaviour
{
    [Header("プレイヤー")]   //Inspectorでプレイヤー選択してください。
    public PlayerList player;
    private Player instplayer;  // 選択したプレイヤーを持たせる。

    public enum PlayerList
    {
        Player1, Player2
    }

    [SerializeField, Header("空白ハートのプレハブ")]
    private Image heartBlankPrefab;

    [SerializeField, Header("ハートのプレハブ")]
    private Image heartPrefab;

    [SerializeField, Header("ハートを並べる親")]
    private Transform heartParent;

    [SerializeField, Header("ハート同士の間隔")]
    private float heartSpacing = 50f;

    private List<GameObject> heartObjects = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (player == PlayerList.Player1)
        {
            instplayer = GameManager.Instance.player1_;
        }
        else if (player == PlayerList.Player2)
        {
            instplayer = GameManager.Instance.player2_;
        }

        UpdateHeartUI();// 初期ハートUIの更新
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateHeartUI()
    {
        // 一度すべてのハートUIを削除
        foreach (var obj in heartObjects)
        {
            Destroy(obj);
        }
        heartObjects.Clear();

        int maxHeart = instplayer.maxHp_/10;
        int currentHeart = instplayer.currentHp_/10;

        // 並べて配置
        for (int i = 0; i < maxHeart; i++)
        {
            // 空白ハート
            var blank = Instantiate(heartBlankPrefab, heartParent);
            RectTransform blankRect = blank.GetComponent<RectTransform>();
            blankRect.anchoredPosition = new Vector2(i * heartSpacing, 0);
            heartObjects.Add(blank.gameObject);

            // ハート（現在HP以下のみ）
            if (i < currentHeart)
            {
                var heart = Instantiate(heartPrefab, heartParent);
                RectTransform heartRect = heart.GetComponent<RectTransform>();
                heartRect.anchoredPosition = new Vector2(i * heartSpacing, 0);
                heartObjects.Add(heart.gameObject);
            }
        }
    }
}
