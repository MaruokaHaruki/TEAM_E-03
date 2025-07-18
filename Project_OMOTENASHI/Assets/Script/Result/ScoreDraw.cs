using UnityEngine;

public class ScoreDraw : MonoBehaviour
{
    [Header("�v���C���[�W�����v(�Ȉ�)")]
    [SerializeField] private bool JumpFlag;
    [SerializeField] private ResultSimpleJump JumpPlayerA;
    [SerializeField] private ResultSimpleJump JumpPlayerB;

    [Header("�X�R�A����p�v���C���[")]
    [SerializeField] private GameObject PlayerA;
    [SerializeField] private GameObject PlayerB;

    [Header("�v���C���[�̍���")]
    [SerializeField] private float PlayerASize;
    [SerializeField] private float PlayerBSize;

    [Header("��")]
    [SerializeField] private GameObject Fish;
    [SerializeField] private bool FishChildObjectFlag = false;

    [Header("�M")]
    [SerializeField] private Rigidbody2D PlayerAPlate;
    [SerializeField] private Rigidbody2D PlayerBPlate;

    [Header("�s��(x)�A���҃T�C�Y(y)�@�T�C�Y")]
    [SerializeField] private Vector2 SetSize = new Vector2(0.5f, 2.0f);

    [Header("�s��(x)�A���҃T�C�Y(y) �ړ��ʔ{��")]
    [SerializeField] private Vector2 SetXMove = new Vector2(2.0f, 1.0f);

    [SerializeField] private int BlownAwayScore = 3;

    [SerializeField] private GameObject PlayerAStartSprite;
    [SerializeField] private GameObject PlayerBStartSprite;

    [SerializeField] private GameObject PlayerALoserSprite;
    [SerializeField] private GameObject PlayerBLoserSprite;

    [SerializeField] private GameObject PlayerAWinnerSprite;
    [SerializeField] private GameObject PlayerBWinnerSprite;

    private int PlayerAScore;
    private int PlayerBScore;

    private Vector2 StartPlayerAPos;
    private Vector2 StartPlayerBPos;

    /// <summary>�`��ς݃X�R�AA</summary>
    private int DrawDoneAScore;
    /// <summary>�`��ς݃X�R�AB</summary>
    private int DrawDoneBScore;

    /// <summary>�X�R�A�ݒ�ς݃t���O</summary>
    private bool SetScoreFlag = false;

    private ResultController Result;

    /// <summary>�����f�[�^</summary>
    private int PlayerWinData;

    /// <summary>���݃v���C���[�T�C�Y</summary>
    private Vector2 NowPlayersSize;

    /// <summary>���̍s���܂ł̎���</summary>
    private float NextMoveTime;

    /// <summary>�J�n����</summary>
    [SerializeField, Header("�J�n����")] private float StartTime = 2.0f;

    /// <summary>���̋���������܂ł̎���</summary>
    [SerializeField, Header("���̋���������܂ł̎���")] private float NextFishTime = 2.0f;

    /// <summary>�傫�����ς�鑬�x</summary>
    [SerializeField, Header("�傫�����ς�鑬�x")] private float SizeChangeSpeed = 0.1f;

    /// <summary>�M��������ԗ�</summary>
    [SerializeField, Header("�M��������ԗ�")] private float PlateUpPower = 100.0f;
    /// <summary>�M����Ԃ܂ł̎���</summary>
    [SerializeField, Header("�M����Ԃ܂ł̎���")] private float PlateUpTime = 0.5f;

    /// <summary>�`��I���t���O</summary>
    private bool DrawEndFlag;

    private void Awake()
    {
        SetScoreFlag = false;
        DrawDoneAScore = 0;
        DrawDoneBScore = 0;
        NowPlayersSize = Vector2.one;

        StartPlayerAPos = PlayerA.transform.position;
        StartPlayerBPos = PlayerB.transform.position;

        PlayerAPlate.constraints = RigidbodyConstraints2D.FreezeAll;
        PlayerAPlate.gravityScale = 0.0f;
        if (FishChildObjectFlag)
        {
            PlayerAPlate.gameObject.transform.parent = PlayerA.gameObject.transform;
        }
        else
        {
            PlayerAPlate.gameObject.transform.parent = null;
        }

        PlayerBPlate.constraints = RigidbodyConstraints2D.FreezeAll;
        PlayerBPlate.gravityScale = 0.0f;
        if (FishChildObjectFlag)
        {
            PlayerBPlate.gameObject.transform.parent = PlayerB.gameObject.transform;
        }
        else
        {
            PlayerBPlate.gameObject.transform.parent = null;
        }

        NextMoveTime = Time.time + StartTime;

        DrawEndFlag = false;
    }

