using TMPro;
using UnityEngine;

public class ResultSceneController : MonoBehaviour
{
    public TextMeshProUGUI resultText;

    public AudioClip next; // 次のシーンへの効果音  たいとる　Iキー　  別モードプレイ　スペースキー　　再プレイ　Wキー

    private string OldSceneName;

    [SerializeField] private GameObject SimpleModeText;
    [SerializeField] private GameObject GimmickModeText;

    private string SimpleModeSceneName;
    private string GimmickModeSceneName;

    void Start()
    {
        string winner = SceneManagerScript.Instance != null ? SceneManagerScript.Instance.winnerName : "Unknown";
        resultText.text = winner;

        Debug.Log($"{SceneManagerScript.Instance.Player1_Score},{SceneManagerScript.Instance.Player2_Score}");

        OldSceneName =  SceneManagerScript.Instance.OldSceneName;
        SimpleModeSceneName = "GameScene_Syuuten";
        GimmickModeSceneName = "GameScene_Rantou";

        if (OldSceneName == GimmickModeSceneName)
        {
            SimpleModeText.SetActive(true);
            GimmickModeText.SetActive(false);
        }
        else if (OldSceneName == SimpleModeSceneName)
        {
            SimpleModeText.SetActive(false);
            GimmickModeText.SetActive(true);
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            string nextSceneName = "";

            if (OldSceneName == GimmickModeSceneName)
            {
                nextSceneName = SimpleModeSceneName;
            }
            else if (OldSceneName == SimpleModeSceneName)
            {
                nextSceneName = GimmickModeSceneName;
            }

            if (nextSceneName != "")
            {
                SceneManagerScript.Instance.FadeOutScene(nextSceneName);
                AudioManager.Instance.PlaySE(next); // 効果音を再生
            }
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            SceneManagerScript.Instance.FadeOutScene(OldSceneName);
            AudioManager.Instance.PlaySE(next); // 効果音を再生
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            SceneManagerScript.Instance.FadeOutScene("Title");
            AudioManager.Instance.PlaySE(next); // 効果音を再生
        }
    }
}
