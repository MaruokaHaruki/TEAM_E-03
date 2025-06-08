using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    // 継承先で「破棄はできないか？」を指定
    protected bool dontDestroyOnLoad = true;

    // 継承したクラスの実体
    private static T instance;

    public static T Instance
    {
        get
        {
            // インスタンスがまだ無い場合
            if (!instance)
            {
                Type t = typeof(T);

                // 継承先のスクリプトをアタッチしているオブジェクトを検索
               // instance = (T)FindObjectOfType(t);
                instance = (T)FindAnyObjectByType(t);

                // 見つからなかった場合
                if (!instance)
                {
                    Debug.LogError(t + " がアタッチされているオブジェクトがありません");
                }
            }

            return instance;
        }
    }

    protected virtual void Awake()
    {
        // インスタンスが複数存在する場合は自身を破棄
        if (this != Instance)
        {
            Destroy(this.gameObject);
            return;
        }

        // 継承先で破棄不可能が指定された場合はシーン遷移時も破棄しない
        if (dontDestroyOnLoad)
        {
            transform.parent = null;

            DontDestroyOnLoad(this.gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        // 破棄された場合は実体の削除を行う
        if (this == Instance)
        {
            instance = null;
        }
    }
}