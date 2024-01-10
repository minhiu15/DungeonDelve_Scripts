using System.Linq;
using DG.Tweening;
using UnityEngine;

public class M_SetEmission : SetMaterials
{
    [Tooltip("Màu cần set"), SerializeField]
    private Color colorSetTo;
    
    [Tooltip("Cường độ hiện tại của màu"), SerializeField]
    private float currentIntensity;
    
    [Tooltip("Cường độ cần set của màu"), SerializeField]
    private float intensitySetTo;
    

    /// <summary>
    /// Thay đổi màu
    /// </summary>
    /// <param name="_value"> Màu mới cần áp dụng </param>
    public void ChangeColorSet(Color _value) => colorSetTo = _value;
    
    /// <summary>
    /// Thay đổi giá trị hiện tại
    /// </summary>
    /// <param name="_value"> Giá trị hiện tại </param>
    public void ChangeCurrentIntensity(float _value) => currentIntensity = _value;
    
    /// <summary>
    /// Thay đổi giá trị áp dụng tới
    /// </summary>
    /// <param name="_value"> Giá trị mới để áp dụng </param>
    public void ChangeIntensitySet(float _value) => intensitySetTo = _value;
    

    public override void Apply()
    {
        _applyTween?.Kill();
        _applyTween = DOVirtual.Float(currentIntensity, intensitySetTo, durationApply, Set);
    }
    private void Set(float _value)
    {
        var _materials = Renderers.SelectMany(_mate => _mate.materials);
        foreach (var _material in _materials)
        {
            _material.SetColor(nameID, colorSetTo *_value); 
        }
    }
    
}
