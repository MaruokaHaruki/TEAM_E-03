using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;


public class SceneManagerScript : SingletonMonoBehaviour<SceneManagerScript>
{
    private string nextScene;
    public string winnerName;

    public GameObject fadePanel;             // フェード用のUIパネル（Image）
    public float fadeDuration = 1.0f;   // フェードの完了にかかる時間

    private GameObject canvas;
   
       
  

    public void FadeOutScene(string scene)
    {

        foreach (var sceneBGM in SoundManager.Instance.sceneBGMList)
        {
            if (sceneBGM.sceneName == scene)
            {
                SoundManager.Instance.PreloadNextBGM(sceneBGM.bgmClip);
                break;
            }
        }
        canvas =Instantiate(fadePanel/*, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity*/);
        
        nextScene = scene;
        StartCoroutine(FadeOutAndLoadScene());

    }

    public IEnumerator FadeOutAndLoadScene()
    {
        canvas.GetComponentInChildren<Image>().enabled = true;                 // パネルを有効化
        float elapsedTime = 0.0f;                 // 経過時間を初期化
        Color startColor = canvas.GetComponentInChildren<Image>().color;       // フェードパネルの開始色を取得
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 1.0f); // フェードパネルの最終色を設定

        // フェードアウトアニメーションを実行
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;                        // 経過時間を増やす
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);  // フェードの進行度を計算
            canvas.GetComponentInChildren<Image>().color = Color.Lerp(startColor, endColor, t); // パネルの色を変更してフェードアウト
            yield return null;                                     // 1フレーム待機
        }

        canvas.GetComponentInChildren<Image>().color = endColor;  // フェードが完了したら最終色に設定
     
        SceneManager.LoadScene(nextScene);

        Destroy(canvas);

    }

    public IEnumerator FadeIn()
    {

        canvas.GetComponentInChildren<Image>().enabled = true;
        float elapsedTime = 0.0f;
        Color startColor = canvas.GetComponentInChildren<Image>().color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0.0f); // α0へ

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);
            canvas.GetComponentInChildren<Image>().color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        canvas.GetComponentInChildren<Image>().color = endColor;
        canvas.GetComponentInChildren<Image>().enabled = false; // フェードが終わったら非表示
    }



    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            FadeOutScene("Result");
        }
    }

    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName); // 現在のシーンを置き換える
    }

    public void LoadSceneByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex); // ビルドインデックスで指定
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    //void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    //{
    //    Debug.Log("Loaded scene: " + scene.name);

    //    if (SoundManager.Instance != null)
    //    {
    //        foreach (var sceneBGM in SoundManager.Instance.sceneBGMList)
    //        {
    //            if (sceneBGM.sceneName == scene.name)
    //            {
    //                SoundManager.Instance.PlayBGM(sceneBGM.bgmClip, 1.0f);
    //                return;
    //            }
    //        }
    //    }
    //}

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Loaded scene: " + scene.name);

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

        if (canvas == null)
        {
            //fadePanel = GameObject.Find("FadePanel")?.GetComponent<Image>();
        }

        // 再取得後に FadeIn 実行
        if (canvas != null)
        {

            Color c = canvas.GetComponentInChildren<Image>().color;
            canvas.GetComponentInChildren<Image>().color = new Color(c.r, c.g, c.b, 1.0f);
            StartCoroutine(FadeIn());
        }

    }

}
