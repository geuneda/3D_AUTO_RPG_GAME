using UnityEngine;

// 플레이어의 이동과 기본 동작을 제어하는 메인 컴포넌트
public class PlayerController : MonoBehaviour
{
    [Header("컴포넌트 참조")]
    private CharacterController characterController;  // Unity의 CharacterController 컴포넌트
    private Animator animator;                        // 애니메이터 컴포넌트
    
    [Header("이동 관련")]
    [SerializeField] private float rotationSpeed = 10f;  // 회전 속도
    private Vector3 moveDirection;                       // 이동 방향
    private PlayerStats stats;                          // 플레이어 스탯
    
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        
        // Resources 폴더에서 플레이어 스탯 데이터를 로드
        stats = Resources.Load<PlayerStats>("PlayerStats");
    }
    
    private void Update()
    {
        HandleMovement();
    }
    
    // 플레이어 이동 처리
    private void HandleMovement()
    {
        // 수평, 수직 입력값 받기
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        // 이동 방향 계산
        Vector3 movement = new Vector3(horizontal, 0f, vertical).normalized;
        
        if (movement.magnitude >= 0.1f)
        {
            // 이동 방향으로 캐릭터 회전
            float targetAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg;
            float angle = Mathf.LerpAngle(transform.eulerAngles.y, targetAngle, Time.deltaTime * rotationSpeed);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            
            // 캐릭터 이동
            moveDirection = movement;
            characterController.Move(moveDirection * stats.moveSpeed * Time.deltaTime);
            
            // 애니메이션 설정
            animator?.SetFloat("Speed", movement.magnitude);
        }
        else
        {
            // 정지 상태 애니메이션
            animator?.SetFloat("Speed", 0f);
        }
        
        // 중력 적용
        if (!characterController.isGrounded)
        {
            characterController.Move(Vector3.down * 9.81f * Time.deltaTime);
        }
    }
} 