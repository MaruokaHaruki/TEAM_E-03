using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnim : MonoBehaviour
{
    public Sprite[] IdleSprites;
    public Sprite[] RunSprites;
    public Sprite[] JumpSprites;
    public Sprite[] AttackSprites;

    private SpriteRenderer spriteRenderer;
    private PlayerState playerState;

    private int currentFrame = 0;
    private float timer = 0f;
    public float frameDelay = 0.2f;

    private PlayerState.State lastState;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerState = GetComponent<PlayerState>(); 
        //  �������I�u�W�F�N�g�ɂ���O��
        //  Player �� SpriteAnim ��ʃI�u�W�F�N�g�ɕ������ꍇ�� public Player player;

        lastState = playerState.CurrentState;
        SetFirstFrame(lastState);
    }

    void Update()
    {
        //�v���C���[�̏��
        PlayerState.State currentState = playerState.CurrentState;

        //���O�̏�Ԃƈ��Ȃ���
        if (currentState != lastState)
        {
            currentFrame = 0;
            SetFirstFrame(currentState);
            lastState = currentState;
        }

        timer += Time.deltaTime;
        if (timer >= frameDelay)//�t���[���X�V
        {
            AdvanceFrame(currentState);
            timer = 0f;
        }
    }

    void SetFirstFrame(PlayerState.State state)
    {
        Sprite[] sprites = GetSpritesForState(state);
        if (sprites.Length > 0)
        {
            spriteRenderer.sprite = sprites[0];
        }
    }

    void AdvanceFrame(PlayerState.State state)
    {
        Sprite[] sprites = GetSpritesForState(state);
        if (sprites.Length == 0) return;

        currentFrame++;
        if (currentFrame >= sprites.Length)
        {
            currentFrame = 0;
        }

        spriteRenderer.sprite = sprites[currentFrame];
    }

    Sprite[] GetSpritesForState(PlayerState.State state)
    {
        switch (state)
        {
            case PlayerState.State.Idle: return IdleSprites;
            case PlayerState.State.Run: return RunSprites;
            case PlayerState.State.Jump: return JumpSprites;
            case PlayerState.State.Attack: return AttackSprites;
            default: return IdleSprites;
        }
    }
}
