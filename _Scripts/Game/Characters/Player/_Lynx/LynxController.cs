using System;
using System.Collections;
using UnityEngine;
using Cinemachine;
using FMODUnity;
using Spine.Unity;


public class LynxController : PlayerController
{
    [Header("SubClass -------")]
    [Tooltip("Script quản lí vũ khí của Archer"), SerializeField]
    private LynxEffects _effects;           
    [Tooltip("Visuel Effect đánh dấu vị trí SpecialAttac xuất hiện")]
    public GameObject indicatorQ;
    
    [Tooltip("Tâm ngắm"), SerializeField]
    private SkeletonGraphic crosshair;
    
    
    [Tooltip("Layer kiểm tra va chạm khi giữ tâm ngắm"), SerializeField]
    private LayerMask crosshairMask;
    
    [Space(10)]
    [Tooltip("Camera khi hold attack"), SerializeField]
    private CinemachineVirtualCamera aimCam;

    [Tooltip("Audio khi Player nhắm."), SerializeField] 
    private EventReference aimAudio;
    
    [Tooltip("Đối tượng mà aim camera theo dõi"),SerializeField] 
    private Transform targetCameraFocus;
    
    [Tooltip("Giá trị xoay, nhạy khi active iamCam")]
    [SerializeField] private AxisState xAxis;
    [SerializeField] private AxisState yAxis;

    public float ChargedAttackTime { get; private set; }
    [HideInInspector] private bool _lockCrosshair;       // có khóa tâm ngắm không
    [HideInInspector] private Vector3 worldPosition;
    [HideInInspector] private float _horizontalBlend;
    [HideInInspector] private float _verticalBlend;
    [HideInInspector] private float _percentDMGCharged;
    private Ray _ray;
    private Coroutine _attackCoroutine;
    
    
    protected override void Update()
    {
        base.Update();
        
        HandleAttack();

        CheckCrosshair();
    }
    
    protected override void SetVariables()
    {
        base.SetVariables();

        _lockCrosshair = false;
        crosshair.gameObject.SetActive(false);
        indicatorQ.transform.SetParent(null);
    }
    private void CheckCrosshair()
    {
        if (_lockCrosshair)
        {
             worldPosition = Vector3.zero;
             var screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
             _ray = _mainCamera.ScreenPointToRay(screenCenterPoint);
             worldPosition = _ray.GetPoint(1000f);
             
             if (Physics.Raycast(_ray, out var raycastHit, 1000f, crosshairMask))
             {
                 worldPosition = raycastHit.point;
             }
             _effects.attackPoint.rotation = Quaternion.LookRotation(worldPosition - _effects.attackPoint.position);
        }
        else
        {
            _effects.attackPoint.rotation = Quaternion.Euler(0f, model.eulerAngles.y, 0f);
        }
    }
    
