using UnityEngine;

public class CollisionDetection : DetectionBase
{
    [Space, Tooltip("Layer cần kiểm tra va chạm")]
    public LayerMask layerToCheck;
    
    private void OnCollisionEnter(Collision other)
    {
        if(!layerToCheck.Contains(other.gameObject)) 
            return;
        
        CollisionEnterEvent?.Invoke(other.gameObject);
        PositionEnterEvent?.Invoke(other.GetContact(0).point);
    }

    private void OnCollisionExit(Collision other)
    {
        if(!layerToCheck.Contains(other.gameObject)) 
            return;
        
        CollisionExitEvent?.Invoke(other.gameObject);
        PositionExitEvent?.Invoke(other.GetContact(0).point);
    }
}
