using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;

[Serializable]
public struct EffectOffset
{
    public Vector3 position;
    public Vector3 rotation;
}
public enum PushDirectionEnum
{
    Forward,
    Behind
}

[Serializable]
public class AttackCustom
{
    [Tooltip("Các lực đẩy mỗi khi attack, lực áp dụng tương ứng thứ n của animationClip Attack")]
    public List<float> pushForce;
    
    [Tooltip("Đẩy đi trong bao lâu"), Range(0f, 1f)]
    public float pushTime;
    
    [Tooltip("Hướng đẩy: Forward -> trước, Behind -> sau")]
    public PushDirectionEnum pushDirectionEnum;
}


public abstract class PlayerController : PlayerStateMachine
{
    
    [SerializeField] private AttackCustom attackCustom;
    public event Action<float> OnElementalSkillCDEvent; 
    public event Action<float> OnElementalBurstCDEvent;

    protected bool IsNormalAttack => input.LeftMouse;
    protected bool IsElementalSkill => input.E && _skillCD_Temp <= 0;
    protected bool IsElementalBurst => input.Q && _burstCD_Temp <= 0;
    
    public float MouseHoldTime { get; private set; }       // thời gian giữ chuột -> >= .4s -> charged Attack
    
    
    // Player
    [HideInInspector] private Vector3 _pushVelocity;       // vận tốc đẩy 
    [HideInInspector] private int _directionPushVelocity;  // hướng đẩy
    [HideInInspector] protected bool _isAttackPressed;     // có nhấn attack k ?
    
    private Coroutine _pushVelocityCoroutine;
    private Coroutine _pushMoveCoroutine;
    private Coroutine _focusEnemyCoroutine;

    
    protected override void SetVariables()
    {
        base.SetVariables();
        
        CanAttack = true;
        
        _directionPushVelocity = attackCustom.pushDirectionEnum switch 
        {
            PushDirectionEnum.Forward => 1, 
            PushDirectionEnum.Behind => -1,   
            _ => 0
        };
    }

    protected void HandleAttack()
    {
        animator.SetFloat(IDStateTime, Mathf.Repeat(animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1f));
        animator.ResetTrigger(IDNormalAttack);
        
        if (animator.IsTag("Attack") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= .8f)
        {
            AttackEnd();
        }
        
        if(!CanAttack || !IsGrounded) return;
        
        Attack();
        ElementalSkill();
        ElementalBurst();
    }
    
    private void Attack()
    {
        if (IsNormalAttack)
        {
            _isAttackPressed = true;
            MouseHoldTime += Time.deltaTime;
        }
        
        switch (_isAttackPressed)
        {
            case true when !IsNormalAttack && MouseHoldTime < .4f:
                NormalAttack();
                break;
            
            case true when IsNormalAttack && MouseHoldTime >= .4f:
                CanAttack = false;
                _isAttackPressed = false;
                ChargedAttack();
                break;
        }
    }
    protected virtual void NormalAttack()
    {
        CanMove = false;
        CanRotation = false;
        _isAttackPressed = false;
        
        MouseHoldTime = 0;
        animator.SetTrigger(IDNormalAttack);
        
        PercentDMG_NA();
        FocusEnemy();
    }
    protected virtual void ChargedAttack()
    {
        animator.SetTrigger(IDChargedAttack);
        PercentDMG_CA();
    }
    protected virtual void ElementalSkill()
    {
        if(!IsElementalSkill) return;

        animator.SetTrigger(IDSkill);
        CanAttack = false;
        CanMove = false;
        CanRotation = false;
        _skillCD_Temp = PlayerConfig.GetElementalSkillCD();
        
        PercentDMG_ES();
        OnSkillCooldownEvent();
    }
    protected virtual void ElementalBurst()
    {
        if(!IsElementalBurst) return;
        
        animator.SetTrigger(IDSpecial);
        CanAttack = false;
        CanMove = false;
        CanRotation = false;
        _burstCD_Temp = PlayerConfig.GetElementalBurstCD();

        PercentDMG_EB();
        OnSpecialCooldownEvent();
    }
    protected void OnSkillCooldownEvent () => OnElementalSkillCDEvent?.Invoke(PlayerConfig.GetElementalSkillCD());
    protected void OnSpecialCooldownEvent () => OnElementalBurstCDEvent?.Invoke(PlayerConfig.GetElementalBurstCD());
    

    public void SetAttackCounter(int count) => _attackCounter = count; // gọi trên event animaiton
    public void AddForceAttack()
    {
        if(_pushMoveCoroutine != null) StopCoroutine(PushMoveCoroutine());
        _pushMoveCoroutine = StartCoroutine(PushMoveCoroutine());
    }
    private IEnumerator PushMoveCoroutine()
    {
        var timePush = attackCustom.pushTime;
        while (timePush > 0)
        {
            _pushVelocity = model.forward * (attackCustom.pushForce[_attackCounter] * _directionPushVelocity);
            characterController.Move(_pushVelocity * Time.deltaTime + new Vector3(0f, -9.81f, 0f) * Time.deltaTime);
            timePush -= Time.deltaTime;
            yield return null;
        }
    }
    
    
    #region Focus về phía Enemy mỗi khi Attack
    private void FocusEnemy()
    {
        if (_focusEnemyCoroutine != null)
            StopCoroutine(FocusEnemyCoroutine());
        _focusEnemyCoroutine = StartCoroutine(FocusEnemyCoroutine());
    }
    private IEnumerator FocusEnemyCoroutine()
    {
        var _checClosestEnemy  = EnemyTracker.FindClosestEnemy(transform, out var target);
        if(!_checClosestEnemy) yield break;
        
        var direction = Quaternion.LookRotation(target - transform.position);
        var directionLocal = Mathf.Floor(transform.eulerAngles.y);
        var directionTaget = Mathf.Floor(direction.eulerAngles.y);

        var rotation = Quaternion.Euler(0, direction.eulerAngles.y, 0);
        while (Mathf.Abs(directionTaget - directionLocal) > .2f)
        {
            model.rotation = Quaternion.RotateTowards(model.rotation, rotation, 20f);
            directionLocal = Mathf.Floor(model.eulerAngles.y);
            yield return null;
        }
    }
    public void AddEnemy(GameObject _enemy) => EnemyTracker.Add(_enemy.transform);
    public void RemoveEnemy(GameObject _enemy) => EnemyTracker.Remove(_enemy.transform);
    #endregion
    
    
    protected virtual void AttackEnd()
    {
        MouseHoldTime = 0;
        CanAttack = true;
        CanMove = true;
        CanRotation = true;
        CanFootstepsAudioPlay = true;
    }
    protected virtual void SkillEnd()
    {
        AttackEnd();
        input.E = false;
    }
    protected virtual void SpecialEnd()
    {
        AttackEnd();
        input.Q = false;
    }
    public override void ReleaseAction()
    {
        input.LeftMouse = false;
        AttackEnd();
    }
    
}
