using UnityEditor;
using UnityEngine;
using TMPro;
using DG.Tweening; // DOTweenを使用するための名前空間

public class StartManager : MonoBehaviour
{
    [HideInInspector]
    [SerializeField] private string sceneToLoad;

    [Header("準備状態表示UI")]
    [SerializeField] private TextMeshProUGUI player1ReadyText;
    [SerializeField] private TextMeshProUGUI player2ReadyText;

    [Header("Canvas Control")]
    [SerializeField] private GameObject playerReadyCanvas;
    [SerializeField] private float canvasDisplayDelay = 1f;
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private float autoHideDelay = 5f; // 無操作時の自動非表示時間

    private bool player1Ready = false;
    private bool player2Ready = false;
    private bool titleLogoCompleted = false;
    private bool canvasDisplayed = false;
    private float lastInputTime = 0f;
    private Tween currentFadeTween;


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

    private void Start()
    {
        // タイトルロゴ完了イベントを購読
        TitleLogController.OnTitleLogoComplete += OnTitleLogoCompleted;
        
        // Canvas初期状態を設定
        if (playerReadyCanvas != null)
        {
            // CanvasGroupを取得または追加
            var canvasGroup = playerReadyCanvas.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = playerReadyCanvas.AddComponent<CanvasGroup>();
            }
            
            // 初期状態設定
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            
            // スケールも初期化
            playerReadyCanvas.transform.localScale = Vector3.zero;
            
            playerReadyCanvas.SetActive(true); // アクティブにしておいて透明状態
        }
    }

    private void OnDestroy()
    {
        // イベント購読解除
        TitleLogController.OnTitleLogoComplete -= OnTitleLogoCompleted;
    }

    private void OnTitleLogoCompleted()
    {
        titleLogoCompleted = true;
        Debug.Log("タイトルロゴ完了 - 何かキーを押してください");
    }

    private void Update()
    {
        // タイトルロゴが完了していない場合は何もしない
        if (!titleLogoCompleted) return;

        // キー入力チェック
        if (Input.anyKeyDown)
        {
            lastInputTime = Time.time;
            
            if (!canvasDisplayed)
            {
                ShowCanvas();
            }
        }

        // Canvas表示中の処理
        if (canvasDisplayed)
        {
            // 無操作時間チェックでフェードアウト
            if (Time.time - lastInputTime > autoHideDelay)
            {
                HideCanvas();
                return;
            }

            // プレイヤー入力処理
            HandlePlayerInput();
        }
    }

    private void HandlePlayerInput()
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

    private void ShowCanvas()
    {
        if (canvasDisplayed) return;
        
        canvasDisplayed = true;
        
        if (playerReadyCanvas != null)
        {
            var canvasGroup = playerReadyCanvas.GetComponent<CanvasGroup>();
            
            if (canvasGroup != null)
            {
                // 既存のアニメーションを停止
                currentFadeTween?.Kill();
                
                // インタラクションを有効化
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
                
                // フェードインアニメーション
                currentFadeTween = canvasGroup.DOFade(1f, fadeInDuration).SetEase(Ease.OutQuart);
            }
        }
    }

    private void HideCanvas()
    {
        if (!canvasDisplayed) return;
        
        if (playerReadyCanvas != null)
        {
            var canvasGroup = playerReadyCanvas.GetComponent<CanvasGroup>();
            
            if (canvasGroup != null)
            {
                // 既存のアニメーションを停止
                currentFadeTween?.Kill();
                
                // フェードアウトアニメーション
                currentFadeTween = canvasGroup.DOFade(0f, fadeOutDuration)
                    .SetEase(Ease.OutQuart)
                    .OnComplete(() => {
                        canvasGroup.interactable = false;
                        canvasGroup.blocksRaycasts = false;
                        canvasDisplayed = false;
                        
                        // 準備状態リセット
                        player1Ready = false;
                        player2Ready = false;
                        UpdatePlayerReadyDisplay();
                    });
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
