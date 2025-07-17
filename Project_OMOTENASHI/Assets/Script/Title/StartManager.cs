using UnityEditor;
using UnityEngine;

public class StartManager : MonoBehaviour
{
    [HideInInspector]
    [SerializeField] private string sceneToLoad;


#if UNITY_EDITOR
    // インスペクタに表示するためのSceneAsset型変数
    [Header("遷移先シーン選択")] // インスペクタに見出しを表示
    [SerializeField] private SceneAsset sceneAsset; // ここにシーンファイルをD&Dする
#endif
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {// 遷移シーンが設定されていたらそのシーンに遷移する
            if (!string.IsNullOrEmpty(sceneToLoad))
            {


                Debug.Log("シーンを切り替え:" + sceneToLoad);
                SceneManagerScript.Instance.FadeOutScene(sceneToLoad);
            }
            else
            {
                Debug.LogError("遷移先のシーン名が設定されてない");
            }
        }
    }

    // OnValidateメソッドもエディタ専用
#if UNITY_EDITOR
    // インスペクタで値が変更された時などに自動で呼ばれるメソッド
    private void OnValidate()
    {
        // sceneAssetフィールドにシーンが設定されたら
        if (sceneAsset != null)
        {
            // そのシーンの名前（文字列）を sceneToLoad 変数にコピーする
            sceneToLoad = sceneAsset.name;
        }
        else
        {
            // SceneAssetが未設定なら文字列も空にする
            sceneToLoad = "";
        }
    }
#endif

}
