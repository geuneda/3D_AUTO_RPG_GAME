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
    private GameObject bossTarget;
    
    [Header("자동전투")]
    [SerializeField] private bool isAutoMode = true; // 자동전투 여부
    [SerializeField] private float pathUpdateRate = 0.2f;
    private float lastPathUpdateTime;
    private GameObject currentTarget;
    private float lastAttackTime;
    private Vector3 finalDestination;
    private bool isMovingToFinalDestination = true;

    private Vector3 bossPosition;

    private float targetCheckInterval = 0.2f;  // 타겟 체크 주기
    private float lastTargetCheckTime;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        stats = GetComponent<PlayerStats>();
        health = GetComponent<PlayerHealth>();

        agent.speed = stats.MoveSpeed;
        agent.stoppingDistance = attackRange * 0.8f;
    }

    private void Start()
    {
        if (bossTarget != null)
        {
            bossPosition = bossTarget.transform.position;
        }
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
        if (bossTarget == null) return;  // 보스가 아직 생성되지 않았으면 리턴

        // 타겟이 없거나, 현재 타겟이 일반 몬스터이면서 죽었을 때 새 타겟 찾기
        if (currentTarget == null || 
            (currentTarget != bossTarget && 
            (!currentTarget.activeInHierarchy || currentTarget.GetComponent<EnemyHealth>()?.IsDead == true)))
        {
            FindNewTarget();
        }

        // 주변에 일반 몬스터가 있으면 처치
        if (currentTarget != null && currentTarget != bossTarget)
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
        }
        // 보스가 있으면 보스로 이동
        else if (bossTarget != null)
        {
            bossPosition = bossTarget.transform.position;
            float distanceToBoss = Vector3.Distance(transform.position, bossPosition);
            
            if (distanceToBoss <= attackRange)
            {
                StopMoving();
                currentTarget = bossTarget;
                AttackTarget();
            }
            else
            {
                MoveToTarget(bossPosition);
            }
        }
        // 보스도 없고 주변 몬스터도 없으면 제자리에 대기
        else
        {
            StopMoving();
        }
    }

    private void FindNewTarget()
    {
        // 주변의 일반 몬스터만 찾기
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange, enemyLayer);
        
        float closestDistance = float.MaxValue;
        GameObject closestEnemy = null;
        
        foreach (Collider col in colliders)
        {
            if (!col.gameObject.activeInHierarchy || 
                col.GetComponent<EnemyHealth>()?.IsDead == true ||
                col.gameObject == bossTarget)  // 보스는 제외
                continue;
            
            float distance = Vector3.Distance(transform.position, col.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = col.gameObject;
            }
        }
        
        currentTarget = closestEnemy;  // 주변에 일반 몬스터가 없으면 null
    }

    private void AttackTarget()
    {
        if (Time.time - lastAttackTime < 1f / stats.AttackSpeed) return;
        if (currentTarget == null) return;

        Vector3 directionToTarget = currentTarget.transform.position - transform.position;
        directionToTarget.y = 0;
        transform.rotation = Quaternion.LookRotation(directionToTarget);

        animator?.SetTrigger("Attack");

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

    public void SetBossTarget(GameObject boss)
    {
        bossTarget = boss;
        if (bossTarget != null)
        {
            bossPosition = bossTarget.transform.position;
        }
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