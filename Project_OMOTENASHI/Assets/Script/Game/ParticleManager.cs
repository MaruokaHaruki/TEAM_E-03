using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance { get; private set; }
    
    [Header("Collision Effects")]
    public ParticleSystem playerCollisionEffect;
    public ParticleSystem wallCollisionEffect;
    
    [Header("Movement Effects")]
    public ParticleSystem movementTrailEffect;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // プレイヤー同士の衝突エフェクト
    public void PlayPlayerCollisionEffect(Vector3 position)
    {
        if (playerCollisionEffect != null)
        {
            playerCollisionEffect.transform.position = position;
            playerCollisionEffect.Play();
        }
    }
    
    // 壁との衝突エフェクト
    public void PlayWallCollisionEffect(Vector3 position)
    {
        if (wallCollisionEffect != null)
        {
            wallCollisionEffect.transform.position = position;
            wallCollisionEffect.Play();
        }
    }
    
    // 移動中のトレイルエフェクト開始
    public void StartMovementEffect(Transform target)
    {
        if (movementTrailEffect != null)
        {
            movementTrailEffect.transform.SetParent(target);
            movementTrailEffect.transform.localPosition = Vector3.zero;
            movementTrailEffect.Play();
        }
    }
    
    // 移動中のトレイルエフェクト停止
    public void StopMovementEffect()
    {
        if (movementTrailEffect != null)
        {
            movementTrailEffect.Stop();
        }
    }
}
