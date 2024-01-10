using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SphereCollider))]
public class CandlesFire : MonoBehaviour
{
    [SerializeField] private new Light light;
    [SerializeField] private ParticleSystem fire;

    public UnityEvent OnFireEnableEvent, OnFireDisableEvent;
    
    [SerializeField, Tooltip("Thời gian Object hoạt động (s)")]
    private float timeActive = 60;

    private Coroutine _disableCoroutine;
    private Tween _lightTween;
    private readonly float _enableDuration = 1.4f;
    private readonly float _disableDuration = .5f;
    private float _currentIntensity;
    private bool _canEnable;
    
    private void Start()
    {
        _canEnable = true;
        _currentIntensity = light.intensity;
        light.intensity = 0;
        light.gameObject.SetActive(false);
        //
        fire.gameObject.SetActive(false);
        fire.Stop();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Fire") || !_canEnable) return;

        _canEnable = false;
        OnFireEnableEvent?.Invoke();
        fire.gameObject.SetActive(true);
        fire.Play();
        
        light.intensity = 0;
        light.gameObject.SetActive(true);
        _lightTween?.Kill();
        _lightTween = DOVirtual.Float(0, _currentIntensity, _enableDuration, SetIntensity);
        
        if (_disableCoroutine != null) 
            StopCoroutine(_disableCoroutine);
        _disableCoroutine = StartCoroutine(DisableCoroutine());
    }
    private IEnumerator DisableCoroutine()
    {
        yield return new WaitForSeconds(timeActive);
        
        _canEnable = true;
        _lightTween?.Kill();
        _lightTween = DOVirtual.Float(_currentIntensity, 0, _disableDuration, SetIntensity).OnComplete(() =>
        {
            light.gameObject.SetActive(false);
        });
        fire.gameObject.SetActive(false);
        fire.Stop();
        OnFireDisableEvent?.Invoke();
    }
    private void SetIntensity(float _value) => light.intensity = _value;
}
