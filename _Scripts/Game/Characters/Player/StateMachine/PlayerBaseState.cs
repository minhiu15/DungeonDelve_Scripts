using UnityEngine;

/// <summary> Định nghĩa các hành vi mà tất cả State khác đều có. </summary>
public abstract class PlayerBaseState
{
    protected PlayerBaseState(PlayerStateMachine _currentContext, PlayerStateFactory factory)
    {
        _machine = _currentContext;
        _factory = factory;
    }

    protected bool _isRoolState = false;
    protected readonly PlayerStateMachine _machine;
    protected readonly PlayerStateFactory _factory;
    protected PlayerBaseState _childState { get; private set; }
    protected PlayerBaseState _parentState { get; private set; }
    public PlayerBaseState ChildState => _childState;
    
    
    public virtual void EnterState() { }
    protected virtual void UpdateState() { }
    protected virtual void ExitState() { }
    public virtual void CheckSwitchState() { } 
    
    public void UpdateStates()
    {
        UpdateState();
        _childState?.UpdateStates();
    }
    public void SwitchState(PlayerBaseState newState)
    {
        // Thoát State, bất kể trạng thái nào.
        ExitState();
        
        // Khởi chạy state mới
        newState.EnterState();
        
        if (_isRoolState)
        {
            _machine.CurrentState = newState;
        }
        else
        {
            _parentState?.SetChildState(newState);
        }
    }
    protected void SetChildState(PlayerBaseState _newChildState)
    {
        _childState = _newChildState;
        _childState.SetParentState(this);
    }
    protected void SetParentState(PlayerBaseState _newParentState)
    {
        _parentState = _newParentState;
    }
    
}

