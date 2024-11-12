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
        var mapGenerator = FindFirstObjectByType<MapGenerator>();
        if (mapGenerator != null)
        {
            mapGenerator.OnPlayerSpawned += SetupCameras;
        }
    }

    // 카메라 초기 설정
    private void SetupCameras(GameObject player)
    {
        if (player != null)
        {
            followCam.Follow = player.transform;
            followCam.LookAt = player.transform;
            combatCam.Follow = player.transform;
            combatCam.LookAt = player.transform;
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