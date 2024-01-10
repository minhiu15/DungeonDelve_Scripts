using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GUI_Login : MonoBehaviour
{
    [field: SerializeField] public TextMeshProUGUI errorText {get; private set; }
    [field: SerializeField] public TMP_InputField emailField {get; private set; }
    [field: SerializeField] public TMP_InputField passwordField {get; private set; }
    [field: SerializeField] public Button startgameBtt {get; private set; }
    [field: SerializeField] public Button quitBtt {get; private set; }
    
    
    private void OnEnable()
    {
        SetDefaultErrorText();
        SetDefaultFieldText();
        GUI_Inputs.InputAction.TESTER.Enter.performed += OnEnterInput;
        GUI_Inputs.InputAction.UI.OpenMenu.performed += OnEscInput;
    }
    private void Start()
    {
        passwordField.asteriskChar = '•';
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
        startgameBtt.onClick.Invoke();
    }
    private void OnEscInput(InputAction.CallbackContext _context)
    {
        quitBtt.onClick.Invoke();
    }

    private void SetDefaultErrorText() => errorText.text = "";
    private void SetDefaultFieldText()
    {
        emailField.text = "";
        passwordField.text = "";
        emailField.Select();
    }
}
