using DG.Tweening;
using TMPro;
using UnityEditor.Animations;
using UnityEngine;

enum DOTWEEN_MOVE_TYPE
{
    SIZE = 0,
    POSITION,
    TEXT_COLOR_TRANSPARENCY,
    PENGUIN,
}

public class TitleLogController : MonoBehaviour
{
    [SerializeField] private float LogMoveStartTime;

    [SerializeField] private float EndTime;

    [SerializeField] private Ease SetDotMoveType = Ease.OutBounce;

    [SerializeField] private DOTWEEN_MOVE_TYPE MoveType;

    [Header("if (StartVolume == (0,0,0))  StartVolume = this;")]
    [SerializeField] private Vector3 StartVolume;
    [Header("if (TargetVolume == (0,0,0))  TargetVolume = Vector3.one;")]
    [SerializeField] private Vector3 TargetVolume;

    private bool AnimatorStartFlag;

    private float StartTime;

    private float NowNumber;

    private bool UpFlag;

    private TextMeshProUGUI MyText;

    private Rigidbody2D MyRigidBody;

    void Start()
    {
        if (StartVolume == Vector3.zero)
        {
            switch (MoveType)
            {
                case DOTWEEN_MOVE_TYPE.SIZE:
                    StartVolume = this.transform.localScale;
                    break;

                case DOTWEEN_MOVE_TYPE.POSITION:
                    StartVolume = this.transform.position;
                    break;

                case DOTWEEN_MOVE_TYPE.TEXT_COLOR_TRANSPARENCY:
                    MyText = this.gameObject.GetComponent<TextMeshProUGUI>();
                    StartVolume = Vector3.zero;
                    break;

                case DOTWEEN_MOVE_TYPE.PENGUIN:
                    MyRigidBody = this.gameObject.GetComponent<Rigidbody2D>();
                    StartVolume = this.transform.position;
                    break;
            }
        }

        if (TargetVolume == Vector3.zero)
        {
            TargetVolume = Vector3.one;
        }

        Init();
    }

    void FixedUpdate()
    {
        if (LogMoveStartTime < (Time.time - StartTime))
        {
            if (!AnimatorStartFlag)
            {
                AnimatorStartFlag = true;

                switch (MoveType)
                {
                    case DOTWEEN_MOVE_TYPE.SIZE:
                        this.transform.DOScale(TargetVolume, EndTime).SetEase(SetDotMoveType);
                        break;

                    case DOTWEEN_MOVE_TYPE.POSITION:
                        this.transform.DOMove(TargetVolume, EndTime).SetEase(SetDotMoveType);
                        break;

                    case DOTWEEN_MOVE_TYPE.TEXT_COLOR_TRANSPARENCY:
                        AnimatorStartFlag = false;
                        NowNumber += TargetVolume.x;
                        if (NowNumber > 1.0f)
                        {
                            NowNumber = 1.0f;
                            TargetVolume.x = -TargetVolume.x;
                            StartTime = Time.time;
                        }
                        else if (NowNumber <= 0.0f)
                        {
                            NowNumber = 0.0f;
                            TargetVolume.x = -TargetVolume.x;
                        }
                        Debug.Log(MyText.color.g);
                        MyText.color = new Color(MyText.color.r, MyText.color.g, MyText.color.b, NowNumber);
                        break;

                    case DOTWEEN_MOVE_TYPE.PENGUIN:
                        MyRigidBody.AddForce(new Vector2(0.0f, TargetVolume.y), ForceMode2D.Impulse);
                        StartTime = Time.time;
                        AnimatorStartFlag = false;
                        break;
                }
            }
        }

        switch (MoveType)
        {
            case DOTWEEN_MOVE_TYPE.PENGUIN:
                this.transform.position -= new Vector3(TargetVolume.x, 0.0f, 0.0f);
                if (this.transform.position.x < TargetVolume.z)
                {
                    this.transform.position = StartVolume;
                }
                break;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Init();
        }
    }

    private void Init()
    {
        StartTime = Time.time;
        AnimatorStartFlag = false;

        switch (MoveType)
        {
            case DOTWEEN_MOVE_TYPE.SIZE:
                this.transform.localScale = StartVolume;
                break;

            case DOTWEEN_MOVE_TYPE.POSITION:
                this.transform.position = StartVolume;
                break;

            case DOTWEEN_MOVE_TYPE.TEXT_COLOR_TRANSPARENCY:
                NowNumber = StartVolume.x;
                MyText.color = new Color(MyText.color.r, MyText.color.g, MyText.color.b, NowNumber);
                break;

            case DOTWEEN_MOVE_TYPE.PENGUIN:
                this.transform.position = StartVolume;
                break;
        }
    }
}
