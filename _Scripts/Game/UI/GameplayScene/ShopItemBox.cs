using System;
using TMPro;
using UnityEngine;

public class ShopItemBox : MonoBehaviour, IPooled<ShopItemBox>
{
    [field: SerializeField] public Animator animator { get; private set; }
    [SerializeField] private UI_Item itemUI;
    [Space]
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI purchaseUpToText;
    
    public event Action<ShopItemBox> OnItemSelectedEvent;

    /// <summary> Thông tin ShopItem mà box đang giữ </summary>
    public ShopItemSetup shopItemSetup { get; private set; }
    
    
    public void SetShopItem(ItemCustom _itemCustom, ShopItemSetup _shopItemSetup)
    {
        shopItemSetup = _shopItemSetup;
        
        itemName.text = _itemCustom.nameItem;
        priceText.text = $"{_shopItemSetup.GetPrice()}";
        purchaseUpToText.text = !shopItemSetup.CanBuyItem() ? "<color=#FF0500>Sold out</color>" : 
            $"Purchase up to {shopItemSetup.GetPurchaseCurrent()}/{shopItemSetup.GetPurchaseMax()}";
        
        var _quantity = _shopItemSetup.GetQuantityReceive();
        itemUI.SetItem(_itemCustom, _quantity);
        itemUI.SetValueText($"{_quantity}");
    }
    public void SelectShopItem() =>  OnItemSelectedEvent?.Invoke(this);
    public void TriggerShopItem(bool _isTigger) => GUI_Shop.IsTriggerShopItemBox(this, _isTigger);
    
    public void Release() => ReleaseCallback?.Invoke(this);
    public Action<ShopItemBox> ReleaseCallback { get; set; }
}
