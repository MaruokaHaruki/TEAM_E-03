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

    // プライベート変数
    private RoundSettings currentRoundSettings;
    private bool isRoundTransition = false;
    private float roundStartTimer = 0f;
    private const float ROUND_START_DISPLAY_TIME = 3f;

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

        UpdateUI();
    }

    /// <summary>
    /// ラウンドの初期化
    /// </summary>
    private void InitializeRound() {
        if (currentRoundNumber <= roundSettingsList.Count) {
            currentRoundSettings = roundSettingsList[currentRoundNumber - 1];
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

        // 勝利条件チェック
        if (player1Score >= targetScore || player2Score >= targetScore) {
            EndGame();
        }
        else {
            NextRound();
        }
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
    /// ゲーム終了処理
    /// </summary>
    private void EndGame() {
        GameManager.Winner finalWinner = player1Score > player2Score ?
            GameManager.Winner.Player1 : GameManager.Winner.Player2;

        Debug.Log($"[ROUND MANAGER] : ゲーム終了！ 最終勝者: {finalWinner}");
        Debug.Log($"[ROUND MANAGER] : 最終スコア - Player1: {player1Score}, Player2: {player2Score}");
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
    /// ゲームリセット
    /// </summary>
    public void ResetGame() {
        currentRoundNumber = 1;
        player1Score = 0;
        player2Score = 0;
        InitializeRound();
    }
}