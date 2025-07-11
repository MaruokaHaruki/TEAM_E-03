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
    [SerializeField] private float victoryZoomSize = 2.5f;
    [SerializeField] private float victoryZoomDuration = 3.0f;
    [SerializeField] private float victoryVignetteIntensity = 0.8f;
    [SerializeField] private float victoryMoveSpeed = 0.5f; // 勝利演出時の移動速度（より遅く）
    [SerializeField] private float victoryZoomSpeed = 0.1f; // 勝利演出時のズーム速度
    
    private bool isVictoryZoom = false;
    private float victoryZoomTimer = 0f;
    private Volume postProcessVolume;
    private Vignette vignette;
    private float originalVignetteIntensity = 0f;
    private float targetVignetteIntensity = 0f; // 目標ビネット強度
    private float currentVignetteIntensity = 0f; // 現在のビネット強度

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
            
            // ビネット効果を滑らかに適用
            UpdateVignetteSmooth();
            
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
        
        // ビネット効果を元に戻す
        targetVignetteIntensity = originalVignetteIntensity;
        
        Debug.Log($"[CAMERA] : 元の設定に戻しました - Pos: {StartPos}, Size: 5.0f");
    }

    // ビネット効果を滑らかに更新
    private void UpdateVignetteSmooth()
    {
        if (vignette != null)
        {
            // 滑らかにビネット強度を変更
            float previousIntensity = currentVignetteIntensity;
            currentVignetteIntensity = Mathf.Lerp(currentVignetteIntensity, targetVignetteIntensity, Time.deltaTime * 2.0f);
            vignette.intensity.value = currentVignetteIntensity;
            
            // 大きく変化した場合のみログ出力
            if (Mathf.Abs(currentVignetteIntensity - previousIntensity) > 0.01f)
            {
                Debug.Log($"[CAMERA] : ビネット強度更新: {currentVignetteIntensity:F3} (目標: {targetVignetteIntensity:F3})");
            }
        }
    }

    // ビネット効果を適用
    private void ApplyVictoryVignette()
    {
        if (vignette != null)
        {
            vignette.intensity.value = victoryVignetteIntensity;
            Debug.Log($"[CAMERA] : ビネット強度を{victoryVignetteIntensity}に設定");
        }
    }

    // ビネット効果をリセット
    private void ResetVignette()
    {
        if (vignette != null)
        {
            vignette.intensity.value = originalVignetteIntensity;
            Debug.Log($"[CAMERA] : ビネット強度を{originalVignetteIntensity}に戻しました");
        }
    }

    public void StartObservation(Vector3 targetPos)
    {
        TargetPos = targetPos + (Vector3.forward * this.transform.position.z);
        ObservationFlag = true;
        ObservationTime = SetTime;
        TargetCameraSize = 2.0f;
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

            // 勝利演出時はよりゆっくりとズーム
            float zoomSpeed = isVictoryZoom ? victoryZoomSpeed : 0.5f;
            SetDiff(ref diffSize, zoomSpeed);
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
                // 勝利演出時は固定の遅い速度を使用
                moveSpeed = victoryMoveSpeed;
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
