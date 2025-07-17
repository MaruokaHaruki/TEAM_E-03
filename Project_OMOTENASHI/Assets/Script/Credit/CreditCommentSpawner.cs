using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CreditCommentSpawner : MonoBehaviour {
    [Header("コメントのプレハブ")]
    public GameObject commentPrefab;

    [Header("コメントの候補リスト")]
    public List<string> commentList;

    [Header("出現範囲（X座標）")]
    public float spawnXMin = -5f;
    public float spawnXMax = 5f;
    public float spawnY = 6f;

    void Update() {
        // Cキーが押されたらコメントを1つ生成
        if (Input.GetKeyDown(KeyCode.C)) {
            SpawnComment();
        }
    }

    private void SpawnComment() {
        // ランダムなX座標を選ぶ
        float x = Random.Range(spawnXMin, spawnXMax);
        Vector2 spawnPos = new Vector2(x, spawnY);

        // プレハブを生成
        GameObject commentObj = Instantiate(commentPrefab, spawnPos, Quaternion.identity, transform);

        // テキストにランダムなコメントを設定
        TextMeshPro text = commentObj.GetComponent<TextMeshPro>();
        if (text != null && commentList.Count > 0) {
            text.text = commentList[Random.Range(0, commentList.Count)];
        }
    }
}
