using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GUI_ForgotPW : MonoBehaviour
{
    [field: SerializeField] public TextMeshProUGUI errorText { get; private  set; }
    [field: SerializeField] public TMP_InputField emailField { get; private  set; }
    [field: SerializeField] public Button verificationBtt { get; private  set; }
    [field: SerializeField] public Button quitBtt {get; private set; }
    
    private void OnEnable()
    {
        SetDefaultErrorText();
        SetDefaultFieldText();
        GUI_Inputs.InputAction.TESTER.Enter.performed += OnEnterInput;
        GUI_Inputs.InputAction.UI.OpenMenu.performed += OnEscInput;
    }
    private void OnDisable()
    {
        GUI_Inputs.InputAction.TESTER.Enter.performed -= OnEnterInput;
        GUI_Inputs.InputAction.UI.OpenMenu.performed -= OnEscInput;
    }
    public void SetErrorText(string _errorText)
    {
        errorText.text = _errorText;
        Invoke(nameof(SetDefaultErrorText), 2.5f);
    }
    
    
    private void OnEnterInput(InputAction.CallbackContext _context)
    {
        verificationBtt.onClick.Invoke();
    }
    private void OnEscInput(InputAction.CallbackContext _context)
    {
        quitBtt.onClick.Invoke();
    }
    private void SetDefaultErrorText()
    {
        errorText.text = "";
    }
    private void SetDefaultFieldText()
    {
        emailField.text = "";
        emailField.Select();
    }
}
