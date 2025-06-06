using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class cube : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.A))
        {
            transform.position += Vector3.left * 0.1f;
        }
       
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += Vector3.right * 0.1f;
        }
    }
}
