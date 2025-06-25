using UnityEngine;

public class InvincibleItemGeneration : MonoBehaviour
{
    [SerializeField] private GameObject InvincibleItem;

    private GameObject CheckObject;

    [SerializeField] protected KeyCode[] Key;


    void Start()
    {
        int plusKeyNumber = 0;
        if (Key != null)
        {
            plusKeyNumber = Key.Length;
        }
        KeyCode[] setKey = Key;
        Key = new KeyCode[2 + plusKeyNumber];
        Key[0] = KeyCode.Alpha0;
        Key[1] = KeyCode.Alpha1;
        for (int i = 0; i < plusKeyNumber; i++)
        {
            Key[2 + i] = setKey[i];
        }
    }


    void Update()
    {
        bool keyPushFlag = true;

        for (int i = 0; i < Key.Length; i++)
        {
            if (!Input.GetKey(Key[i]))
            {
                keyPushFlag = false;
            }
        }

        if (keyPushFlag)
        {
            if (CheckObject == null)
            {
                CheckObject = Instantiate(InvincibleItem, this.transform.position, Quaternion.identity);
            }
        }
    }
}
