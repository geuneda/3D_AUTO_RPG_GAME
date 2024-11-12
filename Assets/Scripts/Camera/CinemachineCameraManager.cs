using Unity.Cinemachine;
using UnityEngine;

public class CinemachineCameraManager : MonoBehaviour
{
    [Header("카메라 참조")]
    [SerializeField] private CinemachineCamera followCam;    // 기본 팔로우 카메라
    [SerializeField] private CinemachineCamera combatCam;    // 전투용 카메라
    
    [Header("카메라 설정")]
    [SerializeField] private float normalPriority = 10;            // 기본 우선순위
    [SerializeField] private float combatPriority = 15;           // 전투시 우선순위

    private void Start()
    {
        // 기본 설정
        if (followCam != null)
        {
            followCam.Priority = (PrioritySettings)normalPriority;
        }
        
        if (combatCam != null)
        {
            combatCam.Priority = (PrioritySettings)(normalPriority - 1);
        }
        
        // 플레이어 찾아서 카메라 타겟 설정
        Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
        {
            SetupCameras(player);
        }
    }

    // 카메라 초기 설정
    private void SetupCameras(Transform target)
    {
        if (followCam != null)
        {
            followCam.Follow = target;
            followCam.LookAt = target;
        }
        
        if (combatCam != null)
        {
            combatCam.Follow = target;
            combatCam.LookAt = target;
        }
    }

    // 전투 상태에 따른 카메라 전환
    public void SetCombatState(bool inCombat)
    {
        if (followCam != null && combatCam != null)
        {
            followCam.Priority = (PrioritySettings)(inCombat ? normalPriority - 1 : normalPriority);
            combatCam.Priority = (PrioritySettings)(inCombat ? combatPriority : normalPriority - 1);
        }
    }
} 