using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ApplicationQuitButton : MonoBehaviour
{
    private Button _button;
    
    private void Awake()
    {
        _button = GetComponent<Button>();
    }
    private void OnEnable()
    {
        _button.onClick.AddListener(Quit);
    }
    private void OnDisable()
    {
        _button.onClick.RemoveListener(Quit);
    }

    private static void Quit() => Application.Quit();
    
}
