using UnityEngine;

public class SpriteAnim : MonoBehaviour
{
    public Sprite[] animationFrames; // �A�j���[�V�����p�X�v���C�g�Q
    public KeyCode advanceKey = KeyCode.RightArrow; // �A�j���[�V������i�߂�L�[

    private SpriteRenderer spriteRenderer;
    private int currentFrame = 0;

    private bool lowFps ; // ��FPS���[�h�̃t���O

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (animationFrames.Length > 0)
        {
            spriteRenderer.sprite = animationFrames[0];
        }
        
        lowFps = false; // ������Ԃł͒�FPS���[�h�ł͂Ȃ�
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
            currentFrame = 0; // ���[�v����ꍇ�B�~�߂����Ȃ� return;
        }

        spriteRenderer.sprite = animationFrames[currentFrame];
    }
}
