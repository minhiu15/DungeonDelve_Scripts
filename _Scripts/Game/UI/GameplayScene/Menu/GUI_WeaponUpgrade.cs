using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUI_WeaponUpgrade : MonoBehaviour, IGUI
{
    [SerializeField] private RawImage rawMesh;
    [SerializeField] private TextMeshProUGUI weaLevelText;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private TextMeshProUGUI currencyText;
    [Space]
    [SerializeField] private UI_Item itemPrefab;
    [SerializeField] private Transform slotItems;

    [Space]
    [SerializeField] private Button cancelBtt;
    [SerializeField] private Button upgradeBtt;
    
    private ObjectPooler<UI_Item> _poolItem;
    private UserData _userData;
    private SO_PlayerConfiguration _playerConfig;
    private PlayerRenderTexture _playerRenderTexture;
    private SO_RequiresWeaponUpgradeConfiguration _weaponUpgradeConfig;
    private SO_GameItemData _gameItemData;
    private Coroutine _upgradeCoroutine;
    private SO_RequiresWeaponUpgradeConfiguration.RequiresData _requiresConfig;
    private bool _isEventRegistered;
    
    private int _coin;
    private int _coinUpgrdeCost;
    private int _weaponLevel;
    private bool _canUpgrade;
    private float _increaseCRITRate => _playerConfig.GetCRITRate() * .065f;
    private float _increaseCRITDMG => _playerConfig.GetCRITDMG() * .15f;
    
    private void OnEnable()
    {
        GUI_Manager.Add(this);
        cancelBtt.onClick.AddListener(OnClickCancelButton);
        upgradeBtt.onClick.AddListener(OnClickUpgradeButton);
    }
    private void OnDisable()
    {
        GUI_Manager.Remove(this);
        cancelBtt.onClick.RemoveListener(OnClickCancelButton);
        upgradeBtt.onClick.RemoveListener(OnClickUpgradeButton);
        
        if(!_isEventRegistered) return;
        _userData.OnCoinChangedEvent -= OnCoinChanged;
        _isEventRegistered = false;
    }
    
    
    public void GetRef(GameManager _gameManager)
    {
        _userData = _gameManager.UserData;
        _gameItemData = _gameManager.GameItemData;
        _playerConfig = _gameManager.Player.PlayerConfig;
        _weaponUpgradeConfig = _gameManager.Player.playerData.WeaponUpgradeConfig;
        _playerRenderTexture = _gameManager.Player.playerData.PlayerRenderTexture;
        rawMesh.texture = _playerRenderTexture.renderTexture;
        
        if (!_isEventRegistered)
        {
            _isEventRegistered = true;
            _userData.OnCoinChangedEvent += OnCoinChanged;
        }
        
        Init();
        UpdateData();
    }
    private void OnCoinChanged(int _value)
    {
        _coin = _value;
        SetCoinText();
    }
    private void Init()
    {
        _poolItem = new ObjectPooler<UI_Item>(itemPrefab, slotItems, _gameItemData.GameItemDatas.Count);
        progressSlider.maxValue = 1;
        progressSlider.minValue = 0;
        progressSlider.value = 0;
    }
    
    
    public void UpdateData()
    {
        GetStats();
        SetItemRequires();
        
        SetCoinText();
        SetWeaponLevelText();
        SetProgressSlider();
        SetUpgradeStateButton();
        SetRenderTexture();
    }
    private void GetStats()
    {
        if (!_playerConfig) return;

        _weaponLevel = _playerConfig.GetWeaponLevel();
    }
    private void SetItemRequires()
    {
        if(!_weaponUpgradeConfig) return;
        foreach (var item in _poolItem.List.Where(x => x.gameObject.activeSelf))
        {
            item.Release();
        }
        
        _canUpgrade = true;
        if (_weaponLevel >= _weaponUpgradeConfig.maxLevelUpgrade)
        {
            _canUpgrade = false;
            return;
        }
        
        _requiresConfig = _weaponUpgradeConfig.GetRequires(_weaponLevel);
        _coinUpgrdeCost = _requiresConfig.coinCost;
        
        foreach (var _requiresItem in _requiresConfig.requiresItem)
        {
            if (!_gameItemData.GetItemCustom(_requiresItem.code, out var _itemCustom))
                continue;
            
            var hasItemValue = _userData.HasItemValue(_itemCustom.code);
            var itemValueToStr = hasItemValue < _requiresItem.value ? $"<color=red>{hasItemValue}</color> / {_requiresItem.value}" : $"<color=white>{hasItemValue}</color> / {_requiresItem.value}" ;
            var item = _poolItem.Get();
            item.SetItem(_itemCustom, hasItemValue);
            item.SetValueText(itemValueToStr);
            
            if (hasItemValue < _requiresItem.value) 
                _canUpgrade = false;
        }
    }


    private void OnClickCancelButton()
    {
        MenuController.Instance.CloseMenu();
    }
    private void OnClickUpgradeButton()
    {
        if(_upgradeCoroutine != null) StopCoroutine(_upgradeCoroutine);
        _upgradeCoroutine = StartCoroutine(UpgradeCoroutine());
    }
    private IEnumerator UpgradeCoroutine()
    {
        while (Math.Abs(progressSlider.value - 1) > 0)
        {
            progressSlider.value += .06f;
            yield return new WaitForSecondsRealtime(.01f);
        }
        
        var currentLevel = _playerConfig.GetWeaponLevel() + 1;
        var currentCRITRate = _playerConfig.GetCRITRate() + _increaseCRITRate;
        var currentCRITDMG = _playerConfig.GetCRITDMG() + Mathf.CeilToInt(_increaseCRITDMG);
        
        UpgradeNoticeManager.Instance.SetLevelText($"Lv. {currentLevel}");
        UpgradeNoticeManager.CreateNoticeBar("CRIT Rate", _playerConfig.GetCRITRate().ToString("F2") + " %",   currentCRITRate.ToString("F2") + " %");
        UpgradeNoticeManager.CreateNoticeBar("CRIT DMG", _playerConfig.GetCRITDMG().ToString("F2") + " %", currentCRITDMG.ToString("F2") + " %");
        UpgradeNoticeManager.Instance.EnableNotice();
        
        _userData.IncreaseCoin(-_coinUpgrdeCost);
        _requiresConfig.requiresItem.ForEach(_itemRequires =>  _userData.IncreaseItemValue(_itemRequires.code, -_itemRequires.value));
        
        _playerConfig.SetWeaponLevel(currentLevel);
        _playerConfig.SetCRITRate(currentCRITRate);
        _playerConfig.SetCRITDMG(currentCRITDMG);
        
        GUI_Manager.UpdateGUIData();
        if(PlayFabHandleUserData.Instance) PlayFabHandleUserData.Instance.UpdateAllData();
    }


    private void SetProgressSlider() => progressSlider.value = _weaponLevel < _weaponUpgradeConfig.maxLevelUpgrade ? 0 : progressSlider.maxValue;

    private void SetUpgradeStateButton() => upgradeBtt.interactable = _canUpgrade && _coin >= _coinUpgrdeCost;
    private void SetWeaponLevelText()
    {
        var _demoLvToStr = _weaponLevel >= _weaponUpgradeConfig.maxLevelUpgrade ? "MAX" : "+ 1";
        weaLevelText.text = $"Lv. {_weaponLevel}  <size=35><color=#FFD900> {_demoLvToStr}</color></size>";
    }
    private void SetCoinText() => currencyText.text = $"{_coin}/{_coinUpgrdeCost}";
    public void SetRenderTexture()
    {
        if (_weaponLevel < _weaponUpgradeConfig.maxLevelUpgrade)
        {
            rawMesh.enabled = false;
            _playerRenderTexture.CloseRenderUI();
            return;
        }

        rawMesh.enabled = true;
        _playerRenderTexture.OpenRenderUI(PlayerRenderTexture.RenderType.Weapon);
    }
}
