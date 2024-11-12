using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [Header("컴포넌트")]
    private NavMeshAgent agent;
    private Animator animator;
    private PlayerStats stats;
    private PlayerHealth health;
    
    [Header("전투 설정")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private LayerMask enemyLayer;
    
    [Header("자동전투")]
    [SerializeField] private bool isAutoMode = true;
    [SerializeField] private float pathUpdateRate = 0.2f;
    private float lastPathUpdateTime;
    private GameObject currentTarget;
    private float lastAttackTime;
    private Vector3 finalDestination;
    private bool isMovingToFinalDestination = true;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        stats = GetComponent<PlayerStats>();
        health = GetComponent<PlayerHealth>();

        agent.speed = stats.MoveSpeed;
        agent.stoppingDistance = attackRange * 0.8f;
    }

    private void Update()
    {
        if (!isAutoMode) return;

        if (Time.time - lastPathUpdateTime > pathUpdateRate)
        {
            UpdateCombatBehavior();
            lastPathUpdateTime = Time.time;
        }
    }

    private void UpdateCombatBehavior()
    {
        if (currentTarget == null || !currentTarget.activeInHierarchy)
        {
            FindNewTarget();
        }

        if (currentTarget != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);

            if (distanceToTarget <= attackRange)
            {
                StopMoving();
                AttackTarget();
            }
            else
            {
                MoveToTarget(currentTarget.transform.position);
            }
            isMovingToFinalDestination = false;
        }

        else if (isMovingToFinalDestination)
        {
            MoveToFinalDestination();
        }
    }

    private void FindNewTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange, enemyLayer);
        
        float closestDistance = float.MaxValue;
        GameObject closestEnemy = null;

        foreach (Collider col in colliders)
        {
            if (!col.gameObject.activeInHierarchy) continue;

            float distance = Vector3.Distance(transform.position, col.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = col.gameObject;
            }
        }

        currentTarget = closestEnemy;
    }

    private void AttackTarget()
    {
        if (Time.time - lastAttackTime < 1f / stats.AttackSpeed) return;

        // 공격 애니메이션 실행
        animator?.SetTrigger("Attack");

        // 데미지 처리
        if (currentTarget.TryGetComponent<EnemyHealth>(out var enemyHealth))
        {
            enemyHealth.TakeDamage(stats.AttackPower);
        }

        lastAttackTime = Time.time;
    }

    private void MoveToTarget(Vector3 targetPosition)
    {
        if (!agent.isOnNavMesh) return;

        agent.isStopped = false;
        agent.SetDestination(targetPosition);
        UpdateMovementAnimation();
    }

    private void MoveToFinalDestination()
    {
        if (!agent.isOnNavMesh) return;

        if (Vector3.Distance(transform.position, finalDestination) <= agent.stoppingDistance)
        {
            StopMoving();
            // 스테이지 클리어 처리
            // GameManager.Instance?.OnStageClear();
        }
        else
        {
            MoveToTarget(finalDestination);
        }
    }

    private void StopMoving()
    {
        if (!agent.isOnNavMesh) return;

        agent.isStopped = true;
        UpdateMovementAnimation();
    }

    private void UpdateMovementAnimation()
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
    }

    public void SetAutoMode(bool enabled)
    {
        isAutoMode = enabled;
        if (!enabled)
        {
            StopMoving();
            currentTarget = null;
        }
    }

    public void SetFinalDestination(Vector3 destination)
    {
        finalDestination = destination;
        isMovingToFinalDestination = true;
    }

    // 디버그용 기즈모
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
} 