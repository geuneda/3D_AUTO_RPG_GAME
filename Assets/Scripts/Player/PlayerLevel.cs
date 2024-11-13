using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    [Header("레벨링 설정")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private float currentExp;
    [SerializeField] private float expToNextLevel = 100f;    // 기본 필요 경험치
    [SerializeField] private float expMultiplier = 1.2f;     // 레벨당 필요 경험치 증가율
    
    private GameEventManager eventManager;
    private EffectManager effectManager;
    
    [Header("Audio")]
    [SerializeField] private AudioClip levelUpSound;
    [SerializeField] private float levelUpSoundVolume = 1f;
    
    private void Start()
    {
        eventManager = GameEventManager.Instance;
        effectManager = EffectManager.Instance;
    }
    
    public void AddExperience(float amount)
    {
        currentExp += amount;
        eventManager.TriggerPlayerExpChanged(currentExp, expToNextLevel);
        
        // 레벨업 체크
        while (currentExp >= expToNextLevel)
        {
            LevelUp();
        }
    }
    
    public void LevelUp()
    {
        currentExp -= expToNextLevel;
        currentLevel++;
        expToNextLevel *= expMultiplier;
        
        eventManager.TriggerPlayerLevelUp(currentLevel);
        eventManager.TriggerPlayerExpChanged(currentExp, expToNextLevel);
        
        effectManager?.PlayLevelUpEffect(transform.position);
        if (levelUpSound != null)
        {
            AudioManager.Instance.PlayOneShot(levelUpSound, transform.position, levelUpSoundVolume);
        }
    }
    
    public int GetCurrentLevel() => currentLevel;
} 