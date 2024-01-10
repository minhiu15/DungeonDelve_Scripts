using System;
using NaughtyAttributes;
using UnityEngine;

public class PhysicsDetection : DetectionBase, IPooled<PhysicsDetection>
{
    [Tooltip("Bán kính kiểm tra va chạm"), Range(0.1f, 20f)]
    public float radiusCheck;
    
    [Tooltip("Layer cần kiểm tra va chạm")]
    public LayerMask layerToCheck;

    [SerializeField] private bool drawGizmos;
    [SerializeField, ShowIf("drawGizmos")] private Color drawColor;
    
    private readonly Collider[] hitColliders = new Collider[10];

    public void CheckCollision()
    {
        var numCol = Physics.OverlapSphereNonAlloc(transform.position, radiusCheck, hitColliders, layerToCheck);
        for (var i = 0; i < numCol; i++)
        {
            CollisionEnterEvent?.Invoke(hitColliders[i].gameObject);
            PositionEnterEvent?.Invoke(hitColliders[i].ClosestPointOnBounds(transform.position));
        }
    }
    
    public void Release() => ReleaseCallback?.Invoke(this);
    public Action<PhysicsDetection> ReleaseCallback { get; set; }
    
    public void OnDrawGizmos()
    {
        if(!drawGizmos) return;
        Gizmos.color = drawColor;
        Gizmos.DrawWireSphere(transform.position, radiusCheck);
    }
}
