using FMODUnity;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    public UnityEvent<Slot, UI_Item> OnSelectSlotEvent;

    [field: SerializeField] public CooldownTime cooldownTime { get; private set; }
    [SerializeField] private TextMeshProUGUI keyText;
    [SerializeField] private EventReference setSlotAudio;
    [SerializeField] private Image iconItem;
    [field: SerializeField, ReadOnly] public string KeyPlayerPrefs { get; private set; }

    private int _clickCount;
    private float _lastClickTimer;
    private readonly float _doubleClickDelay = .3f;
    
    public UI_Item Item { get; private set; }

    
    public void OnDrop(PointerEventData eventData)
    {
        if(!DraggableData.Get(eventData.pointerDrag, out var draggableItem)) 
            return;

        var _item = draggableItem.GetItem();
        if(_item.GetItemCustom.type != ItemType.Consumable || _item == Item) 
            return;
        
        AudioManager.PlayOneShot(setSlotAudio, transform.position);
        OnSelectSlotEvent?.Invoke(this, _item);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        _clickCount++;
        switch (_clickCount)
        {
            case 1:
                _lastClickTimer = Time.unscaledTime;
                break;
            case 2 when Time.unscaledTime - _lastClickTimer < _doubleClickDelay:
                OnSelectSlotEvent?.Invoke(this, null);
                _clickCount = 0;
                break;
            default:
                _clickCount = 1;
                _lastClickTimer = Time.unscaledTime;
                break;
        }
    }

    
    /// <summary>
    /// Set item vào slot này
    /// </summary>
    /// <param name="_newItem"> Item cần set </param>
    public void SetSlot(UI_Item _newItem)
    {
        Item = _newItem;
        if (Item != null)
        {
            iconItem.sprite = Item.GetItemCustom.sprite;
            iconItem.enabled = true;
            return;
        }
        iconItem.enabled = false;
    }

    
    /// <summary>
    /// Set phím tắt Key tương ứng đầu vào Input tương ứng phím tắt Input
    /// </summary>
    /// <param name="_value"> Giá trị Key </param>
    public void SetKeyText(string _value) => keyText.text = _value;

    
    /// <summary>
    /// Set Key mà slot này đang lưu dữ liệu dưới PlayerPrefs
    /// </summary>
    /// <param name="_key"></param>
    public void SetKeyPlayerPrefs(string _key) => KeyPlayerPrefs = _key;

}
