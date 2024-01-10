using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class ReaperEffects : MonoBehaviour, IAttack
{
    [SerializeField] private EnemyController enemyController;
    [SerializeField] private PhysicsDetection NA_Detection;
    [SerializeField] private PhysicsDetection ES_Detection;
    
    [Tooltip("Effect Ping NormalAttack"), SerializeField] 
    private GameObject indicatorNormalAttack;
    
    [Tooltip("Effect Ping Skill"), SerializeField] 
    private GameObject indicatorSkill;
    
    [Tooltip("Vị trí xuất hiện Attack Effect"), SerializeField]
    private Transform effectPosition;
    
    [Tooltip("Góc xoay của từng Effect Slash trên Normal Attack"), SerializeField] 
    private List<EffectOffset> effectOffsets;

    [Header("Prefab projectile")] 
    [SerializeField] private Reference indicatorPrefab;
    [SerializeField] private Reference slashPrefab;
    [SerializeField] private Reference hitPrefab;
    [SerializeField] private PhysicsDetection specialPrefab;

    [Header("Visual Effect")] 
    [SerializeField] private ParticleSystem shieldEffect;
    [SerializeField] private ParticleSystem skillEffect;
    
    private ObjectPooler<Reference> _poolIndicator;
    private ObjectPooler<Reference> _poolSlash;
    private ObjectPooler<PhysicsDetection> _poolSpecial;
    private ObjectPooler<Reference> _poolHit;

    private Transform _slotVFX;
    private Vector3 _posEffect;
    private Quaternion _rotEffect;
    private Coroutine _skillCoroutine;
    
    private void Start()
    {
        Initialized();
        RegisterEvents();
    }
    private void OnDestroy()
    {
        UnRegisterEvents();
    }
    

    private void Initialized()
    {
        _slotVFX = GameObject.FindGameObjectWithTag("SlotsVFX").transform;

        _poolIndicator = new ObjectPooler<Reference>(indicatorPrefab, _slotVFX, 15);
        _poolSlash = new ObjectPooler<Reference>(slashPrefab, _slotVFX, 10);
        _poolHit = new ObjectPooler<Reference>(hitPrefab, _slotVFX, 35);
        _poolSpecial = new ObjectPooler<PhysicsDetection>(specialPrefab, _slotVFX, 50);
        
        skillEffect.transform.SetParent(_slotVFX);
        indicatorSkill.transform.SetParent(_slotVFX);
        indicatorNormalAttack.transform.SetParent(_slotVFX);
    }

    private void RegisterEvents()
    {
        foreach (var VARIABLE in _poolSpecial.List)
        {
            VARIABLE.CollisionEnterEvent.AddListener(Detection_NA);
            VARIABLE.PositionEnterEvent.AddListener(EffectHit);
        }
    }
    private void UnRegisterEvents()
    {
        foreach (var VARIABLE in _poolSpecial.List)
        {
            VARIABLE.CollisionEnterEvent.RemoveListener(Detection_NA);
            VARIABLE.PositionEnterEvent.RemoveListener(EffectHit);
        }
    }
    
    
    private void EffectSlashNA(AnimationEvent eEvent)
    {
        _posEffect = effectPosition.position + effectPosition.rotation * effectOffsets[eEvent.intParameter].position;
        _rotEffect = Quaternion.Euler(effectOffsets[eEvent.intParameter].rotation.x ,
                                    effectOffsets[eEvent.intParameter].rotation.y + effectPosition.eulerAngles.y,
                                      effectOffsets[eEvent.intParameter].rotation.z );
        
        _poolSlash.Get(_posEffect, _rotEffect);
    }
    public void GetIndicator(Vector3 _position) => _poolIndicator.Get(_position);
    public void GetSpecialEffect(Vector3 _position) => _poolSpecial.Get(_position);
    
    public void CheckNACollision() => NA_Detection.CheckCollision(); // gọi trên event Animation
    public void CheckESCollision() => ES_Detection.CheckCollision();
    
    public void EffectHit(Vector3 _pos) => _poolHit.Get(_pos + new Vector3(Random.Range(-.1f, .1f), Random.Range(.5f, 1f), 0));
    public void EffectSkill(Vector3 _pos)
    {
        if(_skillCoroutine != null) StopCoroutine(_skillCoroutine);
        _skillCoroutine = StartCoroutine(SkillCoroutine(_pos));
    }
    private IEnumerator SkillCoroutine(Vector3 _pos)
    {
        shieldEffect.gameObject.SetActive(true);
        shieldEffect.Play();
        indicatorSkill.transform.position = _pos;
        indicatorSkill.gameObject.SetActive(true);
        
        yield return new WaitForSeconds(2.5f);
        indicatorSkill.gameObject.SetActive(false);
        skillEffect.transform.position = _pos;
        skillEffect.gameObject.SetActive(true);
        skillEffect.Play();
        
        yield return new WaitForSeconds(.1f);
        CheckESCollision();
        
        yield return new WaitForSeconds(1.2f);
        skillEffect.Stop();
        skillEffect.gameObject.SetActive(false);
    }

    public void SetAttackCounter(int count) => enemyController.SetAttackCount(count); // Gọi trên animationEvent để set đòn đánh thứ (x)
    public void Detection_NA(GameObject _gameObject) => enemyController.CauseDMG(_gameObject, AttackType.NormalAttack);
    public void Detection_CA(GameObject _gameObject) { }
    public void Detection_ES(GameObject _gameObject) => enemyController.CauseDMG(_gameObject, AttackType.ElementalSkill);
    public void Detection_EB(GameObject _gameObject) => enemyController.CauseDMG(_gameObject, AttackType.ElementalBurst);
}
