using UnityEngine;

public class TriggerDetection : DetectionBase
{
    [Space, Tooltip("Layer cần kiểm tra va chạm")]
    public LayerMask layerToCheck;
    
    private void OnTriggerEnter(Collider other)
    {
        if(!layerToCheck.Contains(other.gameObject)) return;
        
        CollisionEnterEvent?.Invoke(other.gameObject);
        PositionEnterEvent?.Invoke(other.ClosestPointOnBounds(transform.position));
    }

    private void OnTriggerExit(Collider other)
    {
        if(!layerToCheck.Contains(other.gameObject)) return;
        
        CollisionExitEvent?.Invoke(other.gameObject);
        PositionExitEvent?.Invoke(other.ClosestPointOnBounds(transform.position));
    }
}
