using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;

//=============================================================================
/// ゲームマネージャー
public class GameManager : MonoBehaviour
{
    ///--------------------------------------------------------------
    ///						 public変数
    //========================================
    // シングルトン
    public static GameManager Instance { get; private set; }
    
    //========================================
    // ゲームの状態
    public enum GameState
    {
        RoundStart,  // ラウンド開始処理
        Playing,     // ゲームプレイ中
        Paused,      // 一時停止中
        RoundEnd,    // ラウンド終了処理
        GameOver     // ゲームオーバー
    }
    // Gameの状態を保持
    public GameState CurrentGameState = GameState.RoundStart;
    
    //========================================
    // どちらが勝ったかを保持
    public enum Winner {
        None,       // 勝者なし
        Player1,    // プレイヤー1
        Player2     // プレイヤー2
    }
    // 勝者を保持
    public Winner CurrentWinner = Winner.None;

    //========================================
    // プレイヤー管理
    [Header("プレイヤー管理")]
    [Tooltip("プレイヤー1のオブジェクト")]
    public Player player1_;
    
    [Tooltip("プレイヤー2のオブジェクト")]
    public Player player2_;
    
    [Tooltip("プレイヤー1の名前")]
    public string player1Name_ = "Player 1";
    
    [Tooltip("プレイヤー2の名前")]
    public string player2Name_ = "Player 2";

    //========================================
    // アイテム生成物
    [Tooltip("無敵アイテム生成物")]
    public InvincibleItemGeneration invincibleObje;

    ///--------------------------------------------------------------
    ///						 private変数
    //========================================
    // プレイヤーHP管理
    private Dictionary<string, int> playerMaxHp_ = new Dictionary<string, int>();
    private Dictionary<string, int> playerCurrentHp_ = new Dictionary<string, int>();

    // プレイヤー初期位置
    private Vector3 initialPlayer1Position;
    private Vector3 initialPlayer2Position;

    ///--------------------------------------------------------------
    ///						 初期化前初期化
    private void Awake()
    {
        // シングルトンの設定
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーンを跨いでオブジェクトを保持
        }
        else
        {
            Destroy(gameObject); // 既に存在する場合は新しいインスタンスを破棄
        }

