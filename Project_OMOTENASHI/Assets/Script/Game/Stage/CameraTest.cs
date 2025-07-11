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
    }
}
