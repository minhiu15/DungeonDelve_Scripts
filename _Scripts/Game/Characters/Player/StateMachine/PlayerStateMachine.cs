using System;
using Cinemachine;
using FMOD.Studio;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(PlayerDataHandle))]
public abstract class PlayerStateMachine : Damageable
{
    #region Variables
    #region Ref
    [field: Header("BaseClass -------")] // REF
    [field: SerializeField] public PlayerInputs input { get; private set; }
    [field: SerializeField] public Transform model { get; private set; }
    [field: SerializeField] public Animator animator { get; private set; }
    [field: SerializeField] public CharacterController characterController { get; private set; }
    [field: SerializeField] public CameraFOV cameraFOV { get; private set; }
    [field: SerializeField] public ParticleSystem enableEffect { get; private set; }
    [field: SerializeField] public PlayerDataHandle playerData { get; private set; }
    [field: SerializeField] public PlayerVoice voice { get; private set; }
    [field: Space]
    [field: SerializeField] public M_SetEmission setEmission { get; private set; }
    [field: SerializeField] public M_SetFloat setDissolve { get; private set; }
    #endregion

    #region Get & Set Property
    public SO_PlayerConfiguration PlayerConfig => playerData.PlayerConfig;
    public CinemachineFreeLook FreeLookCamera => cameraFOV.cinemachineFreeLook;
    public StatusHandle Health { get; private set; }
    public StatusHandle Stamina { get; private set; }
    public PlayerBaseState CurrentState { get; set; }
    public Vector3 AppliedMovement { get; set; }
    public Vector3 InputMovement { get; set; }
    public float JumpVelocity { get; set; }
    public float Gravity { get; private set; } = -30f;
    public bool IsGrounded => characterController.isGrounded;
    public bool CanMove { get; set; }
    public bool CanRotation { get; protected set; }
    protected bool CanAttack { get; set; }
    public bool IsIdle => input.Move.magnitude == 0;
    public bool IsJump => input.Space && !input.LeftShift && !animator.IsTag("Damage", 1);
    public bool IsWalk => !IsIdle && _movementState == MovementState.StateWalk;
    public bool IsRun => !IsIdle && IsGrounded && !input.LeftShift && _movementState == MovementState.StateRun;
    public bool IsDash => input.LeftShift && IsGrounded && Stamina.CurrentValue >= PlayerConfig.GetDashSTCost();
    public bool CanIncreaseST { get; set; } // có thể tăng ST không ?
    public bool CanFootstepsAudioPlay { get; set; }
    public EventInstance walkFootsteps { get; private set; }
    public EventInstance runFootsteps { get; private set; }
    public EventInstance runfastFootsteps { get; private set; }
    #endregion
    
    #region Animation IDs Paramater
    // FLOAT
    [HideInInspector] public int IDSpeed = Animator.StringToHash("Speed");                
    [HideInInspector] public int IDHorizontal = Animator.StringToHash("Horizontal");       
    [HideInInspector] public int IDVertical = Animator.StringToHash("Vertical");          
    [HideInInspector] public int IDStateTime = Animator.StringToHash("StateTime");         
    // BOOL
    [HideInInspector] public int IDJump = Animator.StringToHash("Jump");                   
    [HideInInspector] public int IDFall = Animator.StringToHash("Fall");                    
    [HideInInspector] public int IDWeaponEquip = Animator.StringToHash("WeaponEquipped "); 
    [HideInInspector] public int ID4Direction = Animator.StringToHash("4Direction");        
    [HideInInspector] public int IDDead = Animator.StringToHash("Dead");                  
    // TRIGGER
    [HideInInspector] public int IDDamageFall = Animator.StringToHash("Damage_Fall");       
    [HideInInspector] public int IDDamageStand = Animator.StringToHash("Damage_Stand");    
    [HideInInspector] public int IDDash = Animator.StringToHash("Dash");                    
    [HideInInspector] public int IDNormalAttack = Animator.StringToHash("NormalAttack");   
    [HideInInspector] public int IDChargedAttack = Animator.StringToHash("ChargedAttack");  
    [HideInInspector] public int IDSkill = Animator.StringToHash("Skill");                  
    [HideInInspector] public int IDSpecial = Animator.StringToHash("Special");             
    #endregion
    
    #region Events
    public event Action OnDashEvent;
    public event Action OnDieEvent;
    public event Action<float> OnRevivalTimeEvent;
    #endregion

