using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;


public class SceneManagerScript : SingletonMonoBehaviour<SceneManagerScript>
{
    private string nextScene;
    public string winnerName;

    public GameObject fadePanel;             // �t�F�[�h�p��UI�p�l���iImage�j
    public float fadeDuration = 1.0f;   // �t�F�[�h�̊����ɂ����鎞��

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
        canvas.GetComponentInChildren<Image>().enabled = true;                 // �p�l����L����
        float elapsedTime = 0.0f;                 // �o�ߎ��Ԃ������
        Color startColor = canvas.GetComponentInChildren<Image>().color;       // �t�F�[�h�p�l���̊J�n�F��擾
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 1.0f); // �t�F�[�h�p�l���̍ŏI�F��ݒ�

        // �t�F�[�h�A�E�g�A�j���[�V��������s
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;                        // �o�ߎ��Ԃ𑝂₷
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);  // �t�F�[�h�̐i�s�x��v�Z
            canvas.GetComponentInChildren<Image>().color = Color.Lerp(startColor, endColor, t); // �p�l���̐F��ύX���ăt�F�[�h�A�E�g
            yield return null;                                     // 1�t���[���ҋ@
        }

        canvas.GetComponentInChildren<Image>().color = endColor;  // �t�F�[�h������������ŏI�F�ɐݒ�
     
        SceneManager.LoadScene(nextScene);

        Destroy(canvas);

    }

    public IEnumerator FadeIn()
    {

        canvas.GetComponentInChildren<Image>().enabled = true;
        float elapsedTime = 0.0f;
        Color startColor = canvas.GetComponentInChildren<Image>().color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0.0f); // ��0��

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);
            canvas.GetComponentInChildren<Image>().color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        canvas.GetComponentInChildren<Image>().color = endColor;
        canvas.GetComponentInChildren<Image>().enabled = false; // �t�F�[�h���I��������\��
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
        SceneManager.LoadScene(sceneName); // ���݂̃V�[����u��������
    }

    public void LoadSceneByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex); // �r���h�C���f�b�N�X�Ŏw��
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

        // �Ď擾��� FadeIn ���s
        if (canvas != null)
        {

            Color c = canvas.GetComponentInChildren<Image>().color;
            canvas.GetComponentInChildren<Image>().color = new Color(c.r, c.g, c.b, 1.0f);
            StartCoroutine(FadeIn());
        }

    }

}
