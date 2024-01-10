using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(MonoBehaviourID))]
public class ItemDropOnDamage : Damageable
{
    [SerializeField] private MonoBehaviourID behaviourID;
    [Space]
    [SerializeField] private int rewardQuantity = 5;
    [SerializeField] private int minutesReset = 60;
    [SerializeField] private ItemReward itemReward;
    [Header("UI")]
    [SerializeField] private ProgressBar healthBar;
    [SerializeField] private CanvasGroup alphaHUD;
    
    private StatusHandle _health;
    private Coroutine _timeCoroutine;
    private Coroutine _disableHUDCoroutine;
    private YieldInstruction _yieldCheckTime;
    private YieldInstruction _yieldDisable;
    private Tween _alphaTween;
    private bool _canTrigger;

    private void Awake()
    {
        _health = new StatusHandle();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        _health.OnDieEvent += Die;
        _health.OnInitValueEvent += healthBar.Init;
        _health.OnCurrentValueChangeEvent += healthBar.OnCurrentValueChange;
    }
    private void Start()
    {
        InitValue();
        
        if (_timeCoroutine != null)
            StopCoroutine(_timeCoroutine);
        _timeCoroutine = StartCoroutine(TimeCoroutine());
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        _health.OnDieEvent -= Die;
        _health.OnInitValueEvent -= healthBar.Init;
        _health.OnCurrentValueChangeEvent -= healthBar.OnCurrentValueChange;
    }

    
#if UNITY_EDITOR
    [ContextMenu("Delete PlayerPrefs Key")]
    private void ResetPlayerPrefsKey()
    {
        PlayerPrefs.DeleteKey(behaviourID.GetID);
        Debug.Log("Delete PlayerPrefs Key Success !");
    }
#endif
    
    private void InitValue()
    {
        alphaHUD.alpha = 0;
        alphaHUD.gameObject.SetActive(false);
        _canTrigger = CheckTime();
        _health.InitValue(rewardQuantity, rewardQuantity);
        _yieldCheckTime = new WaitForSeconds(10f);
        _yieldDisable = new WaitForSeconds(15f);
    }
    private IEnumerator TimeCoroutine()
    {
        while (true)
        {
            if (!_canTrigger && CheckTime())
            {
                Revival();
            }
            yield return _yieldCheckTime;
        }
    }
    private IEnumerator DisableHUDCoroutine()
    {
        if (!alphaHUD.gameObject.activeSelf)
        {
            alphaHUD.gameObject.SetActive(true);
        }
        if (alphaHUD.alpha == 0)
        {
            _alphaTween?.Kill();
            _alphaTween = alphaHUD.DOFade(1, .2f);
        }
        yield return _yieldDisable;
        _alphaTween?.Kill();
        _alphaTween = alphaHUD.DOFade(0, .2f).OnComplete(() =>
        {
            alphaHUD.gameObject.SetActive(false);
        });
    }
    private bool CheckTime()
    {
        var _lastTime = DateTime.Parse(PlayerPrefs.GetString(behaviourID.GetID, DateTime.MinValue.ToString()));
        return DateTime.Now.Subtract(_lastTime).TotalMinutes >= minutesReset;
    }
    
    public override void TakeDMG(int _damage, bool _isCRIT)
    {
        if (!_canTrigger) return;
        _health.Decreases(1);
        RewardManager.Instance.CreateReward(itemReward, alphaHUD.transform.position);
        
        if (_disableHUDCoroutine != null) 
            StopCoroutine(_disableHUDCoroutine);
        _disableHUDCoroutine = StartCoroutine(DisableHUDCoroutine());
    }
    private void Die()
    {
        _canTrigger = false;
        _alphaTween?.Kill();
        _alphaTween = alphaHUD.DOFade(0, .2f).OnComplete(() =>
        {
            alphaHUD.gameObject.SetActive(false);
        });
        PlayerPrefs.SetString(behaviourID.GetID, DateTime.Now.ToString());
    }
    private void Revival()
    {
        
        _canTrigger = true;
        _health.InitValue(rewardQuantity, rewardQuantity);
        PlayerPrefs.SetString(behaviourID.GetID, DateTime.MinValue.ToString());
    }
    
}
    