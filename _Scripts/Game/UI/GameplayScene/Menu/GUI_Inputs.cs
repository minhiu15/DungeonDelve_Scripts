using UnityEngine;

public class GUI_Inputs : MonoBehaviour
{
    public static Inputs InputAction;


    private void Awake() => InputAction = new Inputs();
    private void OnEnable() => EnableInput();
    private void OnDisable() => DisableInput();

    
    public static void EnableInput() => InputAction.Enable();
    public static void DisableInput() => InputAction.Disable();

}
