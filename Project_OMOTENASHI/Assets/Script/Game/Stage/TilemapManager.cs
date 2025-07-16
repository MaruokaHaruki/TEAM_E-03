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
    [Tooltip("現在アクティブなタイルマップオブジェクト（ギミック含む）")]
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

        // 既存のタイルマップ（ギミック含む）を削除
        DestroyCurrentTilemap();

        // 新しいタイルマップ（ギミック含む）を生成
        CreateNewTilemap(roundSettings);

        Debug.Log($"[TILEMAP MANAGER] : ラウンド {roundSettings.roundNumber} のタイルマップ（ギミック含む）変更完了");
    }

    ///--------------------------------------------------------------
    ///						 現在のタイルマップを削除
    private void DestroyCurrentTilemap() {
        if (currentTilemapObject != null) {
            Debug.Log($"[TILEMAP MANAGER] : 既存のタイルマップ '{currentTilemapObject.name}' とその子オブジェクト（ギミック含む）を削除します");
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
        
        // タイルマップを生成（プレハブ内の子オブジェクトも自動的に生成される）
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

        // 子オブジェクトの情報をログ出力
        LogChildObjects(currentTilemapObject);

        Debug.Log($"[TILEMAP MANAGER] : 新しいタイルマップ '{currentTilemapObject.name}' を生成完了");
        Debug.Log($"[TILEMAP MANAGER] : 位置: {position}, 回転: {rotation.eulerAngles}");
    }

    ///--------------------------------------------------------------
    ///						 子オブジェクト情報をログ出力
    private void LogChildObjects(GameObject parent) {
        Transform[] children = parent.GetComponentsInChildren<Transform>();
        int childCount = children.Length - 1; // 親自身を除く

        if (childCount > 0) {
            Debug.Log($"[TILEMAP MANAGER] : タイルマップ内に {childCount} 個の子オブジェクト（ギミック含む）が検出されました：");
            
            foreach (Transform child in children) {
                if (child != parent.transform) {
                    Debug.Log($"[TILEMAP MANAGER] : - {child.name} (位置: {child.position})");
                }
            }
        }
        else {
            Debug.Log("[TILEMAP MANAGER] : タイルマップ内に子オブジェクトは見つかりませんでした");
        }
    }

    ///--------------------------------------------------------------
    ///						 タイルマップリセット
    public void ResetTilemap() {
        DestroyCurrentTilemap();
        Debug.Log("[TILEMAP MANAGER] : タイルマップ（ギミック含む）をリセットしました");
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

    ///--------------------------------------------------------------
    ///						 特定の子オブジェクトを検索
    public GameObject FindChildObject(string objectName) {
        if (currentTilemapObject == null) {
            return null;
        }

        Transform found = currentTilemapObject.transform.Find(objectName);
        return found != null ? found.gameObject : null;
    }

    ///--------------------------------------------------------------
    ///						 特定のコンポーネントを持つ子オブジェクトを検索
    public T FindChildComponent<T>(string objectName) where T : Component {
        GameObject childObject = FindChildObject(objectName);
        return childObject != null ? childObject.GetComponent<T>() : null;
    }

    ///--------------------------------------------------------------
    ///						 全ての子オブジェクトを取得
    public GameObject[] GetAllChildObjects() {
        if (currentTilemapObject == null) {
            return new GameObject[0];
        }

        Transform[] children = currentTilemapObject.GetComponentsInChildren<Transform>();
        List<GameObject> childObjects = new List<GameObject>();

        foreach (Transform child in children) {
            if (child != currentTilemapObject.transform) {
                childObjects.Add(child.gameObject);
            }
        }

        return childObjects.ToArray();
    }
}