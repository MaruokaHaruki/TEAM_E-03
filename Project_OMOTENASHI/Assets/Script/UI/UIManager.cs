using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//=============================================================================
/// UI管理クラス
public class UIManager : MonoBehaviour {
    ///--------------------------------------------------------------
    ///						 public変数
    //========================================
    // シングルトン
    public static UIManager Instance { get; private set; }

    ///--------------------------------------------------------------
    ///						 ゲーム中UI
    //========================================
    [Header("ゲーム中UI")]
    [Tooltip("プレイヤー1のHPバー")]
    public Slider player1HpBar;

    [Tooltip("プレイヤー2のHPバー")]
    public Slider player2HpBar;

    [Tooltip("プレイヤー1のHP数値テキスト")]
    public Text player1HpText;

    [Tooltip("プレイヤー2のHP数値テキスト")]
    public Text player2HpText;

    ///--------------------------------------------------------------
    ///						 ラウンド情報UI
    //========================================
    [Header("ラウンド情報UI")]
    [Tooltip("ラウンド情報表示テキスト")]
    public Text roundInfoText;

    [Tooltip("スコア表示テキスト")]
    public Text scoreText;

    ///--------------------------------------------------------------
    ///						 ラウンド開始UI
    //========================================
    [Header("ラウンド開始UI")]
    [Tooltip("ラウンド開始パネル")]
    public GameObject roundStartPanel;

    [Tooltip("ラウンド名表示テキスト")]
    public Text roundNameText;

    [Tooltip("新機能説明テキスト")]
    public Text newFeaturesText;

    [Tooltip("カウントダウン表示テキスト")]
    public Text countdownText;

    ///--------------------------------------------------------------
    ///						 ゲーム終了UI
    //========================================
    [Header("ゲーム終了UI")]
    [Tooltip("ゲームオーバーパネル")]
    public GameObject gameOverPanel;

    [Tooltip("勝者表示テキスト")]
    public Text winnerText;

    [Tooltip("ゲーム終了パネル")]
    public GameObject gameEndPanel;

    [Tooltip("ゲーム終了テキスト")]
    public Text gameEndText;

    [Tooltip("最終スコア表示テキスト")]
    public Text finalScoreText;

    ///--------------------------------------------------------------
    ///						 初期化前初期化
    private void Awake() {
        // シングルトンの設定
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    ///--------------------------------------------------------------
    ///						 初期化
    void Start() {
        InitializeUI();
    }

    ///--------------------------------------------------------------
    ///						 UI初期化
    private void InitializeUI() {
        // ゲームオーバーパネルを非表示
        if (gameOverPanel != null) {
            gameOverPanel.SetActive(false);
        }

        // 勝者テキストを非表示
        if (winnerText != null) {
            winnerText.gameObject.SetActive(false);
        }

        // ラウンド開始パネルを非表示
        if (roundStartPanel != null) {
            roundStartPanel.SetActive(false);
        }

        // カウントダウンテキストを非表示
        if (countdownText != null) {
            countdownText.gameObject.SetActive(false);
        }

        // ゲーム終了パネルを非表示
        if (gameEndPanel != null) {
            gameEndPanel.SetActive(false);
        }
    }

    ///--------------------------------------------------------------
    ///						 プレイヤーHP表示更新
    public void UpdatePlayerHP(string playerId, int currentHp, int maxHp, string playerName) {
        // プレイヤー1の場合
        if (GameManager.Instance != null && GameManager.Instance.player1_ != null &&
            GameManager.Instance.player1_.playerID_ == playerId) {
            if (player1HpBar != null) {
                player1HpBar.maxValue = maxHp;
                player1HpBar.value = currentHp;
            }
            if (player1HpText != null) {
                player1HpText.text = $"{playerName}: {currentHp}/{maxHp}";
            }
        }
        // プレイヤー2の場合
        else if (GameManager.Instance != null && GameManager.Instance.player2_ != null &&
                 GameManager.Instance.player2_.playerID_ == playerId) {
            if (player2HpBar != null) {
                player2HpBar.maxValue = maxHp;
                player2HpBar.value = currentHp;
            }
            if (player2HpText != null) {
                player2HpText.text = $"{playerName}: {currentHp}/{maxHp}";
            }
        }
    }

    ///--------------------------------------------------------------
    ///						 ラウンド情報更新
    public void UpdateRoundInfo(int currentRound, int totalRounds, int player1Score, int player2Score) {
        if (roundInfoText != null) {
            roundInfoText.text = $"ラウンド {currentRound}/{totalRounds}";
        }

        if (scoreText != null) {
            scoreText.text = $"スコア - P1: {player1Score} | P2: {player2Score}";
        }
    }

    ///--------------------------------------------------------------
    ///						 ラウンド開始UI表示
    public void ShowRoundStart(string roundName, string features) {
        if (roundStartPanel != null) {
            roundStartPanel.SetActive(true);
        }

        if (roundNameText != null) {
            roundNameText.text = roundName;
        }

        if (newFeaturesText != null) {
            newFeaturesText.text = $"新機能: {features}";
        }
    }

    ///--------------------------------------------------------------
    ///						 ラウンド開始UI非表示
    public void HideRoundStart() {
        if (roundStartPanel != null) {
            roundStartPanel.SetActive(false);
        }
    }

    ///--------------------------------------------------------------
    ///						 カウントダウン表示
    public void ShowCountdown(string text) {
        if (countdownText != null) {
            countdownText.gameObject.SetActive(true);
            countdownText.text = text;
        }
    }

    ///--------------------------------------------------------------
    ///						 カウントダウン非表示
    public void HideCountdown() {
        if (countdownText != null) {
            countdownText.gameObject.SetActive(false);
        }
    }

    ///--------------------------------------------------------------
    ///						 ゲームオーバー表示
    public void ShowGameOver(string winnerName) {
        if (gameOverPanel != null) {
            gameOverPanel.SetActive(true);
        }

        if (winnerText != null) {
            winnerText.gameObject.SetActive(true);
            winnerText.text = $"勝者: {winnerName}";
        }
    }

    ///--------------------------------------------------------------
    ///						 ゲーム終了表示
    public void ShowGameEnd(string winnerText, int player1Score, int player2Score) {
        if (gameEndPanel != null) {
            gameEndPanel.SetActive(true);
        }

        if (gameEndText != null) {
            gameEndText.text = $"ゲーム終了\n{winnerText}";
        }

        if (finalScoreText != null) {
            finalScoreText.text = $"最終スコア\nPlayer 1: {player1Score}\nPlayer 2: {player2Score}";
        }
    }

    ///--------------------------------------------------------------
    ///						 ゲーム終了UI非表示
    public void HideGameEnd() {
        if (gameEndPanel != null) {
            gameEndPanel.SetActive(false);
        }
    }

    ///--------------------------------------------------------------
    ///						 全UI初期化
    public void ResetAllUI() {
        HideRoundStart();
        HideCountdown();
        HideGameEnd();

        if (gameOverPanel != null) {
            gameOverPanel.SetActive(false);
        }

        if (winnerText != null) {
            winnerText.gameObject.SetActive(false);
        }
    }
}
