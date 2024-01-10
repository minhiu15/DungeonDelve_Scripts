using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Khi Player Trigger tại 1 điểm có tương tác trong game.
/// Ví dụ: Đứng gần shop thì sẽ Notice: "[F] Open Shop." và trả về Event tương ứng khi người dùng nhấn Input.
/// Nếu nhấn F: Gọi Event PanelOpen
/// Nếu nhấn ESC: Gọi Event PanelClose
/// </summary>
public class InteractiveUI : MonoBehaviour
{
    [SerializeField, Tooltip("Text thông báo khi Player đang Trigger với vị trí có tương tác")] 
    private string noticePlayerTrigger;
    
    public event Action OnPanelOpenEvent;
    public event Action OnPanelCloseEvent;

    private void OpenPanel(InputAction.CallbackContext _context) => OnPanelOpenEvent?.Invoke();
    public void ClosePanel(InputAction.CallbackContext _context) => OnPanelCloseEvent?.Invoke();
    public void OnEnterPlayer()
    {
        GUI_Inputs.InputAction.UI.CollectItem.performed += OpenPanel;
        GUI_Inputs.InputAction.UI.OpenMenu.performed += ClosePanel;
        NoticeManager.Instance.CreateNoticeT3(noticePlayerTrigger);
    }
    public void OnExitPlayer()
    {
        GUI_Inputs.InputAction.UI.CollectItem.performed -= OpenPanel;
        GUI_Inputs.InputAction.UI.OpenMenu.performed -= ClosePanel;
        NoticeManager.Instance.CloseNoticeT3();
    }

    public void SetNoticeText(string _value)
    {
        noticePlayerTrigger = _value;
        NoticeManager.Instance.CreateNoticeT3(noticePlayerTrigger);
    }
}
