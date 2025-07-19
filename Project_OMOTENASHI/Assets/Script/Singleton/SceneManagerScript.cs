using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManagerScript : SingletonMonoBehaviour<SceneManagerScript>
{
    [Header("フェード設定")]
    public GameObject fadePanelPrefab;     // フェード用のパネル（Image付きCanvas）
    public float fadeDuration = 1.0f;      // フェード時間

    [HideInInspector] public string winnerName;
    [HideInInspector] public int Player1_Score;
    [HideInInspector] public int Player2_Score;

    // 内部変数
    private GameObject fadeCanvas;         // 実際に使われるフェード用Canvasのインスタンス
    private string nextScene;
    private bool isFading = false;
   

    #region シーン遷移メソッド

    /// <summary>
    /// フェードアウトしてシーン遷移を開始
    /// </summary>
    /// <param name="sceneName">遷移先のシーン名</param>
    public void FadeOutScene(string sceneName)
    {
        if (isFading) 
        {
            Debug.LogWarning("既にフェード中のため、シーン遷移をキャンセルしました");
            return;
        }

        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("シーン名が無効です");
            return;
        }

        isFading = true;
        nextScene = sceneName;

        // フェードUI生成
        CreateFadeCanvas();

        StartCoroutine(FadeOutAndLoadScene());
    }

    /// <summary>
    /// 即座にシーン遷移（フェードなし）
    /// </summary>
    /// <param name="sceneName">遷移先のシーン名</param>
    public void LoadSceneByName(string sceneName) 
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("シーン名が無効です");
            return;
        }
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// 即座にシーン遷移（インデックス指定）
    /// </summary>
    /// <param name="index">シーンのビルドインデックス</param>
    public void LoadSceneByIndex(int index) => SceneManager.LoadScene(index);

    #endregion

    #region フェード処理

    /// <summary>
    /// フェード用Canvasを生成
    /// </summary>
    private void CreateFadeCanvas()
    {
        if (fadePanelPrefab == null)
        {
            Debug.LogError("fadePanelPrefabが設定されていません");
            return;
        }

        fadeCanvas = Instantiate(fadePanelPrefab);
        DontDestroyOnLoad(fadeCanvas);
    }

    /// <summary>
    /// フェードアウト→シーン読み込みの処理
    /// </summary>
    private IEnumerator FadeOutAndLoadScene()
    {
        if (fadeCanvas == null) 
        {
            Debug.LogError("フェードCanvasが生成されていません");
            yield break;
        }

        Image fadeImage = fadeCanvas.GetComponentInChildren<Image>(true);
        if (fadeImage == null)
        {
            Debug.LogError("フェードCanvasにImageコンポーネントが見つかりません");
            yield break;
        }

        fadeImage.enabled = true;

        // フェードアウト
        yield return StartCoroutine(FadeToColor(fadeImage, 1f));

        // シーン非同期読み込み
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(nextScene);
        if (loadOperation == null)
        {
            Debug.LogError($"シーン '{nextScene}' の読み込みに失敗しました");
            yield break;
        }

        while (!loadOperation.isDone)
            yield return null;
    }

    /// <summary>
    /// フェードイン処理
    /// </summary>
    private IEnumerator FadeIn()
    {
        if (fadeCanvas == null) yield break;

        Image fadeImage = fadeCanvas.GetComponentInChildren<Image>(true);
        if (fadeImage == null) yield break;

        fadeImage.enabled = true;

        // フェードイン
        yield return StartCoroutine(FadeToColor(fadeImage, 0f));

        fadeImage.enabled = false;
        Destroy(fadeCanvas);
        fadeCanvas = null;
        isFading = false;
    }

    /// <summary>
    /// 指定したアルファ値までフェード
    /// </summary>
    /// <param name="fadeImage">フェード対象のImage</param>
    /// <param name="targetAlpha">目標アルファ値</param>
    private IEnumerator FadeToColor(Image fadeImage, float targetAlpha)
    {
        float elapsed = 0f;
        Color startColor = fadeImage.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            fadeImage.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        fadeImage.color = targetColor;
    }

    #endregion

    #region イベント処理

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// シーン読み込み完了時の処理
    /// BGMの自動再生とフェードインを実行
    /// </summary>
    /// <param name="scene">読み込まれたシーン</param>
    /// <param name="mode">読み込みモード</param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"シーン読み込み完了: {scene.name}");

        // BGM自動再生
        PlayBGMForLoadedScene(scene.name);

        // フェードイン開始
        StartFadeInIfNeeded();
    }

    /// <summary>
    /// 読み込まれたシーンのBGMを再生
    /// </summary>
    /// <param name="sceneName">シーン名</param>
    private void PlayBGMForLoadedScene(string sceneName)
    {
        if (AudioManager.Instance != null)
        {
            // シーン名に基づいてBGMを再生
            string bgmName = GetBGMNameForScene(sceneName);
            if (!string.IsNullOrEmpty(bgmName))
            {
                AudioManager.Instance.PlayBGM(bgmName, 1.0f);
                Debug.Log($"シーン '{sceneName}' でBGM '{bgmName}' を再生開始");
            }
            else
            {
                Debug.Log($"シーン '{sceneName}' にはBGM設定がありません");
            }
        }
        else
        {
            Debug.LogWarning("AudioManagerが見つかりません");
        }
    }

    /// <summary>
    /// シーン名に対応するBGM名を取得
    /// </summary>
    /// <param name="sceneName">シーン名</param>
    /// <returns>BGM名（見つからない場合はnull）</returns>
    private string GetBGMNameForScene(string sceneName)
    {
        // シーン名とBGM名のマッピング
        string lowerSceneName = sceneName.ToLower();
        
        switch (lowerSceneName)
        {
            case "title":
            case "titlescene":
                return "TitleBGM";
            
            case "game":
            case "main":
                return "GameBGM";
            
            case "result":
            case "resultscene":
                return "ResultBGM";
            
            case "menu":
            case "menuscene":
                return "MenuBGM";
            
            default:
                // "gamescene"が含まれる場合はGameBGMを再生
                if (lowerSceneName.Contains("gamescene"))
                {
                    return "GameBGM";
                }
                return null; // BGMなし
        }
    }

    /// <summary>
    /// 必要に応じてフェードインを開始
    /// </summary>
    private void StartFadeInIfNeeded()
    {
        if (fadeCanvas != null)
        {
            Image fadeImage = fadeCanvas.GetComponentInChildren<Image>(true);
            if (fadeImage != null)
            {
                // αを1にしておく（シーン読み込み直後は画面を隠す）
                Color color = fadeImage.color;
                fadeImage.color = new Color(color.r, color.g, color.b, 1f);
                StartCoroutine(FadeIn());
            }
        }
    }

    #endregion
}
