using FMODUnity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private UI_Item item;
    [Header("UI")]
    [SerializeField] private Image iconDrag;

    [Header("Audio")] 
    [SerializeField] private EventReference selectAudio;
    
    private readonly Color _enableColor = new(1, 1, 1, 1);
    private readonly Color _disableColor = new(1, 1, 1, 0);
    
    public UI_Item GetItem() => item;

    private void OnEnable() => DraggableData.Add(gameObject, this);
    private void OnDisable() => DraggableData.Remove(gameObject);
    

    public void OnBeginDrag(PointerEventData eventData)
    {
        AudioManager.PlayOneShot(selectAudio, transform.position);
        
        iconDrag.sprite = item.GetItemCustom.sprite;
        iconDrag.color = _enableColor;
        iconDrag.raycastTarget = false;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
    }
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        iconDrag.raycastTarget = true;
        iconDrag.color = _disableColor;
        
        transform.SetParent(item.transform);
        transform.localPosition = Vector3.zero;
    }
}
