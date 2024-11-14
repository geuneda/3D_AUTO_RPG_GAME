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
        
        eventManager.OnPlayerSpawned += SetupCameras;
        eventManager.OnPlayerDeath += HandlePlayerDeath;
        eventManager.OnEnemyDeath += HandleEnemyDeath;
    }

    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.OnPlayerSpawned -= SetupCameras;
            eventManager.OnPlayerDeath -= HandlePlayerDeath;
            eventManager.OnEnemyDeath -= HandleEnemyDeath;
        }
    }

    private void SetupCameras(GameObject player)
    {
        if (player)
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
        if (followCam && combatCam)
        {
            followCam.Priority = (PrioritySettings)(inCombat ? normalPriority - 1 : normalPriority);
            combatCam.Priority = (PrioritySettings)(inCombat ? combatPriority : normalPriority - 1);
        }
    }

    private void HandlePlayerDeath()
    {
        if (combatCam)
        {
            combatCam.Priority = (PrioritySettings)normalPriority;
        }
    }

    private void HandleEnemyDeath(EnemyType enemyType)
    {
        SetCombatState(false);
    }
} 