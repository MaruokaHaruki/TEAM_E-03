using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using System.Collections;



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
    public AudioMixerGroup masterMixerGroup;
    public AudioMixerGroup bgmMixerGroup;
    public AudioMixerGroup seMixerGroup;

    [Header("SE Pool Settings")]
    public int sePoolSize = 10;
    private Queue<AudioSource> sePool = new Queue<AudioSource>();

    [Header("Scene BGM Settings")]
    public List<SceneBGM> sceneBGMList = new List<SceneBGM>();

    private AudioSource bgmSource;
    private Coroutine BGMFadeCoroutine;

    private AudioClip nextBGMClip;//次のシーンのBGMの予約




    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadVolumeSettings();
            InitializebgmSource();
            InitializeSEPool();
        }
        else
        {
            Destroy(gameObject);
        }

        float db;
        if (audioMixer.GetFloat("BGM", out db))
        {
            Debug.Log("現在のBGMミキサー値 (dB): " + db);
        }
        else
        {
            Debug.LogWarning("BGM パラメータがAudioMixerに見つかりません！");
        }

    }

    private void Start()
    {
        float vol;
        audioMixer.GetFloat("BGM", out vol);
    }

    //BGM関連
    private void InitializebgmSource()
    {
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.outputAudioMixerGroup = bgmMixerGroup;
        bgmSource.volume = 1f;

        bgmSource.mute = false;
        audioMixer.SetFloat("BGM", 0); // 完全に0dBに固定

        bgmSource.loop = true;
    }

    public void PlayBGMDirect(AudioClip clip)   //デバッグ
    {
        bgmSource.Stop();
        bgmSource.clip = clip;
        bgmSource.volume = 1f;
        bgmSource.outputAudioMixerGroup = bgmMixerGroup;

        // Mixer BGMボリュームを0dBに固定
        audioMixer.SetFloat("BGM", 0f);

        bgmSource.Play();

        Debug.Log("PlayBGMDirect: " + clip.name);
    }
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
        Debug.Log("BGM再生: " + newClip.name);


    }

    private IEnumerator FadeInNewBGM(AudioClip newClip, float duration)
    {
        // すぐ再生
        bgmSource.clip = newClip;
        bgmSource.Play();

        float targetVolume = GetBGMVolume();
        float currentVolume = 0.0001f;

        // 音量0.0001fで開始
        audioMixer.SetFloat("BGM", Mathf.Log10(0.0001f) * 20);

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            currentVolume = Mathf.Lerp(0f, targetVolume, t / duration);
            audioMixer.SetFloat("BGM", Mathf.Log10(Mathf.Clamp(currentVolume, 0.0001f, 1f)) * 20);
            yield return null;
        }

        audioMixer.SetFloat("BGM", Mathf.Log10(Mathf.Clamp(targetVolume, 0.0001f, 1f)) * 20);
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
            AudioSource source = seObj.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = seMixerGroup;
            source.playOnAwake = false;
            seObj.SetActive(false);
            sePool.Enqueue(source);
        }
    }

    public void PlaySE(AudioClip clip)
    {
        if (clip == null || sePool.Count == 0) return;

        AudioSource source = sePool.Dequeue();
        source.gameObject.SetActive(true);
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

    //音量設定
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("Master", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
        PlayerPrefs.SetFloat("Master", volume);
    }
    public void SetBGMVolume(float volume)
    {
        float db = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;
        Debug.Log($"SetBGMVolume({volume}) → {db} dB");
        audioMixer.SetFloat("BGM", db);
        PlayerPrefs.SetFloat("BGM", volume);
    }
    public void SetSEVolume(float volume)
    {
        audioMixer.SetFloat("SE", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
        PlayerPrefs.SetFloat("SE", volume);
    }
    public float GetMasterVolume() => PlayerPrefs.GetFloat("Master", 1f);
    public float GetBGMVolume()
    { float volume = PlayerPrefs.GetFloat("BGM", 1f);
        return Mathf.Max(volume, 0.0001f); 
    }
    public float GetSEVolume() => PlayerPrefs.GetFloat("SE", 1f);

    private void LoadVolumeSettings()
    {
        float master = Mathf.Max(GetMasterVolume(), 0.0001f);
        float bgm = Mathf.Max(GetBGMVolume(), 0.0001f);
        float se = Mathf.Max(GetSEVolume(), 0.0001f);

        SetMasterVolume(master);
        SetBGMVolume(bgm);
        SetSEVolume(se);
    }
}
