using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 音源データを格納するクラス
/// </summary>
[System.Serializable]
public class AudioClipData {
    public string name;
    public AudioClip clip;
}

/// <summary>
/// ゲーム全体のサウンドを管理するシングルトンクラス
/// BGMとSEを分離して管理し、複数SE同時再生に対応
/// AudioMixerによるエフェクト制御機能付き
/// </summary>
public class AudioManager : MonoBehaviour {
    public static AudioManager Instance { get; private set; }

    [Header("BGM用 AudioSource")]
    [SerializeField] private AudioSource bgmSource_;

    [Header("SE用 AudioSource（プール用）")]
    [SerializeField] private AudioSource sePrefab_;

    [Header("SEプール数")]
    [SerializeField] private int sePoolCount_ = 10;

    [Header("AudioMixer設定")]
    [Tooltip("メインのAudioMixer")]
    [SerializeField] private AudioMixer audioMixer_;
    
    [Tooltip("BGM用のMixerGroup")]
    [SerializeField] private AudioMixerGroup bgmMixerGroup_;
    
    [Tooltip("SE用のMixerGroup")]
    [SerializeField] private AudioMixerGroup seMixerGroup_;

    [Header("音量設定")]
    [Range(0f, 1f)]
    [SerializeField] private float bgmVolume_ = 1f;
    [Range(0f, 1f)]
    [SerializeField] private float seVolume_ = 1f;

    [Header("BGMプリセット")]
    [SerializeField] private AudioClipData[] bgmClips_;

    [Header("SEプリセット")]
    [SerializeField] private AudioClipData[] seClips_;

    private Queue<AudioSource> sePool_ = new Queue<AudioSource>();
    private Dictionary<string, AudioClip> bgmDictionary_ = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> seDictionary_ = new Dictionary<string, AudioClip>();

    public float BGMVolume {
        get => bgmVolume_;
        set {
            bgmVolume_ = Mathf.Clamp01(value);
            if (bgmSource_ != null) bgmSource_.volume = bgmVolume_;
        }
    }

    public float SEVolume {
        get => seVolume_;
        set => seVolume_ = Mathf.Clamp01(value);
    }

