using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    // �p����Łu�j���͂ł��Ȃ����H�v��w��
    protected bool dontDestroyOnLoad = true;

    // �p�������N���X�̎���
    private static T instance;

    public static T Instance
    {
        get
        {
            // �C���X�^���X���܂������ꍇ
            if (!instance)
            {
                Type t = typeof(T);

                // �p����̃X�N���v�g��A�^�b�`���Ă���I�u�W�F�N�g�����
               // instance = (T)FindObjectOfType(t);
                instance = (T)FindAnyObjectByType(t);

                // ������Ȃ������ꍇ
                if (!instance)
                {
                    Debug.LogError(t + " ���A�^�b�`����Ă���I�u�W�F�N�g������܂���");
                }
            }

            return instance;
        }
    }

    protected virtual void Awake()
    {
        // �C���X�^���X���������݂���ꍇ�͎��g��j��
        if (this != Instance)
        {
            Destroy(this.gameObject);
            return;
        }

        // �p����Ŕj���s�\���w�肳�ꂽ�ꍇ�̓V�[���J�ڎ���j�����Ȃ�
        if (dontDestroyOnLoad)
        {
            transform.parent = null;

            DontDestroyOnLoad(this.gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        // �j�����ꂽ�ꍇ�͎��̂̍폜��s��
        if (this == Instance)
        {
            instance = null;
        }
    }
}