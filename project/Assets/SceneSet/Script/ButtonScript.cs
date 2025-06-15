using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
// エディタ専用機能を使う場合
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ButtonScript : MonoBehaviour
{
    [HideInInspector]
    [SerializeField] private string sceneToLoad;


#if UNITY_EDITOR
    // インスペクタに表示するためのSceneAsset型変数
    [Header("遷移先シーン選択")] // インスペクタに見出しを表示
    [SerializeField] private SceneAsset sceneAsset; // ここにシーンファイルをD&Dする
#endif



    [SerializeField, Header("on_ObjList")]
    private GameObject[] onObjs;
    [SerializeField, Header("off_ObjList")]
    private GameObject[] offObjs;


    //別名（name）をキーとした管理用Dictionary
    private Dictionary<string, GameObject> onDictionary = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> offDictionary = new Dictionary<string, GameObject>();

    private void Start()
    {
        foreach (var onObj in onObjs)
        {
            onDictionary.Add(onObj.name, onObj);
        }
        foreach (var offObj in offObjs)
        {
            offDictionary.Add(offObj.name, offObj);
        }
    }

    public void OnClick()
    {
        if (onDictionary.Count > 0)
        {
            foreach (var onObj in onObjs)
            {
                onObj.gameObject.SetActive(true);
            }
        }

        if (offDictionary.Count > 0)
        {
            foreach (var offObj in offObjs)
            {
                offObj.gameObject.SetActive(false);
            }
        }

        // 遷移シーンが設定されていたらそのシーンに遷移する
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            

            Debug.Log("シーンを切り替え:"+ sceneToLoad);
            SceneManagerScript.Instance.FadeOutScene(sceneToLoad);
        }
        else
        {
            Debug.LogError("遷移先のシーン名が設定されてない");
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
