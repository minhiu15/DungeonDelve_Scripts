using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PointerDetection : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public UnityEvent OnEnterEvent;
    public UnityEvent OnClickEvent;
    public UnityEvent OnExitEvent;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnEnterEvent?.Invoke();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        OnExitEvent?.Invoke();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        OnClickEvent?.Invoke();
    }
}
