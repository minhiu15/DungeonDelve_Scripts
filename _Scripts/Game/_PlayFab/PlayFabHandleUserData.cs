using System.Collections.Generic;
using System.Linq;
using DungeonDelve.Project;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.Events;


public class PlayFabHandleUserData : Singleton<PlayFabHandleUserData>
{
    public UnityEvent OnLoadUserDataSuccessEvent;
    public UnityEvent OnLoadUserDataFailureEvent;
    
    public bool IsLogin { get; private set; }
    public UserData UserData { get; private set; }
    public List<ShopItemSetup> ShopItems { get; } = new();
    public List<QuestSetup> Quests { get; } = new();
    public SO_PlayerConfiguration PlayerConfig;
    
    public enum PF_Key : byte // PlayerFab KeyValue
    {
        UserData_Key,
        PlayerConfigData_Key,
        QuestData_Key,
        ShopData_Key
    }
    private void Start()
    {
        IsLogin = false;
        GetInternalGameData();
        PlayFabController.Instance.OnLoginSuccessEvent += OnLoginSuccess;
    }
    private void OnDestroy()
    {
        PlayFabController.Instance.OnLoginSuccessEvent -= OnLoginSuccess;
    }
    private void OnApplicationQuit()
    {
        if (IsLogin) 
            UpdateAllData();
    }
    
    
    private void OnLoginSuccess()
    {
        IsLogin = true;
        GetUserData();
    }
    public void UpdateAllData()
    {
        var _userDataJSON = JsonConvert.SerializeObject(UserData, Formatting.Indented);
        var _playerConfigJSON = JsonConvert.SerializeObject(PlayerConfig, Formatting.Indented);
        var _questJSON = JsonConvert.SerializeObject(Quests.Select(x => x.GetTask()), Formatting.Indented);
        var _shopJSON = JsonConvert.SerializeObject(ShopItems, Formatting.Indented);
        
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { PF_Key.UserData_Key.ToString(), _userDataJSON },
                { PF_Key.PlayerConfigData_Key.ToString(), _playerConfigJSON },
                { PF_Key.QuestData_Key.ToString(), _questJSON },
                { PF_Key.ShopData_Key.ToString(), _shopJSON },
            }
        };
        PlayFabClientAPI.UpdateUserData(request, 
            _ => Debug.Log("UpData Success") , 
            _ => Debug.Log("UpData Failure"));
    }
    public void UpdateData(PF_Key _keySave)
    {
        if(!IsLogin) return;

        var jsonText = _keySave switch
        {
            PF_Key.UserData_Key => JsonConvert.SerializeObject(UserData, Formatting.Indented),
            PF_Key.PlayerConfigData_Key => JsonConvert.SerializeObject(PlayerConfig, Formatting.Indented),
            PF_Key.QuestData_Key => JsonConvert.SerializeObject(Quests.Select(x => x.GetTask()), Formatting.Indented),
            PF_Key.ShopData_Key => JsonConvert.SerializeObject(ShopItems, Formatting.Indented),
            _ => ""
        };
        
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { _keySave.ToString(), jsonText }
            }
        };
        PlayFabClientAPI.UpdateUserData(request, 
            _ => Debug.Log("UpData Success: " + _keySave) , 
            _ => Debug.Log("UpData Failure: " + _keySave) );
    }

    private void GetInternalGameData()
    {
        var _quests = Resources.LoadAll<QuestSetup>("Quest Custom");
        foreach (var questSetup in _quests)
        {
            Quests.Add(Instantiate(questSetup));
        }
        var _shops = Resources.LoadAll<ShopItemSetup>("Shop Custom");
        foreach (var shopItemSetup in _shops)
        {
            ShopItems.Add(Instantiate(shopItemSetup));
        }
    }
    private void GetUserData()
    {
        if(!IsLogin) return;
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnGetUserDataResult, ErrorCallback);
    }
    private void OnGetUserDataResult(GetUserDataResult _result)
    {
        if (_result.Data == null || !_result.Data.ContainsKey($"{PF_Key.PlayerConfigData_Key}"))
        {
            UserData = new UserData(PlayFabController.Instance.username, 25000);
            UpdateData(PF_Key.UserData_Key);
            OnLoadUserDataFailureEvent?.Invoke();
            return;
        }
        
        if (_result.Data.TryGetValue($"{PF_Key.UserData_Key}", out var userDataRecord))
        {
            UserData = JsonConvert.DeserializeObject<UserData>(userDataRecord.Value);
        }
        if (_result.Data.TryGetValue($"{PF_Key.PlayerConfigData_Key}", out var playerConfigDataRecord))
        {
            PlayerConfig = JsonConvert.DeserializeObject<SO_PlayerConfiguration>(playerConfigDataRecord.Value);
        }
        if (_result.Data.TryGetValue($"{PF_Key.QuestData_Key}", out var questDataRecord))
        {
            var _tasks = JsonConvert.DeserializeObject<List<Task>>(questDataRecord.Value);
            for (var i = 0; i < _tasks.Count; i++)
            {
                LoadQuestData(Quests[i], _tasks[i]);
            }
        }
        if (_result.Data.TryGetValue($"{PF_Key.ShopData_Key}", out var shopDataRecord))
        {
            var _shopItem = JsonConvert.DeserializeObject<List<ShopItemSetup>>(shopDataRecord.Value);
            foreach (var oldShopItem in _shopItem)
            {
                LoadShopData(oldShopItem);
            }
        }
        
        OnLoadUserDataSuccessEvent?.Invoke();
    }

    private void LoadQuestData(QuestSetup _questSetup, Task _oldTask)
    {
        _questSetup.SetTask(new Task(_oldTask.IsCompleted(), _oldTask.IsLocked(), _oldTask.IsReceived()));
    }
    private void LoadShopData(ShopItemSetup _oldShopItem)
    {
        var _listShopItem = ShopItems.Where(_shopItem => _shopItem.GetItemCode() == _oldShopItem.GetItemCode());
        foreach (var _shopItem in _listShopItem)
        {
            _shopItem.SetPurchaseCurrent(_oldShopItem.GetPurchaseCurrent());
        }
    }
    
    private static void ErrorCallback(PlayFabError _error) => Debug.LogWarning(_error.Error);
    
}
