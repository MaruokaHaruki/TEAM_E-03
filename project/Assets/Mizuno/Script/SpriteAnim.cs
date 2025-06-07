using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnim : MonoBehaviour
{
    public Sprite[] IdleSprites;
    public Sprite[] RunSprites;
    public Sprite[] JumpSprites;
    public Sprite[] AttackSprites;

    private SpriteRenderer spriteRenderer;
    private Player player;

    private int currentFrame = 0;
    private float timer = 0f;
    public float frameDelay = 0.2f;

    private Player.State lastState;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GetComponent<Player>(); 
        //  �������I�u�W�F�N�g�ɂ���O��
        //  Player �� SpriteAnim ��ʃI�u�W�F�N�g�ɕ������ꍇ�� public Player player;

        lastState = player.CurrentState;
        SetFirstFrame(lastState);
    }

    void Update()
    {
        //�v���C���[�̏��
        Player.State currentState = player.CurrentState;

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

    void SetFirstFrame(Player.State state)
    {
        Sprite[] sprites = GetSpritesForState(state);
        if (sprites.Length > 0)
        {
            spriteRenderer.sprite = sprites[0];
        }
    }

    void AdvanceFrame(Player.State state)
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

    Sprite[] GetSpritesForState(Player.State state)
    {
        switch (state)
        {
            case Player.State.Idle: return IdleSprites;
            case Player.State.Run: return RunSprites;
            case Player.State.Jump: return JumpSprites;
            case Player.State.Attack: return AttackSprites;
            default: return IdleSprites;
        }
    }
}
