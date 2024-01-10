using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI valueSliderText;
    private enum VolumeType
    {
        MASTER,
        MUSIC,
        SFX,
        DIALOGUE
    }
    [SerializeField] private VolumeType type;
    [Space] 
    [SerializeField] private GameObject volumeMinIcon;
    [SerializeField] private GameObject volumeMaxIcon;
    [SerializeField] private GameObject volumeDisableIcon;
    private AudioManager _manager;
    
    
    private void Start()
    {
        _manager = AudioManager.Instance;
        slider.onValueChanged.AddListener(OnValueChanged);
        InitValue();
    }
    private void OnDestroy()
    {
        slider.onValueChanged.RemoveListener(OnValueChanged);    
    }

    public void InitValue()
    {
        slider.minValue = 0;
        slider.maxValue = 10;
        var _value =type switch
        {
            VolumeType.MASTER => AudioManager.masterVolume,
            VolumeType.MUSIC => AudioManager.musicVolume,
            VolumeType.SFX => AudioManager.sfxVolume,
            VolumeType.DIALOGUE => AudioManager.dialogueVolume,
            _ => slider.value
        };
        slider.value = (int)(_value * 10);
        
        SetIcon();
        SetValueText();
    }
    private void OnValueChanged(float _value)
    {
        var _volumeValue = _value / slider.maxValue;
        switch (type)
        {
            case VolumeType.MASTER:
                AudioManager.masterVolume = _volumeValue;
                _manager.masterBus.setVolume(_volumeValue);
                break;
            case VolumeType.MUSIC:
                AudioManager.musicVolume = _volumeValue;
                _manager.musicBus.setVolume(_volumeValue);
                break;
            case VolumeType.SFX:
                AudioManager.sfxVolume = _volumeValue;
                _manager.sfxBus.setVolume(_volumeValue);
                break;            
            case VolumeType.DIALOGUE:
                AudioManager.dialogueVolume = _volumeValue;
                _manager.dialogueBus.setVolume(_volumeValue);
                break;
        }

        SetIcon();
        SetValueText();
    }
    private void SetIcon()
    {
        volumeDisableIcon.SetActive(Math.Abs(slider.value - slider.minValue) == 0);
        volumeMinIcon.SetActive(!volumeDisableIcon.activeSelf && slider.value > slider.minValue);
        volumeMaxIcon.SetActive(!volumeDisableIcon.activeSelf && slider.value >= slider.maxValue / 2);
    }
    private void SetValueText() =>  valueSliderText.text = $"{slider.value}";
}
