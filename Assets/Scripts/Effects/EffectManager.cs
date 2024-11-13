using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : Singleton<EffectManager>
{
    [Header("Effects")]
    [SerializeField] private ParticleSystem attackEffectPrefab;
    [SerializeField] private ParticleSystem levelUpEffectPrefab;

    [Header("Pool Settings")]
    [SerializeField] private int initialPoolSize = 5;

    private Dictionary<ParticleSystem, ObjectPool<ParticleSystem>> effectPools;
    private Dictionary<ParticleSystem, ParticleSystem> instanceToPrefabMap;
    private Transform poolContainer;

    protected override void Awake()
    {
        base.Awake();
        InitializePools();
    }

    private void InitializePools()
    {
        poolContainer = new GameObject("EffectPools").transform;
        poolContainer.parent = transform;
        
        effectPools = new Dictionary<ParticleSystem, ObjectPool<ParticleSystem>>();
        instanceToPrefabMap = new Dictionary<ParticleSystem, ParticleSystem>();
        
        CreatePool(attackEffectPrefab);
        CreatePool(levelUpEffectPrefab);
    }

    private void CreatePool(ParticleSystem prefab)
    {
        if (prefab == null) return;
        
        Transform container = new GameObject(prefab.name + "Pool").transform;
        container.parent = poolContainer;
        
        var pool = new ObjectPool<ParticleSystem>(prefab, initialPoolSize, container);
        effectPools[prefab] = pool;
    }

    private void PlayEffect(ParticleSystem prefab, Vector3 position)
    {
        if (prefab == null || !effectPools.ContainsKey(prefab)) return;

        var effect = effectPools[prefab].Get();
        effect.transform.position = position;
        effect.Play();

        instanceToPrefabMap[effect] = prefab;
        StartCoroutine(ReturnToPool(effect, effect.main.duration));
    }

    private IEnumerator ReturnToPool(ParticleSystem effect, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (effect != null && instanceToPrefabMap.ContainsKey(effect))
        {
            effect.Stop();
            var prefab = instanceToPrefabMap[effect];
            effectPools[prefab].Return(effect);
            instanceToPrefabMap.Remove(effect);
        }
    }

    public void PlayAttackEffect(Vector3 position) => PlayEffect(attackEffectPrefab, position);
    public void PlayLevelUpEffect(Vector3 position) => PlayEffect(levelUpEffectPrefab, position);
} 