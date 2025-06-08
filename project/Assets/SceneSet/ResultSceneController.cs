using TMPro;
using UnityEngine;

public class ResultSceneController : MonoBehaviour
{
    public TextMeshProUGUI resultText;

    void Start()
    {
        string winner = SceneManagerScript.Instance != null ? SceneManagerScript.Instance.winnerName : "Unknown";
        resultText.text = winner;
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SceneManagerScript.Instance.FadeOutScene("Title");
        }
    }
}
