using UnityEngine;

public class HitboxController : MonoBehaviour
{
    public SpriteAnim spriteAnim; // SpriteAnimÇ÷ÇÃéQè∆
    private BoxCollider2D hitbox;

    [System.Serializable]
    public class HitboxData
    {
        public Vector2 offset;
        public Vector2 size;
        public bool enableHitbox;
    }

    [System.Serializable]
    public class AttackPattern
    {
        public PlayerState.State state; // ó·ÅFAttack, SpecialAttack, etc
        public HitboxData[] frames;
    }

    public AttackPattern[] attackPatterns;

    void Start()
    {
        hitbox = GetComponent<BoxCollider2D>();
        if (spriteAnim == null)
        {
            spriteAnim = GetComponent<SpriteAnim>();
        }
    }

    void Update()
    {
        var currentState = spriteAnim.GetCurrentState();
        var currentFrame = spriteAnim.GetCurrentFrame();

        foreach (var pattern in attackPatterns)
        {
            if (pattern.state == currentState)
            {
                if (currentFrame < pattern.frames.Length)
                {
                    var data = pattern.frames[currentFrame];

                    hitbox.enabled = data.enableHitbox;
                    if (data.enableHitbox)
                    {
                        hitbox.offset = data.offset;
                        hitbox.size = data.size;
                    }
                }
                else
                {
                    hitbox.enabled = false;
                }
                return;
            }
        }

        // çUåÇíÜÇ≈Ç»Ç¢èÛë‘
        hitbox.enabled = false;
    }
}
