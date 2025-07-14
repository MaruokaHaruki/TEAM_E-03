using UnityEngine;
using System.Collections.Generic;

public enum EffectType
{
    PlayerCollision,
    WallCollision,
    MovementTrail,
    Explosion,
    Pickup,
    Damage
}

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance { get; private set; }
    
    [Header("Effect Prefabs")]
    public ParticleSystem playerCollisionEffect;
    public ParticleSystem wallCollisionEffect;
    public ParticleSystem movementTrailEffect;
    public ParticleSystem explosionEffect;
    public ParticleSystem pickupEffect;
    public ParticleSystem damageEffect;
    
    [Header("Pool Settings")]
    public int poolSize = 10;
    
    private Dictionary<EffectType, Queue<ParticleSystem>> effectPools = new Dictionary<EffectType, Queue<ParticleSystem>>();
    private Dictionary<EffectType, ParticleSystem> effectPrefabs = new Dictionary<EffectType, ParticleSystem>();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeEffectSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeEffectSystem()
    {
        // プレハブの辞書を設定
        effectPrefabs[EffectType.PlayerCollision] = playerCollisionEffect;
        effectPrefabs[EffectType.WallCollision] = wallCollisionEffect;
        effectPrefabs[EffectType.MovementTrail] = movementTrailEffect;
        effectPrefabs[EffectType.Explosion] = explosionEffect;
        effectPrefabs[EffectType.Pickup] = pickupEffect;
        effectPrefabs[EffectType.Damage] = damageEffect;
        
        // プールを初期化
        foreach (var kvp in effectPrefabs)
        {
            if (kvp.Value != null)
            {
                CreatePool(kvp.Key, kvp.Value);
            }
        }
    }
    
    private void CreatePool(EffectType type, ParticleSystem prefab)
    {
        effectPools[type] = new Queue<ParticleSystem>();
        
        for (int i = 0; i < poolSize; i++)
        {
            ParticleSystem instance = Instantiate(prefab, transform);
            instance.gameObject.SetActive(false);
            effectPools[type].Enqueue(instance);
        }
    }
    
    // 汎用エフェクト再生メソッド
    public void PlayEffect(EffectType type, Vector3 position, float duration = 0f)
    {
        if (!effectPools.ContainsKey(type) || effectPools[type].Count == 0) return;
        
        ParticleSystem effect = effectPools[type].Dequeue();
        effect.transform.position = position;
        effect.gameObject.SetActive(true);
        effect.Play();
        
        if (duration > 0)
        {
            StartCoroutine(ReturnToPoolAfterTime(effect, type, duration));
        }
        else
        {
            StartCoroutine(ReturnToPoolWhenStopped(effect, type));
        }
    }
    
    // 回転付きエフェクト再生
    public void PlayEffect(EffectType type, Vector3 position, Quaternion rotation, float duration = 0f)
    {
        if (!effectPools.ContainsKey(type) || effectPools[type].Count == 0) return;
        
        ParticleSystem effect = effectPools[type].Dequeue();
        effect.transform.position = position;
        effect.transform.rotation = rotation;
        effect.gameObject.SetActive(true);
        effect.Play();
        
        if (duration > 0)
        {
            StartCoroutine(ReturnToPoolAfterTime(effect, type, duration));
        }
        else
        {
            StartCoroutine(ReturnToPoolWhenStopped(effect, type));
        }
    }
    
    // 既存のメソッドを新しいシステムに適応
    public void PlayPlayerCollisionEffect(Vector3 position)
    {
        PlayEffect(EffectType.PlayerCollision, position);
    }
    
    public void PlayWallCollisionEffect(Vector3 position)
    {
        PlayEffect(EffectType.WallCollision, position);
    }
    
    public void StartMovementEffect(Transform target)
    {
        PlayEffect(EffectType.MovementTrail, target.position);
    }
    
    public void StopMovementEffect()
    {
        StopAllEffects(EffectType.MovementTrail);
    }
    
    // 新しい便利メソッド
    public void PlayExplosionEffect(Vector3 position)
    {
        PlayEffect(EffectType.Explosion, position);
    }
    
    public void PlayPickupEffect(Vector3 position)
    {
        PlayEffect(EffectType.Pickup, position, 2f);
    }
    
    public void PlayDamageEffect(Vector3 position)
    {
        PlayEffect(EffectType.Damage, position, 1f);
    }
    
    public void StopAllEffects(EffectType type)
    {
        ParticleSystem[] allEffects = FindObjectsOfType<ParticleSystem>();
        foreach (var effect in allEffects)
        {
            if (effect.gameObject.activeInHierarchy && effect.transform.parent == transform)
            {
                effect.Stop();
            }
        }
    }
    
    private System.Collections.IEnumerator ReturnToPoolAfterTime(ParticleSystem effect, EffectType type, float time)
    {
        yield return new WaitForSeconds(time);
        ReturnToPool(effect, type);
    }
    
    private System.Collections.IEnumerator ReturnToPoolWhenStopped(ParticleSystem effect, EffectType type)
    {
        yield return new WaitUntil(() => !effect.isPlaying);
        ReturnToPool(effect, type);
    }
    
    private void ReturnToPool(ParticleSystem effect, EffectType type)
    {
        effect.gameObject.SetActive(false);
        effectPools[type].Enqueue(effect);
    }
}
