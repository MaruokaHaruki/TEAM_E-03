using TMPro;
using UnityEngine;

public class TimeText : MonoBehaviour
{
    /// <summary>
    /// 時間表示用テキスト
    /// </summary>
    private TextMeshProUGUI TimeDrawText;

    /// <summary>
    /// 開始時間
    /// </summary>
    private float StartTime = -1.0f;

    /// <summary>
    /// 制限時間
    /// </summary>
    private float LimitTime;

    /// <summary>
    /// 制限時間有効フラグ
    /// </summary>
    private bool LimitTimeFlag = false;

    void Start()
    {
        // 時間表示用テキスト取得
        TimeDrawText = this.gameObject.GetComponent<TextMeshProUGUI>();

        // 開始時間設定
        if (StartTime == -1.0f)
        {
            StartTime = Time.time;
        }

        // 描画
        TimeDraw();
    }

    void Update()
    {
        // 描画
        TimeDraw();
    }

    /// <summary>
    /// 時間描画
    /// </summary>
    private void TimeDraw()
    {
        if (LimitTimeFlag)
        {
            // 制限時間描画
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
            // 経過時間描画
            SetTimeText((int)(Time.time - StartTime));
        }
    }

    /// <summary>
    /// テキストに時間設定
    /// </summary>
    private void SetTimeText(int drawTime)
    {
        TimeDrawText.text = ("TIME [ " + (drawTime / 60).ToString().PadLeft(2, '0') + " : " + (drawTime % 60).ToString().PadLeft(2, '0') + " ]");
    }
    
    /// <summary>
    /// 開始時間設定
    /// </summary>
    public void SetStartTime(float time)
    {
        StartTime = time;
    }

    /// <summary>
    /// 終了時間設定
    /// </summary>
    public void SetLimitTime(float time)
    {
        LimitTime = time;
        LimitTimeFlag = true;
    }

    /// <summary>
    /// 制限時間超過フラグ
    /// </summary>
    /// <returns></returns>
    public bool GetLimitTimeExcessFlag()
    {
        return ((Time.time - StartTime) >= LimitTime) && LimitTimeFlag;
    }
}
