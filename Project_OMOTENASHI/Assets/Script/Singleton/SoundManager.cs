using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Timeline;
using Unity.VisualScripting;



[System.Serializable]
public class SceneBGM
{
    //シーンごとのBGM
    public string sceneName;
    public AudioClip bgmClip;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("SE Pool Settings")]
    public int sePoolSize = 10;
    private Queue<AudioSource> sePool = new Queue<AudioSource>();

    [Header("Scene BGM Settings")]
    public List<SceneBGM> sceneBGMList = new List<SceneBGM>();

    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource seSource;
    private Coroutine BGMFadeCoroutine;

    private AudioClip nextBGMClip;//次のシーンのBGMの予約




    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

        }
        else
        {
            Destroy(gameObject);
        }


    }

    private void Start()
    {
    }


    //BGM関連
    public void PlayBGM(AudioClip newClip, float fadeDuration = 2.0f)
    {

        if (newClip == null)
        {
            Debug.LogWarning("PlayBGMにnullのクリップが渡されました");
            return;
        }


        if (BGMFadeCoroutine != null)
            StopCoroutine(BGMFadeCoroutine);

        BGMFadeCoroutine = StartCoroutine(FadeInNewBGM(newClip, fadeDuration));

        audioMixer.GetFloat("BGMVol", out float vol);
        Debug.Log($"BGM再生: {newClip.name}:vol{vol}");



    }

    private IEnumerator FadeInNewBGM(AudioClip newClip, float duration)
    {
        // すぐ再生
        if (newClip == null)
        {
            Debug.LogWarning("PlayBGMにnullのクリップが渡されました");

        }
        bgmSource.AddComponent<AudioSource>();
        bgmSource.clip = newClip;
        bgmSource.Play();

        audioMixer.GetFloat("BGMVol", out float targetVolume);
        float currentVolume = 0.01f;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            currentVolume = Mathf.Lerp(0.0001f, targetVolume, t / duration);
            float db = Mathf.Clamp(Mathf.Log10(currentVolume) * 20f, -80f, 0f);
            audioMixer.SetFloat("BGMVol", db);
            yield return null;
        }

    }

    public void PreloadNextBGM(AudioClip clip)//BGM予約
    {
        nextBGMClip = clip;
    }


    //SE関連
    private void InitializeSEPool()
    {
        for (int i = 0; i < sePoolSize; i++)
        {
            GameObject seObj = new GameObject("SE_Source_" + i);
            seObj.transform.SetParent(transform);
            //AudioSource source = seObj.AddComponent<AudioSource>();
            seSource = seObj.AddComponent<AudioSource>();

            //source.outputAudioMixerGroup = seMixerGroup;
            seSource.playOnAwake = false;
            seSource.pitch = 1f;

            seObj.SetActive(false);
            sePool.Enqueue(seSource);
        }
    }

    public void PlaySE(AudioClip clip)
    {
        if (clip == null || sePool.Count == 0) return;

        AudioSource source = sePool.Dequeue();
        source.gameObject.SetActive(true);
        source.pitch = 1f;

        source.clip = clip;
        source.Play();

        StartCoroutine(ReturnToPoolAfterPlaying(source));
    }

    private IEnumerator ReturnToPoolAfterPlaying(AudioSource source)
    {
        yield return new WaitForSeconds(source.clip.length);
        source.Stop();
        source.clip = null;
        source.gameObject.SetActive(false);
        sePool.Enqueue(source);
    }

}