using UnityEngine;

public class ObjectController : MonoBehaviour
{
    /// <summary>
    /// �ݒ�|�W�V����
    /// </summary>
    [SerializeField, Header("�ݒ�|�W�V���� [0�A0�A0]�Ȃ�u���Ă���|�W�V�����ɕύX�����")] private Vector3 SetPosition;

    /// <summary>
    /// ��������
    /// </summary>
    [SerializeField, Header("��������")] private int StartKeyType;

    /// <summary>
    /// �N���t���O
    /// </summary>
    [SerializeField] private bool MoveFlag;

    /// <summary>
    /// �ړ����x
    /// </summary>
    [SerializeField, Header("�ړ����x")] private float MoveSpeed;

    [SerializeField] private float EndTime;

    /// <summary>
    /// �N���o�ߎ���
    /// </summary>
    private float ElapsedTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (SetPosition == Vector3.zero)
        {
            SetPosition = this.transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (StartKeyType)
        {
            case 0:
                if (Input.GetKey(KeyCode.Alpha0))
                {
                    StartObjectMove();
                }
                break;
            case 1:
                if (Input.GetKey(KeyCode.Alpha1))
                {
                    StartObjectMove();
                }
                break;
            case 2:
                if (Input.GetKey(KeyCode.Alpha2))
                {
                    StartObjectMove();
                }
                break;
            case 3:
                if (Input.GetKey(KeyCode.Alpha3))
                {
                    StartObjectMove();
                }
                break;
            case 4:
                if (Input.GetKey(KeyCode.Alpha4))
                {
                    StartObjectMove();
                }
                break;
            case 5:
                if (Input.GetKey(KeyCode.Alpha5))
                {
                    StartObjectMove();
                }
                break;
            case 6:
                if (Input.GetKey(KeyCode.Alpha6))
                {
                    StartObjectMove();
                }
                break;
            case 7:
                if (Input.GetKey(KeyCode.Alpha7))
                {
                    StartObjectMove();
                }
                break;
            case 8:
                if (Input.GetKey(KeyCode.Alpha8))
                {
                    StartObjectMove();
                }
                break;
            case 9:
                if (Input.GetKey(KeyCode.Alpha9))
                {
                    StartObjectMove();
                }
                break;
        }

        // �N��������
        {
            if (MoveFlag)
            {
                ElapsedTime += Time.deltaTime;

                this.transform.position += new Vector3(0.0f, MoveSpeed * Time.deltaTime, 0.0f);
                if ((SetPosition.y + this.transform.lossyScale.y + 0.1) < this.transform.position.y)
                {
                    this.transform.position = new Vector3(this.transform.position.x, (SetPosition.y + this.transform.lossyScale.y + 0.1f));
                }

                if (ElapsedTime >= EndTime)
                {
                    MoveFlag = false;
                }
            }
            else
            {
                this.transform.position -= Vector3.up * MoveSpeed * Time.deltaTime;

                if (SetPosition.y > this.transform.position.y)
                {
                    this.transform.position = new Vector3(this.transform.position.x, SetPosition.y, this.transform.position.z);
                }
            }
        }
    }

    private void StartObjectMove()
    {
        MoveFlag = true;
        ElapsedTime = 0.0f;
    }
}
