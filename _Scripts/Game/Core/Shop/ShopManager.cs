using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : Singleton<ShopManager>
{
    [SerializeField] private MonoBehaviourID behaviourID;
    [field: SerializeField] public InteractiveUI interactiveUI { get; private set; }
    public static List<ShopItemSetup> ShopData { get; private set; } = new();
    
    private void Start()
    {
        ShopData.Clear();
        if (PlayFabHandleUserData.Instance && PlayFabHandleUserData.Instance.IsLogin)
        {
            ShopData = PlayFabHandleUserData.Instance.ShopItems;
        }
        else
        {
            var _data = Resources.LoadAll<ShopItemSetup>("Shop Custom");
            foreach (var shopItemSetup in _data)
            {
                ShopData.Add(Instantiate(shopItemSetup));
            }
        }
        
        var _lastDay = DateTime.Parse(PlayerPrefs.GetString(behaviourID.GetID, DateTime.MinValue.ToString()));
        if (_lastDay < DateTime.Today)
            LoadNewShopItem();

        SortShopItemData();
    }
    private void OnApplicationQuit()
    {
        PlayerPrefs.SetString(behaviourID.GetID, DateTime.Now.ToString());
    }
    private static void LoadNewShopItem()
    {
        foreach (var shopItemSetup in ShopData)
        {
            shopItemSetup.SetPurchaseCurrent(0);
        }
    }
    public static void SortShopItemData()
    {
        ShopData.Sort((x1, x2) =>
        {
            var compareCanBuyItem = x2.CanBuyItem().CompareTo(x1.CanBuyItem());
            return compareCanBuyItem == 0 ? x1.GetRarity().CompareTo(x2.GetRarity()) : compareCanBuyItem;
        });
    }
    

}
