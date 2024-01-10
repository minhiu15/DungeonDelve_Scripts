using UnityEngine;

public class FixedUpdateFollow : MonoBehaviour
{
    [Tooltip("Mục tiêu cần theo"), SerializeField] 
    public Transform toFollow;
    
    [Tooltip("Offset thêm ?"), SerializeField] 
    private Vector3 offset = Vector3.one;
    
    [Tooltip("Có muốn xoay theo mục tiêu ?"), SerializeField] 
    private bool canRotation;
     
    private Vector3 currentPosition;
    

    private void FixedUpdate()
    {
        if(canRotation)
            transform.rotation = toFollow.rotation;
        
        currentPosition = toFollow.position;
        currentPosition.y += offset.y;
        transform.position = currentPosition + Quaternion.Euler(0, transform.eulerAngles.y, 0) 
            * Vector3.forward * offset.z;
    }
    
    
}
