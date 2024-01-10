using UnityEngine;
using UnityEngine.UI;

public class GuidancePanel : MonoBehaviour
{
    [SerializeField] private Animator panelAnimator;
    [SerializeField] private Button closeBtt;


    private void Start()
    {
        closeBtt.onClick.AddListener(OnClickClosePanelButton);   
    }
    private void OnDestroy()
    {
        closeBtt.onClick.RemoveListener(OnClickClosePanelButton);   
    }
    
    public void OpenGuidancePanel()
    {
        panelAnimator.Play("Panel_IN");
    }
    public void OnClickClosePanelButton()
    {
        panelAnimator.Play("Panel_OUT");
        MenuController.Instance.HandleMenuClose();
    }

}
