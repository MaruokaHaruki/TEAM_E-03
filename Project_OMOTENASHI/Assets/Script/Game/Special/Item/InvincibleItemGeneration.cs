using UnityEngine;

public class InvincibleItemGeneration : MonoBehaviour {
    /// <summary>�����A�C�e��</summary>
    [SerializeField] private GameObject InvincibleItem;

    /// <summary>�E�����L�[</summary>
    [SerializeField] protected KeyCode LeftKey = KeyCode.Alpha1;
    /// <summary>�������L�[</summary>
    [SerializeField] protected KeyCode RightKey = KeyCode.Alpha0;

    /// <summary>���I�u�W�F</summary>
    [SerializeField] private GameObject LeftObj;
    /// <summary>�E�I�u�W�F</summary>
    [SerializeField] private GameObject RightObj;

    /// <summary>�ݒ�A���O��</summary>
    [SerializeField, Range(0.0f, 180.0f)] private float SetAngle;

    /// <summary>��������</summary>
    [SerializeField, Range(0.01f, 1.0f)] private float PopItemRate = 0.1f;

    /// <summary>�J�����x</summary>
    [SerializeField] private float MoveSpeed = 10.0f;

    /// <summary>�A�C�e�������t���O</summary>
    private bool PopItemFlag = false;

    /// <summary>���������j�~�m�F�p</summary>
    private GameObject CheckObject = null;

    /// <summary>�A�C�e�����Ƃ��m�F�p</summary>
    private BoxCollider2D CheckObjectBoxCollider;


    void Awake() {
        // ���E�E�I�u�W�F�擾
        if (LeftObj == null) {
            LeftObj = this.transform.Find("LeftObj").gameObject;
        }
        if (RightObj == null) {
            RightObj = this.transform.Find("RightObj").gameObject;
        }
    }

    private void OnEnable() {
        PopItemFlag = false;
        LeftObj.transform.rotation = Quaternion.identity;
        RightObj.transform.rotation = Quaternion.identity;
    }

    void Update() {
        bool keyPushFlag = true;

        if (Input.GetKey(LeftKey) || PopItemFlag) {
            LeftObj.transform.eulerAngles = new Vector3(0.0f, 0.0f, GetNextAngle(-SetAngle, LeftObj.transform.eulerAngles.z, MoveSpeed));
        }
        else {
            LeftObj.transform.eulerAngles = new Vector3(0.0f, 0.0f, GetNextAngle(0.0f, LeftObj.transform.eulerAngles.z, MoveSpeed));
            keyPushFlag = false;
        }

        if (Input.GetKey(RightKey) || PopItemFlag) {
            RightObj.transform.eulerAngles = new Vector3(0.0f, 0.0f, GetNextAngle(SetAngle, RightObj.transform.eulerAngles.z, MoveSpeed));
        }
        else {
            RightObj.transform.eulerAngles = new Vector3(0.0f, 0.0f, GetNextAngle(0.0f, RightObj.transform.eulerAngles.z, MoveSpeed));
            keyPushFlag = false;
        }


        if (keyPushFlag) {
            // ���G�A�C�e�������t���O�I��
            PopItemFlag = true;
        }

        if ((CheckObject == null) && !PopItemFlag) {
            CheckObject = Instantiate(InvincibleItem, this.transform.position + new Vector3(0.0f, -0.5f, 0.0f), Quaternion.identity);
        }

        // �m�F�����Ƃ�
        CheckPop();
    }

    /// <summary>����𖞂����Ă���ꍇ�A�C�e�����Ƃ�</summary>
    private void CheckPop() {
        if (PopItemFlag) {
            if (((GetCanonicalAngle(LeftObj.transform.eulerAngles.z) / -SetAngle) >= PopItemRate) &&
                ((GetCanonicalAngle(RightObj.transform.eulerAngles.z) / SetAngle) >= PopItemRate)) {
                // ����
                if (CheckObject != null) {
                    CheckObjectBoxCollider = CheckObject.AddComponent<BoxCollider2D>();
                    Debug.Log("ボックスコライダーを追加したよ！！");

                    Rigidbody2D setRigidbody = CheckObject.GetComponent<Rigidbody2D>();
                    if (setRigidbody != null) {
                        setRigidbody.gravityScale = 1.0f;
                    }
                }
                PopItemFlag = false;
            }
            else {
                if (CheckObjectBoxCollider != null) {
                    PopItemFlag = false;
                }
            }
        }
    }

    /// <summary>���̃A���O����擾</summary>
    private float GetNextAngle(float setAngle, float nowAngle, float speed) {

        float deffAngle = setAngle - GetCanonicalAngle(nowAngle);

        if (deffAngle == 0.0f) {
            return setAngle;
        }

        if (deffAngle < 0.0f) {
            deffAngle += speed;
            if (deffAngle > 0.0f) {
                deffAngle = 0.0f;
            }
        }
        else {
            deffAngle -= speed;
            if (deffAngle < 0.0f) {
                deffAngle = 0.0f;
            }
        }

        return setAngle - deffAngle;
    }

    /// <summary>���͈͓�ɂ���</summary>
    private float GetCanonicalAngle(float angle) {
        if (angle > 180.0f) {
            angle -= 360.0f;
        }
        else if (angle < -180.0f) {
            angle += 360.0f;
        }

        return angle;
    }

    /// <summary>���̃I�u�W�F�N�g�̗L����ݒ�</summary>
    internal void SetActiveFlag(bool activeFlag) {
        if (CheckObject != null) {
            Destroy(CheckObject.gameObject);
        }

        this.gameObject.SetActive(activeFlag);
    }
}
