using System;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Create Item Purchase", fileName = "Purchase_")]
public class ShopItemSetup : ScriptableObject
{
    [SerializeField, JsonProperty] private ItemNameCode itemNameCode;
    [JsonIgnore, SerializeField] private int purchaseMax;
    [SerializeField, JsonProperty] private int purchaseCurrent;
    [JsonIgnore, SerializeField] private int price;
    [JsonIgnore, SerializeField] private int quantityReceive;
    [JsonIgnore, SerializeField] private ItemRarity itemRarity;
    
    
    // GETTER
    public ItemNameCode GetItemCode() => itemNameCode;
    public int GetPurchaseMax() => purchaseMax;
    public int GetPurchaseCurrent() => purchaseCurrent;
    public int GetPrice() => price;
    public int GetQuantityReceive() => quantityReceive;
    public ItemRarity GetRarity() => itemRarity;
    public bool CanBuyItem() => purchaseCurrent < purchaseMax;
    
    // SETTER
    public void SetItemCode(ItemNameCode _itemNameCode) => itemNameCode = _itemNameCode;
    public void SetPurchaseMax(int _value) => purchaseMax = _value;
    public void SetPurchaseCurrent(int _value) => purchaseCurrent = _value;
    public void SetPrice(int _value) => price = _value;
    public void SetQuantityReceive(int _value) => quantityReceive = _value;
    public void SetRarity(ItemRarity _itemRarity) => itemRarity = _itemRarity;
    public void Purchase(int _value) => purchaseCurrent = Mathf.Clamp(purchaseCurrent + _value, 0, purchaseMax);
    
}