    private void Start()
    {
        JumpPlayerA.JumpFlag = JumpFlag;
        JumpPlayerB.JumpFlag = JumpFlag;

        SetPlayersObjectActive(0);
    }

    void FixedUpdate()
    {
        if (SetScoreFlag && (NextMoveTime < Time.time))
        {
            // �X�R�A���`��
            if ((DrawDoneAScore != PlayerAScore) || (DrawDoneBScore != PlayerBScore))
            {
                NextMoveTime = Time.time + NextFishTime;

                // �v���C���[A�X�R�A�`��
                if (DrawDoneAScore < PlayerAScore)
                {
                    if (FishChildObjectFlag)
                    {
                        Instantiate(Fish, PlayerA.transform.position + (Vector3.forward * 0.5f), Quaternion.Euler(0.0f, 0.0f, Random.Range(-0.0f, 360.0f))).transform.parent = PlayerA.transform;
                    }
                    else
                    {
                        Instantiate(Fish, PlayerA.transform.position + (Vector3.forward * 0.5f), Quaternion.Euler(0.0f, 0.0f, Random.Range(-0.0f, 360.0f)));
                    }

                    DrawDoneAScore++;
                }

                // �v���C���[B�X�R�A�`��
                if (DrawDoneBScore < PlayerBScore)
                {
                    if (FishChildObjectFlag)
                    {
                        Instantiate(Fish, PlayerB.transform.position + (Vector3.forward * 0.5f), Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f))).transform.parent = PlayerB.transform;
                    }
                    else
                    {
                        Instantiate(Fish, PlayerB.transform.position + (Vector3.forward * 0.5f), Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));
                    }

                    DrawDoneBScore++;
                }

