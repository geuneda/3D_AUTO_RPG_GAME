using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    [Header("레벨링 설정")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private float currentExp;
    [SerializeField] private float expToNextLevel = 100f;    // 기본 필요 경험치
    [SerializeField] private float expMultiplier = 1.2f;     // 레벨당 필요 경험치 증가율
    
    // 레벨, 경험치 변경시 발생하는 이벤트 UI에 적용 예정
    public static event System.Action<int> OnLevelUp;
    public static event System.Action<float, float> OnExpChanged;  // 현재 경험치, 최대 경험치
    
    private EffectManager effectManager;
    
    [Header("Audio")]
    [SerializeField] private AudioClip levelUpSound;
    [SerializeField] private float levelUpSoundVolume = 1f;
    
    private void Start()
    {
        effectManager = EffectManager.Instance;
    }
    
    public void AddExperience(float amount)
    {
        currentExp += amount;
        OnExpChanged?.Invoke(currentExp, expToNextLevel);
        
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
        
        OnLevelUp?.Invoke(currentLevel);
        OnExpChanged?.Invoke(currentExp, expToNextLevel);
        
        effectManager?.PlayLevelUpEffect(transform.position);
        if (levelUpSound != null)
        {
            AudioManager.Instance.PlayOneShot(levelUpSound, transform.position, levelUpSoundVolume);
        }
    }
    
    public int GetCurrentLevel() => currentLevel;
} 