    private void Awake() {
        // シングルトン化
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// オーディオシステムの初期化（SEプール構築）
    /// </summary>
    private void Initialize() {
        try {
            // BGM AudioSourceの設定
            if (bgmSource_ == null) {
                bgmSource_ = gameObject.AddComponent<AudioSource>();
            }
            bgmSource_.loop = true;
            bgmSource_.playOnAwake = false;
            bgmSource_.volume = bgmVolume_;
            
            // BGMにMixerGroupを適用
            if (bgmMixerGroup_ != null) {
                bgmSource_.outputAudioMixerGroup = bgmMixerGroup_;
                Debug.Log("BGM AudioSourceにMixerGroupを適用しました");
            }

            // SE用プレハブがない場合は作成
            if (sePrefab_ == null) {
                GameObject seObj = new GameObject("SE_Source");
                seObj.transform.SetParent(transform);
                sePrefab_ = seObj.AddComponent<AudioSource>();
                sePrefab_.playOnAwake = false;
            }

            // SE用プレハブにMixerGroupを適用
            if (seMixerGroup_ != null) {
                sePrefab_.outputAudioMixerGroup = seMixerGroup_;
            }

            // SEプール構築
            for (int i = 0; i < sePoolCount_; i++) {
                AudioSource se = Instantiate(sePrefab_, transform);
                se.volume = seVolume_;
                
                // 各SE AudioSourceにMixerGroupを適用
                if (seMixerGroup_ != null) {
                    se.outputAudioMixerGroup = seMixerGroup_;
                }
                
                se.gameObject.SetActive(false);
                sePool_.Enqueue(se);
            }

            // プリセット音源の辞書作成
            SetupAudioDictionaries();

            Debug.Log("AudioManager 初期化完了（AudioMixer対応）");
        }
        catch (Exception e) {
            Debug.LogError($"AudioManager初期化エラー: {e.Message}");
        }
    }

    /// <summary>
    /// プリセット音源の辞書を作成
    /// </summary>
    private void SetupAudioDictionaries() {
        // BGM辞書作成
        if (bgmClips_ != null) {
            foreach (var bgm in bgmClips_) {
                if (bgm.clip != null && !string.IsNullOrEmpty(bgm.name)) {
                    bgmDictionary_[bgm.name] = bgm.clip;
                }
            }
        }

        // SE辞書作成
        if (seClips_ != null) {
            foreach (var se in seClips_) {
                if (se.clip != null && !string.IsNullOrEmpty(se.name)) {
                    seDictionary_[se.name] = se.clip;
                }
            }
        }
    }

    /// <summary>
    /// BGMをフェード付きで再生（AudioClip直接指定）
    /// </summary>
    public void PlayBGM(AudioClip clip, float fadeTime = 1f) {
        if (clip == null) {
            Debug.LogWarning("BGM AudioClipがnullです");
            return;
        }
        StopAllCoroutines();
        StartCoroutine(FadeInBGM(clip, fadeTime));
    }

    /// <summary>
    /// BGMをフェード付きで再生（名前指定）
    /// </summary>
    public void PlayBGM(string bgmName, float fadeTime = 1f) {
        if (bgmDictionary_.TryGetValue(bgmName, out AudioClip clip)) {
            PlayBGM(clip, fadeTime);
        }
        else {
            Debug.LogWarning($"BGM '{bgmName}' が見つかりません");
        }
    }

    /// <summary>
    /// BGMを停止
    /// </summary>
    public void StopBGM(float fadeTime = 1f) {
        StopAllCoroutines();
        StartCoroutine(FadeOutBGM(fadeTime));
    }

    /// <summary>
    /// SEを再生（AudioClip直接指定）
    /// </summary>
    public void PlaySE(AudioClip clip) {
        if (clip == null) {
            Debug.LogWarning("SE AudioClipがnullです");
            return;
        }

        if (sePool_.Count == 0) {
            Debug.LogWarning("SEプールが足りません！");
            return;
        }

        AudioSource se = sePool_.Dequeue();
        se.gameObject.SetActive(true);
        se.clip = clip;
        se.volume = seVolume_;
        se.Play();

        StartCoroutine(ReturnToPool(se, clip.length));
    }

    /// <summary>
    /// SEを再生（名前指定）
    /// </summary>
    public void PlaySE(string seName) {
        if (seDictionary_.TryGetValue(seName, out AudioClip clip)) {
            PlaySE(clip);
        }
        else {
            Debug.LogWarning($"SE '{seName}' が見つかりません");
        }
    }

    // フェードイン処理
    private IEnumerator FadeInBGM(AudioClip clip, float time) {
        if (bgmSource_.isPlaying)
            yield return FadeOutBGM(time * 0.5f);

        bgmSource_.clip = clip;
        bgmSource_.volume = 0f;
        bgmSource_.Play();

        float t = 0f;
        while (t < time) {
            bgmSource_.volume = Mathf.Lerp(0f, 1f, t / time);
            t += Time.deltaTime;
            yield return null;
        }
        bgmSource_.volume = 1f;
    }

    // フェードアウト処理
    private IEnumerator FadeOutBGM(float time) {
        if (bgmSource_ == null) yield break;

        float startVol = bgmSource_.volume;
        float t = 0f;

        while (t < time) {
            bgmSource_.volume = Mathf.Lerp(startVol, 0f, t / time);
            t += Time.deltaTime;
            yield return null;
        }

        bgmSource_.Stop();
        bgmSource_.clip = null;
        bgmSource_.volume = bgmVolume_;
    }

    // SEを元に戻す
    private IEnumerator ReturnToPool(AudioSource source, float delay) {
        yield return new WaitForSeconds(delay);
        source.Stop();
        source.gameObject.SetActive(false);
        sePool_.Enqueue(source);
    }

    /// <summary>
    /// AudioMixerのパラメータを設定
    /// </summary>
    /// <param name="parameterName">パラメータ名（例: "BGMVolume", "SEVolume", "BGMPitch"など）</param>
    /// <param name="value">設定値</param>
    public void SetMixerParameter(string parameterName, float value) {
        if (audioMixer_ != null) {
            audioMixer_.SetFloat(parameterName, value);
            Debug.Log($"[MIXER] パラメータ '{parameterName}' を {value} に設定");
        }
        else {
            Debug.LogWarning("AudioMixerが設定されていません");
        }
    }

    /// <summary>
    /// AudioMixerのパラメータを取得
    /// </summary>
    /// <param name="parameterName">パラメータ名</param>
    /// <returns>パラメータの値</returns>
    public float GetMixerParameter(string parameterName) {
        if (audioMixer_ != null) {
            audioMixer_.GetFloat(parameterName, out float value);
            return value;
        }
        Debug.LogWarning("AudioMixerが設定されていません");
        return 0f;
    }

    /// <summary>
    /// BGMの音量をMixer経由で設定（デシベル値）
    /// </summary>
    /// <param name="volumeDb">音量（デシベル、-80〜0）</param>
    public void SetBGMVolumeDb(float volumeDb) {
        SetMixerParameter("BGMVolume", Mathf.Clamp(volumeDb, -80f, 0f));
    }

    /// <summary>
    /// SEの音量をMixer経由で設定（デシベル値）
    /// </summary>
    /// <param name="volumeDb">音量（デシベル、-80〜0）</param>
    public void SetSEVolumeDb(float volumeDb) {
        SetMixerParameter("SEVolume", Mathf.Clamp(volumeDb, -80f, 0f));
    }

    /// <summary>
    /// BGMのピッチを設定
    /// </summary>
    /// <param name="pitch">ピッチ値（0.5〜2.0程度が推奨）</param>
    public void SetBGMPitch(float pitch) {
        SetMixerParameter("BGMPitch", Mathf.Clamp(pitch, 0.1f, 3.0f));
    }

    /// <summary>
    /// SEのピッチを設定
    /// </summary>
    /// <param name="pitch">ピッチ値（0.5〜2.0程度が推奨）</param>
    public void SetSEPitch(float pitch) {
        SetMixerParameter("SEPitch", Mathf.Clamp(pitch, 0.1f, 3.0f));
    }

    /// <summary>
    /// BGMにローパスフィルターを適用
    /// </summary>
    /// <param name="cutoffFreq">カットオフ周波数（Hz、10〜22000）</param>
    public void SetBGMLowpassFilter(float cutoffFreq) {
        SetMixerParameter("BGMLowpass", Mathf.Clamp(cutoffFreq, 10f, 22000f));
    }

    /// <summary>
    /// BGMにハイパスフィルターを適用
    /// </summary>
    /// <param name="cutoffFreq">カットオフ周波数（Hz、10〜22000）</param>
    public void SetBGMHighpassFilter(float cutoffFreq) {
        SetMixerParameter("BGMHighpass", Mathf.Clamp(cutoffFreq, 10f, 22000f));
    }

    /// <summary>
    /// BGMにリバーブエフェクトを適用
    /// </summary>
    /// <param name="reverbLevel">リバーブレベル（0〜1）</param>
    public void SetBGMReverb(float reverbLevel) {
        SetMixerParameter("BGMReverb", Mathf.Clamp01(reverbLevel));
    }

    /// <summary>
    /// SEにディストーションエフェクトを適用
    /// </summary>
    /// <param name="distortionLevel">ディストーションレベル（0〜1）</param>
    public void SetSEDistortion(float distortionLevel) {
        SetMixerParameter("SEDistortion", Mathf.Clamp01(distortionLevel));
    }

    /// <summary>
    /// すべてのエフェクトをリセット
    /// </summary>
    public void ResetAllEffects() {
        if (audioMixer_ == null) return;

        SetBGMVolumeDb(0f);
        SetSEVolumeDb(0f);
        SetBGMPitch(1f);
        SetSEPitch(1f);
        SetBGMLowpassFilter(22000f);
        SetBGMHighpassFilter(10f);
        SetBGMReverb(0f);
        SetSEDistortion(0f);

        Debug.Log("[MIXER] すべてのエフェクトをリセットしました");
    }

    /// <summary>
    /// スナップショットを適用
    /// </summary>
    /// <param name="snapshotName">スナップショット名</param>
    /// <param name="timeToReach">遷移時間（秒）</param>
    public void ApplySnapshot(string snapshotName, float timeToReach = 1f) {
        if (audioMixer_ == null) {
            Debug.LogWarning("AudioMixerが設定されていません");
            return;
        }

        AudioMixerSnapshot snapshot = audioMixer_.FindSnapshot(snapshotName);
        if (snapshot != null) {
            snapshot.TransitionTo(timeToReach);
            Debug.Log($"[MIXER] スナップショット '{snapshotName}' を適用（遷移時間: {timeToReach}秒）");
        }
        else {
            Debug.LogWarning($"スナップショット '{snapshotName}' が見つかりません");
        }
    }
}
