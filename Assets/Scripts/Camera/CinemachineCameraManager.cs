using Unity.Cinemachine;
using UnityEngine;

public class CinemachineCameraManager : MonoBehaviour
{
    [Header("카메라 참조")]
    [SerializeField] private CinemachineCamera followCam;
    [SerializeField] private CinemachineCamera combatCam;
    
    [Header("카메라 설정")]
    [SerializeField] private float normalPriority = 10; 
    [SerializeField] private float combatPriority = 15;

    private GameEventManager eventManager;
    private GameObject player;

    private void Start()
    {
        eventManager = GameEventManager.Instance;
        var mapGenerator = FindFirstObjectByType<MapGenerator>();
        
        if (mapGenerator != null)
        {
            mapGenerator.OnPlayerSpawned += SetupCameras;
        }

        eventManager.OnPlayerDeath += HandlePlayerDeath;
        eventManager.OnEnemyDeath += HandleEnemyDeath;
    }

    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.OnPlayerDeath -= HandlePlayerDeath;
            eventManager.OnEnemyDeath -= HandleEnemyDeath;
        }
    }

    private void SetupCameras(GameObject player)
    {
        if (player != null)
        {
            this.player = player;
            followCam.Follow = player.transform;
            followCam.LookAt = player.transform;
            combatCam.Follow = player.transform;
            combatCam.LookAt = player.transform;
        }
    }

    public void SetCombatState(bool inCombat)
    {
        if (followCam != null && combatCam != null)
        {
            followCam.Priority = (PrioritySettings)(inCombat ? normalPriority - 1 : normalPriority);
            combatCam.Priority = (PrioritySettings)(inCombat ? combatPriority : normalPriority - 1);
        }
    }

    private void HandlePlayerDeath()
    {
        if (combatCam != null)
        {
            combatCam.Priority = (PrioritySettings)normalPriority;
        }
    }

    private void HandleEnemyDeath()
    {
        // TODO : 처치 후 효과
    }
} 