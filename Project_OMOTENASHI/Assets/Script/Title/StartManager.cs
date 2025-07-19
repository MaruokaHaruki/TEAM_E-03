using UnityEditor;
using UnityEngine;
using TMPro;
using DG.Tweening; // DOTween��g�p���邽�߂̖��O���

public class StartManager : MonoBehaviour
{
    [HideInInspector]
    [SerializeField] private string sceneToLoad;

    [Header("������ԕ��UI")]
    [SerializeField] private TextMeshProUGUI player1ReadyText;
    [SerializeField] private TextMeshProUGUI player2ReadyText;

    [Header("Canvas Control")]
    [SerializeField] private GameObject playerReadyCanvas;
    [SerializeField] private float canvasDisplayDelay = 1f;
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private float autoHideDelay = 5f; // �����쎞�̎�����\������

    private bool player1Ready = false;
    private bool player2Ready = false;
    private bool titleLogoCompleted = false;
    private bool canvasDisplayed = false;
    private float lastInputTime = 0f;
    private Tween currentFadeTween;


#if UNITY_EDITOR
    // �C���X�y�N�^�[�ɕ\�����邽�߂�SceneAsset�^�ϐ�
    [Header("�J�ڐ�V�[���I��")] // �C���X�y�N�^�[�Ɍ��o����\��
    [SerializeField] private SceneAsset sceneAsset; // �����ɃV�[���t�@�C����h���b�O&�h���b�v����
#endif



    private void Awake()
    {
        Application.targetFrameRate = 60;
        UpdatePlayerReadyDisplay();
    }

    private void Start()
    {
        // �^�C�g�����S�����C�x���g��w��
        TitleLogController.OnTitleLogoComplete += OnTitleLogoCompleted;

        // Canvas������Ԃ�ݒ�
        if (playerReadyCanvas != null)
        {
            // CanvasGroup��擾�܂��͒ǉ�
            var canvasGroup = playerReadyCanvas.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = playerReadyCanvas.AddComponent<CanvasGroup>();
            }

            // ������Ԑݒ�
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            // �X�P�[���������
            playerReadyCanvas.transform.localScale = Vector3.zero;

            playerReadyCanvas.SetActive(true); // �A�N�e�B�u�ɂ��Ă����ē������
        }
    }

    private void OnDestroy()
    {
        // �C�x���g�w�ǉ��
        TitleLogController.OnTitleLogoComplete -= OnTitleLogoCompleted;
    }

    private void OnTitleLogoCompleted()
    {
        titleLogoCompleted = true;
        Debug.Log("�^�C�g�����S���� - �����L�[������Ă�������");
    }

    private void Update()
    {
        // �^�C�g�����S���������Ă��Ȃ��ꍇ�͉�����Ȃ�
        if (!titleLogoCompleted) return;

        // �L�[���̓`�F�b�N
        if (Input.anyKeyDown)
        {
            lastInputTime = Time.time;

            if (!canvasDisplayed)
            {
                ShowCanvas();
            }
        }

        // Canvas�\�����̏���
        if (canvasDisplayed)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                sceneToLoad = "GameScene_Rantou";
                SceneManagerScript.Instance.FadeOutScene(sceneToLoad);
            }
            else if (Input.GetKeyDown(KeyCode.I))
            {
                sceneToLoad = "GameScene_Syuuten";
                SceneManagerScript.Instance.FadeOutScene(sceneToLoad);
            }

            // �����쎞�ԃ`�F�b�N�Ńt�F�[�h�A�E�g
            if (Time.time - lastInputTime > autoHideDelay)
            {
                HideCanvas();
                return;
            }

            // �v���C���[���͏���
            HandlePlayerInput();
        }
    }

    private void HandlePlayerInput()
    {
        // �v���C���[1�̏�����Ԃ�`�F�b�N�iA,W,D���������j
        bool player1Input = Input.GetKey(KeyCode.W);

        // �v���C���[2�̏�����Ԃ�`�F�b�N�iJ,I,L���������j
        bool player2Input = Input.GetKey(KeyCode.I);

        // ������Ԃ�X�V
        player1Ready = player1Input;
        player2Ready = player2Input;

        // UI�\����X�V
        UpdatePlayerReadyDisplay();

        // ���v���C���[�������������Ă���ꍇ�̂݃V�[����؂�ւ���
        if (player1Ready && player2Ready)
        {
            // �J�ڃV�[�����ݒ肳��Ă����炻�̃V�[���ɑJ�ڂ���
            if (!string.IsNullOrEmpty(sceneToLoad))
            {
                Debug.Log("�V�[����؂�ւ�:" + sceneToLoad);
                SceneManagerScript.Instance.FadeOutScene(sceneToLoad);
            }
            else
            {
                Debug.LogError("�J�ڐ�̃V�[�����ݒ肳��Ă��Ȃ�");
            }
        }
    }

    private void ShowCanvas()
    {
        if (canvasDisplayed) return;

        canvasDisplayed = true;

        if (playerReadyCanvas != null)
        {
            var canvasGroup = playerReadyCanvas.GetComponent<CanvasGroup>();

            if (canvasGroup != null)
            {
                // �����̃A�j���[�V�������~
                currentFadeTween?.Kill();

                // �C���^���N�V������L����
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;

                // �t�F�[�h�C���A�j���[�V����
                currentFadeTween = canvasGroup.DOFade(1f, fadeInDuration).SetEase(Ease.OutQuart);
            }
        }
    }

    private void HideCanvas()
    {
        if (!canvasDisplayed) return;

        if (playerReadyCanvas != null)
        {
            var canvasGroup = playerReadyCanvas.GetComponent<CanvasGroup>();

            if (canvasGroup != null)
            {
                // �����̃A�j���[�V�������~
                currentFadeTween?.Kill();

                // �t�F�[�h�A�E�g�A�j���[�V����
                currentFadeTween = canvasGroup.DOFade(0f, fadeOutDuration)
                    .SetEase(Ease.OutQuart)
                    .OnComplete(() => {
                        canvasGroup.interactable = false;
                        canvasGroup.blocksRaycasts = false;
                        canvasDisplayed = false;

                        // ������ԃ��Z�b�g
                        player1Ready = false;
                        player2Ready = false;
                        UpdatePlayerReadyDisplay();
                    });
            }
        }
    }

    private void UpdatePlayerReadyDisplay()
    {
        if (player1ReadyText != null)
        {
            player1ReadyText.text = player1Ready ?
                "Ready!" :
                "Press W to GimmickMode Ready";
        }

        if (player2ReadyText != null)
        {
            player2ReadyText.text = player2Ready ?
                "Ready!" :
                "Press I to SimpleMode Ready";
        }
    }

    // OnValidate���\�b�h�̓G�f�B�^��p
#if UNITY_EDITOR
    // �C���X�y�N�^�[�Œl���ύX���ꂽ���ȂǂɎ����ŌĂ΂�郁�\�b�h
    private void OnValidate()
    {
        // sceneAsset�t�B�[���h�ɃV�[�����ݒ肳�ꂽ�ꍇ
        if (sceneAsset != null)
        {
            // ���̃V�[���̖��O�i������j�� sceneToLoad �ϐ��ɃR�s�[����
            sceneToLoad = sceneAsset.name;
        }
        else
        {
            // SceneAsset�����ݒ�Ȃ�󕶎��ɂ���
            sceneToLoad = "";
        }
    }
#endif

}
