using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;

//=============================================================================
/// ラウンドマネージャー
public class RoundManager : MonoBehaviour
{
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

    [Tooltip("ラウンドイメージリスト")]
    public Sprite[] roundFeatureImages;

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
    [Tooltip("ゲーム終了演出の表示時間")]
    public float gameEndDisplayTime = 5.0f;

    ///--------------------------------------------------------------
    ///						 ラウンド演出
    //========================================
    [Header("ラウンド演出")]
    [Tooltip("ラウンド間の待機時間")]
    public float roundTransitionDelay = 3.0f;

    ///--------------------------------------------------------------
    ///						 private変数
    //========================================
    // ラウンド管理
    private RoundSettings currentRoundSettings;
    private bool isRoundTransition = false;   
    private bool isGameEnd = false;
    private float roundStartTimer = 0.0f;
    private float gameEndTimer = 0f;
    private float countdownTimer = 0f;
    private bool isCountingDown = false;

    // 定数
    private const float COUNTDOWN_DURATION = 3f;
    private const float ROUND_START_DISPLAY_TIME = 3f;
    private const float ROUND_END_DELAY = 2f;

    ///--------------------------------------------------------------
    ///						 初期化前初期化
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

        Debug.Log("[ROUND MANAGER] : RoundManager初期化完了");
    }

    ///--------------------------------------------------------------
    ///						 初期化
    void Start()
    {
        // Start時点では何もしない（GameManagerのStartから呼び出される）
    }

    ///--------------------------------------------------------------
    ///						 更新
    void Update()
    {
        if (GameManager.Instance == null) return;

        GameManager.GameState currentGameState = GameManager.Instance.GetGameState();

        switch (currentGameState)
        {
            case GameManager.GameState.RoundStart:
                // ラウンド開始タイマー処理
                if (roundStartTimer > 0f)
                {
                    roundStartTimer -= Time.deltaTime;
                    Debug.Log($"[ROUND MANAGER] : roundStartTimer: {roundStartTimer:F2}");
                    
                    if (roundStartTimer <= 0f)
                    {
                        Debug.Log("[ROUND MANAGER] : ラウンド開始タイマー終了、Playingに移行");
                        GameManager.Instance.SetGameState(GameManager.GameState.Playing);
                        EndRoundTransition();
                    }
                }
                break;
            case GameManager.GameState.RoundEnd:
                break;
            default:
                break;
        }

        // カウントダウン処理
        if (isCountingDown)
        {
            UpdateCountdown();
        }

        // ゲーム終了演出の処理
        if (isGameEnd)
        {
            gameEndTimer -= Time.unscaledDeltaTime;
            if (gameEndTimer <= 0f)
            {
                EndGameTransition();
            }
        }

        UpdateUI();
    }

    ///--------------------------------------------------------------
    ///						 最初のラウンド開始
    public void StartFirstRound()
    {
        Debug.Log("[ROUND MANAGER] : 最初のラウンドを開始します");
        currentRoundNumber = 1;
        InitializeRound();
    }

    ///--------------------------------------------------------------
    ///						 ラウンド初期化
    public void InitializeRound()
    {
        Debug.Log($"[ROUND MANAGER] : ラウンド{currentRoundNumber}初期化開始");

        if (currentRoundNumber <= roundSettingsList.Count)
        {
            currentRoundSettings = roundSettingsList[currentRoundNumber - 1];
            
            // GameStateをRoundStartに設定
            if (GameManager.Instance != null && GameManager.Instance.GetGameState() != GameManager.GameState.RoundStart)
            {
                GameManager.Instance.SetGameState(GameManager.GameState.RoundStart);
            }

            // ラウンド開始タイマーを設定
            roundStartTimer = ROUND_START_DISPLAY_TIME;
            
            // ラウンド設定を適用（タイルマップを先に変更）
            ApplyRoundSettings();
            
            // プレイヤーの状態をリセット
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RestartRound();
            }

            // ラウンド開始UI表示
            StartRoundTransition();

            Debug.Log($"[ROUND MANAGER] : ラウンド {currentRoundNumber} 初期化完了");
        }
        else
        {
            Debug.LogError("[ROUND MANAGER] : ラウンド設定が不足しています");
        }
    }

    ///--------------------------------------------------------------
    ///						 ラウンド設定適用
    private void ApplyRoundSettings()
    {
        if (currentRoundSettings == null) 
        {
            Debug.LogError("[ROUND MANAGER] : currentRoundSettingsがnullです");
            return;
        }

        Debug.Log($"[ROUND MANAGER] : ラウンド{currentRoundNumber}設定適用開始");

        // タイルマップを変更（プレハブ内のギミックも自動的に含まれる）
        if (TilemapManager.Instance != null)
        {
            Debug.Log($"[ROUND MANAGER] : タイルマップ変更開始 - {currentRoundSettings.roundName}（プレハブ内ギミック含む）");
            TilemapManager.Instance.ChangeTilemap(currentRoundSettings);
        }
        else
        {
            Debug.LogError("[ROUND MANAGER] : TilemapManagerが見つかりません");
        }

        // GameManagerのプレイヤー参照を取得
        if (GameManager.Instance != null)
        {
            Player player1 = GameManager.Instance.player1_;
            Player player2 = GameManager.Instance.player2_;

            // プレイヤー1に設定を適用
            if (player1 != null)
            {
                ApplySettingsToPlayer(player1);
                Debug.Log($"[ROUND MANAGER] : Player1位置設定 {currentRoundSettings.player1StartPosition}");
                player1.transform.position = currentRoundSettings.player1StartPosition;
            }

            // プレイヤー2に設定を適用
            if (player2 != null)
            {
                ApplySettingsToPlayer(player2);
                Debug.Log($"[ROUND MANAGER] : Player2位置設定 {currentRoundSettings.player2StartPosition}");
                player2.transform.position = currentRoundSettings.player2StartPosition;
            }

            // 無敵アイテム有無設定
            InvincibleItemGeneration invincibleObje = GameManager.Instance.invincibleObje;
            if (invincibleObje != null)
            {
                invincibleObje.SetActiveFlag(currentRoundSettings.InvincibleItemFlag);
            }
        }
        
        Debug.Log($"[ROUND MANAGER] : ラウンド{currentRoundNumber}設定適用完了");
    }

    ///--------------------------------------------------------------
    ///						 個別プレイヤー設定適用
    private void ApplySettingsToPlayer(Player player)
    {
        Debug.Log($"[ROUND MANAGER] : {player.gameObject.name} への設定適用開始");
        
        // まずプレイヤーの状態を完全にリセット
        player.ResetPlayerState();
        
        // 少し待ってから設定を適用（物理演算の安定化のため）
        StartCoroutine(DelayedApplySettings(player));
        
        Debug.Log($"[ROUND MANAGER] : {player.gameObject.name} への設定適用完了");
    }
    
    ///--------------------------------------------------------------
    ///						 遅延設定適用
    private System.Collections.IEnumerator DelayedApplySettings(Player player)
    {
        yield return new UnityEngine.WaitForFixedUpdate();
        
        // ラウンド設定を適用
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
        
        // 連打ゲージ減り速度加速設定を適用
        player.enableGaugeDrainBoost_ = currentRoundSettings.enableGaugeDrainBoost;
        player.gaugeDrainBoostMultiplier_ = currentRoundSettings.gaugeDrainBoostMultiplier;

        // 移動許可を明示的に無効化（カウントダウン終了まで待機）
        player.allowMovement_ = false;
        
        // プレイヤー状態の完全リセット
        player.ForceCompleteReset();
        
        Debug.Log($"[ROUND MANAGER] : {player.gameObject.name} の遅延設定適用完了");
    }
    
    ///--------------------------------------------------------------
    ///						 ラウンド開始演出
    private void StartRoundTransition()
    {
        Debug.Log("[ROUND MANAGER] : ラウンド開始演出開始");

        // ラウンドパネル表示時のSEを再生
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySE("Round_Start_1");
        }

        if (UIManager.Instance != null && currentRoundSettings != null)
        {
            string featuresText = GetNewFeaturesText();
            Sprite roundImage = null;

            if (roundFeatureImages != null && currentRoundNumber - 1 < roundFeatureImages.Length)
            {
                roundImage = roundFeatureImages[currentRoundNumber - 1];
            }

            UIManager.Instance.ShowRoundStartUI(currentRoundSettings.roundName, featuresText, roundImage);
        }
        else
        {
            Debug.LogWarning("[ROUND MANAGER] : UIManagerまたはcurrentRoundSettingsがnullです");
        }
    }

    ///--------------------------------------------------------------
    ///						 ラウンド開始演出終了
    private void EndRoundTransition()
    {
        Debug.Log("[ROUND MANAGER] : ラウンド開始演出終了");

        if (UIManager.Instance != null)
        {
            UIManager.Instance.HideRoundStartUI();
        }

        // カウントダウンを開始
        StartCountdown();
    }

    ///--------------------------------------------------------------
    ///						 カウントダウン開始
    private void StartCountdown()
    {
        Debug.Log("[ROUND MANAGER] : カウントダウン開始");
        isCountingDown = true;
        countdownTimer = COUNTDOWN_DURATION;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowCountdownUI();
        }
    }

    ///--------------------------------------------------------------
    ///						 カウントダウン処理
    private void UpdateCountdown()
    {
        if (!isCountingDown) return;

        countdownTimer -= Time.unscaledDeltaTime;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateCountdownUI(countdownTimer);
        }

        if (countdownTimer <= 0f)
        {
            EndCountdown();
        }
    }

    ///--------------------------------------------------------------
    ///						 カウントダウン終了
    private void EndCountdown()
    {
        isCountingDown = false;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.HideCountdownUI();
        }

        // ゲーム状態をPlayingに変更
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetGameState(GameManager.GameState.Playing);
        }

        // プレイヤーの移動許可を確実に有効化
        StartCoroutine(ForceActivatePlayers());

        Debug.Log($"[ROUND MANAGER] : ラウンド {currentRoundNumber} 開始！");
    }
    
    ///--------------------------------------------------------------
    ///						 プレイヤー強制有効化
    private System.Collections.IEnumerator ForceActivatePlayers()
    {
        // 少し待ってから有効化（UI処理の完了を待つ）
        yield return new UnityEngine.WaitForSeconds(0.1f);
        
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.player1_ != null)
            {
                GameManager.Instance.player1_.allowMovement_ = true;
                GameManager.Instance.player1_.ForceReactivate();
                Debug.Log($"[ROUND MANAGER] : Player1の移動許可を有効化しました");
            }
            
            if (GameManager.Instance.player2_ != null)
            {
                GameManager.Instance.player2_.allowMovement_ = true;
                GameManager.Instance.player2_.ForceReactivate();
                Debug.Log($"[ROUND MANAGER] : Player2の移動許可を有効化しました");
            }
        }
        
        // さらに確認のため、少し待ってからもう一度チェック
        yield return new UnityEngine.WaitForSeconds(0.2f);
        
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.player1_ != null)
            {
                GameManager.Instance.player1_.allowMovement_ = true;
                GameManager.Instance.player1_.ForceReactivate();
                // 追加：物理コンポーネントの強制リセット
                GameManager.Instance.player1_.ForcePhysicsReset();
                Debug.Log($"[ROUND MANAGER] : Player1の移動許可を再有効化しました");
            }
            
            if (GameManager.Instance.player2_ != null)
            {
                GameManager.Instance.player2_.allowMovement_ = true;
                GameManager.Instance.player2_.ForceReactivate();
                // 追加：物理コンポーネントの強制リセット
                GameManager.Instance.player2_.ForcePhysicsReset();
                Debug.Log($"[ROUND MANAGER] : Player2の移動許可を再有効化しました");
            }
        }
        
        // 最終確認（さらに0.5秒後）
        yield return new UnityEngine.WaitForSeconds(0.5f);
        
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.player1_ != null && !GameManager.Instance.player1_.allowMovement_)
            {
                GameManager.Instance.player1_.allowMovement_ = true;
                GameManager.Instance.player1_.ForceReactivate();
                GameManager.Instance.player1_.ForcePhysicsReset();
                Debug.LogWarning($"[ROUND MANAGER] : Player1の移動許可を最終確認で再有効化しました");
            }
            
            if (GameManager.Instance.player2_ != null && !GameManager.Instance.player2_.allowMovement_)
            {
                GameManager.Instance.player2_.allowMovement_ = true;
                GameManager.Instance.player2_.ForceReactivate();
                GameManager.Instance.player2_.ForcePhysicsReset();
                Debug.LogWarning($"[ROUND MANAGER] : Player2の移動許可を最終確認で再有効化しました");
            }
        }
    }
    
    ///--------------------------------------------------------------
    ///						 新機能テキスト生成
    private string GetNewFeaturesText()
    {
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
    public void OnPlayerWin(GameManager.Winner winner)
    {
        if (currentRoundSettings == null) return;

        // 既にラウンド終了処理中の場合は重複実行を防ぐ
        if (GameManager.Instance != null && GameManager.Instance.GetGameState() == GameManager.GameState.RoundEnd) {
            Debug.Log($"[ROUND MANAGER] : 既にラウンド終了処理中のため、重複実行をスキップ");
            return;
        }

        Debug.Log($"[ROUND MANAGER] : プレイヤー勝利処理開始 - Winner: {winner}");

        // ラウンド決着時のSEを再生
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySE("Round_KO_2");
        }

        // ゲーム状態をラウンド終了に変更（プレイヤーの移動を停止）
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetGameState(GameManager.GameState.RoundEnd);
        }

        // プレイヤーの移動を即座に停止
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.player1_ != null)
            {
                GameManager.Instance.player1_.allowMovement_ = false;
                Debug.Log($"[ROUND MANAGER] : Player1の移動を停止しました");
            }
            
            if (GameManager.Instance.player2_ != null)
            {
                GameManager.Instance.player2_.allowMovement_ = false;
                Debug.Log($"[ROUND MANAGER] : Player2の移動を停止しました");
            }
        }

        // スコアを加算
        switch (winner)
        {
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
        if (player1Score >= targetScore || player2Score >= targetScore)
        {
            // 目標スコア到達でゲーム終了
            EndGame();
        }
        else if (currentRoundNumber >= roundSettingsList.Count)
        {
            // 全ラウンド終了でゲーム終了
            EndGame();
        }
        else
        {
            // 次のラウンドに進む
            StartCoroutine(DelayedNextRound());
        }
    }

    ///--------------------------------------------------------------
    ///						 勝利演出開始コルーチン
    private System.Collections.IEnumerator StartVictoryZoomCoroutine(GameManager.Winner winner)
    {
        // 少し待ってから勝利演出を開始
        yield return new UnityEngine.WaitForSeconds(0.5f);

        if (GameManager.Instance != null && GameManager.Instance.cameraController != null)
        {
            Transform winnerTransform = null;
            switch (winner)
            {
                case GameManager.Winner.Player1:
                    winnerTransform = GameManager.Instance.player1_?.transform;
                    break;
                case GameManager.Winner.Player2:
                    winnerTransform = GameManager.Instance.player2_?.transform;
                    break;
            }

            if (winnerTransform != null)
            {
                Debug.Log($"[ROUND MANAGER] : 勝利演出を開始 - {winnerTransform.name} at {winnerTransform.position}");
                GameManager.Instance.cameraController.StartVictoryZoom(winnerTransform);
            }
            else
            {
                Debug.LogError($"[ROUND MANAGER] : 勝者のTransformが見つかりません - Winner: {winner}");
            }
        }
        else
        {
            Debug.LogError("[ROUND MANAGER] : GameManagerまたはCameraControllerが見つかりません");
        }
    }

    ///--------------------------------------------------------------
    ///						 遅延次ラウンド
    private System.Collections.IEnumerator DelayedNextRound()
    {
        // ラウンド終了の表示
        Debug.Log($"[ROUND MANAGER] : ラウンド {currentRoundNumber} 終了。{roundTransitionDelay}秒後に次のラウンドを開始します。");

        yield return new UnityEngine.WaitForSeconds(roundTransitionDelay);

        NextRound();
    }

    ///--------------------------------------------------------------
    ///						 次ラウンド進行
    public void NextRound()
    {
        currentRoundNumber++;
        Debug.Log($"[ROUND MANAGER] : 次のラウンド({currentRoundNumber})に進行");

        if (currentRoundNumber <= roundSettingsList.Count)
        {
            InitializeRound();
        }
        else
        {
            // 全ラウンド終了
            EndGame();
        }
    }

    ///--------------------------------------------------------------
    ///						 ゲーム終了処理
    private void EndGame()
    {
        // 最終勝者を決定
        GameManager.Winner finalWinner;
        if (player1Score > player2Score)
        {
            finalWinner = GameManager.Winner.Player1;
        }
        else if (player2Score > player1Score)
        {
            finalWinner = GameManager.Winner.Player2;
        }
        else
        {
            finalWinner = GameManager.Winner.None;
        }

        Debug.Log($"[ROUND MANAGER] : ゲーム終了！ 最終勝者: {finalWinner}");

        // ゲーム終了演出を開始
        StartGameEndTransition(finalWinner);

        // GameManagerにゲーム終了を通知
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetGameOver(finalWinner);
        }
    }

    ///--------------------------------------------------------------
    ///						 ゲーム終了演出開始
    private void StartGameEndTransition(GameManager.Winner winner)
    {
        isGameEnd = true;
        gameEndTimer = gameEndDisplayTime;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameEndUI(winner, player1Score, player2Score);
        }
    }

    ///--------------------------------------------------------------
    ///						 ゲーム終了演出終了
    private void EndGameTransition()
    {
        isGameEnd = false;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.HideGameEndUI();
        }

        // ゲーム状態をゲームオーバーに設定
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetGameState(GameManager.GameState.GameOver);
            Debug.Log("RoundManager:  GameState:GameOverに変更");

        }
    }

    ///--------------------------------------------------------------
    ///						 UI更新
    private void UpdateUI()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateRoundInfoUI(currentRoundNumber, roundSettingsList.Count, player1Score, player2Score);
        }
    }

    ///--------------------------------------------------------------
    ///						 現在ラウンド設定取得
    public RoundSettings GetCurrentRoundSettings()
    {
        return currentRoundSettings;
    }

    ///--------------------------------------------------------------
    ///						 ゲーム全体リセット
    public void ResetGame()
    {
        currentRoundNumber = 1;
        player1Score = 0;
        player2Score = 0;
        isGameEnd = false;
        isRoundTransition = false;

        // タイルマップをリセット（プレハブ内のギミックも含む）
        if (TilemapManager.Instance != null)
        {
            TilemapManager.Instance.ResetTilemap();
        }

        InitializeRound();

        Debug.Log("[ROUND MANAGER] : ゲーム全体（タイルマップ・プレハブ内ギミック含む）がリセットされました");
    }
}
