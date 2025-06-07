using UnityEngine;
using UnityEngine.InputSystem;

public class PushController : MonoBehaviour
{
 private   Rigidbody2D rigidbody;
        // Start is called once before the first execution of Update after the MonoBehaviour is created

        void Start()
        {
            rigidbody = GetComponent<Rigidbody2D>();
        }
    

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            
        }
    }
}
