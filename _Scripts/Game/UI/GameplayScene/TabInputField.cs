using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TabInputField : MonoBehaviour
{
    [SerializeField] private Selectable firstInput;
    private EventSystem _eventSystem;

    private void OnEnable()
    {
        GUI_Inputs.InputAction.TESTER.Tab.performed += OnTabInput; 
    }
    private void Start()
    {
        _eventSystem = EventSystem.current;
    }
    private void OnDisable()
    {
        GUI_Inputs.InputAction.TESTER.Tab.performed -= OnTabInput;
    }
    
    private void OnTabInput(InputAction.CallbackContext _context)
    {
        var i = _eventSystem.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
        if (i == null) return;
        var _field = i.GetComponent<TMP_InputField>();
        if (_field != null)
        {
            _field.Select();
        }
        else
        {
            firstInput.Select();
        }
        _eventSystem.SetSelectedGameObject(i.gameObject, new BaseEventData(_eventSystem));
    }
    
}
