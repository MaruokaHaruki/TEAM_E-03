using UnityEngine;

public class SpeedGateController : MonoBehaviour
{
    // プレイヤーのRigitBody2Dを設定
    [SerializeField] private Rigidbody2D playerRigitbody_;
    // この速度以上でゲートを開く
    [SerializeField] private float speedThreshold_;
    // 開く対象のエリア
    [SerializeField] private GameObject gateObject_;

    [SerializeField] private GameObject GameManager_;

    // エリア
    private bool isGateOpen_ = false;

    // Update is called once per frame
    void Update()
    {
        // プレイヤーの速度を計算
        float currentSpeed = playerRigitbody_.linearVelocity.magnitude;

        // 一定以上の速度なら開く
        if (currentSpeed >= speedThreshold_ && !isGateOpen_) {
            OpenGate();
        }
        else {
            CloseGate();
        }
    }

    private void OpenGate() {
        isGateOpen_ = true;
        gateObject_.SetActive(true);
    }

    private void CloseGate() {
        isGateOpen_ = false;
        gateObject_.SetActive(true);

    }
}
