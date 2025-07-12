using UnityEngine;

public class CameraTest : MonoBehaviour
{
    [SerializeField] private CameraController TestCamera;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            TestCamera.StartObservation(this.transform.position);
        }
        
        // 勝利演出テスト用
        if (Input.GetKeyDown(KeyCode.V))
        {
            Debug.Log("[CAMERA TEST] : 勝利演出テスト開始");
            TestCamera.StartVictoryZoom(this.transform);
        }
        
        // プレイヤー1での勝利演出テスト
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (GameManager.Instance != null && GameManager.Instance.player1_ != null)
            {
                Debug.Log("[CAMERA TEST] : Player1での勝利演出テスト開始");
                TestCamera.StartVictoryZoom(GameManager.Instance.player1_.transform);
            }
        }
        
        // プレイヤー2での勝利演出テスト
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (GameManager.Instance != null && GameManager.Instance.player2_ != null)
            {
                Debug.Log("[CAMERA TEST] : Player2での勝利演出テスト開始");
                TestCamera.StartVictoryZoom(GameManager.Instance.player2_.transform);
            }
        }
        
        // 強制的にビネットリセット（テスト用）
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("[CAMERA TEST] : ビネット強制リセット");
            TestCamera.StartObservation(this.transform.position);
        }
        
        // スムーズネステスト用（デバッグ情報出力）
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("[CAMERA TEST] : 現在のスムーズネス設定をログ出力");
            Debug.Log($"[CAMERA TEST] : 移動速度: {TestCamera.GetType().GetField("victoryMoveSpeed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(TestCamera)}");
            Debug.Log($"[CAMERA TEST] : ズーム速度: {TestCamera.GetType().GetField("victoryZoomSpeed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(TestCamera)}");
        }
    }
}