        //シーンの起動はラウンド開始処理から
        CurrentGameState = GameState.RoundStart;
    }

    ///--------------------------------------------------------------
    ///						 初期化
    void Start()
    {
        //シーンの起動はラウンド開始処理から
        CurrentGameState = GameState.RoundStart;

        // プレイヤーの初期化
        InitializePlayers();
        
        // UIの初期化
        InitializeUI();
    }

    ///--------------------------------------------------------------
    ///						 更新
    void Update()
    {
        switch (CurrentGameState)
        {
            case GameState.RoundStart:
                Debug.Log("RoundStart");
                // ラウンド開始時の処理
                RoundManager.Instance.InitializeRound();
                break;
            case GameState.RoundEnd:
                break;
            case GameState.Playing:
                break;
            case GameState.GameOver: // ゲームオーバー状態の処理
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.ShowGameOverUI();
                }
                SceneManagerScript.Instance.FadeOutScene("Result");
                break;
            default:
                break;
        }
    }

    ///--------------------------------------------------------------
    ///						 プレイヤー初期化
    private void InitializePlayers() {
        // プレイヤー1の初期化
        if (player1_ != null) {
            string player1Id = player1_.playerID_;
            playerMaxHp_[player1Id] = player1_.maxHp_;
            playerCurrentHp_[player1Id] = player1_.currentHp_;
            initialPlayer1Position = player1_.gameObject.transform.position;    //←プレイヤーの初期座標設定
            Debug.Log($"[GAME MANAGER] : {player1Name_} (ID: {player1Id}) を登録しました。HP: {playerCurrentHp_[player1Id]}/{playerMaxHp_[player1Id]}");
        }

        // プレイヤー2の初期化
        if (player2_ != null) {
            string player2Id = player2_.playerID_;
            playerMaxHp_[player2Id] = player2_.maxHp_;
            playerCurrentHp_[player2Id] = player2_.currentHp_;
            initialPlayer2Position = player2_.gameObject.transform.position;    //←プレイヤーの初期座標設定
            Debug.Log($"[GAME MANAGER] : {player2Name_} (ID: {player2Id}) を登録しました。HP: {playerCurrentHp_[player2Id]}/{playerMaxHp_[player2Id]}");
        }
    }

    ///--------------------------------------------------------------
    ///						 UI初期化
    private void InitializeUI() {
        if (UIManager.Instance != null && player1_ != null && player2_ != null)
        {
            UIManager.Instance.InitializePlayerHPUI(
                player1_.playerID_, 
                player2_.playerID_, 
                playerMaxHp_[player1_.playerID_], 
                playerMaxHp_[player2_.playerID_]
            );
        }
    }

    ///--------------------------------------------------------------
    ///						 ダメージ処理
    public void TakeDamage(string playerId, int amount) {
        if (!playerCurrentHp_.ContainsKey(playerId)) {
            Debug.LogError($"[GAME MANAGER ERROR] : プレイヤーID '{playerId}' が見つかりません。");
            return;
        }

        // HPを減少
        playerCurrentHp_[playerId] -= amount;
        playerCurrentHp_[playerId] = Mathf.Max(0, playerCurrentHp_[playerId]);

        string playerName = GetPlayerName(playerId);
        Debug.Log($"[DAMAGE] : {playerName} が {amount} ダメージを受けた（残りHP: {playerCurrentHp_[playerId]}）");

        // HP0になった場合の処理
        if (playerCurrentHp_[playerId] <= 0) {
            OnPlayerDefeated(playerId);
        }
    }

    ///--------------------------------------------------------------
    ///						 プレイヤー敗北処理
    private void OnPlayerDefeated(string defeatedPlayerId) {
        // ラウンドシステムが有効な場合はゲームオーバーにしない
        if (RoundManager.Instance != null) {
            // 勝者を決定
            if (player1_ != null && player1_.playerID_ == defeatedPlayerId) {
                CurrentWinner = Winner.Player2;
            }
            else if (player2_ != null && player2_.playerID_ == defeatedPlayerId) {
                CurrentWinner = Winner.Player1;
            }

            string defeatedPlayerName = GetPlayerName(defeatedPlayerId);
            string winnerName = GetWinnerName();
            
            Debug.Log($"[ROUND END] : {defeatedPlayerName} が敗北しました。ラウンド勝者は {winnerName} です！");

            // ラウンドマネージャーに勝利を通知（ゲームオーバー状態にはしない）
            RoundManager.Instance.OnPlayerWin(CurrentWinner);
            
            // 勝者をリセット（次のラウンドのため）
            CurrentWinner = Winner.None;
        }
        else {
            // ラウンドシステムが無効な場合は従来通りゲームオーバー
            CurrentGameState = GameState.GameOver;

            // 勝者を決定
            if (player1_ != null && player1_.playerID_ == defeatedPlayerId) {
                CurrentWinner = Winner.Player2;
            }
            else if (player2_ != null && player2_.playerID_ == defeatedPlayerId) {
                CurrentWinner = Winner.Player1;
            }

            string defeatedPlayerName = GetPlayerName(defeatedPlayerId);
            string winnerName = GetWinnerName();
            
            Debug.Log($"[GAME OVER] : {defeatedPlayerName} が敗北しました。勝者は {winnerName} です！");
        }
    }

    ///--------------------------------------------------------------
    ///						 プレイヤー名取得
    private string GetPlayerName(string playerId) {
        if (player1_ != null && player1_.playerID_ == playerId) {
            return player1Name_;
        }
        else if (player2_ != null && player2_.playerID_ == playerId) {
            return player2Name_;
        }
        return $"Player {playerId}";
    }

    ///--------------------------------------------------------------
    ///						 勝者名取得
    private string GetWinnerName() {
        switch (CurrentWinner) {
            case Winner.Player1:
                return player1Name_;
            case Winner.Player2:
                return player2Name_;
            default:
                return "引き分け";
        }
    }

    ///--------------------------------------------------------------
    ///						 HP取得
    public int GetPlayerCurrentHp(string playerId) {
        if (playerCurrentHp_.ContainsKey(playerId)) {
            return playerCurrentHp_[playerId];
        }
        return 0;
    }

    ///--------------------------------------------------------------
    ///						 最大HP取得
    public int GetPlayerMaxHp(string playerId) {
        if (playerMaxHp_.ContainsKey(playerId)) {
            return playerMaxHp_[playerId];
        }
        return 0;
    }

    ///--------------------------------------------------------------
    ///						 ゲームリスタート
    public void RestartGame() {
        // HPを初期値に戻す
        if (player1_ != null) {
            playerCurrentHp_[player1_.playerID_] = playerMaxHp_[player1_.playerID_];
            player1_.currentHp_ = playerMaxHp_[player1_.playerID_];
        }
        if (player2_ != null) {
            playerCurrentHp_[player2_.playerID_] = playerMaxHp_[player2_.playerID_];
            player2_.currentHp_ = playerMaxHp_[player2_.playerID_];
        }

        // ゲーム状態をリセット
        //CurrentGameState = GameState.Playing;
        CurrentWinner = Winner.None;

        Debug.Log("[GAME MANAGER] : ゲームがリスタートされました。");
    }

    ///--------------------------------------------------------------
    ///						 ゲーム終了処理（ラウンドマネージャー用）
    public void SetGameOver(Winner winner) {
        CurrentGameState = GameState.GameOver;
        CurrentWinner = winner;
        
        string winnerName = GetWinnerName();
        Debug.Log($"[FINAL GAME OVER] : 全ラウンド終了！最終勝者は {winnerName} です！");
    }

    ///--------------------------------------------------------------
    ///						 ラウンドリスタート
    public void RestartRound() {
        // HPを最大値に戻す
        if (player1_ != null) {
            // ラウンド設定からHPを取得
            if (RoundManager.Instance != null && RoundManager.Instance.GetCurrentRoundSettings() != null) {
                int roundMaxHp = RoundManager.Instance.GetCurrentRoundSettings().playerMaxHp;
                playerMaxHp_[player1_.playerID_] = roundMaxHp;
                playerCurrentHp_[player1_.playerID_] = roundMaxHp;
                player1_.maxHp_ = roundMaxHp;
                player1_.currentHp_ = roundMaxHp;
                // プレイヤー1の位置を設定
                player1_.transform.position = RoundManager.Instance.GetCurrentRoundSettings().player1StartPosition;
            }
            else {
                playerCurrentHp_[player1_.playerID_] = playerMaxHp_[player1_.playerID_];
                player1_.currentHp_ = playerMaxHp_[player1_.playerID_];
            }
        }

        if (player2_ != null) {
            // ラウンド設定からHPを取得
            if (RoundManager.Instance != null && RoundManager.Instance.GetCurrentRoundSettings() != null) {
                int roundMaxHp = RoundManager.Instance.GetCurrentRoundSettings().playerMaxHp;
                playerMaxHp_[player2_.playerID_] = roundMaxHp;
                playerCurrentHp_[player2_.playerID_] = roundMaxHp;
                player2_.maxHp_ = roundMaxHp;
                player2_.currentHp_ = roundMaxHp;
                // プレイヤー2の位置を設定
                player2_.transform.position = RoundManager.Instance.GetCurrentRoundSettings().player2StartPosition;
            }
            else {
                playerCurrentHp_[player2_.playerID_] = playerMaxHp_[player2_.playerID_];
                player2_.currentHp_ = playerMaxHp_[player2_.playerID_];
            }
        }

        // ゲーム状態をプレイ中に戻す
        //CurrentGameState = GameState.Playing;
        CurrentWinner = Winner.None;

        // プレイヤーの特殊状態をリセット
        ResetPlayerStates();

        Debug.Log("[GAME MANAGER] : ラウンドがリスタートされました。");
    }

    ///--------------------------------------------------------------
    ///						 プレイヤー状態リセット
    private void ResetPlayerStates() {
        if (player1_ != null) {
            // スタン状態をリセット
            player1_.isStunned_ = false;
            player1_.stunTimer_ = 0.0f;
            
            // 無敵状態をリセット
            if (player1_.GetComponent<SpriteRenderer>() != null) {
                player1_.GetComponent<SpriteRenderer>().color = Color.white;
            }
            
            // 2段ジャンプフラグをリセット
            player1_.hasDoubleJumped_ = false;
            
            // 反転ジャンプフラグをリセット
            player1_.shouldReverseOnLanding_ = false;

            // プレイヤーを初期位置に戻す（必要に応じて）    ←InitializePlayersにて設定
            player1_.transform.position = initialPlayer1Position;
        }

        if (player2_ != null) {
            // プレイヤー2も同様にリセット
            player2_.isStunned_ = false;
            player2_.stunTimer_ = 0.0f;
            
            if (player2_.GetComponent<SpriteRenderer>() != null) {
                player2_.GetComponent<SpriteRenderer>().color = Color.white;
            }
            
            player2_.hasDoubleJumped_ = false;
            player2_.shouldReverseOnLanding_ = false;
            
             player2_.transform.position = initialPlayer2Position;
        }
    }

    ///--------------------------------------------------------------
    ///						 CurrentGameState変更
    public void SetGameState(GameState nextGameState)
    {
        //現在のGameState
        switch(CurrentGameState)
        {
            case GameState.RoundStart:
                 break;
            case GameState.Playing:
                break;

            case GameState.RoundEnd:
                break;

            case GameState.GameOver:
                break;

            case GameState.Paused:
                break;

        }

        CurrentGameState = nextGameState;
        
        //次のGameState呼び出し
        switch(CurrentGameState)
        {
            case GameState.RoundStart:
                break;
            case GameState.Playing:
                break;

            case GameState.RoundEnd:
                break;

            case GameState.GameOver:
                break;

            case GameState.Paused:
                break;
        }
    }
    
    public GameState GetGameState()
    {
        return CurrentGameState;
    }

}