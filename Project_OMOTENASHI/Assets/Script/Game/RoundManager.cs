using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

//=============================================================================
/// ラウンドマネージャー
public class RoundManager : MonoBehaviour {
    ///--------------------------------------------------------------
    ///						 public変数
    //========================================
    // シングルトン
    public static RoundManager Instance { get; private set; }

    ///--------------------------------------------------------------
    ///						 ラウンド設定
    //========================================
    [Header("ラウンド設定")]
    [Tooltip("全ラウンドの設定データ")]
    public List<RoundSettings> roundSettingsList = new List<RoundSettings>();

    [Tooltip("現在のラウンド番号（1から開始）")]
    public int currentRoundNumber = 1;

    ///--------------------------------------------------------------
    ///						 UI要素
    //========================================
    [Header("UI要素")]
    [Tooltip("ラウンド情報表示テキスト")]
    public Text roundInfoText;

    [Tooltip("スコア表示テキスト")]
    public Text scoreText;

    [Tooltip("ラウンド開始パネル")]
    public GameObject roundStartPanel;

    [Tooltip("ラウンド開始テキスト")]
    public Text roundStartText;

    ///--------------------------------------------------------------
    ///						 スコア管理
    //========================================
    [Header("スコア管理")]
    [Tooltip("プレイヤー1のスコア")]
    public int player1Score = 0;

    [Tooltip("プレイヤー2のスコア")]
    public int player2Score = 0;

    [Tooltip("勝利に必要なスコア")]
    public int targetScore = 5;

    ///--------------------------------------------------------------
    ///						 ゲーム終了演出
    //========================================
    [Header("ゲーム終了演出")]
    [Tooltip("ゲーム終了パネル")]
    public GameObject gameEndPanel;

    [Tooltip("ゲーム終了テキスト")]
    public Text gameEndText;

    [Tooltip("最終スコア表示テキスト")]
    public Text finalScoreText;

    [Tooltip("ゲーム終了演出の表示時間")]
    public float gameEndDisplayTime = 5.0f;

    ///--------------------------------------------------------------
    ///						 ラウンド演出
    //========================================
    [Header("ラウンド演出")]
    [Tooltip("ラウンド間の待機時間")]
    public float roundTransitionDelay = 2.0f;

    [Tooltip("ラウンド開始カウントダウン表示テキスト")]
    public Text countdownText;

    ///--------------------------------------------------------------
    ///						 private変数
    //========================================
    // ラウンド管理
    private RoundSettings currentRoundSettings;
    private bool isRoundTransition = false;
    private bool isGameEnd = false;
    private float roundStartTimer = 0f;
    private float gameEndTimer = 0f;
    private float countdownTimer = 0f;
    private bool isCountingDown = false;
    
