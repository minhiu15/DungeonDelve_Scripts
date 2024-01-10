using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class ArlanEffects : MonoBehaviour, IAttack
{
    [Tooltip("Script điều khiển chính"), SerializeField] 
    private ArlanController arlanController;

    [Tooltip("Kiểm tra va chạm của Normal Attack"), SerializeField]
    private PhysicsDetection NA_Detection;
    
    [Tooltip("Kiểm tra va chạm của Charged Attack"), SerializeField]
    private PhysicsDetection CA_Detection;
    
    [Tooltip("Kiểm tra va chạm của Elenmental Burst"), SerializeField]
    private PhysicsDetection EB_Detection;
    
    [Tooltip("Đếm ngược thời gian mà Shield(Skil) của Warrior hoạt động"), SerializeField]
    private CooldownTime shieldCooldownTime;
    
    [Space, Tooltip("Vị trí sẽ xuất hiện effects")] 
    public Transform effectPoint;
    
    [Tooltip("Góc xoay của từng Effect chém")] 
    public List<Vector3> effectAngle;
    
    [Header("Prefab projectile")] 
    [SerializeField] private Reference swordSlashPrefab;
    [SerializeField] private Reference swordPrickPrefab;
    [SerializeField] private Reference swordHoldingPrefab;

    [Space]
    [SerializeField] private Reference hitPrefab;

    [Header("Visual Effects")] 
    [SerializeField] private ParticleSystem skill;
    [SerializeField] private ParticleSystem special;

    
    private Transform slotsVFX;
    private ObjectPooler<Reference> _poolSwordSlash;
    private ObjectPooler<Reference> _poolSwordPrick;
    private ObjectPooler<Reference> _poolSwordHolding;

    private ObjectPooler<Reference> _poolHit;
    
    private Vector3 _posEffect;
    private Quaternion _rotEffect;

    
    // Coroutine
    private Coroutine _skillCoroutine;

    
    private void Start()
    {
        Initialized();
    }
    private void Initialized()
    {
        slotsVFX = GameObject.FindWithTag("SlotsVFX").transform;
        _poolSwordSlash = new ObjectPooler<Reference>(swordSlashPrefab, slotsVFX, 5);
        _poolSwordPrick = new ObjectPooler<Reference>(swordPrickPrefab, slotsVFX, 5);
        _poolSwordHolding = new ObjectPooler<Reference>(swordHoldingPrefab, slotsVFX, 5);
        _poolHit = new ObjectPooler<Reference>(hitPrefab, slotsVFX, 5);
    }
    

    private void EffectSlash(AnimationEvent eEvent)
    {
        _posEffect = effectPoint.position;
        _rotEffect = Quaternion.Euler(effectAngle[eEvent.intParameter].x, 
                                    effectAngle[eEvent.intParameter].y + effectPoint.eulerAngles.y, 
                                      effectAngle[eEvent.intParameter].z);
        
        _poolSwordSlash.Get(_posEffect, _rotEffect);
        arlanController.AddForceAttack();
    }
    private void EffectPrick(AnimationEvent eEvent)
    {
        _posEffect = effectPoint.position;
        _rotEffect = effectPoint.rotation;
        
        _poolSwordPrick.Get(_posEffect, _rotEffect);
        arlanController.AddForceAttack();
    }
    private void EffectHolding(AnimationEvent eEvent)
    {
        _posEffect = effectPoint.position;
        _rotEffect = Quaternion.Euler(-66f, -105f + effectPoint.eulerAngles.y, -122f);
        
       _poolSwordHolding.Get(_posEffect, _rotEffect);
    }
    private void EffectSkill(AnimationEvent eEvent)
    {
        skill.Clear();
        skill.gameObject.SetActive(true);
        skill.Play();
        
        if(_skillCoroutine != null)
            StopCoroutine(_skillCoroutine);
        _skillCoroutine = StartCoroutine(SkillCoroutine());
    }
    private IEnumerator SkillCoroutine()
    {
        arlanController.BuffSkill();
        var shieldTime = 15f;
        shieldCooldownTime.StartCooldownTime(shieldTime);
        yield return new WaitForSeconds(shieldTime);
        skill.Stop();
        skill.gameObject.SetActive(false);
        arlanController.UnBuffSkill();
    }
    private void EffectSpecial(AnimationEvent eEvent)
    {
        special.gameObject.SetActive(true);
        special.Play();
    }

    
    public void EffectHit(Vector3 _pos) => _poolHit.Get(RandomPosition(_pos, -.15f, .15f));
    private static Vector3 RandomPosition(Vector3 _posCurrent, float minVal, float maxVal)
    {
        return _posCurrent + new Vector3(Random.Range(minVal, maxVal), 
                                         Random.Range(minVal, maxVal), 
                                         Random.Range(minVal, maxVal));
    }

    
    public void CheckNACollision() => NA_Detection.CheckCollision(); // gọi trên Event Animation
    public void CheckCACollision() => CA_Detection.CheckCollision(); // gọi trên Event Animation
    public void CheckEBCollision() => EB_Detection.CheckCollision(); // gọi trên ParticalSystem
    
    public void Detection_NA(GameObject _gameObject) => arlanController.CauseDMG(_gameObject, AttackType.NormalAttack);
    public void Detection_CA(GameObject _gameObject) => arlanController.CauseDMG(_gameObject, AttackType.ChargedAttack);
    public void Detection_ES(GameObject _gameObject) => arlanController.CauseDMG(_gameObject, AttackType.ElementalSkill);
    public void Detection_EB(GameObject _gameObject) => arlanController.CauseDMG(_gameObject, AttackType.ElementalBurst);
}
