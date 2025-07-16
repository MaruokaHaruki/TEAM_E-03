using TMPro;
using UnityEngine;

public class ResultSceneController : MonoBehaviour
{
    public TextMeshProUGUI resultText;

    public AudioClip next; // Ÿ‚ÌƒV[ƒ“‚Ö‚ÌŒø‰Ê‰¹

    void Start()
    {
        string winner = SceneManagerScript.Instance != null ? SceneManagerScript.Instance.winnerName : "Unknown";
        resultText.text = winner;

        Debug.Log($"{SceneManagerScript.Instance.Player1_Score},{SceneManagerScript.Instance.Player2_Score}");
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {/*
            SceneManagerScript.Instance.FadeOutScene("Title");
            AudioManager.Instance.PlaySE(next); // Œø‰Ê‰¹‚ğÄ¶*/
        }
    }
}
