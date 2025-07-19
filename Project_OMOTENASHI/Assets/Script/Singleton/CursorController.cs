using UnityEngine;

public class CursorController : SingletonMonoBehaviour<CursorController>
{
    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }
}
