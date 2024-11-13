using UnityEngine;
using UnityEngine.AI;
using Unity.Cinemachine;

public class PlayerController : MonoBehaviour
{
    [Header("전투 설정")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private LayerMask enemyLayer;
    
    private GameObject currentTarget;
    private float lastAttackTime;
    private float lastPathUpdateTime;
    private float pathUpdateRate = 0.2f;
    private bool isAutoMode = true;

    private NavMeshAgent agent;
    private Animator animator;
    private PlayerStats stats;
    private CinemachineCameraManager cameraManager;
    private bool isInCombat;
    private EffectManager effectManager;

    [Header("Audio")]
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private float attackSoundVolume = 0.8f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        stats = GetComponent<PlayerStats>();
        cameraManager = FindFirstObjectByType<CinemachineCameraManager>();
        effectManager = EffectManager.Instance;

        if (stats != null)
        {
            agent.speed = stats.MoveSpeed;
            agent.stoppingDistance = attackRange * 0.8f;
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
        // 주변 몬스터 찾기
        if (currentTarget == null || !currentTarget.activeInHierarchy || 
            currentTarget.GetComponent<EnemyHealth>()?.IsDead == true)
        {
            FindNewTarget();
        }

        // 전투 상태 체크 및 카메라 전환
        bool newCombatState = currentTarget != null;
        if (newCombatState != isInCombat)
        {
            isInCombat = newCombatState;
            cameraManager?.SetCombatState(isInCombat);
        }

        // 주변에 몬스터가 있으면 처치
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
        }
        // 주변에 몬스터가 없으면 맵의 끝점으로 이동
        else
        {
            Vector3 endPosition = new Vector3(150f, 1f, 150f);  // 맵의 끝점 위치
            MoveToTarget(endPosition);
        }
    }

    private void FindNewTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange, enemyLayer);
        
        float closestDistance = float.MaxValue;
        GameObject closestEnemy = null;
        
        foreach (Collider col in colliders)
        {
            if (!col.gameObject.activeInHierarchy || 
                col.GetComponent<EnemyHealth>()?.IsDead == true)
                continue;
            
            float distance = Vector3.Distance(transform.position, col.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = col.gameObject;
            }
        }
        
        currentTarget = closestEnemy;
    }

    private void MoveToTarget(Vector3 targetPosition)
    {
        if (!agent.isOnNavMesh) return;
        agent.isStopped = false;
        agent.SetDestination(targetPosition);
        UpdateMovementAnimation();
    }

    private void StopMoving()
    {
        if (!agent.isOnNavMesh) return;
        agent.isStopped = true;
        UpdateMovementAnimation();
    }

    private void AttackTarget()
    {
        if (stats == null) return;
        if (Time.time - lastAttackTime < 1f / stats.AttackSpeed) return;

        if (currentTarget != null)
        {
            Vector3 directionToTarget = currentTarget.transform.position - transform.position;
            directionToTarget.y = 0;
            transform.rotation = Quaternion.LookRotation(directionToTarget);
        }

        animator?.SetTrigger("Attack");

        if (currentTarget != null)
        {
            if (currentTarget.TryGetComponent<EnemyHealth>(out var enemyHealth))
            {
                enemyHealth.TakeDamage(stats.AttackPower);
            }
        }

        lastAttackTime = Time.time;

        Vector3 effectPosition = transform.position + transform.forward * 1f;
        effectManager?.PlayAttackEffect(effectPosition);

        if (attackSound != null)
        {
            AudioManager.Instance.PlayOneShot(attackSound, transform.position, attackSoundVolume);
        }
    }

    private void UpdateMovementAnimation()
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
    }
} 