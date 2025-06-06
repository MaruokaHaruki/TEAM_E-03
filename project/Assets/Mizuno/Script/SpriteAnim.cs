using UnityEngine;

public class SpriteAnim : MonoBehaviour
{
    public Sprite[] animationFrames; // アニメーション用スプライト群
    public KeyCode advanceKey = KeyCode.RightArrow; // アニメーションを進めるキー

    private SpriteRenderer spriteRenderer;
    private int currentFrame = 0;

    private bool lowFps ; // 低FPSモードのフラグ

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (animationFrames.Length > 0)
        {
            spriteRenderer.sprite = animationFrames[0];
        }
        
        lowFps = false; // 初期状態では低FPSモードではない
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            lowFps = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            lowFps = false;
        }


        if (lowFps)
        {

            AdvanceFrame();
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            AdvanceFrame();
        }
    }

    void AdvanceFrame()
    {
        if (animationFrames.Length == 0) return;

        currentFrame++;
        if (currentFrame >= animationFrames.Length)
        {
            currentFrame = 0; // ループする場合。止めたいなら return;
        }

        spriteRenderer.sprite = animationFrames[currentFrame];
    }
}