    #region Variables Config
    private PlayerStateFactory _state;
    protected enum MovementState
    {
        StateWalk,
        StateRun
    }
    [HideInInspector] protected MovementState _movementState;
    [HideInInspector] protected Camera _mainCamera;
    [HideInInspector] private float _delayIncreaseST;
    [HideInInspector] protected int _attackCounter;
    [HideInInspector] protected float _skillCD_Temp;
    [HideInInspector] protected float _burstCD_Temp;
    [HideInInspector] private int _currentHP;
    [HideInInspector] private PLAYBACK_STATE _footstepsPLAYBACK_STATE;
    [HideInInspector] public EventInstance _footstepsInstance;
    #endregion
    #endregion

    private void Awake()
    {
        GetReference();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        
        SetVariables();
        HandleEnable();
        Health.OnDieEvent += () => CurrentState.ChildState.SwitchState(_state.Dead());
    }
    private void Start()
    {
        CreateAudioRef();
    }
    protected virtual void Update()
    {
        HandleInput();
        
        CurrentState.UpdateStates();

        HandleMovement();
        
        HandleStamina();

        HandleFootstepsAudio();
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        
        Health.OnDieEvent -= () => CurrentState.ChildState.SwitchState(_state.Dead());
    }

    private void GetReference()
    {
        _mainCamera = Camera.main;
        _state = new PlayerStateFactory(this);
        Health = new StatusHandle();
        Stamina = new StatusHandle();
    }
    private void CreateAudioRef()
    {
        walkFootsteps = AudioManager.CreateInstance(FMOD_Events.Instance.walkFootsteps);
        runFootsteps = AudioManager.CreateInstance(FMOD_Events.Instance.runFootsteps);
        runfastFootsteps = AudioManager.CreateInstance(FMOD_Events.Instance.runfastFootsteps);
    }

    
    /// <summary> Khởi tạo giá trị biến ban đầu, và sẽ gọi hàm mỗi lần object này được OnEnable. </summary>
    protected virtual void SetVariables()
    { 
        // Set State
        CurrentState = _state.Grounded();
        CurrentState.EnterState();
        
        // Set Config
        CanMove = true;
        CanRotation = true;
        CanIncreaseST = true;
        _movementState = MovementState.StateRun;
        characterController.enabled = true;
        input.PlayerInput.Enable();
    }
    
    private void HandleEnable()
    {
        setDissolve.ChangeDurationApply(1f);
        setDissolve.ChangeCurrentValue(1f);
        setDissolve.ChangeValueSet(0f);
        setDissolve.Apply();
        enableEffect.gameObject.SetActive(true);
        enableEffect.Play();
    }
    private void HandleInput()
    {
        // Giá trị di chuyển
        var trans = transform;
        InputMovement = trans.right * input.Move.x + trans.forward * input.Move.y;
        InputMovement = Quaternion.AngleAxis(_mainCamera.transform.rotation.eulerAngles.y, Vector3.up) * InputMovement;
        
        // Thời gian hồi chiêu
        _skillCD_Temp = _skillCD_Temp > 0 ? _skillCD_Temp - Time.deltaTime : 0;
        _burstCD_Temp = _burstCD_Temp > 0 ? _burstCD_Temp - Time.deltaTime : 0;
        
        if (!input.ChangeState) return; // Chuyển mode: Walk <=> Run
        input.ChangeState = false;
        _movementState = _movementState == MovementState.StateRun ? MovementState.StateWalk : MovementState.StateRun;
    }
    private void HandleMovement()
    {
        if (!CanMove || !characterController.enabled) 
            return;
        
        characterController.Move(AppliedMovement * Time.deltaTime 
                                 + new Vector3(0f, JumpVelocity, 0f) * Time.deltaTime);
    }
    private void HandleStamina()
    {
        if(!CanIncreaseST || Stamina.CurrentValue >= Stamina.MaxValue) return;

        if (_delayIncreaseST > 0)
        {
            _delayIncreaseST -= Time.deltaTime;
            return;
        }

        _delayIncreaseST = .15f;
        Stamina.Increases(2);
    }
    private void HandleFootstepsAudio()
    {
        if (!IsIdle && CanFootstepsAudioPlay && !animator.IsTag("Attack"))
        {
            _footstepsInstance.getPlaybackState(out _footstepsPLAYBACK_STATE);
            if (_footstepsPLAYBACK_STATE == PLAYBACK_STATE.STOPPED)
            {
                _footstepsInstance.start();
            }
        }
        else
        {
            _footstepsInstance.stop(STOP_MODE.ALLOWFADEOUT);
        }
    }
    