    protected override void ChargedAttack()
    {
        if(_attackCoroutine != null) 
            StopCoroutine(_attackCoroutine);
        _attackCoroutine = StartCoroutine(AttackHoldingCoroutine());
        
        base.ChargedAttack();
    }
    private IEnumerator AttackHoldingCoroutine()
    {
        InitChargedAttackValue();
        
        while (IsNormalAttack)
        {
            BlendAnimationValue();
            AimCamRotation();
            _lockCrosshair = IsNormalAttack;
            animator.SetBool(ID4Direction, IsNormalAttack);

            ChargedAttackTime += Time.deltaTime;
            _percentDMGCharged = Mathf.MoveTowards(_percentDMGCharged, PlayerConfig.GetChargedAttackMultiplier()[1].GetMultiplier()[PlayerConfig.GetWeaponLevel() - 1], 15f * Time.deltaTime);
            yield return null;
        }
        PercentDMG_CA();
        EndChargedAttack();
    }
    // ReSharper disable Unity.PerformanceAnalysis
    private void InitChargedAttackValue()
    {
        // Animation
        _horizontalBlend = 0;
        _verticalBlend = 0;
        
        // Aim Camera
        aimCam.gameObject.SetActive(true);
        xAxis.Value = FreeLookCamera.m_XAxis.Value;
        yAxis.Value = FreeLookCamera.m_YAxis.Value;
        targetCameraFocus.rotation = _mainCamera.transform.rotation;
        
        // Crosshair
        crosshair.gameObject.SetActive(true);
        crosshair.SetAnimation("Center_IN", false);
        crosshair.AddAnimation("Center_Wait", false, 1);

        // State
        _movementState = MovementState.StateWalk;
        
        // DMG
        ChargedAttackTime = 0;
        _percentDMGCharged = PlayerConfig.GetChargedAttackMultiplier()[0].GetMultiplier()[PlayerConfig.GetWeaponLevel() - 1];
        
        // Audio
        AudioManager.PlayOneShot(aimAudio, transform.position);
    }
    private void EndChargedAttack()
    {
        _movementState = MovementState.StateRun;
        animator.SetBool(ID4Direction, false);
        FreeLookCamera.m_XAxis.Value = xAxis.Value;
        
        if(IsJump || IsDash) return;
        CanMove = false;
        CanRotation = false;
    }
    private void BlendAnimationValue()
    {
        _horizontalBlend = Mathf.MoveTowards(_horizontalBlend, input.Move.x, 5 * Time.deltaTime);
        _verticalBlend = Mathf.MoveTowards(_verticalBlend, input.Move.y, 5 * Time.deltaTime);

        animator.SetFloat(IDHorizontal, _horizontalBlend);
        animator.SetFloat(IDVertical, _verticalBlend);
    }
    private void AimCamRotation()
    {
        xAxis.Update(Time.fixedDeltaTime);
        yAxis.Update(Time.fixedDeltaTime);
        targetCameraFocus.eulerAngles = new Vector3(yAxis.Value, xAxis.Value, 0);

        model.rotation = Quaternion.Slerp(model.rotation, Quaternion.Euler(0, _mainCamera.transform.eulerAngles.y, 0), 50f * Time.deltaTime);
    }
    protected override void ElementalBurst()
    {
        if(!IsElementalBurst) return;
        
        if(_attackCoroutine != null) 
            StopCoroutine(_attackCoroutine);
        _attackCoroutine = StartCoroutine(SpecialCoroutine());
    }
    private IEnumerator SpecialCoroutine()
    {
        // Visual Effect
        indicatorQ.SetActive(true);

        input.Q = false;
        input.LeftMouse = false;
        CanAttack = false;
        while (true)
        {
            var cameraTransform = _mainCamera.transform;
            var cameraForward = cameraTransform.forward;
            var ray = new Ray(cameraTransform.position + new Vector3(0f, 2f, 0), cameraForward);

            if (Physics.Raycast(ray, out var raycastHit, 40, crosshairMask))
            {
                cameraForward.y = 0;
                indicatorQ.transform.position = raycastHit.point + new Vector3(0, .15f, 0);
                indicatorQ.transform.rotation = Quaternion.FromToRotation(Vector3.up, raycastHit.normal) * Quaternion.LookRotation(cameraForward);
            }
            else
            {
                indicatorQ.SetActive(false);
                CanAttack = true;
                yield break;
            }

            if (input.Q || input.LeftMouse)
            {
                CanMove = false;
                CanRotation = false;
                CanAttack = false;
                model.rotation = Quaternion.Euler(0, _mainCamera.transform.eulerAngles.y, 0);
                animator.SetTrigger(IDSpecial);
                
                _burstCD_Temp = PlayerConfig.GetElementalBurstCD();
                OnSpecialCooldownEvent();
                
                yield break;
            }
            yield return null;
        }
    }

    protected override void AttackEnd()
    {
        base.AttackEnd();
        _lockCrosshair = false;
        aimCam.gameObject.SetActive(false);
        crosshair.gameObject.SetActive(false);
        indicatorQ.gameObject.SetActive(false);
    }
    protected override void SpecialEnd()
    {
        base.SpecialEnd();
        
        animator.ResetTrigger(IDSpecial);
        indicatorQ.SetActive(false);
    }
    public override void ReleaseAction()
    {
        base.ReleaseAction();
        _effects.TurnOffFxHold();
    }
    
    public override float PercentDMG_CA() => _percentDMGCharged; // tìm %DMG dựa theo thời gian Holding, % tối đa = PlayerConfig.ChargedAttackMultiplier[1].Multiplier[PlayerConfig.WeaponLevel - 1]
    public override float PercentDMG_ES() => base.PercentDMG_ES() + 36; // % cộng thêm từ vũ khí 
    public override float PercentDMG_EB() => base.PercentDMG_EB() + 36; // % cộng thêm từ vũ khí 
}
