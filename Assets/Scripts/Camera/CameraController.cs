using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("추적 설정")]
    [SerializeField] private Transform target;               // 추적할 대상 (플레이어)
    [SerializeField] private Vector3 offset = new Vector3(0, 10, -8); // 카메라 오프셋
    [SerializeField] private float smoothSpeed = 5f;         // 카메라 이동 부드러움
    
    [Header("줌 설정")]
    [SerializeField] private float minZoom = 5f;            // 최소 줌 거리
    [SerializeField] private float maxZoom = 15f;           // 최대 줌 거리
    [SerializeField] private float zoomSpeed = 2f;          // 줌 속도
    [SerializeField] private float currentZoom;             // 현재 줌 레벨
    
    [Header("전투 뷰 설정")]
    [SerializeField] private float combatZoomOut = 3f;      // 전투시 추가 줌아웃 거리
    private bool isInCombat;                                // 전투 상태 여부
    
    private void Start()
    {
        // 타겟이 지정되지 않았다면 플레이어 찾기
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
        
        currentZoom = Mathf.Lerp(minZoom, maxZoom, 0.5f); // 중간 줌 레벨로 시작
    }
    
    private void LateUpdate()
    {
        if (target == null) return;
        
        // 전투 상태에 따른 줌 레벨 조정
        float targetZoom = currentZoom + (isInCombat ? combatZoomOut : 0);
        
        // 카메라 위치 계산
        Vector3 desiredPosition = target.position + offset.normalized * targetZoom;
        
        // 부드러운 카메라 이동
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
        
        // 타겟을 바라보도록 회전
        transform.LookAt(target.position);
    }
    
    // 전투 상태 설정
    public void SetCombatState(bool inCombat)
    {
        isInCombat = inCombat;
    }
    
    // 수동 줌 조정 (필요한 경우)
    public void AdjustZoom(float zoomDelta)
    {
        currentZoom = Mathf.Clamp(currentZoom + zoomDelta * zoomSpeed, minZoom, maxZoom);
    }
} 