using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WinProduction : MonoBehaviour
{
    public Vector3 EndSize = Vector3.one;
    public float EndTime;
 
    public RectTransform []PlayerAWinRectTransform;
    public Image []PlayerAWinRectImage;
    public RectTransform []PlayerBWinRectTransform;
    public Image []PlayerBWinRectImage;
    private RectTransform []WinPlayerRectTransform;

    public Color PlayerAColor;
    public Color PlayerBColor;

    private Tween []CheckTween;

    void Start()
    {
        Init();
    }

    public void Init()
    {
        for (int i = 0; i < CheckTween.Length; i++)
        {
            if ((CheckTween[i] != null) && CheckTween[i].IsActive())
            {
                CheckTween[i].Kill();
            }
        }
        for (int i = 0; i < PlayerAWinRectTransform.Length; i++)
        {
            PlayerAWinRectTransform[i].localScale = Vector3.zero;
        }
        for (int i = 0; i < PlayerBWinRectTransform.Length; i++)
        {
            PlayerBWinRectTransform[i].localScale = Vector3.zero;
        }
        WinPlayerRectTransform = null;
    }

    public void SetStart(string winPlayerID, bool setColorFlag)
    {
        if (winPlayerID == "PlayerA")
        {
            WinPlayerRectTransform = PlayerAWinRectTransform;
            if (setColorFlag)
            {
                for (int i = 0; i < PlayerAWinRectImage.Length; i++)
                {
                    PlayerAWinRectImage[i].color = PlayerAColor;
                }
            }
        }
        else if (winPlayerID == "PlayerB")
        {
            WinPlayerRectTransform = PlayerBWinRectTransform;
            if (setColorFlag)
            {
                for (int i = 0; i < PlayerBWinRectImage.Length; i++)
                {
                    PlayerBWinRectImage[i].color = PlayerBColor;
                }
            }
        }

        CheckTween = null;
        CheckTween = new Tween[WinPlayerRectTransform.Length];
        for (int i = 0; i < WinPlayerRectTransform.Length; i++)
        {
            if (WinPlayerRectTransform[i] != null)
            {
                CheckTween[i] = WinPlayerRectTransform[i].transform.DOScale(EndSize, EndTime).SetEase(Ease.OutElastic);
            }
        }
    }
    
    void Update()
    {

/*
        if (Input.GetKeyDown(KeyCode.M))
        {
            Init();
        }
        
        if (Input.GetKeyDown(KeyCode.Z))
        {
            SetStart("PlayerA", false);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            SetStart("PlayerB", false);
        }*/
    }
}
