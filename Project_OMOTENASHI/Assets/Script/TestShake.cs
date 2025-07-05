using UnityEngine;

public class TestShake : MonoBehaviour
{
   [SerializeField,Header("—h‚ç‚·‚à‚Ì")] 
    private CameraShake[] shake;


    [SerializeField, Header("—h‚ç‚·ŽžŠÔ")]
    private float duration;
    [SerializeField, Header("—h‚ç‚·‹­‚³")]
    private float magnitude;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            shake[0].Shake(duration, magnitude);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            shake[1].Shake(duration, magnitude);
        }
       

    }
}
