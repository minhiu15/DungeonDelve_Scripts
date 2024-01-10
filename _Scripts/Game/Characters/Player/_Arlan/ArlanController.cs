using System.Collections;
using UnityEngine;

public class ArlanController : PlayerController
{
    [Header("SubClass -------")] 
    [Tooltip("Thời gian chuyển đổi dạng anim: từ cầm vũ khí sang không cầm vũ khí"), SerializeField]
    private float conversionTime;

    [Tooltip("SkinMesh weapon: Sau lưng"), SerializeField]
    private GameObject swordOnShoulder;

    [Tooltip("SkinMesh weapon: Đang cầm"), SerializeField]
    private GameObject sword;
    
    private float _conversionTimeTemp;
    private Coroutine _weaponUnEquippedCoroutine;

    
    protected override void Update()
    {
        base.Update();
        
        HandleAttack();
    }
    
    
    #region Đổi anim cầm và không cầm vũ khí
    private IEnumerator WeaponUnEquippedCoroutine()
    {
        while (true)
        {
            _conversionTimeTemp = _conversionTimeTemp > 0 ? _conversionTimeTemp - Time.deltaTime : 0;
            if (_conversionTimeTemp <= 0 && IsIdle && !swordOnShoulder.activeInHierarchy)
            {
                animator.SetBool(IDWeaponEquip, false);
                CanMove = false;
                CanRotation = false;
            }
            
            else if (!IsIdle || !IsGrounded)
            {
                _conversionTimeTemp = conversionTime;
            }
            yield return null;
        }
    }
    private void WeaponEquipped() // Cầm vũ khí 
    {
        _conversionTimeTemp = conversionTime;
        animator.SetBool(IDWeaponEquip, true);
        sword.SetActive(true);
        swordOnShoulder.SetActive(false);
    }
    private void WeaponUnEquipped() // Không cầm vũ khí 
    {
        CanMove = true;
        CanRotation = true;
        swordOnShoulder.SetActive(true);
        sword.SetActive(false);
    }
    #endregion
    
    public void BuffSkill()
    {
        Debug.Log("Enable Skill");
    }
    public void UnBuffSkill()
    {
        Debug.Log("Disable Skill");
    }   
    
    // OverridingMethods
    protected override void SetVariables()
    {
        base.SetVariables();
        
        _conversionTimeTemp = 0;
        if(_weaponUnEquippedCoroutine !=null) StopCoroutine(_weaponUnEquippedCoroutine);
        _weaponUnEquippedCoroutine = StartCoroutine(WeaponUnEquippedCoroutine());
    }
    public override void ReleaseAction()
    {
        base.ReleaseAction();
        
        if(sword.activeSelf) 
            WeaponEquipped();
        else if (swordOnShoulder.activeSelf)
            WeaponUnEquipped();
    }
    protected override void HandleDamage()
    {
        WeaponEquipped();
        base.HandleDamage();
    }
    protected override void NormalAttack()
    {
        WeaponEquipped();
        base.NormalAttack();
    }
    protected override void ChargedAttack()
    {
        WeaponEquipped();
        CanMove = false;
        CanRotation = false;
        base.ChargedAttack();
    }
    protected override void ElementalSkill()
    {
        if(!IsElementalSkill) return;
        
        WeaponEquipped();
        base.ElementalSkill();
    }
    protected override void ElementalBurst()
    {
        if(!IsElementalBurst) return;
        
        WeaponEquipped();
        base.ElementalBurst();
    }

    public override float PercentDMG_NA() => base.PercentDMG_NA() + (PlayerConfig.GetDEF() * .33f); // % cộng thêm từ vũ khí 
}
