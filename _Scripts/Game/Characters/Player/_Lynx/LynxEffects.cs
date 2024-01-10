using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class LynxEffects : MonoBehaviour, IAttack
{
    [Tooltip("Script điều khiển chính"), SerializeField]
    private LynxController lynxController;
    
    [Space(10), Tooltip("Góc độ bắn mũi tên lên"), SerializeField, Range(-40, 0)] 
    private float angleXAttack;  
    
    [Tooltip("Vị trí sẽ xuất hiện projectile")] 
    public Transform attackPoint;
    
    [Header("Prefab projectile")] 
    [SerializeField] private EffectBase arrowNormalPrefab;
    [SerializeField] private EffectBase arrowChargedPrefab;
    [SerializeField] private EffectBase arrowChargedNoFullyPrefab;
    [SerializeField] private EffectBase arrowFire;
    
    [Header("Visual Effect")]
    [SerializeField] private ParticleSystem effectHolding;
    [SerializeField] private ParticleSystem effectSpecial;
    
    private Transform slotsVFX;
    private ObjectPooler<EffectBase> _poolArrowNormal;
    private ObjectPooler<EffectBase> _poolArrowSkill;
    private ObjectPooler<EffectBase> _poolArrowChargedFully;
    private ObjectPooler<EffectBase> _poolArrowChargedNoFully;
    private ObjectPooler<EffectBase> _poolArrowBurst;

    private float angleYAttack => lynxController.model.eulerAngles.y;
    
    // Coroutine
    private Coroutine _specialCoroutine;

    
    private void Start()
    {
        Initialized();
        RegisterEvent();
    }
    private void OnDestroy()
    {
        UnRegisterEvent();
    }

    
    private void Initialized()
    {
        slotsVFX = GameObject.FindWithTag("SlotsVFX").transform; 
        _poolArrowNormal = new ObjectPooler<EffectBase>(arrowNormalPrefab, slotsVFX, 25);
        _poolArrowChargedFully = new ObjectPooler<EffectBase>(arrowChargedPrefab, slotsVFX, 15);
        _poolArrowChargedNoFully = new ObjectPooler<EffectBase>(arrowChargedNoFullyPrefab, slotsVFX, 15);
        _poolArrowSkill = new ObjectPooler<EffectBase>(arrowNormalPrefab, slotsVFX, 25);
        _poolArrowBurst = new ObjectPooler<EffectBase>(arrowFire, slotsVFX, 50);
        
        effectSpecial.transform.SetParent(slotsVFX);
    }
    private void RegisterEvent()
    {
        _poolArrowNormal.List.ForEach(arrow => arrow.detectionType.CollisionEnterEvent.AddListener(Detection_NA));
        _poolArrowChargedFully.List.ForEach(arrow => arrow.detectionType.CollisionEnterEvent.AddListener(Detection_CA));
        _poolArrowChargedNoFully.List.ForEach(arrow => arrow.detectionType.CollisionEnterEvent.AddListener(Detection_CA));
        _poolArrowSkill.List.ForEach(arrow => arrow.detectionType.CollisionEnterEvent.AddListener(Detection_ES));
        _poolArrowBurst.List.ForEach(arrow => arrow.detectionType.CollisionEnterEvent.AddListener(Detection_EB));
    }
    private void UnRegisterEvent()
    {
        _poolArrowNormal.List.ForEach(arrow => arrow.detectionType.CollisionEnterEvent.RemoveListener(Detection_NA));
        _poolArrowChargedFully.List.ForEach(arrow => arrow.detectionType.CollisionEnterEvent.RemoveListener(Detection_CA));
        _poolArrowChargedNoFully.List.ForEach(arrow => arrow.detectionType.CollisionEnterEvent.RemoveListener(Detection_CA));
        _poolArrowSkill.List.ForEach(arrow => arrow.detectionType.CollisionEnterEvent.RemoveListener(Detection_ES));
        _poolArrowBurst.List.ForEach(arrow => arrow.detectionType.CollisionEnterEvent.RemoveListener(Detection_EB));
    }
    
    
    private void EffectArrowCombo(AnimationEvent eEvent)
    {
        Quaternion _quaternion;
        var _checClosestEnemy  = EnemyTracker.FindClosestEnemy(transform, out var posTarget);
        if (!_checClosestEnemy)
        {
            _quaternion = Quaternion.Euler(angleXAttack, angleYAttack, 0f);
        }
        else
        {
            posTarget.y += 1.3f;
            var randRotX = Random.Range(-2f, 2f);
            var randRotY = Random.Range(-2f, 2f);
            _quaternion = Quaternion.LookRotation(posTarget - attackPoint.transform.position) * Quaternion.Euler(randRotX, randRotY, 0);
        }
                
        var arrow = _poolArrowNormal.Get(attackPoint.position, _quaternion);
        arrow.FIRE();
        lynxController.AddForceAttack();
    }
    private void EffectArrowHold(AnimationEvent eEvent)
    {
        TurnOffFxHold();
        EffectBase arrow;
        if (lynxController.ChargedAttackTime >= 3.5f)
        {
            arrow = _poolArrowChargedFully.Get(attackPoint.position, attackPoint.rotation);
            lynxController.voice.PlayMidAttack();
        }
        else
        {
            arrow = _poolArrowChargedNoFully.Get(attackPoint.position, attackPoint.rotation);
        }
        arrow.FIRE();
    }
    public void TurnOnFxHold()
    {
        effectHolding.gameObject.SetActive(true);
        effectHolding.Play();
    }
    public void TurnOffFxHold()
    {
        effectHolding.Stop();
        effectHolding.gameObject.SetActive(false);
    }
    
    private void Effect_Skill(AnimationEvent eEvent)
    {
        var position = attackPoint.position;
        var rotation = Quaternion.Euler(EnemyTracker.DetectEnemy ? -6f : angleXAttack, 
                                                                                attackPoint.eulerAngles.y + eEvent.intParameter,
                                                                                attackPoint.eulerAngles.z);
        var arrow = _poolArrowSkill.Get(position, rotation);
        arrow.FIRE();
    }
    private void EffectSpecial(AnimationEvent eEvent)
    {
        if (_specialCoroutine != null) 
            StopCoroutine(_specialCoroutine);
        _specialCoroutine = StartCoroutine(ActiveFXSpecial());
    }
    private IEnumerator ActiveFXSpecial()
    {
        // In
        effectSpecial.Play();
        yield return new WaitForSeconds(.3f);
        effectSpecial.transform.position = attackPoint.position;
        effectSpecial.transform.rotation = Quaternion.Euler(-50f, angleYAttack, 0);
        effectSpecial.gameObject.SetActive(true);
        yield return new WaitForSeconds(.85f);
        
        // Out
        var maxRadius = 2.5f;
        for (var i = 0; i < 30; i++)
        {
            // lấy 1 vị tri ngẫu nhiên trong bán kính maxRadius
            var randomPoint = Random.insideUnitCircle * maxRadius;

            var _target = lynxController.indicatorQ.transform.position;
            var currentPos = _target + new Vector3(randomPoint.x, 5f, randomPoint.y);
            var targetPos = _target + new Vector3(randomPoint.x, 0.1f, randomPoint.y);
            
            var arrow = _poolArrowBurst.Get(currentPos, Quaternion.LookRotation(targetPos - currentPos));
            arrow.FIRE();
            yield return new WaitForSeconds(0.1f);
        }
        
        effectSpecial.gameObject.SetActive(false);
        effectSpecial.Stop();
    }

    
    public void Detection_NA(GameObject _gameObject) => lynxController.CauseDMG(_gameObject, AttackType.NormalAttack);
    public void Detection_CA(GameObject _gameObject) => lynxController.CauseDMG(_gameObject, AttackType.ChargedAttack);
    public void Detection_ES(GameObject _gameObject) => lynxController.CauseDMG(_gameObject, AttackType.ElementalSkill);
    public void Detection_EB(GameObject _gameObject) => lynxController.CauseDMG(_gameObject, AttackType.ElementalBurst);
}