using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneManagerScript : SingletonMonoBehaviour<SceneManagerScript>
{
    [Header("フェード設定")]
    public GameObject fadePanelPrefab;     // フェード用のパネル（Image付きCanvas）
    public float fadeDuration = 1.0f;      // フェード時間

    [HideInInspector] public string winnerName;  

    private GameObject canvas;             // 実際に使われるフェード用Canvasのインスタンス
    private string nextScene;
    private bool isFading = false;

    // フェードアウトしてシーン遷移
    public void FadeOutScene(string scene)
    {
        if (isFading) return;
        isFading = true;

        nextScene = scene;

        // BGM事前読み込み
        foreach (var sceneBGM in SoundManager.Instance.sceneBGMList)
        {
            if (sceneBGM.sceneName == scene)
            {
                SoundManager.Instance.PreloadNextBGM(sceneBGM.bgmClip);
                break;
            }
        }

        // フェードUI生成（または再利用）
        canvas = Instantiate(fadePanelPrefab);
        DontDestroyOnLoad(canvas);

        StartCoroutine(FadeOutAndLoadScene());
    }

    private IEnumerator FadeOutAndLoadScene()
    {
        Image fadeImage = canvas.GetComponentInChildren<Image>(true);
        fadeImage.enabled = true;

        float elapsed = 0f;
        Color start = fadeImage.color;
        Color end = new Color(start.r, start.g, start.b, 1f);

        // フェードアウト
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            fadeImage.color = Color.Lerp(start, end, t);
            yield return null;
        }

        fadeImage.color = end;

        // シーン非同期読み込み
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        while (!op.isDone)
            yield return null;

    }

    private IEnumerator FadeIn()
    {
        if (canvas == null) yield break;

        Image fadeImage = canvas.GetComponentInChildren<Image>(true);
        fadeImage.enabled = true;

        float elapsed = 0f;
        Color start = fadeImage.color;
        Color end = new Color(start.r, start.g, start.b, 0f);

        // フェードイン
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            fadeImage.color = Color.Lerp(start, end, t);
            yield return null;
        }

        fadeImage.color = end;
        fadeImage.enabled = false;

        Destroy(canvas);
        isFading = false;
    }

    public void LoadSceneByName(string sceneName) => SceneManager.LoadScene(sceneName);
    public void LoadSceneByIndex(int index) => SceneManager.LoadScene(index);

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene loaded: " + scene.name);

        // BGM再生
        if (SoundManager.Instance != null)
        {
            foreach (var sceneBGM in SoundManager.Instance.sceneBGMList)
            {
                if (sceneBGM.sceneName == scene.name)
                {
                    SoundManager.Instance.PlayBGM(sceneBGM.bgmClip, 1.0f);
                    break;
                }
            }
        }

        // フェードパネルがあればフェードイン開始
        if (canvas != null)
        {
            Image fadeImage = canvas.GetComponentInChildren<Image>(true);
            if (fadeImage != null)
            {
                // αを1にしておく（次のシーンでも見えるように）
                Color c = fadeImage.color;
                fadeImage.color = new Color(c.r, c.g, c.b, 1f);
                StartCoroutine(FadeIn());
            }
        }
    }
}
