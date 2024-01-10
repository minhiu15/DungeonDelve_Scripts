using UnityEngine;

public enum AttackType
{
    NormalAttack,
    ChargedAttack,
    ElementalSkill,
    ElementalBurst
}

public abstract class Damageable : MonoBehaviour
{
    protected virtual void OnEnable() => DamageableData.Add(gameObject, this);
    protected virtual void OnDisable() => DamageableData.Remove(gameObject);

    
    /// <summary>
    /// Tính lượng DMG(x) của từng nhân vật theo theo attackType và áp dụng lượng DMG này vào TakeDMG(x) trên _gameObject
    /// </summary>
    /// <param name="_gameObject"> Đối tượng bị TakeDMG (nếu có) </param>
    public virtual void CauseDMG(GameObject _gameObject, AttackType _attackType) { }
    
    /// <summary>
    /// Nhận sát thương vào
    /// </summary>
    /// <param name="_damage"> Lượng sát thương nhận vào </param>
    /// <param name="_isCRIT"> Sát thương có kích bạo không ? </param>
    public virtual void TakeDMG(int _damage, bool _isCRIT) { }


    /// <summary> Phần trăm sát thương của Normal Attack </summary>  
    public virtual float PercentDMG_NA() => 0;


    /// <summary> Phần trăm sát thương của Charged Attack </summary>  
    public virtual float PercentDMG_CA() => 0;
    
    
    /// <summary> Phần trăm sát thương của Elemental Skill </summary>  
    public virtual float PercentDMG_ES() => 0;
    
    
    /// <summary> Phần trăm sát thương của Elemental Burst </summary>  
    public virtual float PercentDMG_EB() => 0;
    
    
    /// <summary> Chuyển phần trăm sát thương thành sát thương đầu ra </summary>  
    public virtual int CalculationDMG(float _percent) => 0;
    
}
