using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
// �G�f�B�^��p�@�\���g���ꍇ
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ButtonScript : MonoBehaviour
{
    [HideInInspector]
    [SerializeField] private string sceneToLoad;


#if UNITY_EDITOR
    // �C���X�y�N�^�ɕ\�����邽�߂�SceneAsset�^�ϐ�
    [Header("�J�ڐ�V�[���I��")] // �C���X�y�N�^�Ɍ��o����\��
    [SerializeField] private SceneAsset sceneAsset; // �����ɃV�[���t�@�C����D&D����
#endif



    [SerializeField, Header("on_ObjList")]
    private GameObject[] onObjs;
    [SerializeField, Header("off_ObjList")]
    private GameObject[] offObjs;


    //�ʖ��iname�j���L�[�Ƃ����Ǘ��pDictionary
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

        // �J�ڃV�[�����ݒ肳��Ă����炻�̃V�[���ɑJ�ڂ���
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            

            Debug.Log("�V�[����؂�ւ�:"+ sceneToLoad);
            SceneManagerScript.Instance.FadeOutScene(sceneToLoad);
        }
        else
        {
            Debug.LogError("�J�ڐ�̃V�[�������ݒ肳��ĂȂ�");
        }
    }


    // OnValidate���\�b�h���G�f�B�^��p
#if UNITY_EDITOR
    // �C���X�y�N�^�Œl���ύX���ꂽ���ȂǂɎ����ŌĂ΂�郁�\�b�h
    private void OnValidate()
    {
        // sceneAsset�t�B�[���h�ɃV�[�����ݒ肳�ꂽ��
        if (sceneAsset != null)
        {
            // ���̃V�[���̖��O�i������j�� sceneToLoad �ϐ��ɃR�s�[����
            sceneToLoad = sceneAsset.name;
        }
        else
        {
            // SceneAsset�����ݒ�Ȃ當�������ɂ���
            sceneToLoad = "";
        }
    }
#endif
}
