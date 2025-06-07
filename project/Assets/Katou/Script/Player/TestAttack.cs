using UnityEngine;

public class TestAttack : MonoBehaviour
{
    /// <summary>
    /// 与ダメージ量
    /// </summary>
    [SerializeField, Header("与ダメージ量")] private int ToDamageVolume = 0;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController setPlayer = collision.gameObject.GetComponent<PlayerController>();
        if (setPlayer != null)
        {
            setPlayer.Damage(ToDamageVolume);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log("何かに当たっている");
    }
}
