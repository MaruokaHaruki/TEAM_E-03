using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// シーン別BGM設定用のデータクラス
/// </summary>
[System.Serializable]
public class SceneBGM
{
    [Tooltip("シーン名")]
    public string sceneName;
    [Tooltip("そのシーンで再生するBGM")]
    public AudioClip bgmClip;
}

/// <summary>
/// ゲーム全体のサウンド管理を行うシングルトンクラス
/// BGMのフェード再生、SE管理、音量設定の保存/読み込みを担当
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("オーディオミキサー設定")]
    public AudioMixer audioMixer;
    public AudioMixerGroup masterMixerGroup;
    public AudioMixerGroup bgmMixerGroup;
    public AudioMixerGroup seMixerGroup;

    [Header("SEプール設定")]
    [SerializeField] private int sePoolSize = 10;
    private Queue<AudioSource> sePool = new Queue<AudioSource>();

    [Header("シーン別BGM設定")]
    public List<SceneBGM> sceneBGMList = new List<SceneBGM>();

    // BGM関連の内部変数
    private AudioSource bgmSource;
    private Coroutine BGMFadeCoroutine;
    private bool isInitialized = false;

    /// <summary>
    /// シングルトンパターンの実装とオーディオシステムの初期化
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// オーディオシステム全体の初期化処理
    /// </summary>
    private void InitializeAudioSystem()
    {
        if (isInitialized) return;

        InitializebgmSource();
        InitializeSEPool();
        LoadVolumeSettings();
        
        // AudioMixerの初期化確認
        ValidateAudioMixer();
        
        isInitialized = true;
        Debug.Log("SoundManager初期化完了");
    }

    /// <summary>
    /// AudioMixerの設定を検証
    /// </summary>
    private void ValidateAudioMixer()
    {
        if (audioMixer == null)
        {
            Debug.LogError("AudioMixerが設定されていません！");
            return;
        }

        // 各パラメータの存在確認
        float testValue;
        if (!audioMixer.GetFloat("MasterVol", out testValue))
            Debug.LogError("AudioMixerに 'MasterVol' パラメータが見つかりません");
        if (!audioMixer.GetFloat("BGMVol", out testValue))
            Debug.LogError("AudioMixerに 'BGMVol' パラメータが見つかりません");
        if (!audioMixer.GetFloat("SEVol", out testValue))
            Debug.LogError("AudioMixerに 'SEVol' パラメータが見つかりません");
    }

    #region BGM関連メソッド

    /// <summary>
    /// BGM用AudioSourceの初期化
    /// </summary>
    private void InitializebgmSource()
    {
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.outputAudioMixerGroup = bgmMixerGroup;
            bgmSource.loop = true;
            bgmSource.playOnAwake = false;
            bgmSource.volume = 1f; // AudioSource自体のボリュームは最大に
            bgmSource.pitch = 1f; // ピッチを通常に設定
            
            Debug.Log($"BGM AudioSource初期化完了 - MixerGroup: {(bgmMixerGroup != null ? bgmMixerGroup.name : "None")}");
        }
    }

    /// <summary>
    /// BGMを即座に再生（デバッグ用）
    /// フェード処理なしで直接再生
    /// </summary>
    /// <param name="clip">再生するBGMクリップ</param>
    public void PlayBGMDirect(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogError("PlayBGMDirect: クリップがnullです");
            return;
        }
        
        if (bgmSource == null)
        {
            Debug.LogError("PlayBGMDirect: bgmSourceがnullです");
            return;
        }

        // 進行中のフェード処理を停止
        if (BGMFadeCoroutine != null)
        {
            StopCoroutine(BGMFadeCoroutine);
            BGMFadeCoroutine = null;
        }

        bgmSource.Stop();
        bgmSource.clip = clip;
        bgmSource.volume = 1f;
        bgmSource.pitch = 1f; // ピッチを通常に設定
        
        // 保存されたBGMボリューム設定をAudioMixerに適用
        float savedVolume = GetBGMVolume();
        SetBGMVolumeToMixer(savedVolume);
        
        bgmSource.Play();
        
        Debug.Log($"PlayBGMDirect: {clip.name} - Volume: {savedVolume} - Pitch: {bgmSource.pitch} - Playing: {bgmSource.isPlaying}");
    }

    /// <summary>
    /// BGMをフェード効果付きで再生
    /// </summary>
    /// <param name="newClip">再生するBGMクリップ</param>
    /// <param name="fadeDuration">フェード時間（秒）</param>
    public void PlayBGM(AudioClip newClip, float fadeDuration = 2.0f)
    {
        if (newClip == null)
        {
            Debug.LogError("PlayBGM: クリップがnullです");
            return;
        }
        
        if (bgmSource == null)
        {
            Debug.LogError("PlayBGM: bgmSourceがnullです");
            return;
        }

        // 進行中のフェード処理を停止
        if (BGMFadeCoroutine != null)
            StopCoroutine(BGMFadeCoroutine);

        BGMFadeCoroutine = StartCoroutine(FadeBGM(newClip, fadeDuration));
        Debug.Log($"BGM再生開始: {newClip.name}");
    }

    /// <summary>
    /// BGMのフェードアウト→フェードイン処理
    /// </summary>
    /// <param name="newClip">新しいBGMクリップ</param>
    /// <param name="duration">全体のフェード時間</param>
    private IEnumerator FadeBGM(AudioClip newClip, float duration)
    {
        float targetVolume = GetBGMVolume();
        bool wasPlaying = bgmSource.isPlaying;

        // フェードアウト（既存BGMがある場合）
        if (wasPlaying && bgmSource.clip != null)
        {
            float currentVol = targetVolume;
            for (float t = 0; t < duration * 0.5f; t += Time.deltaTime)
            {
                float vol = Mathf.Lerp(currentVol, 0f, (t / (duration * 0.5f)));
                SetBGMVolumeToMixer(vol);
                yield return null;
            }
        }

        // 新しいBGMを設定して再生開始
        bgmSource.clip = newClip;
        bgmSource.volume = 1f;
        bgmSource.pitch = 1f; // ピッチを通常に設定
        bgmSource.Play();

        // フェードイン
        for (float t = 0; t < duration * 0.5f; t += Time.deltaTime)
        {
            float vol = Mathf.Lerp(0f, targetVolume, (t / (duration * 0.5f)));
            SetBGMVolumeToMixer(vol);
            yield return null;
        }

        // 最終的な音量に設定
        SetBGMVolumeToMixer(targetVolume);
        BGMFadeCoroutine = null;
    }

    /// <summary>
    /// BGMをフェードアウトして停止
    /// </summary>
    /// <param name="fadeDuration">フェードアウト時間（秒）</param>
    public void StopBGM(float fadeDuration = 1.0f)
    {
        if (bgmSource == null || !bgmSource.isPlaying) return;

        if (BGMFadeCoroutine != null)
            StopCoroutine(BGMFadeCoroutine);

        BGMFadeCoroutine = StartCoroutine(FadeOutBGM(fadeDuration));
    }

    /// <summary>
    /// BGMフェードアウト処理
    /// </summary>
    /// <param name="duration">フェードアウト時間</param>
    private IEnumerator FadeOutBGM(float duration)
    {
        float currentVolume = GetBGMVolume();
        
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float vol = Mathf.Lerp(currentVolume, 0f, t / duration);
            SetBGMVolumeToMixer(vol);
            yield return null;
        }

        bgmSource.Stop();
        BGMFadeCoroutine = null;
    }

    #endregion

    #region シーン別BGM管理

    /// <summary>
    /// 指定されたシーン名のBGMクリップを取得
    /// </summary>
    /// <param name="sceneName">シーン名</param>
    /// <returns>BGMクリップ（見つからない場合はnull）</returns>
    public AudioClip GetBGMForScene(string sceneName)
    {
        foreach (var sceneBGM in sceneBGMList)
        {
            if (sceneBGM.sceneName == sceneName)
            {
                return sceneBGM.bgmClip;
            }
        }
        return null;
    }

    /// <summary>
    /// 指定されたシーンのBGMを再生
    /// </summary>
    /// <param name="sceneName">シーン名</param>
    /// <param name="fadeDuration">フェード時間（秒）</param>
    /// <returns>BGMが見つかって再生開始した場合true</returns>
    public bool PlayBGMForScene(string sceneName, float fadeDuration = 2.0f)
    {
        AudioClip bgmClip = GetBGMForScene(sceneName);
        if (bgmClip != null)
        {
            PlayBGM(bgmClip, fadeDuration);
            return true;
        }
        else
        {
            Debug.LogWarning($"シーン '{sceneName}' のBGMが見つかりませんでした");
            return false;
        }
    }

    #endregion

    #region SE関連メソッド

    /// <summary>
    /// SE用AudioSourceプールの初期化
    /// </summary>
    private void InitializeSEPool()
    {
        for (int i = 0; i < sePoolSize; i++)
        {
            GameObject seObj = new GameObject("SE_Source_" + i);
            seObj.transform.SetParent(transform);
            AudioSource source = seObj.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = seMixerGroup;
            source.playOnAwake = false;
            source.loop = false;
            source.volume = 1f; // AudioSource自体のボリュームは最大に
            source.pitch = 1f; // ピッチを通常に設定
            seObj.SetActive(false);
            sePool.Enqueue(source);
        }
        Debug.Log($"SEプール初期化完了: {sePoolSize}個のAudioSource作成");
    }

    /// <summary>
    /// SEを再生（プールから取得したAudioSourceを使用）
    /// </summary>
    /// <param name="clip">再生するSEクリップ</param>
    public void PlaySE(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("PlaySE: クリップがnullです");
            return;
        }
        
        if (sePool.Count == 0)
        {
            Debug.LogWarning("PlaySE: SEプールが空です");
            return;
        }

        AudioSource source = sePool.Dequeue();
        source.gameObject.SetActive(true);
        source.clip = clip;
        source.volume = 1f;
        source.pitch = 1f; // 再生時にピッチを通常にリセット
        source.Play();

        Debug.Log($"SE再生: {clip.name} - Playing: {source.isPlaying}");
        StartCoroutine(ReturnToPoolAfterPlaying(source));
    }

    /// <summary>
    /// SE再生終了後にAudioSourceをプールに戻す
    /// </summary>
    /// <param name="source">返却するAudioSource</param>
    private IEnumerator ReturnToPoolAfterPlaying(AudioSource source)
    {
        yield return new WaitForSeconds(source.clip.length);
        source.Stop();
        source.clip = null;
        source.gameObject.SetActive(false);
        sePool.Enqueue(source);
    }

    #endregion

    #region 音量設定メソッド

    /// <summary>
    /// マスター音量を設定
    /// </summary>
    /// <param name="volume">音量（0.0～1.0）</param>
    public void SetMasterVolume(float volume)
    {
        float clampedVolume = Mathf.Clamp(volume, 0f, 1f);
        float db = ConvertVolumeToDecibel(clampedVolume);
        
        if (audioMixer != null)
        {
            audioMixer.SetFloat("MasterVol", db);
            Debug.Log($"マスター音量設定: {volume} → {db:F1} dB");
        }
        PlayerPrefs.SetFloat("MasterVol", volume);
    }

    /// <summary>
    /// BGM音量を設定
    /// </summary>
    /// <param name="volume">音量（0.0～1.0）</param>
    public void SetBGMVolume(float volume)
    {
        float clampedVolume = Mathf.Clamp(volume, 0f, 1f);
        SetBGMVolumeToMixer(clampedVolume);
        PlayerPrefs.SetFloat("BGMVol", volume);
        Debug.Log($"BGM音量設定保存: {volume}");
    }

    /// <summary>
    /// BGM音量をAudioMixerに直接設定
    /// </summary>
    /// <param name="volume">音量（0.0001～1.0）</param>
    private void SetBGMVolumeToMixer(float volume)
    {
        if (audioMixer == null) return;
        
        float db = ConvertVolumeToDecibel(volume);
        audioMixer.SetFloat("BGMVol", db);
    }

    /// <summary>
    /// SE音量を設定
    /// </summary>
    /// <param name="volume">音量（0.0～1.0）</param>
    public void SetSEVolume(float volume)
    {
        float clampedVolume = Mathf.Clamp(volume, 0f, 1f);
        float db = ConvertVolumeToDecibel(clampedVolume);
        
        if (audioMixer != null)
        {
            audioMixer.SetFloat("SEVol", db);
            Debug.Log($"SE音量設定: {volume} → {db:F1} dB");
        }
        PlayerPrefs.SetFloat("SEVol", volume);
    }

    /// <summary>
    /// 音量値をデシベルに変換
    /// </summary>
    /// <param name="volume">音量（0.0～1.0）</param>
    /// <returns>デシベル値</returns>
    private float ConvertVolumeToDecibel(float volume)
    {
        // volumeが0の場合、-80dB（ほぼ無音）を返す
        if (volume <= 0)
        {
            return -80f;
        }
        return Mathf.Log10(volume) * 20f;
    }

    /// <summary>マスター音量を取得</summary>
    public float GetMasterVolume() => PlayerPrefs.GetFloat("MasterVol", 1f);

    /// <summary>BGM音量を取得</summary>
    public float GetBGMVolume()
    { 
        float volume = PlayerPrefs.GetFloat("BGMVol", 1f);
        return Mathf.Clamp(volume, 0f, 1f); 
    }

    /// <summary>SE音量を取得</summary>
    public float GetSEVolume() => PlayerPrefs.GetFloat("SEVol", 1f);

    /// <summary>
    /// 保存された音量設定を読み込んでAudioMixerに適用
    /// </summary>
    private void LoadVolumeSettings()
    {
        float masterVol = GetMasterVolume();
        float bgmVol = GetBGMVolume();
        float seVol = GetSEVolume();
        
        SetMasterVolume(masterVol);
        SetBGMVolume(bgmVol);
        SetSEVolume(seVol);
        
        Debug.Log($"音量設定読み込み完了 - Master: {masterVol}, BGM: {bgmVol}, SE: {seVol}");
    }

    /// <summary>
    /// 現在の音量設定をデバッグ出力
    /// </summary>
    [ContextMenu("デバッグ: 現在の音量設定を表示")]
    public void DebugCurrentVolumeSettings()
    {
        if (audioMixer == null)
        {
            Debug.LogError("AudioMixerが設定されていません");
            return;
        }

        float masterDb, bgmDb, seDb;
        audioMixer.GetFloat("MasterVol", out masterDb);
        audioMixer.GetFloat("BGMVol", out bgmDb);
        audioMixer.GetFloat("SEVol", out seDb);

        Debug.Log($"現在のAudioMixer設定:\n" +
                 $"Master: {masterDb:F1} dB\n" +
                 $"BGM: {bgmDb:F1} dB\n" +
                 $"SE: {seDb:F1} dB\n" +
                 $"BGM再生中: {(bgmSource != null ? bgmSource.isPlaying.ToString() : "bgmSource is null")}");
    }

    #endregion
}
