using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraController : MonoBehaviour
{
    /// <summary>中央ポジション</summary>
    [SerializeField] private RectTransform MiddlePos;

    /// <summary>設定時間</summary>
    [SerializeField] private float SetTime = 2.0f;
    
    /// <summary>観察フラグ</summary>
    [SerializeField] private bool ObservationFlag;

    /// <summary>観察時間</summary>
    [SerializeField] private float ObservationTime;

    /// <summary>最初のポジション</summary>
    [SerializeField] private Vector3 StartPos;

    /// <summary>ターゲットポジション</summary>
    [SerializeField] private Vector3 TargetPos;

    /// <summary>ターゲットカメラサイズ</summary>
    [SerializeField] private float TargetCameraSize;

    /// <summary>カメラ</summary>
    [SerializeField] private Camera MainCamera;

    /// <summary>全UI</summary>
    [SerializeField] private RectTransform[] AllUiTransform;

    private Vector3[] AllUiStartPos;
    private Vector3[] AllUiStartSize;

    /// <summary>移動割合</summary>
    [SerializeField] private float MoveRatio = 10.0f;

    // 勝利演出用の変数
    [Header("勝利演出設定")]
    [SerializeField, Tooltip("勝利演出時のカメラズームサイズ（小さいほど拡大）")]
    private float victoryZoomSize = 2.5f;
    
    [SerializeField, Tooltip("勝利演出の継続時間（秒）")]
    private float victoryZoomDuration = 3.0f;
    
    [SerializeField, Tooltip("勝利演出時のビネット強度（0-1、大きいほど画面端が暗くなる）")]
    private float victoryVignetteIntensity = 0.8f;
    
    [Header("勝利演出スムーズネス設定")]
    [SerializeField, Tooltip("勝利演出時のカメラ移動速度（0.1-2.0、小さいほどゆっくり移動）")]
    [Range(0.1f, 2.0f)]
    private float victoryMoveSpeed = 0.5f;
    
    [SerializeField, Tooltip("勝利演出時のズーム速度（0.05-1.0、小さいほどゆっくりズーム）")]
    [Range(0.05f, 1.0f)]
    private float victoryZoomSpeed = 0.1f;
    
    [SerializeField, Tooltip("勝利演出時のカメラ移動スムーズネス（1.0-10.0、大きいほど滑らか）")]
    [Range(1.0f, 10.0f)]
    private float victoryMoveSmoothness = 5.0f;
    
    [SerializeField, Tooltip("勝利演出時のズームスムーズネス（1.0-10.0、大きいほど滑らか）")]
    [Range(1.0f, 10.0f)]
    private float victoryZoomSmoothness = 3.0f;
    
    [Header("ビネット効果設定")]
    [SerializeField, Tooltip("ビネット効果が強くなる速度（勝利演出開始時）")]
    [Range(0.5f, 10.0f)]
    private float vignetteIntensifySpeed = 3.0f;
    
    [SerializeField, Tooltip("ビネット効果が戻る速度（勝利演出終了時）")]
    [Range(0.5f, 10.0f)]
    private float vignetteReturnSpeed = 2.0f;
    
    [SerializeField, Tooltip("ビネット効果のスムーズネス（1.0-10.0、大きいほど滑らか）")]
    [Range(0.0f, 10.0f)]
    private float vignetteSmoothness = 5.0f;
    
    [SerializeField, Tooltip("ビネット効果をスムージングするか（チェックを外すと即座に変化）")]
    private bool enableVignetteSmoothing = true;
    
    [SerializeField, Tooltip("勝利演出のEasing Type")]
    private EasingType victoryEasingType = EasingType.EaseInOutQuad;
    
    // イージングタイプの列挙型
    public enum EasingType
    {
        Linear,
        EaseInQuad,
        EaseOutQuad,
        EaseInOutQuad,
        EaseInCubic,
        EaseOutCubic,
        EaseInOutCubic
    }

    private bool isVictoryZoom = false;
    private float victoryZoomTimer = 0f;
    private Volume postProcessVolume;
    private Vignette vignette;
    private float originalVignetteIntensity = 0f;
    private float targetVignetteIntensity = 0f;
    private float currentVignetteIntensity = 0f;
    private bool shouldReturnVignetteImmediately = false; // 即座に戻すフラグ

    void Start()
    {
        TargetPos = StartPos = this.transform.position;

        ObservationFlag = false;
        ObservationTime = 0.0f;

        if (MainCamera == null)
        {
            MainCamera = this.gameObject.GetComponent<Camera>();
        }

        TargetCameraSize = 5.0f;

        AllUiStartPos = new Vector3[AllUiTransform.Length];
        AllUiStartSize= new Vector3[AllUiTransform.Length];
        for (int i = 0; i < AllUiTransform.Length; i++)
        {
            AllUiStartPos[i] = AllUiTransform[i].position;
            AllUiStartSize[i] = AllUiTransform[i].localScale;
        }

        // Post Process Volumeを取得
        InitializePostProcessing();
    }

    void Update()
    {
        if (ObservationFlag)
        {
            ObservationTime -= Time.deltaTime;
            if (ObservationTime <= 0.0f)
            {
                ObservationFlag = false;
                TargetPos = StartPos;
                TargetCameraSize = 5.0f;
            }
        }

        // 勝利演出の処理
        if (isVictoryZoom)
        {
            victoryZoomTimer -= Time.deltaTime;
            
            // デバッグ情報を定期的に出力
            if (Time.frameCount % 30 == 0) // 30フレームごと
            {
                Debug.Log($"[CAMERA] : 勝利演出中 - Timer: {victoryZoomTimer:F2}, CameraSize: {MainCamera.orthographicSize:F2}, Target: {TargetCameraSize:F2}");
            }
            
            if (victoryZoomTimer <= 0f)
            {
                EndVictoryZoom();
            }
        }
        
        // ビネット効果を常に更新
        UpdateVignetteEffect();
    }

    private void FixedUpdate()
    {
        CameraMoveProcess();
    }

    // Post Processing初期化
    private void InitializePostProcessing()
    {
        // シーン内のVolumeを検索
        postProcessVolume = FindObjectOfType<Volume>();
        
        if (postProcessVolume != null)
        {
            Debug.Log($"[CAMERA] : Post Process Volume発見: {postProcessVolume.name}");
            
            if (postProcessVolume.profile != null)
            {
                Debug.Log($"[CAMERA] : Profile発見: {postProcessVolume.profile.name}");
                
                if (postProcessVolume.profile.TryGet<Vignette>(out vignette))
                {
                    originalVignetteIntensity = vignette.intensity.value;
                    currentVignetteIntensity = originalVignetteIntensity;
                    Debug.Log($"[CAMERA] : Vignette発見 - 初期強度: {originalVignetteIntensity}");
                }
                else
                {
                    Debug.LogWarning("[CAMERA] : ProfileにVignetteが見つかりません");
                }
            }
            else
            {
                Debug.LogWarning("[CAMERA] : Post Process VolumeにProfileが設定されていません");
            }
        }
        else
        {
            Debug.LogWarning("[CAMERA] : Post Process Volumeが見つかりません");
        }
    }

    // 勝利演出開始
    public void StartVictoryZoom(Transform winnerTransform)
    {
        if (winnerTransform == null) 
        {
            Debug.LogError("[CAMERA] : winnerTransformがnullです");
            return;
        }

        Debug.Log($"[CAMERA] : 勝利演出開始 - {winnerTransform.name}");
        Debug.Log($"[CAMERA] : 勝者位置: {winnerTransform.position}");
        Debug.Log($"[CAMERA] : 現在カメラ位置: {transform.position}");
        Debug.Log($"[CAMERA] : 現在カメラサイズ: {MainCamera.orthographicSize}");
        
        // 勝者の位置にカメラを向ける
        Vector3 winnerPosition = winnerTransform.position;
        winnerPosition.z = transform.position.z; // Z座標は維持
        
        TargetPos = winnerPosition;
        TargetCameraSize = victoryZoomSize;
        
        Debug.Log($"[CAMERA] : 目標位置: {TargetPos}");
        Debug.Log($"[CAMERA] : 目標サイズ: {TargetCameraSize}");
        
        // 勝利演出フラグを設定
        isVictoryZoom = true;
        victoryZoomTimer = victoryZoomDuration;
        
        // ビネット効果を滑らかに適用開始
        targetVignetteIntensity = victoryVignetteIntensity;
        shouldReturnVignetteImmediately = false;
        
        Debug.Log($"[CAMERA] : 勝利演出設定完了 - Duration: {victoryZoomDuration}, VignetteTarget: {targetVignetteIntensity}");
    }

    // 勝利演出終了
    private void EndVictoryZoom()
    {
        Debug.Log("[CAMERA] : 勝利演出終了");
        
        isVictoryZoom = false;
        
        // カメラを元の位置とサイズに戻す
        TargetPos = StartPos;
        TargetCameraSize = 5.0f;
        
        // ビネット効果を即座に元に戻すフラグを設定
        targetVignetteIntensity = originalVignetteIntensity;
        shouldReturnVignetteImmediately = true;
        
        Debug.Log($"[CAMERA] : 元の設定に戻しました - Pos: {StartPos}, Size: 5.0f, VignetteTarget: {targetVignetteIntensity}");
        Debug.Log($"[CAMERA] : 現在のビネット強度: {currentVignetteIntensity:F3}, 目標: {targetVignetteIntensity:F3}");
    }

    // ビネット効果を更新（名前変更とロジック改善）
    private void UpdateVignetteEffect()
    {
        if (vignette == null) 
        {
            // ビネットが見つからない場合の警告（初回のみ）
            if (Time.frameCount % 300 == 0) // 5秒に1回
            {
                Debug.LogWarning("[CAMERA] : Vignetteが見つかりません。ポストプロセス設定を確認してください。");
            }
            return;
        }

        // 目標値と現在値が異なる場合のみ更新
        if (Mathf.Abs(currentVignetteIntensity - targetVignetteIntensity) > 0.001f)
        {
            float previousIntensity = currentVignetteIntensity;
            
            // スムージングが無効な場合は即座に変更
            if (!enableVignetteSmoothing)
            {
                currentVignetteIntensity = targetVignetteIntensity;
                vignette.intensity.value = currentVignetteIntensity;
                Debug.Log($"[CAMERA] : ビネット強度即座に変更: {currentVignetteIntensity:F3}");
                return;
            }
            
            // 即座に戻すフラグが立っている場合は高速で戻す
            if (shouldReturnVignetteImmediately)
            {
                float fastReturnSpeed = vignetteReturnSpeed * vignetteSmoothness;
                currentVignetteIntensity = Mathf.Lerp(currentVignetteIntensity, targetVignetteIntensity, Time.deltaTime * fastReturnSpeed);
                
                // 目標値に十分近づいたら即座に戻すフラグを解除
                if (Mathf.Abs(currentVignetteIntensity - targetVignetteIntensity) < 0.05f)
                {
                    currentVignetteIntensity = targetVignetteIntensity;
                    shouldReturnVignetteImmediately = false;
                    Debug.Log("[CAMERA] : ビネット即座戻し完了");
                }
            }
            else
            {
                // 通常のスムージング処理（スムーズネスを適用）
                float changeSpeed = isVictoryZoom ? 
                    vignetteIntensifySpeed * vignetteSmoothness : 
                    vignetteReturnSpeed * vignetteSmoothness;
                currentVignetteIntensity = Mathf.Lerp(currentVignetteIntensity, targetVignetteIntensity, Time.deltaTime * changeSpeed);
            }
            
            vignette.intensity.value = currentVignetteIntensity;
            
            // 変化をログ出力
            if (Mathf.Abs(currentVignetteIntensity - previousIntensity) > 0.005f)
            {
                string mode = shouldReturnVignetteImmediately ? "即座戻し中" : 
                             isVictoryZoom ? "勝利演出中" : "通常戻り中";
               // Debug.Log($"[CAMERA] : ビネット強度更新({mode}): {currentVignetteIntensity:F3} → 目標: {targetVignetteIntensity:F3}");
            }
        }
        else
        {
            // 目標値に十分近づいた場合、完全に同じ値にする
            if (Mathf.Abs(currentVignetteIntensity - targetVignetteIntensity) > 0.0001f)
            {
                currentVignetteIntensity = targetVignetteIntensity;
                vignette.intensity.value = currentVignetteIntensity;
                shouldReturnVignetteImmediately = false;
               // Debug.Log($"[CAMERA] : ビネット強度調整完了: {currentVignetteIntensity:F3}");
            }
        }
    }

    public void StartObservation(Vector3 targetPos)
    {
        TargetPos = targetPos + (Vector3.forward * this.transform.position.z);
        ObservationFlag = true;
        ObservationTime = SetTime;
        TargetCameraSize = 2.0f;
        
        // 観察開始時に勝利演出をリセット
        if (isVictoryZoom)
        {
            isVictoryZoom = false;
            targetVignetteIntensity = originalVignetteIntensity;
            shouldReturnVignetteImmediately = true; // 即座に戻すフラグを設定
            Debug.Log("[CAMERA] : 観察開始により勝利演出をリセット、ビネットを即座に戻します");
        }
    }

    private void CameraMoveProcess()
    {
        Vector3 uiSetScale;
        bool SetScaleFlag = false;

        Vector3 cameraUiPosition;
        Vector3 uiSetPos;

        if (MainCamera.orthographicSize != TargetCameraSize)
        {
            SetScaleFlag = true;
            float diffSize = TargetCameraSize - MainCamera.orthographicSize;

            // 勝利演出時はスムーズネスを適用したズーム
            float zoomSpeed = isVictoryZoom ? 
                victoryZoomSpeed * victoryZoomSmoothness : 
                0.5f;
            
            // イージングを適用
            if (isVictoryZoom)
            {
                float t = 1.0f - (victoryZoomTimer / victoryZoomDuration);
                t = ApplyEasing(t, victoryEasingType);
                float easedSpeed = zoomSpeed * (1.0f + t * 2.0f); // Easing効果を速度に反映
                SetDiff(ref diffSize, easedSpeed);
            }
            else
            {
                SetDiff(ref diffSize, zoomSpeed);
            }
            
            MainCamera.orthographicSize = TargetCameraSize - diffSize;
            
            // 勝利演出中のズーム進行をログ出力
            if (isVictoryZoom && Time.frameCount % 30 == 0)
            {
                Debug.Log($"[CAMERA] : ズーム進行中 - Current: {MainCamera.orthographicSize:F2}, Target: {TargetCameraSize:F2}, Diff: {diffSize:F2}");
            }
        }
        uiSetScale = (Vector3.one * (1.0f - (MainCamera.orthographicSize / 5.0f))) * 1.5f;
        if (SetScaleFlag)
        {
            for (int i = 0; i < AllUiTransform.Length; i++)
            {
                AllUiTransform[i].localScale = new Vector3(AllUiStartSize[i].x + uiSetScale.x, AllUiStartSize[i].y + uiSetScale.y, AllUiStartSize[i].z + uiSetScale.z);
            }
        }

        if (TargetPos != this.transform.position)
        {
            Vector3 diffPos = TargetPos - this.transform.position;

            float moveSpeed;
            
            if (isVictoryZoom)
            {
                // 勝利演出時はスムーズネスを適用した移動速度
                moveSpeed = victoryMoveSpeed * victoryMoveSmoothness;
                
                // イージングを適用
                float t = 1.0f - (victoryZoomTimer / victoryZoomDuration);
                t = ApplyEasing(t, victoryEasingType);
                moveSpeed *= (1.0f + t); // Easing効果を速度に反映
            }
            else
            {
                // 通常時の移動速度計算
                moveSpeed = (Mathf.Abs(TargetPos.x - StartPos.x) + Mathf.Abs(TargetPos.y - StartPos.y)) / MoveRatio;
                if (moveSpeed == 0.0f)
                {
                    moveSpeed = 1.0f;
                }
            }

            float moveDenominator = Mathf.Abs(diffPos.x) + Mathf.Abs(diffPos.y);
            if (moveDenominator == 0.0f)
            {
                moveDenominator = 0.1f;
            }
            SetDiff(ref diffPos.x, moveSpeed * (Mathf.Abs(diffPos.x) / moveDenominator));
            SetDiff(ref diffPos.y, moveSpeed * (Mathf.Abs(diffPos.y) / moveDenominator));
            SetDiff(ref diffPos.z, 0.5f);

            this.transform.position = TargetPos - diffPos;

            // 勝利演出中の移動進行をログ出力
            if (isVictoryZoom && Time.frameCount % 30 == 0)
            {
                Debug.Log($"[CAMERA] : 移動進行中 - Current: {transform.position}, Target: {TargetPos}, Diff: {diffPos}, Speed: {moveSpeed}");
            }

            cameraUiPosition = (this.transform.position - StartPos) * 100.0f;

            for (int i = 0; i < AllUiTransform.Length; i++)
            {
                uiSetPos = (AllUiStartPos[i] - MiddlePos.position) - cameraUiPosition;
                uiSetPos.x *= (uiSetScale.x + 1.0f);
                uiSetPos.y *= (uiSetScale.y + 1.0f);
                AllUiTransform[i].position = new Vector3(MiddlePos.position.x + uiSetPos.x, MiddlePos.position.y + uiSetPos.y, AllUiStartPos[i].z);
            }
        }
    }

    // イージング関数
    private float ApplyEasing(float t, EasingType easingType)
    {
        switch (easingType)
        {
            case EasingType.Linear:
                return t;
            case EasingType.EaseInQuad:
                return t * t;
            case EasingType.EaseOutQuad:
                return 1f - (1f - t) * (1f - t);
            case EasingType.EaseInOutQuad:
                return t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;
            case EasingType.EaseInCubic:
                return t * t * t;
            case EasingType.EaseOutCubic:
                return 1f - Mathf.Pow(1f - t, 3f);
            case EasingType.EaseInOutCubic:
                return t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
            default:
                return t;
        }
    }

    private void SetDiff(ref float diff, float speed)
    {
        if (diff > 0.0f)
        {
            diff -= speed;
            if (diff < 0.0f)
            {
                diff = 0.0f;
            }
        }
        else
        {
            diff += speed;
            if (diff > 0.0f)
            {
                diff = 0.0f;
            }
        }
    }
}
