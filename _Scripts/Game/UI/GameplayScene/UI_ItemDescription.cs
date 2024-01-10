using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ItemDescription : MonoBehaviour
{
    [SerializeField] private UI_Item item;
    [SerializeField] private RectTransform descriptionPanel;
    [Space, Header("UI")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI desText;

    [Tooltip("Bù thêm giá trị trục X để xem Panel Description của Item có nằm ngoài phạm vi hiển thị của màn hình không ?")]
    [SerializeField] private float widthOffset = 250f;
    [Tooltip("Bù thêm giá trị trục Y để xem Panel Description của Item có nằm ngoài phạm vi hiển thị của màn hình không ?")]
    [SerializeField] private float heightOffset = 500f;
    
    private RectTransform _itemRectTransform;
    private Vector3 _currentPosition;
    private Canvas _canvas;

    private float _posWidth;
    private float _posHieght;
    
    private void Start()
    {
        _canvas = GetComponentInParent<Canvas>();
        _itemRectTransform = item.GetComponent<RectTransform>();
        descriptionPanel.gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        CloseDescriptionPanel();
    }
    private void OnDisable()
    {
        descriptionPanel.gameObject.SetActive(false);
    }

    public void OpenDescriptionPanel()
    {
        itemIcon.sprite = item.GetItemCustom.sprite;
        nameText.text = item.GetItemCustom.nameItem;
        typeText.text = item.GetItemCustom.type switch
        {
            ItemType.Consumable => "Consumable Item",
            ItemType.Upgrade => "Upgrade/Enhancement Item",
            ItemType.Quest => "Quest Item",
            ItemType.Currency => "Currency",
            _ => "???"
        };
        desText.text = item.GetItemCustom.description;

        CheckPositionActive();
    }
    private void CheckPositionActive()
    {
        var itemCorners = new Vector3[4];
        _itemRectTransform.GetWorldCorners(itemCorners);
        var rightEdge = itemCorners[2].x;
        var bottomEdge = itemCorners[0].y;
        
        _posWidth = rightEdge + widthOffset < Screen.width ? 250f : -250f;     // nếu nhỏ hơn -> chưa ra khỏi màn hình, ngược lại thì Flip trục X
        _posHieght = bottomEdge - heightOffset > 0 ? -137.5f : 137.5f;         // nếu nhỏ hơn 0 -> ra khỏi màn hình -> Flip trục Y

        _currentPosition = new Vector3(_posWidth, _posHieght, 0);
        descriptionPanel.localPosition = _currentPosition;
        descriptionPanel.transform.SetParent(_canvas.transform);
        descriptionPanel.gameObject.SetActive(true);
    }
    public void CloseDescriptionPanel()
    {
        descriptionPanel.gameObject.SetActive(false);
        descriptionPanel.transform.SetParent(item.transform);
    }
    
}
