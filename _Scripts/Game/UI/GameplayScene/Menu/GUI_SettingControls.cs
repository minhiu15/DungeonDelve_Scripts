using Cinemachine;
using UnityEngine;


public class GUI_SettingControls : MonoBehaviour, IGUI
{
    [SerializeField] private SliderBar cameraSensitivity;
    
    private CinemachineFreeLook _cinemachineFreeLook;
    
    // Key PlayerPrefs
    private readonly string PP_SensitivityIndex = "SensitivityIndex";
    
    private void OnEnable()
    {
        GUI_Manager.Add(this);
        cameraSensitivity.InitValue(100, 500, PlayerPrefs.GetInt(PP_SensitivityIndex, 222));
    }
    private void OnDisable() => GUI_Manager.Remove(this);
    
    
    public void GetRef(GameManager _gameManager)
    {
        _cinemachineFreeLook = _gameManager.Player.FreeLookCamera;
        
        var sensitivityValue = PlayerPrefs.GetFloat(PP_SensitivityIndex, 150);
        SetCameraSensitivity(sensitivityValue);
    }
    public void UpdateData() { }
    
    
    public void SetCameraSensitivity(float _value)
    {
        if (!_cinemachineFreeLook) return;
        
        PlayerPrefs.SetInt(PP_SensitivityIndex, (int)_value);
        _cinemachineFreeLook.m_XAxis.m_MaxSpeed = _value;
    }
    
}
