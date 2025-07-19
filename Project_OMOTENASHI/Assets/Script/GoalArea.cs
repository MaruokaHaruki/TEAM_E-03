using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalArea : MonoBehaviour {
    [Header("ゴール設定")]
    public string titleSceneName = "TitleScene";
    public float transitionDelay = 1.0f;

    private HashSet<GameObject> playersInArea = new HashSet<GameObject>();
    private bool isTransitioning = false;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            playersInArea.Add(other.gameObject);
            CheckGoalCondition();
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            playersInArea.Remove(other.gameObject);
        }
    }

    private void CheckGoalCondition() {
        // エリア内に2体以上のプレイヤーがいる場合
        if (playersInArea.Count >= 2 && !isTransitioning) {
            StartCoroutine(TransitionToTitle());
        }
    }

    private IEnumerator TransitionToTitle() {
        isTransitioning = true;

        // ゴール音を再生
        if (AudioManager.Instance != null) {
            AudioManager.Instance.PlaySE("Goal");
        }

        // ゴール演出があれば実行
        // 例：プレイヤーの動きを停止
        foreach (GameObject player in playersInArea) {
            Player playerScript = player.GetComponent<Player>();
            if (playerScript != null) {
                playerScript.allowMovement_ = false;
            }
        }

        // 指定した遅延時間を待つ
        yield return new WaitForSeconds(transitionDelay);

        // タイトルシーンに遷移
        if (SceneManagerScript.Instance != null) {
            SceneManagerScript.Instance.FadeOutScene(titleSceneName);
        }
        else {
            UnityEngine.SceneManagement.SceneManager.LoadScene(titleSceneName);
        }
    }

    private void OnDrawGizmosSelected() {
        // エディタでエリアを可視化
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}