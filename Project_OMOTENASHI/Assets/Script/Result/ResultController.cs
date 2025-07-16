using UnityEngine;

public class ResultController : MonoBehaviour
{
    /// <summary>スコア描画用</summary>
    [SerializeField, Header("スコア描画用")] private ScoreDraw SetScore;

    /// <summary>描画終了フラグ</summary>
    internal bool DrawEndFlag;

    void Start()
    {
        DrawEndFlag = false;

        // スコア描画開始
        SetScore.SetScore(SceneManagerScript.Instance.Player1_Score, SceneManagerScript.Instance.Player2_Score, this);
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
