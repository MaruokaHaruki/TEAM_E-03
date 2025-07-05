using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class AudioVolume : MonoBehaviour
{
    public AudioMixer audioMixer;
    public static AudioVolume Instance;

    void Awake()
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
        SetBGM(1);
        audioMixer.GetFloat("BGMVol", out float bgmvol);
        Debug.Log($"[Audio] BGM: {bgmvol}");
        SetSE(1);
        audioMixer.GetFloat("SEVol", out float sevol);
        Debug.Log($"[Audio] SE: {sevol}");
        SetMaster(1);
        audioMixer.GetFloat("MasterVol", out float vol);
        Debug.Log($"[Audio] Master: {vol}");

    }

    public void SetBGM(float vol)
    {
        //dB‚É•ÏŠ·
        float db = Mathf.Clamp(Mathf.Log10(vol) * 20f, -80f, 0f);
        audioMixer.SetFloat("BGMVol", db);
    }

    public void SetSE(float vol)
    {
        //dB‚É•ÏŠ·
        float db = Mathf.Clamp(Mathf.Log10(vol) * 20f, -80f, 0f);
        audioMixer.SetFloat("SEVol", db);
    }


    public void SetMaster(float vol)
    {
        //dB‚É•ÏŠ·
        float db = Mathf.Clamp(Mathf.Log10(vol) * 20f, -80f, 0f);
        audioMixer.SetFloat("MasterVol", db);
    }
}
