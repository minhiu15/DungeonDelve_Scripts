using System.Collections.Generic;
using UnityEngine;

public class GUI_SettingGraphics : MonoBehaviour
{
    [SerializeField] private DropdownBar displayModeDropdown;
    [SerializeField] private DropdownBar fpsDropdown;
    
    
    private readonly List<string> _fps = new() { "24", "30", "45", "60", "120", "144", "Unlimit" };
    private readonly List<Resolution> _resolutions = new() {
        new Resolution { width = 3840, height = 2160 },  // 4K
        new Resolution { width = 2560, height = 1440 },  // 2K
        new Resolution { width = 1920, height = 1080 },  // FHD
        new Resolution { width = 1600, height =  900 },  
        new Resolution { width = 1280, height =  720 },  
        new Resolution { width = 1024, height =  576 },  
        new Resolution { width =  854, height =  480 },  
        new Resolution { width =  640, height =  360 },  
        new Resolution { width =  426, height =  240 },  
    };
    
    // Key PlayerPrefs
    private readonly string PP_CurrentResolutionIndex = "ResolutionIndex";
    private readonly string PP_CurrentFPSIndex = "FPSIndex";
    
    
    private void Start()
    {
        Initialized();
    }

    
    private void Initialized()
    {
         List<string> _options = new();
         foreach (var _resolution in _resolutions)
         {
             var typeMode = CheckFullscreenResolution(_resolution) ? "Fullscreen" : "Windowed";
             _options.Add($"{_resolution.width} x {_resolution.height} {typeMode}");
         }
         
         var resolutionIdx = PlayerPrefs.GetInt(PP_CurrentResolutionIndex, 2);  // tìm độ phân giải trước đó đã lưu (nếu có)
         var fpsIdx = PlayerPrefs.GetInt(PP_CurrentFPSIndex, 3);

         if (displayModeDropdown) displayModeDropdown.InitValue(_options, resolutionIdx);
         if (fpsDropdown) fpsDropdown.InitValue(_fps, fpsIdx);
         
         OnValueDisplayModeChanged(resolutionIdx);
         OnValueFPSChanged(fpsIdx);
    }
    private static bool CheckFullscreenResolution(Resolution _resolution) => _resolution is { width: >= 1920, height: >= 1080 };
    
    
    public void OnValueDisplayModeChanged(int _index)
    {
        PlayerPrefs.SetInt(PP_CurrentResolutionIndex, _index);
        var _res = _resolutions[_index];
        Screen.SetResolution(_res.width, _res.height, CheckFullscreenResolution(_res));
    }
    public void OnValueFPSChanged(int _index)
    {
        PlayerPrefs.SetInt(PP_CurrentFPSIndex, _index);
        var fpsIdx = _index >= 6 ? -1 : int.Parse(_fps[_index]);
        Application.targetFrameRate = fpsIdx;
    }
    
}
