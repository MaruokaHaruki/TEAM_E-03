using UnityEngine;

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
    public GameState CurrentGameState  = GameState.Playing;
    //========================================
    // どちらが勝ったかを保持
    public enum Winner {
        None,       // 勝者なし
        Player1,    // プレイヤー1
        Player2     // プレイヤー2
    }
    // 勝者を保持
    public Winner CurrentWinner = Winner.None;

    ///--------------------------------------------------------------
    ///						 private変数
    //========================================

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
    }

    ///--------------------------------------------------------------
    ///						 初期化
    void Start()
    {
        
    }

    ///--------------------------------------------------------------
    ///						 更新
    void Update()
    {
        
    }
}
