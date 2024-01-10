using UnityEngine;
using Random = UnityEngine.Random;

public class GoblinSlingshot_Effects : MonoBehaviour, IAttack
{
    [SerializeField] private EnemyController enemyController;

    [Tooltip("Điểm xuất hiện effect"), SerializeField] 
    private Transform effectPoint;

    [Header("Prefab projectile"),SerializeField] 
    private EffectBase projectilePrefab;

    private ObjectPooler<EffectBase> _poolProjectile;
    private Transform slotsVFX;


    private void Awake()
    {
        InitValue();
    }
    private void OnEnable()
    {
        RegisterEvents();
    }
    private void OnDisable()
    {
        UnRegisterEvents();
    }


    private void InitValue()
    {
        slotsVFX = GameObject.FindWithTag("SlotsVFX").transform;
        _poolProjectile = new ObjectPooler<EffectBase>(projectilePrefab, slotsVFX, 8);
    }
    private void RegisterEvents()
    {        
        _poolProjectile.List.ForEach(projectile => projectile.detectionType.CollisionEnterEvent.AddListener(Detection_NA));
    }
    private void UnRegisterEvents()
    {
        _poolProjectile.List.ForEach(projectile => projectile.detectionType.CollisionEnterEvent.RemoveListener(Detection_NA));
    }
    public void EffectAttack()
    {
        var playerPos = enemyController.PlayerPosition;
        playerPos.y += Random.Range(1f, 1.5f);
        var rotation = Quaternion.LookRotation(playerPos - effectPoint.position);
        var projectile = _poolProjectile.Get(effectPoint.position, rotation);
        projectile.FIRE();
    }
    
    public void Detection_NA(GameObject _gameObject) => enemyController.CauseDMG(_gameObject, AttackType.NormalAttack);
    public void Detection_CA(GameObject _gameObject) { }
    public void Detection_ES(GameObject _gameObject) { }
    public void Detection_EB(GameObject _gameObject) { }
}
