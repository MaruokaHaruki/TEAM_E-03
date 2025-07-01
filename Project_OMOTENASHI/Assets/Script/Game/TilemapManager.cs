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
        if (roundSettings == null || roundSettings.tilemapPrefab == null) {
            Debug.LogWarning("[TILEMAP MANAGER] : ラウンド設定またはタイルマッププレハブが設定されていません");
            return;
        }

        // 既存のタイルマップを削除
        DestroyCurrentTilemap();

        // 新しいタイルマップを生成
        CreateNewTilemap(roundSettings);

        Debug.Log($"[TILEMAP MANAGER] : ラウンド {roundSettings.roundNumber} のタイルマップに変更しました");
    }

    ///--------------------------------------------------------------
    ///						 現在のタイルマップを削除
    private void DestroyCurrentTilemap() {
        if (currentTilemapObject != null) {
            Debug.Log("[TILEMAP MANAGER] : 既存のタイルマップを削除します");
            Destroy(currentTilemapObject);
            currentTilemapObject = null;
        }
    }

    ///--------------------------------------------------------------
    ///						 新しいタイルマップを生成
    private void CreateNewTilemap(RoundSettings roundSettings) {
        // タイルマップを生成
        Vector3 position = roundSettings.tilemapPosition;
        Quaternion rotation = Quaternion.Euler(roundSettings.tilemapRotation);

        currentTilemapObject = Instantiate(roundSettings.tilemapPrefab, position, rotation);

        // 親オブジェクトが設定されている場合は子に設定
        if (tilemapParent != null) {
            currentTilemapObject.transform.SetParent(tilemapParent);
        }

        // タイルマップの名前を設定
        currentTilemapObject.name = $"Tilemap_Round_{roundSettings.roundNumber}";

        Debug.Log($"[TILEMAP MANAGER] : 新しいタイルマップ '{currentTilemapObject.name}' を生成しました");
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