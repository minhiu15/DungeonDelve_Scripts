using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    /*
        - Để nhận giá trị từ EventInput: Các function cần có từ khóa 'On' phía trước tên Actions đã setup trong InputAsset
        + Ví dụ: Action: Move -> Function: OnMove
     */
    
    public Inputs PlayerInput { get; private set; }
    private void Awake()
    {
        PlayerInput = new Inputs();
    }

    private void OnEnable()
    {
        PlayerInput.Player.Enable();
        
        PlayerInput.Player.Move.performed  += OnMovePressed;
        PlayerInput.Player.Move.canceled += OnMovePressed;
        
        PlayerInput.Player.SwitchMode.started  += OnChangeStatePressed;
        PlayerInput.Player.SwitchMode.canceled += OnChangeStatePressed;
        
        PlayerInput.Player.Jump.started  += OnJumpPressed;
        PlayerInput.Player.Jump.canceled += OnJumpPressed;
        
        PlayerInput.Player.Dash.started  += OnDashPressed;
        PlayerInput.Player.Dash.canceled += OnDashPressed;
        
        PlayerInput.Player.NormalAttack.started += OnAttackPressed;
        PlayerInput.Player.NormalAttack.canceled += OnAttackPressed;
        
        PlayerInput.Player.ElementalSkill.started += OnSkillPressed;
        PlayerInput.Player.ElementalSkill.canceled += OnSkillPressed;
        
        PlayerInput.Player.ElementalBurst.started += OnSkillSpecialPressed;
        PlayerInput.Player.ElementalBurst.canceled += OnSkillSpecialPressed;
        
    }
    private void OnDisable()
    {
        PlayerInput.Player.Move.performed  -= OnMovePressed;
        PlayerInput.Player.Move.canceled -= OnMovePressed;
        
        PlayerInput.Player.SwitchMode.started  -= OnChangeStatePressed;
        PlayerInput.Player.SwitchMode.canceled -= OnChangeStatePressed;
        
        PlayerInput.Player.Jump.started  -= OnJumpPressed;
        PlayerInput.Player.Jump.canceled -= OnJumpPressed;
        
        PlayerInput.Player.Dash.started  -= OnDashPressed;
        PlayerInput.Player.Dash.canceled -= OnDashPressed;
        
        PlayerInput.Player.NormalAttack.started -= OnAttackPressed;
        PlayerInput.Player.NormalAttack.canceled -= OnAttackPressed;
        
        PlayerInput.Player.ElementalSkill.started -= OnSkillPressed;
        PlayerInput.Player.ElementalSkill.canceled -= OnSkillPressed;
        
        PlayerInput.Player.ElementalBurst.started -= OnSkillSpecialPressed;
        PlayerInput.Player.ElementalBurst.canceled -= OnSkillSpecialPressed;
        
        PlayerInput.Player.Disable();
    }


 
    /// <summary>
    /// Nhận giá trị của 4 phím: A W S D
    /// </summary>
    public Vector2 Move;
    private void OnMovePressed(InputAction.CallbackContext context) => Move = context.ReadValue<Vector2>();


    /// <summary>
    /// Chuyển đổi giữa Walk và Run
    /// </summary>
    public bool ChangeState;
    private void OnChangeStatePressed(InputAction.CallbackContext context) =>  ChangeState = context.ReadValueAsButton();

    
    /// <summary>
    /// Trả về TRUE nếu nhấn phím: ShiftLeft or RightMouseButton
    /// </summary>
    public bool LeftShift;
    private void OnDashPressed(InputAction.CallbackContext context) => LeftShift = context.ReadValueAsButton();
    
    
    /// <summary>
    /// Trả về TRUE nếu nhấn phím: Space
    /// </summary>
    public bool Space;
    private void OnJumpPressed(InputAction.CallbackContext context) => Space = context.ReadValueAsButton();

    
    /// <summary>
    /// Trả về TRUE nếu nhấn phím: LeftMouseButton
    /// </summary>
    public bool LeftMouse;
    private void OnAttackPressed(InputAction.CallbackContext context) => LeftMouse = context.ReadValueAsButton();

    
    /// <summary>
    /// Trả về TRUE nếu nhấn phím: E
    /// </summary>
    public bool E;
    private void OnSkillPressed(InputAction.CallbackContext context) => E = context.ReadValueAsButton();

    
    /// <summary>
    /// Trả về TRUE nếu nhấn phím: Q
    /// </summary>
    public bool Q;
    private void OnSkillSpecialPressed(InputAction.CallbackContext context) => Q = context.ReadValueAsButton();


}
