using UnityEngine;

public class CameraTest : MonoBehaviour
{
    [SerializeField] private CameraController TestCamera;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            //TestCamera.StartObservation(this.transform.position);
        }
    }
}
