using System.Collections;
using UnityEngine;

public class PlayerDashState : PlayerBaseState
{
    public PlayerDashState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) {}

    private int stCost => _machine.PlayerConfig.GetDashSTCost();
    private readonly float dashSpeed = 14f;
    private float speedPushDash;
    private Vector3 direction;
    
    public override void EnterState()
    {
        _machine.voice.PlayDash();
        _machine.animator.Rebind();
        _machine.animator.SetTrigger(_machine.IDDash);
        _machine.Stamina.Decreases(stCost);
        _machine.ReleaseAction();
        _machine.CallbackDashEvent();
        
        speedPushDash = .3f;
        direction = Vector3.zero;
    }
    protected override void UpdateState()
    {
        speedPushDash = speedPushDash > 0 ? speedPushDash - Time.deltaTime : 0;
        if (speedPushDash <= 0)
        {
            CheckSwitchState();
            return;
        }
        _machine.input.LeftMouse = false;
        direction = _machine.model.forward.normalized * dashSpeed;
        _machine.characterController.Move(direction * Time.deltaTime);
    }
    protected override void ExitState()
    {
        _machine.input.LeftMouse = false;
        _machine.InputMovement = Vector3.zero;
        _machine.AppliedMovement = Vector3.zero;
    }
    public override void CheckSwitchState()
    {
        // // Kiểm tra các trạng thái khi nhân vật đang đứng dưới đất
        if (_machine.IsWalk)
        {
            SwitchState(_factory.Walk());
        }
        else if (_machine.IsRun)
        {
            SwitchState(_factory.Run());
        }
        else
        {
            SwitchState(_factory.Idle());
        }
    }
    
    
}