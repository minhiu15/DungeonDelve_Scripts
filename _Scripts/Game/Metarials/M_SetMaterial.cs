using UnityEngine;

public class M_SetMaterial : SetMaterials
{
    [SerializeField] private Material oldMaterial;
    [SerializeField] private Material newMaterial;

    private Material _oldMaterial;
    private Material _newMaterial;
    private Material _changeMaterial;

    protected override void Init()
    {
        base.Init();

        _oldMaterial = new Material(oldMaterial);
        _newMaterial = new Material(newMaterial);
    }
    public void SetOldMaterial()
    {
        _changeMaterial = _oldMaterial;
        Apply();
    }
    public void SetNewMaterial()
    {
        _changeMaterial = _newMaterial;
        Apply();
    }
    public override void Apply()
    {
        foreach (var _render in Renderers)
        {
            _render.sharedMaterial = _changeMaterial;
        }
    }
    
}