                if ((DrawDoneAScore == PlayerAScore) && (DrawDoneBScore == PlayerBScore))
                {
                    if (JumpFlag)
                    {
                        switch (PlayerWinData)
                        {
                            case 0:
                                JumpPlayerA.JumpFlag = false;
                                JumpPlayerB.JumpFlag = false;
                                break;
                            case 1:
                                JumpPlayerB.JumpFlag = false;
                                break;
                            case 2:
                                JumpPlayerA.JumpFlag = false;
                                break;
                        }
                    }

                    NextMoveTime = Time.time + (NextFishTime * 0.5f);
                }
            }
            else
            {
                //                NextMoveTime = Time.time + 0.05f;
                if (!DrawEndFlag)
                {
                    Vector2 diff = Vector2.zero;

                    // ���s�ɂ���ăL�����N�^�[�̑傫����|�W�V������ς���
                    switch (PlayerWinData)
                    {
                        case 0:
                            DrawEndFlag = true;
                            SetPlayersObjectActive(3);
                            break;

                        case 1:
                            diff = new Vector2(SetSize.x - NowPlayersSize.y, SetSize.y - NowPlayersSize.x);
                            SetDiffPos(ref diff.x);
                            SetDiffPos(ref diff.y);
                            NowPlayersSize = new Vector2(SetSize.y - diff.y, SetSize.x - diff.x);

                            {// PlayerB
                                PlayerB.transform.localScale = Vector3.one * NowPlayersSize.y;
                                PlayerB.transform.position = new Vector3(StartPlayerBPos.x - ((NowPlayersSize.y - 1.0f) * SetXMove.x), StartPlayerBPos.y + ((NowPlayersSize.y - 1.0f) * (PlayerBSize * 0.5f)), PlayerB.transform.position.z);
                                JumpPlayerB.SetGroundHeight = (NowPlayersSize.y - 1.0f) * 1.0f;
                            }
                            {// PlayerA
                                PlayerA.transform.localScale = Vector3.one * NowPlayersSize.x;
                                PlayerA.transform.position = new Vector3(StartPlayerAPos.x + ((NowPlayersSize.x - 1.0f) * SetXMove.y), StartPlayerAPos.y + ((NowPlayersSize.x - 1.0f) * (PlayerASize * 0.5f)), PlayerA.transform.position.z);
                                JumpPlayerA.SetGroundHeight = (NowPlayersSize.x - 1.0f) * 1.0f;
                            }

                            if ((SetSize.x == NowPlayersSize.y) && (SetSize.y == NowPlayersSize.x))
                            {
                                DrawEndFlag = true;
                                NextMoveTime = Time.time + PlateUpTime;
                                SetPlayersObjectActive(2);
                            }
                            break;

                        case 2:
                            diff = new Vector2(SetSize.x - NowPlayersSize.x, SetSize.y - NowPlayersSize.y);
                            SetDiffPos(ref diff.x);
                            SetDiffPos(ref diff.y);
                            NowPlayersSize = new Vector2(SetSize.x - diff.x, SetSize.y - diff.y);

                            {// PlayerB
                                PlayerB.transform.localScale = Vector3.one * NowPlayersSize.y;
                                PlayerB.transform.position = new Vector3(StartPlayerBPos.x - ((NowPlayersSize.y - 1.0f) * SetXMove.y), StartPlayerBPos.y + ((NowPlayersSize.y - 1.0f) * (PlayerBSize * 0.5f)), PlayerB.transform.position.z);
                                JumpPlayerB.SetGroundHeight = (NowPlayersSize.y - 1.0f) * 1.0f;
                            }
                            {// PlayerA
                                PlayerA.transform.localScale = Vector3.one * NowPlayersSize.x;
                                PlayerA.transform.position = new Vector3(StartPlayerAPos.x + ((NowPlayersSize.x - 1.0f) * SetXMove.x), StartPlayerAPos.y + ((NowPlayersSize.x - 1.0f) * (PlayerASize * 0.5f)), PlayerA.transform.position.z);
                                JumpPlayerA.SetGroundHeight = (NowPlayersSize.x - 1.0f) * 1.0f;
                            }

                            if ((SetSize.x == NowPlayersSize.x) && (SetSize.y == NowPlayersSize.y))
                            {
                                DrawEndFlag = true;
                                NextMoveTime = Time.time + PlateUpTime;
                                SetPlayersObjectActive(1);
                            }
                            break;
                    }
                }
            }

            // �X�R�A�`��I��
            if (DrawEndFlag && (NextMoveTime < Time.time) && (JumpPlayerA.GroundFlag && JumpPlayerB.GroundFlag))
            {
                if (PlayerAScore <= BlownAwayScore)
                {
                    PlayerAPlate.constraints = RigidbodyConstraints2D.None;
                    PlayerAPlate.AddForceY(PlateUpPower, ForceMode2D.Impulse);
                }

                if (PlayerBScore <= BlownAwayScore)
                {
                    PlayerBPlate.constraints = RigidbodyConstraints2D.None;
                    PlayerBPlate.AddForceY(PlateUpPower, ForceMode2D.Impulse);
                }

                Result.DrawEndFlag = true;
                //Destroy(this);
            }
        }
    }

    /// <summary>�������ϓ�������</summary>
    private void SetDiffPos(ref float diff)
    {
        if (diff > 0.0f)
        {
            diff -= SizeChangeSpeed;
            if (diff < 0.0f)
            {
                diff = 0.0f;
            }
        }
        else
        {
            diff += SizeChangeSpeed;
            if (diff > 0.0f)
            {
                diff = 0.0f;
            }
        }
    }

    /// <summary>�X�R�A�ݒ�</summary>
    internal void SetScore(int playerAScore, int playerBScore, ResultController result)
    {
        // �X�R�A
        PlayerAScore = playerAScore;
        PlayerBScore = playerBScore;

        // ���s
        if (playerAScore == playerBScore)
        {
            PlayerWinData = 0;
        }
        else if (playerAScore > playerBScore)
        {
            PlayerWinData = 1;
        }
        else
        {
            PlayerWinData = 2;
        }

        // ���U���g
        Result = result;

        // �X�R�A�f�[�^�ݒ肵���̂ŗL����
        SetScoreFlag = true;
    }

    private void SetPlayersObjectActive(int setNumber)
    {
        bool[] set = { false, false, false, false, false, false };
        switch (setNumber)
        {
            case 0:
                set[0] = true;
                set[1] = true;
                break;

            case 1:
                set[2] = true;
                set[5] = true;
                break;
            case 2:
                set[4] = true;
                set[3] = true;
                break;

            case 3:
                set[0] = true;
                set[1] = true;
                break;
        }
        PlayerAStartSprite.SetActive(set[0]);
        PlayerBStartSprite.SetActive(set[1]);
        PlayerALoserSprite.SetActive(set[2]);
        PlayerBLoserSprite.SetActive(set[3]);
        PlayerAWinnerSprite.SetActive(set[4]);
        PlayerBWinnerSprite.SetActive(set[5]);
    }
}
