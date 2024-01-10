using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class M_SetFloat : SetMaterials
{
    [Tooltip("Giá trị hiện tại"), SerializeField]
    private float currentValue;
    [Tooltip("Giá trị cần set"), SerializeField]
    private float valueSetTo;
    
    /// <summary>
    /// Thay đổi giá trị hiện tại
    /// </summary>
    /// <param name="_value"> Giá trị hiện tại </param>
    public void ChangeCurrentValue(float _value) => currentValue = _value;
    
    /// <summary>
    /// Thay đổi giá trị áp dụng tới
    /// </summary>
    /// <param name="_value"> Giá trị mới để áp dụng </param>
    public void ChangeValueSet(float _value) => valueSetTo = _value;
    

    public override void Apply()
    {
        _applyTween?.Kill();
        _applyTween = DOVirtual.Float(currentValue, valueSetTo, durationApply, Set);
    }    
    private void Set(float _value)
    {
        var _materials = Renderers.SelectMany(_mate => _mate.materials);
        foreach (var material in _materials)
        {
            material.SetFloat(nameID, _value);
        }
    }
    
}
