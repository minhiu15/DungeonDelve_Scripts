using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Config", menuName = "Characters Configuration/Enemy")]
public class SO_EnemyConfiguration : SO_CharacterConfiguration
{
    
    [Header("STATS MULTIPLIER")]
    [Tooltip("Tỉ lệ HP của Enemy so với người chơi"), SerializeField]
    private float HPRatio;
    
    [Tooltip("Tỉ lệ DEF của Enemy so với người chơi"), SerializeField]
    private float DEFRatio;

    
    [Tooltip("Tỉ lệ Level của Enemy so với người chơi"), SerializeField]
    private float LevelRatio;
    
    [Header("COOLDOWN")]
    [Tooltip("Thời gian chờ lần tấn công tiếp theo"), SerializeField]
    private float NormalAttackCD;
    
    [Tooltip("Thời gian chờ lần Skill tiếp theo"), SerializeField]
    private float SkillAttackCD;
        
    [Tooltip("Thời gian chờ lần Special tiếp theo"), SerializeField]
    private float SpecialAttackCD;

    
    // Func
    public float GetHPRatio() => HPRatio;
    public void SetHPRatio(float _value) => HPRatio = _value;
    
    public float GetDEFRatio() => DEFRatio;
    public void SetDEFRatio(float _value) => DEFRatio = _value;
    
    public float GetLevelRatio() => LevelRatio;
    public void SetLevelRatio(float _value) => LevelRatio = _value;
    
    public float GetNormalAttackCD() => NormalAttackCD;
    public void SetNormalAttackCD(float _value) => NormalAttackCD = _value;
    
    public float GetSkillAttackCD() => SkillAttackCD;
    public void SetSkillAttackCD(float _value) => SkillAttackCD = _value;  
    
    public float GetSpecialAttackCD() => SpecialAttackCD;
    public void SetSpecialAttackCD(float _value) => SpecialAttackCD = _value;
}
