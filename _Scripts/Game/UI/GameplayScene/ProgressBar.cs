using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [Tooltip("Slider tiến trình chính"), Required] 
    public Slider mainProgressSlider;
    [Tooltip("Slider tiến trình phụ")] 
    public Slider backProgressSlider;

    [Space, Tooltip("Text hiển thị tiến trình"), ShowIf("ShowText")] 
    public TextMeshProUGUI progressText;
    [Tooltip("Có cập nhật giá trị vào Text không")]
    public bool ShowText;
    
    [Space, Tooltip("Thời gian chạy 1 Tween của Fill chính"), SerializeField]
    private float mainDuration = .25f;
    [Tooltip("Thời gian chạy 1 Tween của Fill phụ"), SerializeField]
    private float backDuration = .8f;
    
    private Tween mainProgressTween, backProgressTween;


    private void OnEnable()
    {
        if (!ShowText || !progressText) return;
        mainProgressSlider.onValueChanged.AddListener(SliderChangeValue);
    }
    private void OnDisable()
    {
        if (!ShowText || !progressText) return;
        mainProgressSlider.onValueChanged.RemoveListener(SliderChangeValue);
    }


    /// <summary>
    /// Khởi tạo giá trị ban đầu
    /// </summary>
    /// <param name="_maxValue"> Giá trị cần khởi tạo </param>
    public void Init(int _currentValue, int _maxValue)
    {
        mainProgressSlider.minValue = 0;
        mainProgressSlider.maxValue = _maxValue;
        mainProgressSlider.value = _currentValue;

        backProgressSlider.minValue = 0;
        backProgressSlider.maxValue = _maxValue;
        backProgressSlider.value = _currentValue;
    }
    public void OnCurrentValueChange(int _currentValue)
    {
        _currentValue = (int)Mathf.Clamp(_currentValue, mainProgressSlider.minValue, mainProgressSlider.maxValue);
        
        mainProgressTween?.Kill();
        mainProgressTween = mainProgressSlider.DOValue(_currentValue, mainDuration);

        backProgressTween?.Kill();
        backProgressTween = backProgressSlider.DOValue(_currentValue, backDuration);
    }
    public void OnMaxValueChange(int _maxValue)
    {
        mainProgressSlider.maxValue = _maxValue;
        backProgressSlider.maxValue = _maxValue;
        
        if (!ShowText || !progressText) return;
        SliderChangeValue(mainProgressSlider.value);
    }
    
    private void SliderChangeValue(float _value) => progressText.text = $"{_value} / {mainProgressSlider.maxValue}";
    
}
