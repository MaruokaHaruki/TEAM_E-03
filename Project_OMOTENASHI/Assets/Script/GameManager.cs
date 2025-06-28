using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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
        Playing,     // ゲームプレイ中
        Paused,      // 一時停止中
        GameOver     // ゲームオーバー
    }
    // Gameの状態を保持
    public GameState CurrentGameState = GameState.Playing;
    
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
    // UI要素
    [Header("UI要素")]
    [Tooltip("プレイヤー1のHPバー")]
    public Slider player1HpBar_;

    [Tooltip("プレイヤー2のHPバー")]
    public Slider player2HpBar_;
    
    [Tooltip("プレイヤー1のHP数値テキスト")]
    public Text player1HpText_;
    
    [Tooltip("プレイヤー2のHP数値テキスト")]
    public Text player2HpText_;
    
    [Tooltip("勝者表示テキスト")]
    public Text winnerText_;
    
    [Tooltip("ゲームオーバーパネル")]
    public GameObject gameOverPanel_;

    ///--------------------------------------------------------------
    ///						 private変数
    //========================================
    // プレイヤーHP管理
    private Dictionary<string, int> playerMaxHp_ = new Dictionary<string, int>();
    private Dictionary<string, int> playerCurrentHp_ = new Dictionary<string, int>();

    ///--------------------------------------------------------------
    ///						 初期化前初期化
    private void Awake() {
        // シングルトンの設定
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーンを跨いでオブジェクトを保持
        }
        else {
            Destroy(gameObject); // 既に存在する場合は新しいインスタンスを破棄
        }

        //シーンの起動はプレイ中
        CurrentGameState = GameState.Playing;
    }

    ///--------------------------------------------------------------
    ///						 初期化
    void Start()
    {
        // プレイヤーの初期化
        InitializePlayers();
        
        // UIの初期化
        InitializeUI();
    }

    ///--------------------------------------------------------------
    ///						 更新
    void Update()
    {
        // UIの更新
        UpdateUI();
        
        // ゲームオーバー状態の処理
        if (CurrentGameState == GameState.GameOver && gameOverPanel_ != null) {
            gameOverPanel_.SetActive(true);
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
            Debug.Log($"[GAME MANAGER] : {player1Name_} (ID: {player1Id}) を登録しました。HP: {playerCurrentHp_[player1Id]}/{playerMaxHp_[player1Id]}");
        }

        // プレイヤー2の初期化
        if (player2_ != null) {
            string player2Id = player2_.playerID_;
            playerMaxHp_[player2Id] = player2_.maxHp_;
            playerCurrentHp_[player2Id] = player2_.currentHp_;
            Debug.Log($"[GAME MANAGER] : {player2Name_} (ID: {player2Id}) を登録しました。HP: {playerCurrentHp_[player2Id]}/{playerMaxHp_[player2Id]}");
        }
    }

    ///--------------------------------------------------------------
    ///						 UI初期化
    private void InitializeUI() {
        // HPバーの初期化
        if (player1HpBar_ != null && player1_ != null) {
            player1HpBar_.maxValue = playerMaxHp_[player1_.playerID_];
            player1HpBar_.value = playerCurrentHp_[player1_.playerID_];
        }

        if (player2HpBar_ != null && player2_ != null) {
            player2HpBar_.maxValue = playerMaxHp_[player2_.playerID_];
            player2HpBar_.value = playerCurrentHp_[player2_.playerID_];
        }

        // ゲームオーバーパネルを非表示
        if (gameOverPanel_ != null) {
            gameOverPanel_.SetActive(false);
        }

        // 勝者テキストを非表示
        if (winnerText_ != null) {
            winnerText_.gameObject.SetActive(false);
        }
    }

    ///--------------------------------------------------------------
    ///						 UI更新
    private void UpdateUI() {
        // プレイヤー1のHP表示更新
        if (player1_ != null) {
            string player1Id = player1_.playerID_;
            if (playerCurrentHp_.ContainsKey(player1Id)) {
                if (player1HpBar_ != null) {
                    player1HpBar_.value = playerCurrentHp_[player1Id];
                }
                if (player1HpText_ != null) {
                    player1HpText_.text = $"{player1Name_}: {playerCurrentHp_[player1Id]}/{playerMaxHp_[player1Id]}";
                }
            }
        }

        // プレイヤー2のHP表示更新
        if (player2_ != null) {
            string player2Id = player2_.playerID_;
            if (playerCurrentHp_.ContainsKey(player2Id)) {
                if (player2HpBar_ != null) {
                    player2HpBar_.value = playerCurrentHp_[player2Id];
                }
                if (player2HpText_ != null) {
                    player2HpText_.text = $"{player2Name_}: {playerCurrentHp_[player2Id]}/{playerMaxHp_[player2Id]}";
                }
            }
        }

        // 勝者表示の更新
        if (CurrentGameState == GameState.GameOver && winnerText_ != null) {
            winnerText_.gameObject.SetActive(true);
            string winnerName = GetWinnerName();
            winnerText_.text = $"勝者: {winnerName}";
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
        CurrentGameState = GameState.Playing;
        CurrentWinner = Winner.None;

        // UIをリセット
        if (gameOverPanel_ != null) {
            gameOverPanel_.SetActive(false);
        }
        if (winnerText_ != null) {
            winnerText_.gameObject.SetActive(false);
        }

        Debug.Log("[GAME MANAGER] : ゲームがリスタートされました。");
    }
}
