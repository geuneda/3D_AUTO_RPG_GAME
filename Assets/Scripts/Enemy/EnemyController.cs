using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    [Header("컴포넌트")]
    private NavMeshAgent agent;
    private EnemyHealth healthSystem;
    private Animator animator;
    
    [Header("스탯")]
    [SerializeField] private EnemyStats stats;
    
    [Header("전투")]
    private Transform target;
    private bool isAttacking;
    private float lastAttackTime;
    
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        healthSystem = GetComponent<EnemyHealth>();
        animator = GetComponent<Animator>();

        if (!stats) return;
        agent.speed = stats.moveSpeed;
        agent.stoppingDistance = stats.attackRange * 0.8f;
    }

#pragma warning disable CS1998 // 이 비동기 메서드에는 'await' 연산자가 없으며 메서드가 동시에 실행됩니다.
    private async void Start()
#pragma warning restore CS1998 // 이 비동기 메서드에는 'await' 연산자가 없으며 메서드가 동시에 실행됩니다.
    {
        target = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        _ = StartAIUpdateLoop();
    }
    
    private async Awaitable StartAIUpdateLoop()
    {
        while (true)
        {
            await Awaitable.NextFrameAsync();
            
            if (!target || !stats) continue;
            
            var distanceToTarget = Vector3.SqrMagnitude(transform.position - target.position);
            var detectionRangeSqr = stats.detectionRange * stats.detectionRange;
            
            if (distanceToTarget <= detectionRangeSqr)
            {
                var attackRangeSqr = stats.attackRange * stats.attackRange;
                if (distanceToTarget <= attackRangeSqr)
                {
                    StopMoving();
                    AttackTarget();
                }
                else
                {
                    MoveToTarget();
                }
            }
            else
            {
                StopMoving();
            }
        }
    }
    
    private void MoveToTarget()
    {
        if (!agent.isOnNavMesh) return;
        
        agent.isStopped = false;
        agent.SetDestination(target.position);
        //animator?.SetBool("IsMoving", true);
    }
    
    private void StopMoving()
    {
        if (!agent.isOnNavMesh) return;
        
        agent.isStopped = true;
        //animator?.SetBool("IsMoving", false);
    }
    
    private void AttackTarget()
    {
        if (Time.time - lastAttackTime < 1f / stats.attackSpeed) return;
        
        //animator?.SetTrigger("Attack");
        
        if (target.TryGetComponent<PlayerHealth>(out var playerHealth))
        {
            playerHealth.TakeDamage(stats.attackPower);
        }
        
        lastAttackTime = Time.time;
    }
    
    public EnemyStats GetStats()
    {
        return stats;
    }
} 