    #region Handle Damageable
    public override void CauseDMG(GameObject _gameObject, AttackType _attackType)
    {
        if (!DamageableData.Contains(_gameObject, out var iDamageable)) return;
        var _percentDMG = _attackType switch
        {
            AttackType.NormalAttack => PercentDMG_NA(),
            AttackType.ChargedAttack => PercentDMG_CA(),
            AttackType.ElementalSkill => PercentDMG_ES(),
            AttackType.ElementalBurst => PercentDMG_EB(),
            _ => 1
        };
        var _finalDMG = Mathf.CeilToInt(PlayerConfig.GetATK() * (1 + _percentDMG / 100.0f));

        var _isCrit = false;  // Có kích CRIT không ?
        if (PlayerConfig.IsCRIT)
        {
            var _percentCritDMG = 1 + PlayerConfig.GetCRITDMG() / 100.0f;
            _finalDMG = Mathf.CeilToInt(_finalDMG * _percentCritDMG);
            _isCrit = true;
        } 
        
        // Gọi takeDMG trên đối tượng vừa va chạm
        iDamageable.TakeDMG(_finalDMG, _isCrit);
    }
    public override void TakeDMG(int _damage, bool _isCRIT) 
    {
        InputMovement = Vector3.zero;
        AppliedMovement = Vector3.zero;
        input.Move =Vector3.zero;
        if (animator.IsTag("Dead", 1)) return;
        
        // If (CRIT) -> lấy Random DEF từ 0 -> DEF ban đầu / 2.
        // Else      -> lấy 100% DEF ban đầu
        var _valueDef = _isCRIT ? Random.Range(0, PlayerConfig.GetDEF() * 0.5f) : PlayerConfig.GetDEF();
        
        // Tính DMG thực nhận vào sau khi trừ đi lượng DEF
        var _finalDmg = (int)Mathf.Max(0, _damage - Mathf.Max(0, _valueDef));
        
        Health.Decreases(_finalDmg);
        DMGPopUpGenerator.Instance.Create(transform.position, _finalDmg, _isCRIT, false);
 
        if (Health.CurrentValue <= 0)
        {
            CurrentState.ChildState.SwitchState(_state.Dead());
            return;
        }
        
        if (animator.IsTag("Damage", 1)) return;
        HandleDamage();
        SetPlayerInputState(false);
        CurrentState.ChildState.SwitchState(_isCRIT ? _state.DamageFall() : _state.DamageStand());
    }

    public override float PercentDMG_NA() => PlayerConfig.GetNormalAttackMultiplier()[_attackCounter].GetMultiplier()[PlayerConfig.GetWeaponLevel() - 1];
    public override float PercentDMG_CA() => PlayerConfig.GetChargedAttackMultiplier()[0].GetMultiplier()[PlayerConfig.GetWeaponLevel() - 1];
    public override float PercentDMG_ES() => PlayerConfig.GetElementalSkillMultiplier()[0].GetMultiplier()[PlayerConfig.GetWeaponLevel() - 1]; 
    public override float PercentDMG_EB() => PlayerConfig.GetElementalBurstMultiplier()[0].GetMultiplier()[PlayerConfig.GetWeaponLevel() - 1];
    public void ReleaseDamageState() // gọi trên animationEvent để giải phóng trạng thái TakeDamage
    {
        CanMove = true;
        CurrentState.ChildState.SwitchState(_state.Idle());
        ResetDamageTrigger();
        SetPlayerInputState(true);
    }
    public void SetPlayerInputState(bool _stateValue)
    {
        if (_stateValue)
        {
            input.PlayerInput.Player.Move.Enable();
            input.PlayerInput.Player.Jump.Enable();
            input.PlayerInput.Player.NormalAttack.Enable();
            input.PlayerInput.Player.ElementalSkill.Enable();
            input.PlayerInput.Player.ElementalBurst.Enable();
            return;
        }
        input.PlayerInput.Player.Move.Disable();
        input.PlayerInput.Player.Jump.Disable();
        input.PlayerInput.Player.NormalAttack.Disable();
        input.PlayerInput.Player.ElementalSkill.Disable();
        input.PlayerInput.Player.ElementalBurst.Disable();
    }
    public void ResetDamageTrigger()
    {
        animator.ResetTrigger(IDDamageFall);
        animator.ResetTrigger(IDDamageStand);
    }
    #endregion
    
    #region Event Callback
    public void CallbackDashEvent() => OnDashEvent?.Invoke();
    public void CallbackDieEvent() => OnDieEvent?.Invoke();
    public void CallbackRevivalTimeEvent(float _time) => OnRevivalTimeEvent?.Invoke(_time);
    #endregion
    

    /// <summary> Khi nhận sát thương, nếu nhân vật cần thực hiện hành vi thì Override lại. </summary>
    protected virtual void HandleDamage() { }

    /// <summary> Giải phóng tất cả trạng thái khi nhảy, lướt,... </summary>
    public abstract void ReleaseAction();
}

