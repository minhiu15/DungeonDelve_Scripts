using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DropdownBar : MonoBehaviour
{
    public TMP_Dropdown Dropdown;
    
    
    /// <summary>
    /// Thiết lập các option ban đầu của Dropdown
    /// </summary>
    /// <param name="_options"> Danh sách Option được định nghĩa trước </param>
    /// <param name="_currentValue"> Giá trị ban đầu của Dropdown </param>
    public void InitValue(List<string> _options, int _currentValue)
    {
        Dropdown.ClearOptions();
        Dropdown.AddOptions(_options);
        Dropdown.value = _currentValue;
        Dropdown.RefreshShownValue();
    }
    
    /// <summary>
    /// Thiết lập các option ban đầu của Dropdown
    /// </summary>
    /// <param name="_options"> Danh sách Option được định nghĩa trước </param>
    /// <param name="_currentValue"> Giá trị ban đầu của Dropdown </param>
    /// <param name="_converter"> Phương thức Delegate để chuyển giá trị T(Input) sang string(Output) </param>
    public void InitValue <T> (List<T> _options, int _currentValue, Converter<T, string> _converter)
    {
        var optionData = _options.ConvertAll(_converter);
        InitValue(optionData, _currentValue);
    }
    
}
