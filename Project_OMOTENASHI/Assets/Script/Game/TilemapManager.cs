using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TilemapManager : MonoBehaviour {
    ///--------------------------------------------------------------
    ///						 シングルトン
    public static TilemapManager Instance { get; private set; }

    ///--------------------------------------------------------------
    ///						 タイルマップ管理
    [Header("タイルマップ管理")]
    [Tooltip("現在アクティブなタイルマップオブジェクト")]
    public GameObject currentTilemapObject;

    [Tooltip("タイルマップの親オブジェクト")]
    public Transform tilemapParent;

    ///--------------------------------------------------------------
    ///						 初期化
    private void Awake() {
        // シングルトンの設定
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    ///--------------------------------------------------------------
    ///						 タイルマップ変更
    public void ChangeTilemap(RoundSettings roundSettings) {
        Debug.Log($"[TILEMAP MANAGER] : タイルマップ変更処理開始 - ラウンド{roundSettings?.roundNumber}");
        
        if (roundSettings == null) {
            Debug.LogError("[TILEMAP MANAGER] : roundSettingsがnullです");
            return;
        }
        
        if (roundSettings.tilemapPrefab == null) {
            Debug.LogError($"[TILEMAP MANAGER] : ラウンド{roundSettings.roundNumber}のtilemapPrefabがnullです");
            return;
        }

        // 既存のタイルマップを削除
        DestroyCurrentTilemap();

        // 新しいタイルマップを生成
        CreateNewTilemap(roundSettings);

        Debug.Log($"[TILEMAP MANAGER] : ラウンド {roundSettings.roundNumber} のタイルマップに変更完了");
    }

    ///--------------------------------------------------------------
    ///						 現在のタイルマップを削除
    private void DestroyCurrentTilemap() {
        if (currentTilemapObject != null) {
            Debug.Log($"[TILEMAP MANAGER] : 既存のタイルマップ '{currentTilemapObject.name}' を削除します");
            Destroy(currentTilemapObject);
            currentTilemapObject = null;
        }
        else {
            Debug.Log("[TILEMAP MANAGER] : 削除する既存タイルマップはありません");
        }
    }

    ///--------------------------------------------------------------
    ///						 新しいタイルマップを生成
    private void CreateNewTilemap(RoundSettings roundSettings) {
        Debug.Log($"[TILEMAP MANAGER] : 新しいタイルマップ生成開始 - {roundSettings.tilemapPrefab.name}");
        
        // タイルマップを生成
        Vector3 position = roundSettings.tilemapPosition;
        Quaternion rotation = Quaternion.Euler(roundSettings.tilemapRotation);

        currentTilemapObject = Instantiate(roundSettings.tilemapPrefab, position, rotation);

        if (currentTilemapObject == null) {
            Debug.LogError("[TILEMAP MANAGER] : タイルマップの生成に失敗しました");
            return;
        }

        // 親オブジェクトが設定されている場合は子に設定
        if (tilemapParent != null) {
            currentTilemapObject.transform.SetParent(tilemapParent);
            Debug.Log($"[TILEMAP MANAGER] : タイルマップを親オブジェクト '{tilemapParent.name}' の子に設定");
        }

        // タイルマップの名前を設定
        currentTilemapObject.name = $"Tilemap_Round_{roundSettings.roundNumber}";

        Debug.Log($"[TILEMAP MANAGER] : 新しいタイルマップ '{currentTilemapObject.name}' を生成完了");
        Debug.Log($"[TILEMAP MANAGER] : 位置: {position}, 回転: {rotation.eulerAngles}");
    }

    ///--------------------------------------------------------------
    ///						 タイルマップリセット
    public void ResetTilemap() {
        DestroyCurrentTilemap();
        Debug.Log("[TILEMAP MANAGER] : タイルマップをリセットしました");
    }

    ///--------------------------------------------------------------
    ///						 現在のタイルマップ取得
    public GameObject GetCurrentTilemap() {
        return currentTilemapObject;
    }

    ///--------------------------------------------------------------
    ///						 タイルマップ存在確認
    public bool HasActiveTilemap() {
        return currentTilemapObject != null;
    }
}