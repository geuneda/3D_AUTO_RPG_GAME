using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("이동 관련")]
    [SerializeField] private float detectionRadius = 10f;    // 몬스터 감지 범위
    [SerializeField] private float stopDistance = 2f;        // 공격을 위한 정지 거리
    [SerializeField] private LayerMask enemyLayer;           // 적 레이어
    
    [Header("컴포넌트 참조")]
    private CharacterController characterController;
    private Animator animator;
    private PlayerStats stats;
    
    [Header("상태 변수")]
    private Vector3 targetPosition;              // 목표 위치
    private GameObject currentTarget;            // 현재 타겟
    private bool isMoving = true;               // 이동 중인지 여부
    private bool isAttacking = false;           // 공격 중인지 여부
    private float lastAttackTime;               // 마지막 공격 시간
    
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        stats = Resources.Load<PlayerStats>("PlayerStats");
    }
    
    private void Update()
    {
        AutoBehavior();
    }
    
    private void AutoBehavior()
    {
        // 현재 타겟이 없으면 새로 찾기
        if (currentTarget == null || !currentTarget.activeSelf)
        {
            FindNewTarget();
        }
        
        if (currentTarget != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);
            
            // 범위 체크
            if (distanceToTarget > stopDistance)
            {
                MoveToTarget();
            }
            // 공격
            else
            {
                AttackTarget();
            }
        }
        else
        {
            // 타겟이 없으면 보스한테 이동
            MoveToFinalDestination();
        }
        
        // 중력
        if (!characterController.isGrounded)
        {
            characterController.Move(Vector3.down * 9.81f * Time.deltaTime);
        }
    }
    
    private void FindNewTarget()
    {
        // 주변 적 탐지
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);
        
        if (hitColliders.Length > 0)
        {
            // 가장 가까운 적을 선택
            float closestDistance = float.MaxValue;
            foreach (var hitCollider in hitColliders)
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    currentTarget = hitCollider.gameObject;
                }
            }
        }
    }
    
    // 타겟을 향해 이동
    private void MoveToTarget()
    {
        Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
        
        // 회전적용
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        
        // 이동
        characterController.Move(direction * stats.moveSpeed * Time.deltaTime);
        animator?.SetFloat("Speed", 1f);
    }
    
    private void AttackTarget()
    {
        // 타겟 방향으로 회전
        Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(direction);
        
        // 공격 쿨다운 체크
        if (Time.time - lastAttackTime >= 1f / stats.attackSpeed)
        {
            animator?.SetTrigger("Attack");
            
            // 데미지 처리
            if (currentTarget.TryGetComponent<EnemyHealth>(out var enemyHealth))
            {
                float damage = stats.attackPower;
                // 크확 계산
                if (Random.value <= stats.criticalChance)
                {
                    damage *= stats.criticalDamage;
                }
                enemyHealth.TakeDamage(damage);
            }
            
            lastAttackTime = Time.time;
        }
        
        animator?.SetFloat("Speed", 0f);
    }
    
    // 최종 목적지로 이동
    private void MoveToFinalDestination()
    {
        // 스테이지의 최종 목적지 위치를 가져와서 이동
        // StageManager에서 목적지 정보를 가져와야 함
        // 임시로 forward 방향으로 이동
        characterController.Move(transform.forward * stats.moveSpeed * Time.deltaTime);
        animator?.SetFloat("Speed", 1f);
    }
} 