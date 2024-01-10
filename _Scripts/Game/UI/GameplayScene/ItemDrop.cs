using System;
using System.Collections;
using FMODUnity;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemDrop : MonoBehaviour, IPooled<ItemDrop>
{
    [SerializeField] private StudioEventEmitter studioEventEmitter;
    [SerializeField] private SpriteRenderer spriteRender;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float _jumpForce = 10f;

    private ItemReward _itemReward;
    private Coroutine _timeActiveCoroutine;
    private bool _canAudioPlay;
    
    
    private void OnEnable()
    {
        _canAudioPlay = true;
        
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
        var jumpDirection = new Vector3(Random.Range(-.4f, .4f), 1f, Random.Range(-.4f, .4f)).normalized;
        rb.velocity = jumpDirection * (_jumpForce + Random.Range(0, 1.5f));
        rb.useGravity = true;
        
        if(_timeActiveCoroutine != null) StopCoroutine(TimerCoroutine());
        _timeActiveCoroutine = StartCoroutine(TimerCoroutine());
    }
    
    private IEnumerator TimerCoroutine()
    {
        yield return new WaitForSeconds(Random.Range(80, 100));
        if(gameObject.activeSelf) Release();
    }
    public void SetItemDrop(Sprite _sprite, ItemReward _itemRewardData)
    {
        _itemReward = _itemRewardData;
        spriteRender.sprite = _sprite;
    }
    
    
    public void OnTriggerEnterPlayer() => RewardManager.Instance.AddNoticeReward(this, _itemReward);
    public void OnTriggerExitPlayer() => RewardManager.Instance.RemoveNoticeReward(this);
    
    private void OnCollisionEnter(Collision other)
    {
        if (!_canAudioPlay) return;
        _canAudioPlay = false;
        studioEventEmitter.Play();
    }
    public void Release()
    {
        studioEventEmitter.Stop();
        ReleaseCallback?.Invoke(this);
    }
    public Action<ItemDrop> ReleaseCallback { get; set; }
}
