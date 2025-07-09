using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //========================================
    // シングルトン
    public static UIManager Instance { get; private set; }

    //========================================
    // プレイヤーHP UI
    [Header("プレイヤーHP UI")]
    [Tooltip("プレイヤー1のHPバー")]
    public Slider player1HpBar;
    
    [Tooltip("プレイヤー2のHPバー")]
    public Slider player2HpBar;
    
    [Tooltip("プレイヤー1のHP数値テキスト")]
    public Text player1HpText;
    
    [Tooltip("プレイヤー2のHP数値テキスト")]
    public Text player2HpText;

    //========================================
    // ゲーム状態UI
    [Header("ゲーム状態UI")]
    [Tooltip("勝者表示テキスト")]
    public Text winnerText;
    
    [Tooltip("ゲームオーバーパネル")]
    public GameObject gameOverPanel;

    //========================================
    // ラウンド情報UI
    [Header("ラウンド情報UI")]
    [Tooltip("ラウンド情報表示テキスト")]
    public Text roundInfoText;

    [Tooltip("スコア表示テキスト")]
    public Text scoreText;

    [Tooltip("現在ラウンド数用テキスト")]
    public Text currentRoundText;

    [Tooltip("最大ラウンド数用テキスト")]
    public Text maxRoundText;

    //========================================
    // ラウンド演出UI
    [Header("ラウンド演出UI")]
    [Tooltip("ラウンド開始パネル")]
    public GameObject roundStartPanel;

    [Tooltip("ラウンド開始テキスト")]
    public Text roundStartText;
   
    [Tooltip("ラウンドイメージ")]
    public Image roundFeatureImage;

    [Tooltip("ラウンド開始カウントダウン表示テキスト")]
    public Text countdownText;


    //========================================
    // ゲーム終了演出UI
    [Header("ゲーム終了演出UI")]
    [Tooltip("ゲーム終了パネル")]
    public GameObject gameEndPanel;

    [Tooltip("ゲーム終了テキスト")]
    public Text gameEndText;

    [Tooltip("最終スコア表示テキスト")]
    public Text finalScoreText;

    //========================================
    // RoundDirectionController
    public RoundDirectionController roundDirectionController;


    private void Awake()
    {
        // シングルトンの設定
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeUI();
    }

    void Update()
    {
        UpdatePlayerHPUI();
        UpdateGameStateUI();
    }

    //========================================
    // UI初期化
    private void InitializeUI()
    {
        // ゲームオーバーパネルを非表示
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // 勝者テキストを非表示
        if (winnerText != null)
        {
            winnerText.gameObject.SetActive(false);
        }

        // ラウンド開始パネルを非表示
        if (roundStartPanel != null)
        {
            roundStartPanel.SetActive(false);
        }

        // カウントダウンテキストを非表示
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }

        // ゲーム終了パネルを非表示
        if (gameEndPanel != null)
        {
            gameEndPanel.SetActive(false);
        }
    }

    //========================================
    // プレイヤーHP UI初期化
    public void InitializePlayerHPUI(string player1Id, string player2Id, int player1MaxHp, int player2MaxHp)
    {
        // HPバーの初期化
        if (player1HpBar != null)
        {
            player1HpBar.maxValue = player1MaxHp;
            player1HpBar.value = player1MaxHp;
        }

        if (player2HpBar != null)
        {
            player2HpBar.maxValue = player2MaxHp;
            player2HpBar.value = player2MaxHp;
        }
    }

    //========================================
    // プレイヤーHP UI更新
    private void UpdatePlayerHPUI()
    {
        if (GameManager.Instance == null) return;

        // プレイヤー1のHP表示更新
        if (GameManager.Instance.player1_ != null)
        {
            string player1Id = GameManager.Instance.player1_.playerID_;
            int currentHp = GameManager.Instance.GetPlayerCurrentHp(player1Id);
            int maxHp = GameManager.Instance.GetPlayerMaxHp(player1Id);

            if (player1HpBar != null)
            {
                player1HpBar.value = currentHp;
            }
            if (player1HpText != null)
            {
                player1HpText.text = $"{GameManager.Instance.player1Name_}: {currentHp}/{maxHp}";
            }
        }

        // プレイヤー2のHP表示更新
        if (GameManager.Instance.player2_ != null)
        {
            string player2Id = GameManager.Instance.player2_.playerID_;
            int currentHp = GameManager.Instance.GetPlayerCurrentHp(player2Id);
            int maxHp = GameManager.Instance.GetPlayerMaxHp(player2Id);

            if (player2HpBar != null)
            {
                player2HpBar.value = currentHp;
            }
            if (player2HpText != null)
            {
                player2HpText.text = $"{GameManager.Instance.player2Name_}: {currentHp}/{maxHp}";
            }
        }
    }

    //========================================
    // ゲーム状態UI更新
    private void UpdateGameStateUI()
    {
        if (GameManager.Instance == null) return;

        // 勝者表示の更新
        if (GameManager.Instance.CurrentGameState == GameManager.GameState.GameOver && winnerText != null)
        {
            winnerText.gameObject.SetActive(true);
            string winnerName = GetWinnerName();
            winnerText.text = $"勝者: {winnerName}";
        }
    }

    //========================================
    // ラウンド情報UI更新
    public void UpdateRoundInfoUI(int currentRound, int totalRounds, int player1Score, int player2Score)
    {
        if (roundInfoText != null)
        {
            roundInfoText.text = $"ラウンド {currentRound}/{totalRounds}";
        }

        if (scoreText != null)
        {
            scoreText.text = $"スコア - P1: {player1Score} | P2: {player2Score}";
        }

        if (currentRoundText != null)
        {
            currentRoundText.text = currentRound.ToString();
        }

        if (maxRoundText != null)
        {
            maxRoundText.text = totalRounds.ToString();
        }
    }

    //========================================
    // ラウンド開始UI表示
    public void ShowRoundStartUI(string roundName, string featuresText, Sprite roundImage)
    {
        if (roundStartPanel != null)
        {
            roundStartPanel.SetActive(true);
        }

        if (roundStartText != null)
        {
            roundStartText.text = $"{roundName}\n" +
                                  $"新機能: {featuresText}\n\n" +
                                  $"プレイヤーの準備をしてください";
        }

        if (roundFeatureImage != null && roundImage != null)
        {
            roundFeatureImage.overrideSprite = roundImage;
        }

        if(roundDirectionController!=null)
        {
            roundDirectionController.RoundDirectonStart();
        }
    }

    //========================================
    // ラウンド開始UI非表示
    public void HideRoundStartUI()
    {
        if (roundStartPanel != null)
        {
            roundStartPanel.SetActive(false);
        }

        if (roundDirectionController != null)
        {
            roundDirectionController.RoundDirectonEnd();
        }
    }

    //========================================
    // カウントダウンUI表示
    public void ShowCountdownUI()
    {
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
        }
    }

    //========================================
    // カウントダウンUI更新
    public void UpdateCountdownUI(float remainingTime)
    {
        if (countdownText != null)
        {
            if (remainingTime > 1f)
            {
                countdownText.text = Mathf.Ceil(remainingTime).ToString();
            }
            else if (remainingTime > 0f)
            {
                countdownText.text = "START!";
            }
        }
    }

    //========================================
    // カウントダウンUI非表示
    public void HideCountdownUI()
    {
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }
    }

    //========================================
    // ゲーム終了UI表示
    public void ShowGameEndUI(GameManager.Winner winner, int player1Score, int player2Score)
    {
        if (gameEndPanel != null)
        {
            gameEndPanel.SetActive(true);
        }

        if (gameEndText != null)
        {
            string winnerText = GetWinnerDisplayText(winner);
            gameEndText.text = $"ゲーム終了\n{winnerText}";
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = $"最終スコア\nPlayer 1: {player1Score}\nPlayer 2: {player2Score}";
        }
    }

    //========================================
    // ゲーム終了UI非表示
    public void HideGameEndUI()
    {
        if (gameEndPanel != null)
        {
            gameEndPanel.SetActive(false);
        }
    }

    //========================================
    // ゲームオーバーUI表示
    public void ShowGameOverUI()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    //========================================
    // 勝者名取得
    private string GetWinnerName()
    {
        if (GameManager.Instance == null) return "不明";

        switch (GameManager.Instance.CurrentWinner)
        {
            case GameManager.Winner.Player1:
                return GameManager.Instance.player1Name_;
            case GameManager.Winner.Player2:
                return GameManager.Instance.player2Name_;
            default:
                return "引き分け";
        }
    }

    //========================================
    // 勝者表示テキスト取得
    private string GetWinnerDisplayText(GameManager.Winner winner)
    {
        switch (winner)
        {
            case GameManager.Winner.Player1:
                return "Player 1 の勝利！";
            case GameManager.Winner.Player2:
                return "Player 2 の勝利！";
            case GameManager.Winner.None:
                return "引き分け！";
            default:
                return "結果不明";
        }
    }
}
