using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using System.Collections;



[System.Serializable]
public class SceneBGM
{
    //�V�[�����Ƃ�BGM
    public string sceneName;
    public AudioClip bgmClip;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;
    //public AudioMixerGroup masterMixerGroup;
    public AudioMixerGroup bgmMixerGroup;
    public AudioMixerGroup seMixerGroup;

    [Header("SE Pool Settings")]
    public int sePoolSize = 10;
    private Queue<AudioSource> sePool = new Queue<AudioSource>();

    [Header("Scene BGM Settings")]
    public List<SceneBGM> sceneBGMList = new List<SceneBGM>();

    private AudioSource bgmSource;
    private Coroutine bgmFadeCoroutine;

    private AudioClip nextBGMClip;//���̃V�[����BGM�̗\��

   



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

        InitializeBGMSource();
        InitializeSEPool();

        float vol;
        audioMixer.GetFloat("Master", out vol);
        Debug.Log("Master�~�L�T�[�l (dB): " + vol);
        audioMixer.GetFloat("BGM", out vol);
        Debug.Log("BGM�~�L�T�[�l (dB): " + vol);
        audioMixer.GetFloat("SE", out vol);
        Debug.Log("SE�~�L�T�[�l (dB): " + vol);

        
    }

    IEnumerator Start()
    {
        yield return null;

        //�N���^�C�~���O����������Awake()���玝���Ă���
        LoadVolumeSettings();

        //float vol;
        //audioMixer.GetFloat("Master", out vol);
        //Debug.Log("Master�~�L�T�[�l (dB): " + vol);
        //audioMixer.GetFloat("BGM", out vol);
        //Debug.Log("BGM�~�L�T�[�l (dB): " + vol);
        //audioMixer.GetFloat("SE", out vol);
        //Debug.Log("SE�~�L�T�[�l (dB): " + vol);


        InitializeBGMSource();
        InitializeSEPool();

    }

    //Denug
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("BGM���: " + bgmSource.isPlaying);
            Debug.Log("Clip: " + bgmSource.clip?.name);
            Debug.Log("Volume: " + bgmSource.volume);
            Debug.Log("Group: " + bgmSource.outputAudioMixerGroup?.name);
        }
    }


    //BGM�֘A
    private void InitializeBGMSource()
    {
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.outputAudioMixerGroup = bgmMixerGroup;
        bgmSource.loop = true;
    }

    public void PlayBGM(AudioClip newClip, float fadeDuration = 2.0f)
    {
        if (bgmFadeCoroutine != null)
            StopCoroutine(bgmFadeCoroutine);

        bgmFadeCoroutine = StartCoroutine(FadeInNewBGM(newClip, fadeDuration));
    }

    private IEnumerator FadeInNewBGM(AudioClip newClip, float duration)
    {
        // �����Đ�
        bgmSource.clip = newClip;
        bgmSource.Play();

        float targetVolume = GetBGMVolume();
        float currentVolume = 0f;

        // ����0�ŊJ�n
        audioMixer.SetFloat("BGM", Mathf.Log10(0.0001f) * 20);

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            currentVolume = Mathf.Lerp(0f, targetVolume, t / duration);
            audioMixer.SetFloat("BGM", Mathf.Log10(Mathf.Clamp(currentVolume, 0.0001f, 1f)) * 20);
            yield return null;
        }

        audioMixer.SetFloat("BGM", Mathf.Log10(Mathf.Clamp(targetVolume, 0.0001f, 1f)) * 20);
    }

    public void PreloadNextBGM(AudioClip clip)//BGM�\��
    {
        nextBGMClip = clip;
    }


    //SE�֘A
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

    //���ʐݒ�
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("Master", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
        PlayerPrefs.SetFloat("Master", volume);
    }
    public void SetBGMVolume(float volume)
    {
        audioMixer.SetFloat("BGM", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
        PlayerPrefs.SetFloat("BGM", volume);
    }
    public void SetSEVolume(float volume)
    {
        audioMixer.SetFloat("SE", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
        PlayerPrefs.SetFloat("SE", volume);
    }
    public float GetMasterVolume() => PlayerPrefs.GetFloat("Master", 1f);
    public float GetBGMVolume() => PlayerPrefs.GetFloat("BGM", 1f);
    public float GetSEVolume() => PlayerPrefs.GetFloat("SE", 1f);

    private void LoadVolumeSettings()
    {
        SetMasterVolume(GetMasterVolume());
        SetBGMVolume(GetBGMVolume());
        SetSEVolume(GetSEVolume());
    }
}
