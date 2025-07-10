using UnityEngine;

public class ResultController : MonoBehaviour
{
    /// <summary>スコア描画用</summary>
    [SerializeField, Header("スコア描画用")] private ScoreDraw SetScore;

    /// <summary>スコア描画用</summary>
    private RoundManager GameScore;

    /// <summary>描画終了フラグ</summary>
    internal bool DrawEndFlag;

    void Start()
    {
        DrawEndFlag = false;

        // ゲームシーンから来たデータを取得
        GameScore = GameObject.Find("RoundManager").GetComponent<RoundManager>();

        // スコア描画開始
        SetScore.SetScore(GameScore.player1Score, GameScore.player2Score, this);
    }

    void Update()
    {
        if (DrawEndFlag)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("シーン移動");
            }
        }
    }
}
