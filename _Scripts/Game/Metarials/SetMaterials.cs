using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Renderers
{
    public Material[] materials;
    public Renderers(int sizeArray)
    {
        materials = new Material[sizeArray];
    }
}

public abstract class SetMaterials : MonoBehaviour
{
    [Tooltip("Danh sách Renderer"), SerializeField]
    protected List<Renderer> Renderers;

    [Space, Tooltip("Tên property cần set trong Shader"), SerializeField]
    private string propertyToID = "";
    
    [Tooltip("Thời gian set từ giá trị hiện tại đến giá trị cần set"), SerializeField]
    protected float durationApply;
    
    private List<Renderers> _copiesRenderer; 
    protected int nameID;
    protected Tween _applyTween;
    
    
    private void Start()
    {
        nameID = Shader.PropertyToID(propertyToID);
        Init();
    }
    
    protected virtual void Init()
    {
        _copiesRenderer = new List<Renderers>(Renderers.Count); // tạo 1 danh sách RendererCopy 
        
        for (var i = 0; i < Renderers.Count; i++)
        {
            _copiesRenderer.Add(new Renderers(Renderers[i].materials.Length));

            // trong mỗi RendererCopy tạo mới 1 mảng Material với số phần tử = phần tử của materials trong renderer
            var newMaterialArray = new Material[Renderers[i].materials.Length];
            for (var j = 0; j < newMaterialArray.Length; j++)
            {
                // với mỗi mảng mới vừa tạo, tạo mới các MaterialCopy tương ứng trong renderer
                newMaterialArray[j] = new Material(Renderers[i].materials[j]);
            }
            _copiesRenderer[i].materials = newMaterialArray;

            Renderers[i].materials = _copiesRenderer[i].materials; // gán bản Material Copy vào renderer
        }
    }
    
    
    /// <summary>
    /// Thay đổi thời gian áp dụng
    /// </summary>
    /// <param name="_value"> Thời gian set Value mới vào Material </param>
    public void ChangeDurationApply(float _value) => durationApply = _value;
    public abstract void Apply();
}
