using System.Linq;
using UnityEngine;

public class M_SetTexture : SetMaterials
{
    private Texture _texture;
    public void SetTexture(Sprite _sprite) => _texture = _sprite.texture;

    public override void Apply() => Set();
    private void Set()
    {
        if(_texture == null) return;
        
        var _materials = Renderers.SelectMany(_mate => _mate.materials);
        foreach (var _material in _materials)
        {
            Debug.Log(_material);
            _material.SetTexture(nameID, _texture);
        }
    }
    
}
