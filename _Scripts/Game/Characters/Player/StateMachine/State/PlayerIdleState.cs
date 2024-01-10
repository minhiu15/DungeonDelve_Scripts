using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) {}
    
    
    private float currentBlend;
    
    public override void EnterState()
    {
        _machine.CanFootstepsAudioPlay = false;
        _machine.AppliedMovement = Vector3.zero;
        currentBlend = _machine.animator.GetFloat(_machine.IDSpeed);
    }
    protected override void UpdateState()
    {
        currentBlend = Mathf.MoveTowards(currentBlend, 0, 5f * Time.deltaTime);
        _machine.animator.SetFloat(_machine.IDSpeed, currentBlend);
        
        CheckSwitchState();
    }
    public override void CheckSwitchState()
    {
        // // Kiểm tra các trạng thái khi nhân vật đang đứng dưới đất
        if (_machine.IsDash)
        {
            SwitchState(_factory.Dash());
        }
        else if (_machine.IsRun)
        {
            SwitchState(_factory.Run());
        }
        else if (_machine.IsWalk)
        {
            SwitchState(_factory.Walk());
        }
    }
    
}
