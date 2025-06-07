using TMPro;
using UnityEngine;

public class TimeText : MonoBehaviour
{
    /// <summary>
    /// ���ԕ\���p�e�L�X�g
    /// </summary>
    private TextMeshProUGUI TimeDrawText;

    /// <summary>
    /// �J�n����
    /// </summary>
    private float StartTime = -1.0f;

    /// <summary>
    /// ��������
    /// </summary>
    private float LimitTime;

    /// <summary>
    /// �������ԗL���t���O
    /// </summary>
    private bool LimitTimeFlag = false;

    void Start()
    {
        // ���ԕ\���p�e�L�X�g�擾
        TimeDrawText = this.gameObject.GetComponent<TextMeshProUGUI>();

        // �J�n���Ԑݒ�
        if (StartTime == -1.0f)
        {
            StartTime = Time.time;
        }

        // �`��
        TimeDraw();
    }

    void Update()
    {
        // �`��
        TimeDraw();
    }

    /// <summary>
    /// ���ԕ`��
    /// </summary>
    private void TimeDraw()
    {
        if (LimitTimeFlag)
        {
            // �������ԕ`��
            if (!GetLimitTimeExcessFlag())
            {
                SetTimeText((int)(LimitTime - (Time.time - StartTime)));
            }
            else
            {
                SetTimeText(0);
            }
        }
        else
        {
            // �o�ߎ��ԕ`��
            SetTimeText((int)(Time.time - StartTime));
        }
    }

    /// <summary>
    /// �e�L�X�g�Ɏ��Ԑݒ�
    /// </summary>
    private void SetTimeText(int drawTime)
    {
        TimeDrawText.text = ("TIME [ " + (drawTime / 60).ToString().PadLeft(2, '0') + " : " + (drawTime % 60).ToString().PadLeft(2, '0') + " ]");
    }
    
    /// <summary>
    /// �J�n���Ԑݒ�
    /// </summary>
    public void SetStartTime(float time)
    {
        StartTime = time;
    }

    /// <summary>
    /// �I�����Ԑݒ�
    /// </summary>
    public void SetLimitTime(float time)
    {
        LimitTime = time;
        LimitTimeFlag = true;
    }

    /// <summary>
    /// �������Ԓ��߃t���O
    /// </summary>
    /// <returns></returns>
    public bool GetLimitTimeExcessFlag()
    {
        return ((Time.time - StartTime) >= LimitTime) && LimitTimeFlag;
    }
}
