using FMOD.Studio;
using UnityEngine;

public class PlayerRunState : PlayerBaseState
{
    public PlayerRunState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) 
        : base(currentContext, playerStateFactory) {}
    
    
    private float currentBlend;
    private float lastInputLeftShift; // thời gian nhấn phím shift 
    private bool isLeftShiftPressed; // có nhấn phím shift ?

    
    public override void EnterState()
    {
        _machine._footstepsInstance = _machine.runFootsteps;
        _machine.CanFootstepsAudioPlay = true;
        currentBlend = _machine.animator.GetFloat(_machine.IDSpeed);
    }
    protected override void UpdateState()
    {
        _machine.AppliedMovement = _machine.InputMovement.normalized * _machine.PlayerConfig.GetRunSpeed();
        
        currentBlend = Mathf.MoveTowards(currentBlend, 1, 5f * Time.deltaTime);
        _machine.animator.SetFloat(_machine.IDSpeed, currentBlend);

        CheckSwitchState();
    }
    protected override void ExitState()
    {
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
        else if (_machine.IsWalk)
        {
            SwitchState(_factory.Walk());
        }

        if (_machine.input.LeftShift)
        {
            isLeftShiftPressed = true;
            lastInputLeftShift += Time.deltaTime;
        }
        switch (isLeftShiftPressed)
        {
            case true when !_machine.input.LeftShift && lastInputLeftShift < .4f:
                if(_machine.Stamina.CurrentValue >= _machine.PlayerConfig.GetDashSTCost()) 
                    SwitchState(_factory.Dash());
                
                lastInputLeftShift = 0;
                isLeftShiftPressed = false;
                _machine.input.LeftShift = false;
                break;
            
            case true when _machine.input.LeftShift && lastInputLeftShift >= .4f:
                SwitchState(_factory.RunFast());
                
                lastInputLeftShift = 0;
                isLeftShiftPressed = false;
                break;
        }  
        
    }
    
}