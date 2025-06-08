using UnityEngine;
using UnityEngine.UI;

public class GoalArea : MonoBehaviour
{
    [Header("ê›íË")]
    public float requiredTimeToWin = 10f;
    public float speedThreshold = 1.5f;

    [Header("UI")]
    public Image player1Bar;
    public Image player2Bar;

    [Header("åãâ UI")]
    public GameObject resultPanel;
    public Text resultText;

    private float p1Time = 0f;
    private float p2Time = 0f;

    private void OnTriggerStay2D(Collider2D other)
    {
        Rigidbody2D rb = other.attachedRigidbody;
        if (rb == null) return;

        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc == null || pc.GetComponent<PlayerHealth>().isDead) return;

        float speed = rb.linearVelocity.magnitude;
        if (speed > speedThreshold) return;

        if (other.CompareTag("Player1"))
        {
            p1Time += Time.deltaTime;
            UpdateBar(player1Bar, p1Time);
            if (p1Time >= requiredTimeToWin) DeclareWinner("Player 1");
        }
        else if (other.CompareTag("Player2"))
        {
            p2Time += Time.deltaTime;
            UpdateBar(player2Bar, p2Time);
            if (p2Time >= requiredTimeToWin) DeclareWinner("Player 2");
        }
    }

    void UpdateBar(Image bar, float time)
    {
        if (bar != null)
            bar.fillAmount = time / requiredTimeToWin;
    }

    void DeclareWinner(string winner)
    {
        if (resultPanel != null) resultPanel.SetActive(true);
        if (resultText != null) resultText.text = $"{winner} wins!";
        Time.timeScale = 0f; // àÍéûí‚é~
    }
}