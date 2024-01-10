using System.Linq;
using DG.Tweening;
using UnityEngine;

public class M_SetColor : SetMaterials
{
    [Tooltip("Màu cần set"), SerializeField]
    private Color colorSetTo;

    private Color currentColor;


    /// <summary>
    /// Thay đổi màu hiện tại 
    /// </summary>
    /// <param name="_value"> Giá trị của màu hiện tại </param>
    public void ChangeCurrentColor(Color _value) => currentColor = _value;
    
    /// <summary>
    /// Thay đổi màu của cần set tới
    /// </summary>
    /// <param name="_value"> Giá trị của màu mới cần set </param>
    public void ChangeColorSet(Color _value) => colorSetTo = _value;
    
    
    public override void Apply()
    {
        _applyTween?.Kill();
        _applyTween = DOVirtual.Color(currentColor, colorSetTo, durationApply, Set);
    }
    private void Set(Color _value)
    {
        var _materials = Renderers.SelectMany(_mate => _mate.materials);
        foreach (var _material in _materials)
        {
            _material.SetColor(nameID, _value);  
        }
    }
    
}
