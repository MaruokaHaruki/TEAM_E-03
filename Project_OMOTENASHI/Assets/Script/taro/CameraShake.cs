using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

/// <summary>
/// カメラシェイクの設定クラス
/// </summary>
[System.Serializable]
public class ShakeSettings
{
    [Tooltip("揺れの強度")]
    public float strength = 0.1f;
    [Tooltip("揺れの速度")]
    public float speed = 10f;
    [Tooltip("揺れの持続時間")]
    public float duration = 0.2f;
    [Tooltip("ランダム性を有効にするか")]
    public bool randomness = true;
    [Tooltip("元の位置に戻る時間")]
    public float fadeOut = 0.1f;
}

/// <summary>
/// カメラシェイク機能を提供するクラス
/// </summary>
public class CameraShake : MonoBehaviour
{
    [Header("基本設定")]
    public ShakeSettings defaultShake = new ShakeSettings();
    
    [Header("事前定義された揺れパターン")]
    public ShakeSettings lightShake = new ShakeSettings { strength = 0.05f, speed = 15f, duration = 0.1f };
    public ShakeSettings mediumShake = new ShakeSettings { strength = 0.1f, speed = 10f, duration = 0.2f };
    public ShakeSettings heavyShake = new ShakeSettings { strength = 0.3f, speed = 8f, duration = 0.5f };
    
    [Header("イベント")]
    public UnityEvent OnShakeStart;
    public UnityEvent OnShakeEnd;
    
    [Header("デバッグ")]
    public bool enableDebugKeys = true;
    
    /// <summary>
    /// 対象となるカメラ
    /// </summary>
    private Camera targetCamera;
    /// <summary>
    /// カメラの初期位置
    /// </summary>
    private Vector3 originalPosition;
    /// <summary>
    /// 現在シェイク中かどうか
    /// </summary>
    private bool isShaking = false;
    /// <summary>
    /// 現在実行中のTween
    /// </summary>
    private Tween currentTween;

    /// <summary>
    /// 初期化処理
    /// </summary>
    void Start()
    {
        InitializeCamera();
    }

    /// <summary>
    /// デバッグ用のキー入力チェック
    /// </summary>
    void Update()
    {
        //↓デバッグ用
        //if(Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    ShakeCamera(ShakeType.Light);
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    ShakeCamera(ShakeType.Medium);
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha3))
        //{
        //    ShakeCamera(ShakeType.Heavy);
        //}
    }

    /// <summary>
    /// カメラの初期化を行う
    /// </summary>
    private void InitializeCamera()
    {
        // メインカメラを取得
        targetCamera = Camera.main;
        if (targetCamera == null)
        {
            // メインカメラが見つからない場合は、シーン内の最初のカメラを取得
            targetCamera = FindObjectOfType<Camera>();
        }
        
        if (targetCamera == null)
        {
            Debug.LogError("CameraShake: カメラが見つかりません。");
            enabled = false;
            return;
        }
        
        // 初期位置を保存
        originalPosition = targetCamera.transform.position;
    }

    /// <summary>
    /// シェイクの種類を定義する列挙型
    /// </summary>
    public enum ShakeType
    {
        Light,   // 軽い揺れ
        Medium,  // 中程度の揺れ
        Heavy,   // 重い揺れ
        Custom   // カスタム設定
    }

    /// <summary>
    /// 指定されたタイプでカメラを揺らす
    /// </summary>
    /// <param name="type">シェイクの種類</param>
    public void ShakeCamera(ShakeType type = ShakeType.Custom)
    {
        // タイプに応じた設定を取得
        ShakeSettings settings = type switch
        {
            ShakeType.Light => lightShake,
            ShakeType.Medium => mediumShake,
            ShakeType.Heavy => heavyShake,
            _ => defaultShake
        };
        
        ShakeCamera(settings);
    }

    /// <summary>
    /// 指定された設定でカメラを揺らす
    /// </summary>
    /// <param name="settings">シェイク設定</param>
    public void ShakeCamera(ShakeSettings settings = null)
    {
        // 既にシェイク中、またはカメラが無効な場合は処理を中止
        if (isShaking || targetCamera == null) return;
        
        // 設定がnullの場合はデフォルト設定を使用
        settings ??= defaultShake;
        StartShake(settings);
    }

    /// <summary>
    /// シェイクを開始する
    /// </summary>
    /// <param name="settings">シェイク設定</param>
    private void StartShake(ShakeSettings settings)
    {
        isShaking = true;
        OnShakeStart?.Invoke(); // シェイク開始イベント発火
        
        // 既存のTweenを停止
        currentTween?.Kill();
        
        // ランダム性に応じてシェイクベクトルを設定
        Vector3 shakeVector = settings.randomness ? 
            new Vector3(settings.strength, settings.strength, 0f) : 
            Vector3.one * settings.strength;

        Vector3 shakeRotation = new Vector3(0f, 0f, settings.strength * 20f); // Z回転用（強度は調整可）

        Sequence shakeSequence = DOTween.Sequence();

        shakeSequence.Join(targetCamera.transform.DOShakePosition(
            settings.duration,
            shakeVector,
            (int)settings.speed,
            90f,
            settings.randomness
        ));

        shakeSequence.Join(targetCamera.transform.DOShakeRotation(
            settings.duration,
            shakeRotation,
            (int)settings.speed,
            90f,
            settings.randomness
        ));

        shakeSequence.OnComplete(() => ReturnToOriginalPosition(settings.fadeOut));

        currentTween = shakeSequence;


        //↓OLD
        //// DOTweenを使用してカメラを揺らす
        //currentTween = targetCamera.transform.DOShakePosition(
        //    settings.duration, 
        //    shakeVector, 
        //    (int)settings.speed, 
        //    90f, 
        //    settings.randomness
        //).OnComplete(() => ReturnToOriginalPosition(settings.fadeOut));
    }

    /// <summary>
    /// カメラを元の位置に戻す
    /// </summary>
    /// <param name="fadeOutDuration">元の位置に戻る時間</param>
    private void ReturnToOriginalPosition(float fadeOutDuration)
    {
        currentTween = targetCamera.transform.DOMove(originalPosition, fadeOutDuration)
            .OnComplete(() => {
                isShaking = false;
                OnShakeEnd?.Invoke(); // シェイク終了イベント発火
            });
        //targetCamera.transform.position = originalPosition; // 元の位置に戻す
    }

    /// <summary>
    /// シェイクを強制停止する
    /// </summary>
    public void StopShake()
    {
        if (!isShaking) return;
        
        currentTween?.Kill();
        ReturnToOriginalPosition(0.1f);
    }

    /// <summary>
    /// 対象カメラを設定する
    /// </summary>
    /// <param name="camera">設定するカメラ</param>
    public void SetCamera(Camera camera)
    {
        targetCamera = camera;
        if (targetCamera != null)
        {
            originalPosition = targetCamera.transform.position;
        }
    }

    /// <summary>
    /// オブジェクト破棄時の処理
    /// </summary>
    void OnDestroy()
    {
        // 実行中のTweenを停止
        currentTween?.Kill();
    }
}