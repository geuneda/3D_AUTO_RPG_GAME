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
        
        if (stats != null)
        {
            agent.speed = stats.moveSpeed;
            agent.stoppingDistance = stats.attackRange * 0.8f;
        }
    }
    
    private async void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        _ = StartAIUpdateLoop();
    }
    
    private async Awaitable StartAIUpdateLoop()
    {
        while (true)
        {
            await Awaitable.NextFrameAsync();
            
            if (target == null || stats == null) continue;
            
            float distanceToTarget = Vector3.SqrMagnitude(transform.position - target.position);
            float detectionRangeSqr = stats.detectionRange * stats.detectionRange;
            
            if (distanceToTarget <= detectionRangeSqr)
            {
                float attackRangeSqr = stats.attackRange * stats.attackRange;
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