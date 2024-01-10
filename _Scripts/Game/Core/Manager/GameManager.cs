using System;
using FMODUnity;
using UnityEditor;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private StudioListener fmodListener;
    
    /// <summary> Dữ liệu tất cả Item trong game </summary>
    [field: SerializeField] public SO_CharacterData CharacterData { get; private set; }

    /// <summary> Dữ liệu về các thông tin và yêu cầu khi nâng cấp Level của nhân vật </summary>
    [field: SerializeField] public SO_CharacterUpgradeData CharacterUpgradeData { get; private set; }
    
    /// <summary> Dữ liệu tất cả Item trong game </summary>
    [field: SerializeField] public SO_GameItemData GameItemData { get; private set; }
    
    /// <summary> Dữ liệu người dùng </summary>
    public UserData UserData { get; private set; }
    public PlayerController Player { get; private set; }
    public PlayerHUD PlayerHUD { get; private set; }
    
    //
    private SO_PlayerConfiguration _playerConfig;
    private PlayerController _playerPrefab;
    
    private void OnEnable()
    {
        CharacterUpgradeData.RenewValue();
        if(!PlayFabHandleUserData.Instance)
        {
            UserData = new UserData("Test Editor", 500000);
            _playerPrefab = CharacterData.GetPrefab(CharacterNameCode.Lynx);
            _playerConfig = Instantiate(_playerPrefab.PlayerConfig);
        }
        else
        {
            UserData = PlayFabHandleUserData.Instance.UserData;
            _playerConfig = PlayFabHandleUserData.Instance.PlayerConfig;
            _playerPrefab =  CharacterData.GetPrefab(_playerConfig.NameCode);
        }
        _playerConfig.ChapterIcon = _playerPrefab.PlayerConfig.ChapterIcon;
        InstancePlayer();   
    }
    private void InstancePlayer()
    {
        Player = Instantiate(_playerPrefab, Vector3.zero, Quaternion.identity);
        Player.playerData.SetConfiguration(_playerConfig);
        PlayerHUD = Player.GetComponentInChildren<PlayerHUD>();
        // Set Status Value
        var _value = _playerConfig.GetHP();
        Player.Health.InitValue(_value, _value);
        _value = _playerConfig.GetST();
        Player.Stamina.InitValue(_value, _value);
    }
    private void Start()
    {
        fmodListener.SetAttenuationObject(Player.gameObject);
    }
    private void OnApplicationQuit() => EnemyTracker.Clear();

    
    
    [ContextMenu("Delete All PlayerPrefs Key")]
    private void DeleteAllPlayerPrefsKey()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Delete PlayerPrefs Key Success !");
    }
    
}
