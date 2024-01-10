using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;

public class PlayerDashEffect : MonoBehaviour
{
    private PlayerController player;

    [Space, Tooltip("Chỉnh Visual của camera khi dash")]
    public Volume visualVolume;
    [Tooltip("Các hạt partical khi lướt")]
    public ParticleSystem dashPartical;
    
    private Tween _volumeTween;
    
    private void Start()
    {
        player = GetComponentInParent<PlayerController>();
        if(player)
            player.OnDashEvent += OnEnableDashEventVisual;
        dashPartical.gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        if(player)
            player.OnDashEvent -= OnEnableDashEventVisual;
    }


    public void OnEnableDashEventVisual()
    {
        dashPartical.gameObject.SetActive(true);
        dashPartical.Play();
        
        _volumeTween?.Kill();
        _volumeTween = DOVirtual.Float(0, 1, .15f, SetValueVolume).OnComplete(() =>
        {
            DOVirtual.Float(1, 0, .7f, SetValueVolume);
        });
    }

    private void SetValueVolume(float value)
    {
        visualVolume.weight = value;
    }
    
}
