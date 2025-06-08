using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;

    private bool gameEnded_ = false;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void GameOver(GameObject winner) {
        if (gameEnded_) return;

        gameEnded_ = true;
        Debug.Log("勝者: " + winner.name);
        // TODO: リザルト画面に遷移など
    }
}
