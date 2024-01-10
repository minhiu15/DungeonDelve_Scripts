using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TextBar_3 : MonoBehaviour, IPooled<TextBar_3>
{
    public Animator animator;
    [SerializeField] private Image iconTextBar;
    [SerializeField] private TextMeshProUGUI valueText;
    [Space]
    public UnityEvent OnActiveTextBarEvent;
    
    
    /// <summary>
    /// Cập nhật thông tin cho textBar
    /// </summary>
    /// <param name="_textValue"> Giá trị cập nhật vào text </param>
    /// <param name="_spriteIcon"> Icon hiển thị của textBar </param>
    public void SetTextBar(string _textValue, Sprite _spriteIcon)
    {
        valueText.text = _textValue;
        iconTextBar.sprite = _spriteIcon;
        iconTextBar.SetNativeSize();

        OnActiveTextBarEvent?.Invoke();
    }
    
    public void Release() => ReleaseCallback?.Invoke(this);
    public Action<TextBar_3> ReleaseCallback { get; set; }
}
