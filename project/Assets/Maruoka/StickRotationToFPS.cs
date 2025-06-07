using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// 左スティックの回転量から仮想FPSを計算し、UIに表示する
/// </summary>
public class StickRotationToFPS : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI fpsText_;     // テキストUI（仮想FPS表示用）
    [SerializeField] private Slider fpsSlider_;            // スライダーUI（視覚表示用）
    [SerializeField] private float minFps_ = 12f;          // 最小FPS
    [SerializeField] private float maxFps_ = 60f;          // 最大FPS
    [SerializeField] private float rotationToFpsFactor_ = 0.8f; // 回転角→FPS変換倍率
    [SerializeField, Range(0f, 1f)] private float decayRate_ = 0.9f; // 減衰率（1で減衰なし、0で即ゼロ）

    private PlayerInputActions inputActions_;
    private Vector2 lastInput_;          // 前フレームのスティック入力
    private float accumulatedAngle_;     // 累積回転角（速度の代わり）

    void Awake() {
        inputActions_ = new PlayerInputActions();
        inputActions_.Gameplay.Enable();
    }

    void Update() {
        Vector2 stickInput = inputActions_.Gameplay.Move.ReadValue<Vector2>();

        // スティックの回転を計算（2つのベクトルの角度差）
        if (stickInput.magnitude > 0.3f && lastInput_.magnitude > 0.3f) {
            float angle = Vector2.SignedAngle(lastInput_, stickInput);
            accumulatedAngle_ += Mathf.Abs(angle);  // 絶対値で積算
        }

        lastInput_ = stickInput;

        // 回転量に応じたFPSの変化
        float virtualFps = Mathf.Clamp(minFps_ + accumulatedAngle_ * rotationToFpsFactor_, minFps_, maxFps_);

        // テキストとスライダーに反映
        if (fpsText_ != null)
            fpsText_.text = $"ChrFPS: {virtualFps:F1}";

        if (fpsSlider_ != null)
            fpsSlider_.value = (virtualFps - minFps_) / (maxFps_ - minFps_);

        // 回転角度を減衰（徐々に戻る）
        accumulatedAngle_ *= decayRate_;
    }
}
