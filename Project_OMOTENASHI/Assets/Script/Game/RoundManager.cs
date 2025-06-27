using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RoundManager : MonoBehaviour {
    [Header("ラウンド設定")]
    [Tooltip("全ラウンドの設定データ")]
    public List<RoundSettings> roundSettingsList = new List<RoundSettings>();

    [Tooltip("現在のラウンド番号（1から開始）")]
    public int currentRoundNumber = 1;

    [Header("UI要素")]
    [Tooltip("ラウンド情報表示テキスト")]
    public Text roundInfoText;

    [Tooltip("スコア表示テキスト")]
    public Text scoreText;

    [Tooltip("ラウンド開始パネル")]
    public GameObject roundStartPanel;

    [Tooltip("ラウンド開始テキスト")]
    public Text roundStartText;

    [Header("スコア管理")]
    [Tooltip("プレイヤー1のスコア")]
    public int player1Score = 0;

    [Tooltip("プレイヤー2のスコア")]
    public int player2Score = 0;

    [Tooltip("勝利に必要なスコア")]
    public int targetScore = 5;

    [Header("ラウンド勝利演出")]
    [Tooltip("ラウンド勝利パネル")]
    public GameObject roundWinPanel;

    [Tooltip("ラウンド勝利テキスト")]
    public Text roundWinText;

    [Tooltip("ラウンド勝利演出の表示時間")]
    public float roundWinDisplayTime = 2.0f;

    [Header("ゲーム終了演出")]
    [Tooltip("ゲーム終了パネル")]
    public GameObject gameEndPanel;

    [Tooltip("ゲーム終了テキスト")]
    public Text gameEndText;

    [Tooltip("最終スコア表示テキスト")]
    public Text finalScoreText;

    [Tooltip("ゲーム終了演出の表示時間")]
    public float gameEndDisplayTime = 5.0f;

    // プライベート変数
    private RoundSettings currentRoundSettings;
    private bool isRoundTransition = false;
    private bool isGameEnd = false;
    private bool isRoundWinDisplay = false;
    private float roundStartTimer = 0f;
    private float gameEndTimer = 0f;
    private float roundWinTimer = 0f;
    private const float ROUND_START_DISPLAY_TIME = 3f;
    private const float ROUND_END_DELAY = 2f; // ラウンド終了から次ラウンド開始までの遅延

    // シングルトン
    public static RoundManager Instance { get; private set; }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    void Start() {
        InitializeRound();
    }

    void Update() {
        // ラウンド開始演出の処理
        if (isRoundTransition) {
            roundStartTimer -= Time.deltaTime;
            if (roundStartTimer <= 0f) {
                EndRoundTransition();
            }
        }

        // ラウンド勝利演出の処理
        if (isRoundWinDisplay) {
            roundWinTimer -= Time.deltaTime;
            if (roundWinTimer <= 0f) {
                EndRoundWinDisplay();
            }
        }

        // ゲーム終了演出の処理
        if (isGameEnd) {
            gameEndTimer -= Time.deltaTime;
            if (gameEndTimer <= 0f) {
                // ゲーム終了演出終了後の処理
                EndGameTransition();
            }
        }

        UpdateUI();
    }

    /// <summary>
    /// ラウンドの初期化
    /// </summary>
    private void InitializeRound() {
        if (currentRoundNumber <= roundSettingsList.Count) {
            currentRoundSettings = roundSettingsList[currentRoundNumber - 1];
            
            // プレイヤーの状態をリセット
            if (GameManager.Instance != null) {
                GameManager.Instance.RestartRound();
            }
            
            ApplyRoundSettings();
            StartRoundTransition();
            Debug.Log($"[ROUND MANAGER] : ラウンド {currentRoundNumber} を開始します");
        }
        else {
            Debug.LogError("[ROUND MANAGER] : ラウンド設定が不足しています");
        }
    }

    /// <summary>
    /// 現在のラウンド設定をプレイヤーに適用
    /// </summary>
    private void ApplyRoundSettings() {
        if (currentRoundSettings == null) return;

        // GameManagerのプレイヤー参照を取得
        if (GameManager.Instance != null) {
            Player player1 = GameManager.Instance.player1_;
            Player player2 = GameManager.Instance.player2_;

            // プレイヤー1に設定を適用
            if (player1 != null) {
                ApplySettingsToPlayer(player1);
            }

            // プレイヤー2に設定を適用
            if (player2 != null) {
                ApplySettingsToPlayer(player2);
            }
        }
    }

    /// <summary>
    /// 個別プレイヤーに設定を適用
    /// </summary>
    private void ApplySettingsToPlayer(Player player) {
        player.enableDoubleJump_ = currentRoundSettings.enableDoubleJump;
        player.enableStomp_ = currentRoundSettings.enableStomp;
        player.enableReverseJump_ = currentRoundSettings.enableReverseJump;
        player.enableSpeedTransfer_ = currentRoundSettings.enableSpeedTransfer;
        player.maxHp_ = currentRoundSettings.playerMaxHp;
        player.currentHp_ = currentRoundSettings.playerMaxHp;
        player.maxSpeed_ = currentRoundSettings.baseSpeed;
        player.jumpForce_ = currentRoundSettings.jumpForce;
        player.invincibilityDuration_ = currentRoundSettings.invincibilityDuration;
        player.stompStunDuration_ = currentRoundSettings.stompStunDuration;

        Debug.Log($"[ROUND MANAGER] : {player.gameObject.name} にラウンド{currentRoundNumber}の設定を適用しました");
    }

    /// <summary>
    /// ラウンド開始演出
    /// </summary>
    private void StartRoundTransition() {
        isRoundTransition = true;
        roundStartTimer = ROUND_START_DISPLAY_TIME;

        if (roundStartPanel != null) {
            roundStartPanel.SetActive(true);
        }

        if (roundStartText != null) {
            roundStartText.text = $"{currentRoundSettings.roundName}\n" +
                                  $"新機能: {GetNewFeaturesText()}";
        }

        // ゲームを一時停止
        if (GameManager.Instance != null) {
            GameManager.Instance.CurrentGameState = GameManager.GameState.Paused;
        }
    }

    /// <summary>
    /// ラウンド開始演出終了
    /// </summary>
    private void EndRoundTransition() {
        isRoundTransition = false;

        if (roundStartPanel != null) {
            roundStartPanel.SetActive(false);
        }

        // ゲームを再開
        if (GameManager.Instance != null) {
            GameManager.Instance.CurrentGameState = GameManager.GameState.Playing;
        }
    }

    /// <summary>
    /// 新機能のテキストを生成
    /// </summary>
    private string GetNewFeaturesText() {
        List<string> features = new List<string>();

        if (currentRoundSettings.enableDoubleJump)
            features.Add("2段ジャンプ");
        if (currentRoundSettings.enableStomp)
            features.Add("踏みつけ");
        if (currentRoundSettings.enableReverseJump)
            features.Add("反転ジャンプ");
        if (!currentRoundSettings.enableSpeedTransfer)
            features.Add("速度交換無効");

        return features.Count > 0 ? string.Join(", ", features) : "なし";
    }

    /// <summary>
    /// プレイヤーが勝利した際に呼ばれる
    /// </summary>
    public void OnPlayerWin(GameManager.Winner winner) {
        if (currentRoundSettings == null) return;

        // スコアを加算
        switch (winner) {
            case GameManager.Winner.Player1:
                player1Score += currentRoundSettings.winPoints;
                Debug.Log($"[ROUND MANAGER] : Player1 が {currentRoundSettings.winPoints} ポイント獲得！ 総スコア: {player1Score}");
                break;
            case GameManager.Winner.Player2:
                player2Score += currentRoundSettings.winPoints;
                Debug.Log($"[ROUND MANAGER] : Player2 が {currentRoundSettings.winPoints} ポイント獲得！ 総スコア: {player2Score}");
                break;
        }

        // ラウンド勝利表示を開始
        StartRoundWinDisplay(winner);
    }

    /// <summary>
    /// ラウンド勝利表示開始
    /// </summary>
    private void StartRoundWinDisplay(GameManager.Winner winner) {
        isRoundWinDisplay = true;
        roundWinTimer = roundWinDisplayTime;

        if (roundWinPanel != null) {
            roundWinPanel.SetActive(true);
        }

        if (roundWinText != null) {
            string winnerText = "";
            switch (winner) {
                case GameManager.Winner.Player1:
                    winnerText = "Player 1 の勝利！";
                    break;
                case GameManager.Winner.Player2:
                    winnerText = "Player 2 の勝利！";
                    break;
                case GameManager.Winner.None:
                    winnerText = "引き分け！";
                    break;
            }
            roundWinText.text = $"ラウンド {currentRoundNumber} 終了\n{winnerText}\n\n現在のスコア\nP1: {player1Score}  P2: {player2Score}";
        }

        // ゲームを一時停止
        if (GameManager.Instance != null) {
            GameManager.Instance.CurrentGameState = GameManager.GameState.Paused;
        }

        Debug.Log($"[ROUND MANAGER] : ラウンド {currentRoundNumber} の勝利表示を開始します");
    }

    /// <summary>
    /// ラウンド勝利表示終了
    /// </summary>
    private void EndRoundWinDisplay() {
        isRoundWinDisplay = false;

        if (roundWinPanel != null) {
            roundWinPanel.SetActive(false);
        }

        // 勝利条件チェック
        if (player1Score >= targetScore || player2Score >= targetScore) {
            // 目標スコア到達でゲーム終了
            Debug.Log($"[ROUND MANAGER] : 目標スコア到達！ゲーム終了へ移行");
            EndGame();
        }
        else if (currentRoundNumber >= roundSettingsList.Count) {
            // 全ラウンド終了でゲーム終了
            Debug.Log($"[ROUND MANAGER] : 全ラウンド終了！ゲーム終了へ移行");
            EndGame();
        }
        else {
            // 次のラウンドに進む
            Debug.Log($"[ROUND MANAGER] : 次のラウンドへ進行");
            StartCoroutine(DelayedNextRound());
        }
    }

    /// <summary>
    /// 遅延付きで次のラウンドに進む
    /// </summary>
    private System.Collections.IEnumerator DelayedNextRound() {
        // ラウンド終了の表示
        Debug.Log($"[ROUND MANAGER] : ラウンド {currentRoundNumber} 終了。{ROUND_END_DELAY}秒後に次のラウンドを開始します。");
        
        yield return new UnityEngine.WaitForSeconds(ROUND_END_DELAY);
        
        NextRound();
    }

    /// <summary>
    /// 次のラウンドに進む
    /// </summary>
    public void NextRound() {
        currentRoundNumber++;

        if (currentRoundNumber <= roundSettingsList.Count) {
            InitializeRound();
        }
        else {
            // 全ラウンド終了
            EndGame();
        }
    }

    /// <summary>
    /// ゲーム終了演出開始
    /// </summary>
    private void StartGameEndTransition(GameManager.Winner winner) {
        isGameEnd = true;
        gameEndTimer = gameEndDisplayTime;

        if (gameEndPanel != null) {
            gameEndPanel.SetActive(true);
        }

        if (gameEndText != null) {
            string winnerText = "";
            switch (winner) {
                case GameManager.Winner.Player1:
                    winnerText = "Player 1 の勝利！";
                    break;
                case GameManager.Winner.Player2:
                    winnerText = "Player 2 の勝利！";
                    break;
                case GameManager.Winner.None:
                    winnerText = "引き分け！";
                    break;
            }
            gameEndText.text = $"ゲーム終了\n{winnerText}";
        }

        if (finalScoreText != null) {
            finalScoreText.text = $"最終スコア\nPlayer 1: {player1Score}\nPlayer 2: {player2Score}";
        }

        // ゲームを一時停止
        if (GameManager.Instance != null) {
            GameManager.Instance.CurrentGameState = GameManager.GameState.Paused;
        }
    }

    /// <summary>
    /// ゲーム終了処理
    /// </summary>
    private void EndGame() {
        // 最終勝者を決定
        GameManager.Winner finalWinner;
        if (player1Score > player2Score) {
            finalWinner = GameManager.Winner.Player1;
        }
        else if (player2Score > player1Score) {
            finalWinner = GameManager.Winner.Player2;
        }
        else {
            // 同点の場合
            finalWinner = GameManager.Winner.None;
        }

        Debug.Log($"[ROUND MANAGER] : ゲーム終了！ 最終勝者: {finalWinner}");
        Debug.Log($"[ROUND MANAGER] : 最終スコア - Player1: {player1Score}, Player2: {player2Score}");

        // ゲーム終了演出を開始（finalWinnerを渡す）
        StartGameEndTransition(finalWinner);

        // GameManagerにゲーム終了を通知（finalWinnerを渡す）
        if (GameManager.Instance != null) {
            GameManager.Instance.SetGameOver(finalWinner);
        }
    }

    /// <summary>
    /// ゲーム終了演出終了
    /// </summary>
    private void EndGameTransition() {
        isGameEnd = false;

        if (gameEndPanel != null) {
            gameEndPanel.SetActive(false);
        }

        // ゲーム状態をゲームオーバーに設定
        if (GameManager.Instance != null) {
            GameManager.Instance.CurrentGameState = GameManager.GameState.GameOver;
        }
    }

    /// <summary>
    /// UI更新
    /// </summary>
    private void UpdateUI() {
        if (roundInfoText != null && currentRoundSettings != null) {
            roundInfoText.text = $"ラウンド {currentRoundNumber}/{roundSettingsList.Count}";
        }

        if (scoreText != null) {
            scoreText.text = $"スコア - P1: {player1Score} | P2: {player2Score}";
        }
    }

    /// <summary>
    /// 現在のラウンド設定を取得
    /// </summary>
    public RoundSettings GetCurrentRoundSettings() {
        return currentRoundSettings;
    }

    /// <summary>
    /// ゲーム全体をリセット
    /// </summary>
    public void ResetGame() {
        currentRoundNumber = 1;
        player1Score = 0;
        player2Score = 0;
        isGameEnd = false;
        isRoundTransition = false;
        isRoundWinDisplay = false;
        
        if (gameEndPanel != null) {
            gameEndPanel.SetActive(false);
        }
        
        if (roundWinPanel != null) {
            roundWinPanel.SetActive(false);
        }
        
        InitializeRound();
        
        Debug.Log("[ROUND MANAGER] : ゲーム全体がリセットされました");
    }

    /// <summary>
    /// 現在がラウンド進行中かどうかを判定
    /// </summary>
    public bool IsRoundInProgress() {
        return !isRoundTransition && !isGameEnd && !isRoundWinDisplay && GameManager.Instance.CurrentGameState == GameManager.GameState.Playing;
    }

    /// <summary>
    /// 現在のラウンド進捗情報を取得
    /// </summary>
    public string GetRoundProgressInfo() {
        return $"ラウンド {currentRoundNumber}/{roundSettingsList.Count} - スコア P1:{player1Score} P2:{player2Score} (目標:{targetScore})";
    }
}