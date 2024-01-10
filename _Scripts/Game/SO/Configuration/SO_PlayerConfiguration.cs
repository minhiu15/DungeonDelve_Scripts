using System;
using Newtonsoft.Json;
using UnityEngine;


[Serializable, CreateAssetMenu(fileName = "Player Config", menuName = "Characters Configuration/Player/Player Config")]
public class SO_PlayerConfiguration : SO_CharacterConfiguration
{
    
    [Tooltip("Sức bền tối đa"), SerializeField, JsonProperty] 
    private int MaxST = 100;
    
    [Tooltip("Sức bền tiêu hao khi Charged Attack"), SerializeField, JsonProperty]
    private int ChargedAttackStaminaCost = 20;
    
    [Tooltip("Tốc độ chạy nhanh"), SerializeField, JsonProperty] 
    private float RunFastSpeed = 8f;
    
    [Tooltip("Năng lượng cho mỗi lần lướt"), SerializeField, JsonProperty]
    private int DashStaminaCost = 25;
    
    [Tooltip("Độ cao khi nhảy"), SerializeField, JsonProperty] 
    private float JumpHeight = 1.2f;
    
    [Header("COOLDOWN")]
    [Tooltip("Thời gian nhảy cho lần tiếp theo"), SerializeField, JsonProperty] 
    private float JumpCD;
    
    [Tooltip("Thời gian hồi kỹ năng (s)"), SerializeField, JsonProperty] 
    private float ElementalSkillCD;
    
    [Tooltip("Thời gian hồi kỹ năng đặc biệt (s)"), SerializeField, JsonProperty] 
    private float ElementalBurstCD;

    [Header("WEAPON")] 
    [Tooltip("Tên của vũ khí"), SerializeField, JsonProperty]
    private string WeaponName;
    
    [Tooltip("Thông tin của vũ khí"), SerializeField, JsonProperty]
    private string WeaponInfo;
    
    [Tooltip("Cấp độ của vũ khí"), SerializeField, JsonProperty, Range(1, 10)]
    private int WeaponLevel;
    
    
    [Header("TYPE")]
    public CharacterNameCode NameCode;

    [JsonConverter(typeof(SpriteConverter))] 
    public Sprite ChapterIcon;
    
    
    // Func
    
    public int GetST() => MaxST;
    public int SetST(int _value) => MaxST = _value;
    
    public int GetChargedAttackSTCost() => ChargedAttackStaminaCost;
    public int SetChargedAttackSTCost(int _value) => ChargedAttackStaminaCost = _value;
    
    public float GetRunFastSpeed() => RunFastSpeed;
    public float SetRunFastSpeed(float _value) => RunFastSpeed = _value;
    
    public int GetDashSTCost() => DashStaminaCost;
    public int SetDashSTCost(int _value) => DashStaminaCost = _value;
    
    public float GetJumpHeight() => JumpHeight;
    public float SetJumpHeight(float _value) => JumpHeight = _value;
    
    public float GetElementalSkillCD() => ElementalSkillCD;
    public float SetElementalSkillCD(float _value) => ElementalSkillCD = _value;

    public float GetElementalBurstCD() => ElementalBurstCD;
    public float SetElementalBurstCD(float _value) => ElementalBurstCD = _value;
    
    public string GetWeaponName() => WeaponName;
    public string SetWeaponName(string _value) => WeaponName = _value;
    
    public string GetWeaponInfo() => WeaponInfo;
    public string SetWeaponInfo(string _value) => WeaponInfo = _value;
    
    public int GetWeaponLevel() => WeaponLevel;
    public int SetWeaponLevel(int _value) => WeaponLevel = _value;
    
}
