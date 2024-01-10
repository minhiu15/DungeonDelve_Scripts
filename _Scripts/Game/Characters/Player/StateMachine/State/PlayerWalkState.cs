using FMOD.Studio;
using UnityEngine;

public class PlayerWalkState : PlayerBaseState
{
    public PlayerWalkState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) 
        : base(currentContext, playerStateFactory) {}
    
    private float currentBlend;
    
    public override void EnterState()
    {
        _machine._footstepsInstance = _machine.walkFootsteps;
        _machine.CanFootstepsAudioPlay = true;
        _machine.animator.speed = 1.25f;
        currentBlend = _machine.animator.GetFloat(_machine.IDSpeed);
    }
    protected override void UpdateState()
    {
        _machine.AppliedMovement = _machine.InputMovement.normalized * _machine.PlayerConfig.GetWalkSpeed();
        
        currentBlend = Mathf.MoveTowards(currentBlend, .5f, 5f * Time.deltaTime);
        _machine.animator.SetFloat(_machine.IDSpeed, currentBlend);

        CheckSwitchState();
    }
    protected override void ExitState()
    {
        _machine.animator.speed = 1f;
        _machine.CanFootstepsAudioPlay = false;
        _machine._footstepsInstance.stop(STOP_MODE.IMMEDIATE);
    }
    public override void CheckSwitchState()
    {
        // // Kiểm tra các trạng thái khi nhân vật đang đứng dưới đất
        if (_machine.IsIdle)
        {  
            SwitchState(_factory.Idle());
        }
        else if (_machine.IsDash)
        {
            SwitchState(_factory.Dash());
        }
        else if (_machine.IsRun)
        {
            SwitchState(_factory.Run());
        }
    }
    
    
}
