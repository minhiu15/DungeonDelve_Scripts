using System.Collections;
using UnityEngine;

public class PlayerDeadState : PlayerBaseState
{
    public PlayerDeadState(PlayerStateMachine _currentContext, PlayerStateFactory factory)
        : base(_currentContext, factory) { }
    
    private Coroutine _revivalCorouine;
    private readonly float _revivalTime = 5f;
    private Vector3 _gravity = new(0f, -9.81f, 0f);
    
    public override void EnterState()
    {
        EnemyTracker.Clear();
        _gravity.y = _machine.Gravity;
        
        if (_revivalCorouine != null)
            _machine.StopCoroutine(_revivalCorouine);
        _revivalCorouine = _machine.StartCoroutine(RevivalCoroutine());
    }
    protected override void UpdateState()
    {
        _machine.characterController.Move(_gravity * Time.deltaTime);
    }
    protected override void ExitState()
    {
        ExitDeadState();
    }
    
    private IEnumerator RevivalCoroutine()
    {
        SetInputState(false);
        InitDeadState();
        _machine.CallbackDieEvent();
        
        yield return new WaitForSeconds(2f);
        DeadDissolve();
        _machine.CallbackRevivalTimeEvent(_revivalTime);
        
        yield return new WaitForSeconds(_revivalTime);
        LoadingPanel.Instance.Active(.75f);
        _machine.CallbackRevivalTimeEvent(0);
        RevialDissolve();
        SetTransform();
        InitStatus();
        
        yield return new WaitForSeconds(1f);
        SetInputState(true);
        _machine.ReleaseDamageState();
        _machine.cameraFOV.FOVStart();
    }
    private void InitDeadState()
    {
        _machine.voice.PlayDie();
        _machine.CanMove = false;
        _machine.CanFootstepsAudioPlay = false;
        _machine.animator.SetBool(_machine.IDDead, true);
        _machine.animator.SetFloat(_machine.IDSpeed, 0);
        _machine.FreeLookCamera.enabled = false;
        _machine.CurrentState = _factory.Grounded();
    }
    private void DeadDissolve()
    {
        _machine.setEmission.ChangeCurrentIntensity(-3f);   
        _machine.setEmission.ChangeIntensitySet(5f);
        _machine.setEmission.ChangeDurationApply(.15f);
        _machine.setEmission.Apply();
        
        _machine.setDissolve.ChangeCurrentValue(0);
        _machine.setDissolve.ChangeValueSet(1);
        _machine.setDissolve.ChangeDurationApply(2f);
        _machine.setDissolve.Apply();
    }
    private void RevialDissolve()
    {
        _machine.setDissolve.ChangeDurationApply(1f);
        _machine.setDissolve.ChangeCurrentValue(1f);
        _machine.setDissolve.ChangeValueSet(0f);
        _machine.setDissolve.Apply();
        
        _machine.setEmission.ChangeIntensitySet(0);
        _machine.setEmission.ChangeDurationApply(0);
        _machine.setEmission.Apply();
    }
    private void SetInputState(bool _state)
    {
        if (_state)
        {
            _machine.FreeLookCamera.m_XAxis.Value = 0;
            _machine.FreeLookCamera.m_YAxis.Value = .5f;
            _machine.FreeLookCamera.enabled = true;
            _machine.input.PlayerInput.Enable();
            GUI_Inputs.InputAction.Enable();
            return;
        }
        _machine.FreeLookCamera.enabled = false;
        _machine.input.PlayerInput.Disable();
        GUI_Inputs.InputAction.Disable();
    }
    private void SetTransform()
    {
        _machine.characterController.enabled = false;
        _machine.transform.position = Vector3.zero;
        _machine.characterController.enabled = true;
        _machine.model.rotation = Quaternion.Euler(Vector3.zero);
    }
    private void InitStatus()
    {
        var _hp = _machine.PlayerConfig.GetHP();
        var _st = _machine.PlayerConfig.GetST();
        _machine.Health.InitValue(_hp, _hp);
        _machine.Stamina.InitValue(_st, _st);
    }
    private void ExitDeadState()
    {
        _machine.JumpVelocity = -9.81f;
        _machine.animator.SetFloat(_machine.IDSpeed, 0);
        _machine.animator.SetBool(_machine.IDDead,false);
        _machine.animator.SetBool(_machine.IDJump, false);
        _machine.animator.SetBool(_machine.IDFall, false);
    }
    
}
