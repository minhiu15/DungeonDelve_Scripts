using UnityEngine;

public class DrawDirection : MonoBehaviour
{
    [Tooltip("Hướng muốn vẽ")]
    public DirectionEnums directionEnum;

    [Tooltip("Đảo ngược")]
    public bool reverse;
        
    [Tooltip("The color")]
    public Color color = Color.white;
        
    [Tooltip("Duration the line will be visible for in seconds.\nDefault: 0 means 1 frame.")]
    public float lengthDraw;


    public enum DirectionEnums
    {
        X,
        Y,
        Z
    }

    private void OnDrawGizmos()
    {
        var currentPosition = transform.position;
        var direction = directionEnum switch
        {
            DirectionEnums.X => transform.right,
            DirectionEnums.Y => transform.up,
            DirectionEnums.Z => transform.forward,
            _ => Vector3.zero
        };

        direction *= reverse ? 1 : -1;
        var endPosition = currentPosition + direction * lengthDraw;
        
        Gizmos.color = color;
        Gizmos.DrawLine(currentPosition, endPosition);
    }

        
        
       
        
}