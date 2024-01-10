using UnityEngine;

public class CursorHandle : MonoBehaviour
{
    private void Update()
    {
        Cursor.lockState = _isLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !_isLocked;
    }
    
    
    private static bool _isLocked;
    public static void Locked() => _isLocked = true;
    public static void NoneLocked() => _isLocked = false;

}
