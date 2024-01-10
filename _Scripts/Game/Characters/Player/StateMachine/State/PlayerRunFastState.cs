using System.Collections;
using FMOD.Studio;
using UnityEngine;

public class PlayerRunFastState : PlayerBaseState
{
    public PlayerRunFastState(PlayerStateMachine _currentContext, PlayerStateFactory factory) 
        : base(_currentContext, factory) {}


    private float currentBlend;
    private readonly float _delayDecreaseST = 0.07f;
    private float _delayDecreaseSTTemp;
    private Coroutine _subtractSTCoroutine;

   
    public override void EnterState()
    {
        _machine._footstepsInstance = _machine.runfastFootsteps;
        _machine.CanFootstepsAudioPlay = true;
        currentBlend = _machine.animator.GetFloat(_machine.IDSpeed);
        _machine.animator.speed = 1.4f;
        _machine.CanIncreaseST = false;
    }
    protected override void UpdateState()
    {
        CheckSwitchState();
        
        _machine.animator.SetFloat(_machine.IDSpeed, currentBlend);
        _machine.AppliedMovement = _machine.InputMovement.normalized * _machine.PlayerConfig.GetRunFastSpeed();
        currentBlend = Mathf.MoveTowards(currentBlend, 1, 5f * Time.deltaTime);

        _delayDecreaseSTTemp = _delayDecreaseSTTemp > 0 ? _delayDecreaseSTTemp - Time.deltaTime : 0;
        if (_delayDecreaseSTTemp > 0) 
            return;
        
        _machine.Stamina.Decreases(1);
        _delayDecreaseSTTemp = _delayDecreaseST;
    }
    protected override void ExitState()
    {
        _machine.CanFootstepsAudioPlay = false;
        _machine._footstepsInstance.stop(STOP_MODE.IMMEDIATE);
        _machine.input.LeftShift = false;
        _machine.animator.speed = 1f;
        _machine.CanIncreaseST = true;
        
        if (_subtractSTCoroutine != null)
            _machine.StopCoroutine(_subtractSTCoroutine);
    }
    public override void CheckSwitchState()
    {
        // // Kiểm tra các trạng thái khi nhân vật đang đứng dưới đất
        if (_machine.IsIdle)
        {  
            SwitchState(_factory.Idle());
        }
        else if (_machine.IsRun || _machine.Stamina.CurrentValue <= 0)
        {
            SwitchState(_factory.Run());
        }
    }
    
 
    
}
