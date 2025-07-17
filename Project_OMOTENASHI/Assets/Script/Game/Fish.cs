using UnityEngine;

public class Fish : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// プレイヤーと接触した時に呼ばれる
    /// </summary>
    /// <param name="other">接触したオブジェクト</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // プレイヤータグのオブジェクトと接触した場合
        if (other.gameObject.tag == "Player")
        {
            // 魚オブジェクトを破壊
            Destroy(gameObject);
        }
    }
}