    // 定数
    private const float COUNTDOWN_DURATION = 3f;
    private const float ROUND_START_DISPLAY_TIME = 2f;
    private const float ROUND_END_DELAY = 2f;

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
        InitializeRound();
    }

    ///--------------------------------------------------------------
    ///						 更新
    void Update() {
        // ラウンド開始演出の処理
        if (isRoundTransition) {
            roundStartTimer -= Time.unscaledDeltaTime;
            if (roundStartTimer <= 0f) {
                EndRoundTransition();
            }
        }

        // カウントダウン処理
        if (isCountingDown) {
            UpdateCountdown();
        }

        // ゲーム終了演出の処理
        if (isGameEnd) {
            gameEndTimer -= Time.unscaledDeltaTime;
            if (gameEndTimer <= 0f) {
                EndGameTransition();
            }
        }

        UpdateUI();
    }

    ///--------------------------------------------------------------
    ///						 ラウンド初期化
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

    ///--------------------------------------------------------------
    ///						 ラウンド設定適用
    private void ApplyRoundSettings() {
        if (currentRoundSettings == null) return;

        // GameManagerのプレイヤー参照を取得
        if (GameManager.Instance != null) {
            Player player1 = GameManager.Instance.player1_;
            Player player2 = GameManager.Instance.player2_;

            // プレイヤー1に設定を適用
            if (player1 != null) {
                ApplySettingsToPlayer(player1);
                // プレイヤー1の位置を設定
                player1.transform.position = currentRoundSettings.player1StartPosition;
            }

            // プレイヤー2に設定を適用
            if (player2 != null) {
                ApplySettingsToPlayer(player2);
                // プレイヤー2の位置を設定
                player2.transform.position = currentRoundSettings.player2StartPosition;
            }
        }
    }

    ///--------------------------------------------------------------
    ///						 個別プレイヤー設定適用
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

    ///--------------------------------------------------------------
    ///						 ラウンド開始演出
    private void StartRoundTransition() {
        isRoundTransition = true;
        roundStartTimer = ROUND_START_DISPLAY_TIME;

        if (roundStartPanel != null) {
            roundStartPanel.SetActive(true);
        }

        if (roundStartText != null) {
            roundStartText.text = $"{currentRoundSettings.roundName}\n" +
                                  $"新機能: {GetNewFeaturesText()}\n\n" +
                                  $"プレイヤーの準備をしてください";
        }

        // カウントダウンテキストを非表示
        if (countdownText != null) {
            countdownText.gameObject.SetActive(false);
        }

        // ゲームを一時停止
        if (GameManager.Instance != null) {
            GameManager.Instance.CurrentGameState = GameManager.GameState.Paused;
        }
    }

    ///--------------------------------------------------------------
    ///						 ラウンド開始演出終了
    private void EndRoundTransition() {
        isRoundTransition = false;

        if (roundStartPanel != null) {
            roundStartPanel.SetActive(false);
        }

        // カウントダウンを開始
        StartCountdown();
    }

    ///--------------------------------------------------------------
    ///						 カウントダウン開始
    private void StartCountdown() {
        isCountingDown = true;
        countdownTimer = COUNTDOWN_DURATION;

        if (countdownText != null) {
            countdownText.gameObject.SetActive(true);
        }

        // ゲームは一時停止のまま
        if (GameManager.Instance != null) {
            GameManager.Instance.CurrentGameState = GameManager.GameState.Paused;
        }
    }

    ///--------------------------------------------------------------
    ///						 カウントダウン処理
    private void UpdateCountdown() {
        if (!isCountingDown) return;

        countdownTimer -= Time.unscaledDeltaTime; // unscaledDeltaTimeを使用して一時停止の影響を受けない

        if (countdownText != null) {
            if (countdownTimer > 1f) {
                countdownText.text = Mathf.Ceil(countdownTimer).ToString();
            }
            else if (countdownTimer > 0f) {
                countdownText.text = "START!";
            }
            else {
                // カウントダウン終了
                EndCountdown();
            }
        }
    }

    ///--------------------------------------------------------------
    ///						 カウントダウン終了
    private void EndCountdown() {
        isCountingDown = false;

        if (countdownText != null) {
            countdownText.gameObject.SetActive(false);
        }

        // ゲームを再開
        if (GameManager.Instance != null) {
            GameManager.Instance.CurrentGameState = GameManager.GameState.Playing;
        }

        Debug.Log($"[ROUND MANAGER] : ラウンド {currentRoundNumber} 開始！");
    }

    ///--------------------------------------------------------------
    ///						 新機能テキスト生成
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

    ///--------------------------------------------------------------
    ///						 プレイヤー勝利処理
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

        // 勝利条件チェック
        if (player1Score >= targetScore || player2Score >= targetScore) {
            // 目標スコア到達でゲーム終了
            EndGame();
        }
        else if (currentRoundNumber >= roundSettingsList.Count) {
            // 全ラウンド終了でゲーム終了
            EndGame();
        }
        else {
            // 次のラウンドに進む
            StartCoroutine(DelayedNextRound());
        }
    }

    ///--------------------------------------------------------------
    ///						 遅延次ラウンド
    private System.Collections.IEnumerator DelayedNextRound() {
        // ラウンド終了の表示
        Debug.Log($"[ROUND MANAGER] : ラウンド {currentRoundNumber} 終了。{roundTransitionDelay}秒後に次のラウンドを開始します。");
        
        yield return new UnityEngine.WaitForSeconds(roundTransitionDelay);
        
        NextRound();
    }

    ///--------------------------------------------------------------
    ///						 次ラウンド進行
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

    ///--------------------------------------------------------------
    ///						 ゲーム終了処理
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

        // ゲーム終了演出を開始
        StartGameEndTransition(finalWinner);

        // GameManagerにゲーム終了を通知
        if (GameManager.Instance != null) {
            GameManager.Instance.SetGameOver(finalWinner);
        }
    }

    ///--------------------------------------------------------------
    ///						 ゲーム終了演出開始
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

    ///--------------------------------------------------------------
    ///						 ゲーム終了演出終了
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

    ///--------------------------------------------------------------
    ///						 UI更新
    private void UpdateUI() {
        if (roundInfoText != null && currentRoundSettings != null) {
            roundInfoText.text = $"ラウンド {currentRoundNumber}/{roundSettingsList.Count}";
        }

        if (scoreText != null) {
            scoreText.text = $"スコア - P1: {player1Score} | P2: {player2Score}";
        }
    }

    ///--------------------------------------------------------------
    ///						 現在ラウンド設定取得
    public RoundSettings GetCurrentRoundSettings() {
        return currentRoundSettings;
    }

    ///--------------------------------------------------------------
    ///						 ゲーム全体リセット
    public void ResetGame() {
        currentRoundNumber = 1;
        player1Score = 0;
        player2Score = 0;
        isGameEnd = false;
        isRoundTransition = false;
        
        if (gameEndPanel != null) {
            gameEndPanel.SetActive(false);
        }
        
        InitializeRound();
        
        Debug.Log("[ROUND MANAGER] : ゲーム全体がリセットされました");
    }

    ///--------------------------------------------------------------
    ///						 ラウンド進行中判定
    public bool IsRoundInProgress() {
        return !isRoundTransition && !isGameEnd && GameManager.Instance.CurrentGameState == GameManager.GameState.Playing;
    }

    ///--------------------------------------------------------------
    ///						 ラウンド進捗情報取得
    public string GetRoundProgressInfo() {
        return $"ラウンド {currentRoundNumber}/{roundSettingsList.Count} - スコア P1:{player1Score} P2:{player2Score} (目標:{targetScore})";
    }
}