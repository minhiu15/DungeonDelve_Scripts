using UnityEngine;

public class PlayerDamageFallState : PlayerBaseState
{
    public PlayerDamageFallState(PlayerStateMachine _currentContext, PlayerStateFactory factory)
        : base(_currentContext, factory) { }
    
    private readonly float _force = 6f;
    private float _timePush;
    private Vector3 _pushVelocity;
    private Vector3 _gravity = new(0f, -9.81f, 0f);
    private bool _canMoveBehind;
    
    public override void EnterState()
    {
        _timePush = .2f;
        _gravity.y = _machine.Gravity;
        _machine.voice.PlayHeavyHit();
        _machine.animator.SetTrigger(_machine.IDDamageFall);
    }
    protected override void UpdateState()
    {
        CheckSwitchState();
        if(_timePush <= 0)
        {
            _machine.characterController.Move(_gravity * Time.deltaTime);
            return;
        }
        
        _pushVelocity = -_machine.model.forward * _force;
        _machine.characterController.Move(_pushVelocity * Time.deltaTime + _gravity * Time.deltaTime);
        _timePush -= Time.deltaTime;
    }
    protected override void ExitState()
    {
        _machine.ResetDamageTrigger();
    }
    public override void CheckSwitchState()
    {
        if (!_machine.IsDash) return;
        SwitchState(_factory.Dash());
        _machine.SetPlayerInputState(true);
    }
    
}
