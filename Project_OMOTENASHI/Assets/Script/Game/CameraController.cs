using UnityEngine;

public class CameraController : MonoBehaviour
{
    /// <summary>�����|�W�V����</summary>
    [SerializeField] private RectTransform MiddlePos;

    /// <summary>�ݒ�^�C��</summary>
    [SerializeField] private float SetTime = 2.0f;
    
    /// <summary>�����t���O</summary>
    [SerializeField] private bool ObservationFlag;

    /// <summary>��������</summary>
    [SerializeField] private float ObservationTime;

    /// <summary>�ŏ��̃|�W�V����</summary>
    [SerializeField] private Vector3 StartPos;

    /// <summary>�ڕW�|�W�V����</summary>
    [SerializeField] private Vector3 TargetPos;

    /// <summary>�ڕW�J�����T�C�Y</summary>
    [SerializeField] private float TargetCameraSize;

    /// <summary>�J����</summary>
    [SerializeField] private Camera MainCamera;

    /// <summary>�SUI</summary>
    [SerializeField] private RectTransform[] AllUiTransform;

    private Vector3[] AllUiStartPos;
    private Vector3[] AllUiStartSize;
    public Vector3 UiMoveVolume = new Vector3(1.0f, 1.0f, 1.5f);

    void Start()
    {
        TargetPos = StartPos = this.transform.position;

        ObservationFlag = false;
        ObservationTime = 0.0f;

        if (MainCamera == null)
        {
            MainCamera = this.gameObject.GetComponent<Camera>();
        }

        TargetCameraSize = 5.0f;

        AllUiStartPos = new Vector3[AllUiTransform.Length];
        AllUiStartSize= new Vector3[AllUiTransform.Length];
        for (int i = 0; i < AllUiTransform.Length; i++)
        {
            AllUiStartPos[i] = AllUiTransform[i].position;
            AllUiStartSize[i] = AllUiTransform[i].localScale;
        }
    }

    void Update()
    {
        if (ObservationFlag)
        {
            ObservationTime -= Time.deltaTime;
            if (ObservationTime <= 0.0f)
            {
                ObservationFlag = false;
                TargetPos = StartPos;

                TargetCameraSize = 5.0f;
            }
        }
    }

    private void FixedUpdate()
    {
        CameraMoveProcess();
    }

    public void StartObservation(Vector3 targetPos)
    {
        TargetPos = targetPos + (Vector3.forward * this.transform.position.z);
        ObservationFlag = true;
        ObservationTime = SetTime;
        TargetCameraSize = 2.0f;
    }

    private void CameraMoveProcess()
    {
        Vector3 uiSetScale = Vector3.zero;

        Vector3 cameraUiPosition;
        Vector3 uiSetPos;

        if (MainCamera.orthographicSize != TargetCameraSize)
        {
            float diffSize = TargetCameraSize - MainCamera.orthographicSize;

            SetDiff(ref diffSize, 0.5f);
            MainCamera.orthographicSize = TargetCameraSize - diffSize;

            uiSetScale = (Vector3.one * (1.0f - (MainCamera.orthographicSize / 5.0f))) * UiMoveVolume.z;
            for (int i = 0; i < AllUiTransform.Length; i++)
            {
                AllUiTransform[i].localScale = new Vector3(AllUiStartSize[i].x + uiSetScale.x, AllUiStartSize[i].y + uiSetScale.y, AllUiStartSize[i].z + uiSetScale.z);
            }
        }

        if (TargetPos != this.transform.position)
        {
            Vector3 diffPos = TargetPos - this.transform.position;

            SetDiff(ref diffPos.x, 0.5f);
            SetDiff(ref diffPos.y, 0.5f);
            SetDiff(ref diffPos.z, 0.5f);

            this.transform.position = TargetPos - diffPos;

            cameraUiPosition = (this.transform.position - StartPos) * 100.0f;

            for (int i = 0; i < AllUiTransform.Length; i++)
            {
                uiSetPos = (AllUiStartPos[i] - MiddlePos.position) - cameraUiPosition;
                uiSetPos.x *= (uiSetScale.x + 1.0f);
                uiSetPos.y *= (uiSetScale.y + 1.0f);
                AllUiTransform[i].position = new Vector3(MiddlePos.position.x + uiSetPos.x, MiddlePos.position.y + uiSetPos.y, AllUiStartPos[i].z);
            }
        }
    }

    private void SetDiff(ref float diff, float speed)
    {
        if (diff > 0.0f)
        {
            diff -= speed;
            if (diff < 0.0f)
            {
                diff = 0.0f;
            }
        }
        else
        {
            diff += speed;
            if (diff > 0.0f)
            {
                diff = 0.0f;
            }
        }
    }
}
