using UnityEditor;
using UnityEngine;
using TMPro;

public class StartManager : MonoBehaviour
{
    [HideInInspector]
    [SerializeField] private string sceneToLoad;

    [Header("準備状態表示UI")]
    [SerializeField] private TextMeshProUGUI player1ReadyText;
    [SerializeField] private TextMeshProUGUI player2ReadyText;

    private bool player1Ready = false;
    private bool player2Ready = false;


#if UNITY_EDITOR
    // インスペクターに表示するためのSceneAsset型変数
    [Header("遷移先シーン選択")] // インスペクターに見出しを表示
    [SerializeField] private SceneAsset sceneAsset; // ここにシーンファイルをドラッグ&ドロップする
#endif



    private void Awake()
    {
        Application.targetFrameRate = 60;
        UpdatePlayerReadyDisplay();
    }

    private void Update()
    {
        // プレイヤー1の準備状態をチェック（A,W,D同時押し）
        bool player1Input = Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D);
        
        // プレイヤー2の準備状態をチェック（J,I,L同時押し）
        bool player2Input = Input.GetKey(KeyCode.J) && Input.GetKey(KeyCode.I) && Input.GetKey(KeyCode.L);

        // 準備状態を更新
        player1Ready = player1Input;
        player2Ready = player2Input;

        // UI表示を更新
        UpdatePlayerReadyDisplay();

        // 両プレイヤーが準備完了している場合のみシーンを切り替える
        if (player1Ready && player2Ready)
        {
            // 遷移シーンが設定されていたらそのシーンに遷移する
            if (!string.IsNullOrEmpty(sceneToLoad))
            {
                Debug.Log("シーンを切り替え:" + sceneToLoad);
                SceneManagerScript.Instance.FadeOutScene(sceneToLoad);
            }
            else
            {
                Debug.LogError("遷移先のシーンが設定されていない");
            }
        }
    }

    private void UpdatePlayerReadyDisplay()
    {
        if (player1ReadyText != null)
        {
            player1ReadyText.text = player1Ready ? 
                "Player 1 Ready!" : 
                "Player 1: Press A+W+D to Ready";
        }

        if (player2ReadyText != null)
        {
            player2ReadyText.text = player2Ready ? 
                "Player 2 Ready!" : 
                "Player 2: Press J+I+L to Ready";
        }
    }

    // OnValidateメソッドはエディタ専用
#if UNITY_EDITOR
    // インスペクターで値が変更された時などに自動で呼ばれるメソッド
    private void OnValidate()
    {
        // sceneAssetフィールドにシーンが設定された場合
        if (sceneAsset != null)
        {
            // そのシーンの名前（文字列）を sceneToLoad 変数にコピーする
            sceneToLoad = sceneAsset.name;
        }
        else
        {
            // SceneAssetが未設定なら空文字にする
            sceneToLoad = "";
        }
    }
#endif

}
