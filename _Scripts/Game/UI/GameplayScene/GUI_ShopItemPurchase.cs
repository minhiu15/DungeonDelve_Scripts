using System;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUI_ShopItemPurchase : MonoBehaviour, IGUI
{
    [SerializeField] private GUI_Shop guiShop;
    [SerializeField] private Animator panelAnimator;
    [Space]
    [SerializeField] private Slider quantitySlider;
    [SerializeField] private TextMeshProUGUI quantityUseText;
    [SerializeField] private TextMeshProUGUI minQuantityValueText;
    [SerializeField] private TextMeshProUGUI maxQuantityValueText;
    [SerializeField] private TextMeshProUGUI costText;
    
    [BoxGroup("BUTTON"), SerializeField] private Button cancelBtt;
    [BoxGroup("BUTTON"), SerializeField] private Button purchaseBtt;
    [BoxGroup("BUTTON"), SerializeField] private Button increaseBtt;
    [BoxGroup("BUTTON"), SerializeField] private Button decreaseBtt;

    private ItemCustom _itemCustom;
    private ShopItemBox _shopItemBox;
    private ShopItemSetup _shopItemSetup;
    private float _quantityPurchase;
    private readonly int _minPurchase = 1;
    private int _maxPurchase;
    private int _price;
    private int _quantityReceive;
    private UserData _userData;

    
    private void OnEnable()
    {
        RegisterEvent();
        panelAnimator.Play("Panel_Disable");
    }
    private void OnDisable()
    {
        UnRegisterEvent();
    }
    private void RegisterEvent()
    {
        GUI_Manager.Add(this);
        cancelBtt.onClick.AddListener(OnClickCancelButton);
        purchaseBtt.onClick.AddListener(OnClickPurchaseButton);
        quantitySlider.onValueChanged.AddListener(SliderOnValueChange);
        increaseBtt.onClick.AddListener(OnClickIncreaseQuantityButton);
        decreaseBtt.onClick.AddListener(OnClickDecreaseQuantityButton);
    }
    private void UnRegisterEvent()
    {
        GUI_Manager.Remove(this);
        cancelBtt.onClick.RemoveListener(OnClickCancelButton);
        purchaseBtt.onClick.RemoveListener(OnClickPurchaseButton);
        quantitySlider.onValueChanged.RemoveListener(SliderOnValueChange);
        increaseBtt.onClick.RemoveListener(OnClickIncreaseQuantityButton);
        decreaseBtt.onClick.RemoveListener(OnClickDecreaseQuantityButton);
    }
    
    public void GetRef(GameManager _gameManager)
    {
        _userData = _gameManager.UserData;
    }
    public void UpdateData() { }
    
    
    public void SetPurchasePanel(ShopItemBox _shopItemBox, ItemCustom _itemCustom)
    {
        panelAnimator.Play("Panel_IN");
        this._itemCustom = _itemCustom;
        this._shopItemBox = _shopItemBox;
        //
        _shopItemSetup = _shopItemBox.shopItemSetup;
        _price = _shopItemSetup.GetPrice();
        _maxPurchase = _shopItemSetup.GetPurchaseMax() - _shopItemSetup.GetPurchaseCurrent();
        _quantityReceive = _shopItemSetup.GetQuantityReceive();
        //
        minQuantityValueText.text = $"{_minPurchase}";
        maxQuantityValueText.text = $"{_maxPurchase}";
        quantitySlider.minValue = _minPurchase;
        quantitySlider.maxValue = _maxPurchase;
        quantitySlider.value = _minPurchase;

        guiShop.OnOpenPurchasePanel();
        SliderOnValueChange(1);
    }
    
    // EventCallback
    public void SliderOnValueChange(float _value)
    {
        _quantityPurchase = _value;
        SetQuantityUseText();
        SetChangeQuantityButtonState();
        SetPriceText();
        SetPurchaseButtonState();
    }
    private void OnClickIncreaseQuantityButton()
    {
        _quantityPurchase++;
        quantitySlider.value = _quantityPurchase;
        SliderOnValueChange(_quantityPurchase);
    }
    private void OnClickDecreaseQuantityButton()
    {
        _quantityPurchase--;
        quantitySlider.value = _quantityPurchase;
        SliderOnValueChange(_quantityPurchase);
    }
    private void OnClickCancelButton()
    {
        panelAnimator.Play("Panel_OUT");
        guiShop.OnClosePurchasePanel();
    }
    private void OnClickPurchaseButton()
    {
        OnClickCancelButton();
        var _totalReceive = (int)_quantityPurchase * _quantityReceive;
        _userData.IncreaseCoin(-_price * (int)_quantityPurchase);
        _userData.IncreaseItemValue(_itemCustom.code, _totalReceive);
        //
        _shopItemSetup.Purchase((int)_quantityPurchase);
        _shopItemBox.SetShopItem(_itemCustom, _shopItemSetup);
        //
        ItemObtainedPanel.Instance.OpenPanel(_itemCustom, _totalReceive);
        guiShop.HandleItemPurchaseSuccess();
    }

    
    private void SetChangeQuantityButtonState()
    {
        decreaseBtt.interactable = _quantityPurchase > _minPurchase;
        increaseBtt.interactable = _quantityPurchase < _maxPurchase;
    }
    private void SetPurchaseButtonState() => purchaseBtt.interactable = _userData.Coin >= _price * _quantityPurchase;
    private void SetQuantityUseText() => quantityUseText.text = $"Qty.\n{_quantityPurchase}";
    private void SetPriceText() => costText.text = $"{_price * _quantityPurchase}";
    
}
