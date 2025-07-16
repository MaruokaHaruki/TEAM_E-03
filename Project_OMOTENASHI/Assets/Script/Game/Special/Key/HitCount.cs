using UnityEngine;

public class HitCount : MonoBehaviour
{
    public int MyHitCount;

    void Start()
    {
        MyHitCount = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        MyHitCount++;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        MyHitCount--;
        if (MyHitCount < 0)
        {
            MyHitCount = 0;
        }
    }
}
