using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderBar : MonoBehaviour
{
    [SerializeField] private bool useText;
    [ShowIf("useText"), SerializeField] private TextMeshProUGUI valueSliderText;
    public Slider Slider;


    /// <summary>
    /// Khởi tạo giá trị của Slider ban đầu
    /// </summary>
    /// <param name="_minValue"> Giá trị tối thiểu của slider </param>
    /// <param name="_maxValue"> Giá trị tối đa của slider </param>
    /// <param name="_value"> Giá trị hiện tại của slider </param>
    public void InitValue(float _minValue, float _maxValue, float _value)
    {
        Slider.maxValue = _maxValue;
        Slider.minValue = _minValue;
        Slider.value = _value;
    }
    
    public void OnSetValueText(float _value) => valueSliderText.text = $"{_value}";
}
