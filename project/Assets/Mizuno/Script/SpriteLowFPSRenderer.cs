using UnityEngine;
using UnityEngine.UI;

public class SpriteLowFPSRenderer : MonoBehaviour
{
    public Camera spriteCamera;
    public RawImage outputImage;
    public int targetFPS = 2;
    public int renderWidth = 640;
    public int renderHeight = 360;

    private RenderTexture renderTexture;
    private float frameInterval;
    private float timer;

    void Start()
    {
        renderTexture = new RenderTexture(renderWidth, renderHeight, 24);
        spriteCamera.targetTexture = renderTexture;
        outputImage.texture = renderTexture;

        frameInterval = 0.5f;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= frameInterval)
        {
            spriteCamera.Render();
            timer = 0f;
        }
    }
}
