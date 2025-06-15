using UnityEngine;

public class namechan : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SceneManagerScript.Instance.winnerName = "test";
            SceneManagerScript.Instance.FadeOutScene("Result");
        }

    }
}
