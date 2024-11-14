using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : Singleton<EffectManager>
{
    [Header("이펙트 프리팹")]
    [SerializeField] private GameObject levelUpEffectPrefab;
    [SerializeField] private GameObject attackEffectPrefab;
    
    private ObjectPool<ParticleSystem> levelUpEffectPool;
    private ObjectPool<ParticleSystem> attackEffectPool;
    private GameEventManager eventManager;
    
    private const int PoolSize = 5;

    protected override void Awake()
    {
        base.Awake();
        InitializePools();
        eventManager = GameEventManager.Instance;
    }

    private void Start()
    {
        eventManager.OnPlayerLevelUp += HandleLevelUp;
    }

    private void OnDestroy()
    {
        if (eventManager)
        {
            eventManager.OnPlayerLevelUp -= HandleLevelUp;
        }
    }

    private void InitializePools()
    {
        if (levelUpEffectPrefab && levelUpEffectPrefab.TryGetComponent<ParticleSystem>(out var levelUpPS))
            levelUpEffectPool = new ObjectPool<ParticleSystem>(levelUpPS, PoolSize, transform);

        if (attackEffectPrefab && attackEffectPrefab.TryGetComponent<ParticleSystem>(out var attackPS))
            attackEffectPool = new ObjectPool<ParticleSystem>(attackPS, PoolSize, transform);
    }

    private void HandleLevelUp(int level)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player)
        {
            PlayLevelUpEffect(player.transform.position);
        }
    }

    public void PlayLevelUpEffect(Vector3 position)
    {
        if (levelUpEffectPool == null) return;
        var effect = levelUpEffectPool.Get();
        effect.transform.position = position;
        StartCoroutine(ReturnToPool(effect, effect.main.duration, levelUpEffectPool));
    }

    public void PlayAttackEffect(Vector3 position)
    {
        if (attackEffectPool == null) return;
        var effect = attackEffectPool.Get();
        effect.transform.position = position;
        StartCoroutine(ReturnToPool(effect, effect.main.duration, attackEffectPool));
    }

    private IEnumerator ReturnToPool(ParticleSystem effect, float duration, ObjectPool<ParticleSystem> pool)
    {
        effect.Play();
        yield return new WaitForSeconds(duration);
        pool.Return(effect);
    }
} 