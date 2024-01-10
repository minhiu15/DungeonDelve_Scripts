using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DungeonDelve.Project;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUI_Shop : MonoBehaviour, IGUI
{
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GUI_ShopItemPurchase itemPurchase;
    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private TextMeshProUGUI refreshesText;
    [SerializeField] private Button purchaseBtt;
    [Space]
    [SerializeField, BoxGroup("INFOR ITEM")] private ShopItemBox shopItemBoxPrefab;
    [SerializeField, BoxGroup("INFOR ITEM")] private Transform itemBoxContent;
    [Space] 
    [SerializeField, BoxGroup("INFOR ITEM")] private GameObject inforItemPanel;
    [SerializeField, BoxGroup("INFOR ITEM")] private TextMeshProUGUI nameItemText;
    [SerializeField, BoxGroup("INFOR ITEM")] private TextMeshProUGUI typeItemText;
    [SerializeField, BoxGroup("INFOR ITEM")] private TextMeshProUGUI desItemText;
    [SerializeField, BoxGroup("INFOR ITEM")] private Image iconItem, frameNameColor, frameIconColor, frameGradientIconColor;

    [Foldout("FRAME NAME COLOR"), SerializeField] private Color nameCommonColor;
    [Foldout("FRAME NAME COLOR"), SerializeField] private Color nameUncommonColor;
    [Foldout("FRAME NAME COLOR"), SerializeField] private Color nameRareColor;
    [Foldout("FRAME NAME COLOR"), SerializeField] private Color nameEpicColor;
    [Foldout("FRAME NAME COLOR"), SerializeField] private Color nameLegendaryColor;
    //
    [Foldout("FRAME ICON COLOR"), SerializeField] private Color iconCommonColor;
    [Foldout("FRAME ICON COLOR"), SerializeField] private Color iconUncommonColor;
    [Foldout("FRAME ICON COLOR"), SerializeField] private Color iconRareColor;
    [Foldout("FRAME ICON COLOR"), SerializeField] private Color iconEpicColor;
    [Foldout("FRAME ICON COLOR"), SerializeField] private Color iconLegendaryColor;
    //
    [Foldout("FRAME GRADIENT ICON COLOR"), SerializeField] private Color gradientIconCommonColor;
    [Foldout("FRAME GRADIENT ICON COLOR"), SerializeField] private Color gradientIconUncommonColor;
    [Foldout("FRAME GRADIENT ICON COLOR"), SerializeField] private Color gradientIconRareColor;
    [Foldout("FRAME GRADIENT ICON COLOR"), SerializeField] private Color gradientIconEpicColor;
    [Foldout("FRAME GRADIENT ICON COLOR"), SerializeField] private Color gradientIconLegendaryColor;
    
    private static ObjectPooler<ShopItemBox> _poolItemBox;
    private List<ShopItemSetup> _shopData;
    private SO_GameItemData _itemData;
    private ShopItemBox _currentShopItemBox;
    private bool _canPanelOpen;
    private Coroutine _timeCoroutine;
    private UserData _userData;
     
    private void OnEnable()
    {
        _canPanelOpen = true;
        shopPanel.gameObject.SetActive(false);
        RegisterEvent();
    }
    private void Start()
    {
        _shopData = ShopManager.ShopData;
        _poolItemBox = new ObjectPooler<ShopItemBox>(shopItemBoxPrefab, itemBoxContent, _shopData.Count);
        _poolItemBox.List.ForEach(box => box.OnItemSelectedEvent += OnSelectedShopItem);
    }
    private void OnDisable()
    {
        UnRegisterEvent();
    }
    private void OnDestroy()
    {
        _poolItemBox.List.ForEach(box => box.OnItemSelectedEvent -= OnSelectedShopItem);
    }

    private void RegisterEvent()
    {
        GUI_Manager.Add(this);
        purchaseBtt.onClick.AddListener(OnClickPurchaseButton);
        
        if (!ShopManager.Instance) return;
        ShopManager.Instance.interactiveUI.OnPanelOpenEvent += OnOpenPanelEvent;
        ShopManager.Instance.interactiveUI.OnPanelCloseEvent += OnClosePanelEvent;
    }
    private void UnRegisterEvent()
    {
        GUI_Manager.Remove(this);
        purchaseBtt.onClick.RemoveListener(OnClickPurchaseButton);
        _userData.OnCoinChangedEvent -= SetCurrencyText;
        
        if (!ShopManager.Instance) return;
        ShopManager.Instance.interactiveUI.OnPanelOpenEvent -= OnOpenPanelEvent;
        ShopManager.Instance.interactiveUI.OnPanelCloseEvent -= OnClosePanelEvent;
    }
    
    
    public void GetRef(GameManager _gameManager)
    {
        _itemData = _gameManager.GameItemData;
        _userData = _gameManager.UserData;
        _userData.OnCoinChangedEvent += SetCurrencyText;
    }
    public void UpdateData() { }
    
    
    private void OnOpenPanelEvent()
    {
        if (!_canPanelOpen) return;
        _canPanelOpen = false;
        
        shopPanel.SetActive(true);
        ShowShop();
        
        if (_timeCoroutine != null) 
            StopCoroutine(_timeCoroutine);
        _timeCoroutine = StartCoroutine(TimeCoroutine());
        
        GUI_Inputs.InputAction.UI.OpenBag.Disable();
        MenuController.Instance.HandleMenuOpen();
    }
    private void OnClosePanelEvent()
    {
        if (_canPanelOpen) return;
        _canPanelOpen = true;
        
        shopPanel.SetActive(false);
        
        if (_timeCoroutine != null) 
            StopCoroutine(_timeCoroutine);
        
        GUI_Inputs.InputAction.UI.OpenBag.Enable();
        MenuController.Instance.HandleMenuClose();
    }
    public void OnClickClosePanelButton() => ShopManager.Instance.interactiveUI.ClosePanel(default);
    private IEnumerator TimeCoroutine()
    {
        while (true)
        {
            var _currentTime = DateTime.Now;
            var _nextMidnight = _currentTime.Date.AddDays(1);
            var _time = _nextMidnight - _currentTime;
            refreshesText.text = $"Items refresh in: {_time.Hours} hour and {_time.Minutes} minute.";
            yield return new WaitForSecondsRealtime(10f);
        }
    }
    private void ShowShop()
    {
        ShopManager.SortShopItemData();
        inforItemPanel.SetActive(false);
        _poolItemBox.List.ForEach(x => x.Release());
        var _itemCount = _shopData.Count;
        for (var i = 0; i < _itemCount; i++)
        {
            _poolItemBox.Get();
        }
        _itemCount = 0;
        var _pools = _poolItemBox.List.Where(x => x.gameObject.activeSelf);
        foreach (var shopItemBox in _pools)
        {
            var _shopItemSetup = _shopData[_itemCount];
            var _itemCustom = _itemData.GetItemCustom(_shopItemSetup.GetItemCode());
            shopItemBox.SetShopItem(_itemCustom,_shopItemSetup);
            _itemCount++;
        }
        purchaseBtt.interactable = false;
    }
    private void OnSelectedShopItem(ShopItemBox _shopItemBox)
    {
        _currentShopItemBox = _shopItemBox;
        purchaseBtt.interactable = _shopItemBox.shopItemSetup.CanBuyItem();
        inforItemPanel.SetActive(true);
        SetAnimSelectedShopItem(_shopItemBox);
        SetInforShopItem(_shopItemBox.shopItemSetup);
    }
    private void SetAnimSelectedShopItem(ShopItemBox _shopItemBox)
    {
        var _currentList = _poolItemBox.List.Where(box => box.gameObject.activeSelf);
        foreach (var _shopItem in _currentList)
        {
            var _animator = _shopItem.animator;
            if (_animator.IsTag("Selected") && _shopItem != _currentShopItemBox)
                _shopItem.animator.Play(SwitchPanelControl.NameHashID_NonTrigger);
            else if (!_animator.IsTag("Selected") && _animator == _shopItemBox.animator)
                _shopItem.animator.Play(SwitchPanelControl.NameHashID_Selected);
        }
    }
    private void SetInforShopItem(ShopItemSetup _shopItemSetup)
    {
        var itemCustom = _itemData.GetItemCustom(_shopItemSetup.GetItemCode());
        iconItem.sprite = itemCustom.sprite;
        nameItemText.text = $"{itemCustom.nameItem}";
        typeItemText.text = itemCustom.type switch
        {
            ItemType.Consumable => "Consumable Item",
            ItemType.Upgrade => "Upgrade/Enhancement Item",
            ItemType.Quest => "Quest Item",
            ItemType.Currency => "Currency",
            _ => "???"
        };
        desItemText.text = $"{itemCustom.description}"; 
        switch (itemCustom.ratity)
        {
            case ItemRarity.Common:
                frameNameColor.color = nameCommonColor;
                frameIconColor.color = iconCommonColor;
                frameGradientIconColor.color = gradientIconCommonColor;
                break;
            case ItemRarity.Uncommon:
                frameNameColor.color = nameUncommonColor;
                frameIconColor.color = iconUncommonColor;
                frameGradientIconColor.color = gradientIconUncommonColor;
                break;
            case ItemRarity.Rare:
                frameNameColor.color = nameRareColor;
                frameIconColor.color = iconRareColor;
                frameGradientIconColor.color = gradientIconRareColor;
                break;
            case ItemRarity.Epic:
                frameNameColor.color = nameEpicColor;
                frameIconColor.color = iconEpicColor;
                frameGradientIconColor.color = gradientIconEpicColor;
                break;
            case ItemRarity.Legendary:
                frameNameColor.color = nameLegendaryColor;
                frameIconColor.color = iconLegendaryColor;
                frameGradientIconColor.color = gradientIconLegendaryColor;
                break;
        }
    }
    public static void IsTriggerShopItemBox(ShopItemBox _shopItemBox, bool _hasTrigger)
    {
        var _shopItemList = _poolItemBox.List.Where(box => box.gameObject.activeSelf).ToList()
            .Where(box => !box.animator.IsTag("Selected", 0) && !box.animator.IsTag("SelectedQuest", 0));
        
        int _hashID;
        if (_hasTrigger)
        {
            foreach (var _shopItem in _shopItemList)
            {
                _hashID = _shopItem == _shopItemBox ? SwitchPanelControl.NameHashID_Trigger : SwitchPanelControl.NameHashID_NonTrigger;
                _shopItem.animator.Play(_hashID);
            }   
        }
        else
        {
            foreach (var _shopItem in _shopItemList)
            {
                _hashID = SwitchPanelControl.NameHashID_NonTrigger;
                _shopItem.animator.Play(_hashID);
            }
        }
    }
    private void OnClickPurchaseButton()
    {
        var _itemCustom = _itemData.GetItemCustom(_currentShopItemBox.shopItemSetup.GetItemCode());
        itemPurchase.SetPurchasePanel(_currentShopItemBox, _itemCustom);
    }
    
    public void HandleItemPurchaseSuccess()
    {
        ShowShop();
            
        if (PlayFabHandleUserData.Instance)
            PlayFabHandleUserData.Instance.UpdateData(PlayFabHandleUserData.PF_Key.ShopData_Key);
    }
    public void OnOpenPurchasePanel()
    {
        ShopManager.Instance.interactiveUI.OnPanelCloseEvent -= OnClosePanelEvent;
    }
    public void OnClosePurchasePanel()
    {
        ShopManager.Instance.interactiveUI.OnPanelCloseEvent += OnClosePanelEvent;
    }
    
    private void SetCurrencyText(int _value) => currencyText.text = $"{_value}";
}
