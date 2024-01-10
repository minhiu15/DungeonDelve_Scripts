using System;
using TMPro;
using UnityEngine;

public class TextBar : MonoBehaviour, IPooled<TextBar>
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI valueText;

    
    /// <summary>
    /// Cập nhật giá trị của component Title Text
    /// </summary>
    /// <param name="_value"> Giá trị cập nhật vào text </param>
    public void SetTitleText(string _value) => titleText.text = _value;
        
    
    /// <summary>
    /// Cập nhật giá trị của component Value Text
    /// </summary>
    /// <param name="_value"> Giá trị cập nhật vào text </param>
    public void SetValueText(string _value) => valueText.text = _value;


    public void Release() => ReleaseCallback?.Invoke(this);
    public Action<TextBar> ReleaseCallback { get; set; }
}
