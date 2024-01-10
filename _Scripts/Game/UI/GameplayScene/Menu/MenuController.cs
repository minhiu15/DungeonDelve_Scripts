using System.Collections;
using DG.Tweening;
using DungeonDelve.Project;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class MenuController : Singleton<MenuController>
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GuidancePanel guidancePanel;
    [SerializeField] private TextMeshProUGUI currencyText;
    [Space]
    public UnityEvent OnClickEscOpenMenuEvent;
    public UnityEvent OnClickBOpenMenuEvent;
    public UnityEvent OnCloseMenuEvent;
    
    private UserData _userData;
    private PlayerController _player;
    private GameManager _gameManager;
    private PlayerHUD _playerHUD;
    
    private bool _isOpenMenu;
    private bool _isEventRegistered;
    private readonly string PP_CanGuidance = "Guidance";
    
    private void OnEnable()
    {
        RegisterEvent();
    }
    private void Start()
    {
        _isOpenMenu = false;
        CursorHandle.Locked();
        StartCoroutine(CheckGuidance());
    }
    private void OnDisable()
    {
        UnRegisterEvent();
    }

    private void RegisterEvent()
    {
        GUI_Inputs.InputAction.UI.OpenMenu.performed += OpenMenu;
        GUI_Inputs.InputAction.UI.OpenBag.performed += OpenBag;

        _gameManager = GameManager.Instance;
        if(!_gameManager) return;
        GUI_Manager.SendRef(_gameManager);

        _player = _gameManager.Player;
        _playerHUD = _gameManager.PlayerHUD;
        _userData = _gameManager.UserData;
        
        if (!_isEventRegistered)
        {
            _isEventRegistered = true;
            _userData.OnCoinChangedEvent += OnCoinChanged;
        }
        _userData.SendEventCoinChaged();
    }
    private void UnRegisterEvent()
    {
        GUI_Inputs.InputAction.UI.OpenMenu.performed -= OpenMenu;
        GUI_Inputs.InputAction.UI.OpenBag.performed -= OpenBag;
        
        if(!_isEventRegistered) return;
        _userData.OnCoinChangedEvent -= OnCoinChanged;
        _isEventRegistered = false;
    }
    
    private IEnumerator CheckGuidance()
    {
        yield return null;
        if (PlayerPrefs.HasKey(PP_CanGuidance)) yield break;
        
        var _guidence = Instantiate(guidancePanel, transform);
        _guidence.OpenGuidancePanel();
        HandleMenuOpen();
        PlayerPrefs.SetString(PP_CanGuidance, "No");
    }

    
    
    private void OpenMenu(InputAction.CallbackContext _context)
    {
        if (Time.timeScale == 0 && !_isOpenMenu) return;
        _isOpenMenu = !_isOpenMenu;
        if(_isOpenMenu)
        {
            OpenMenu();
            OnClickEscOpenMenuEvent?.Invoke(); 
        }
        else
        {
            CloseMenu();
        }
    }
    private void OpenBag(InputAction.CallbackContext _context)
    {
        if(_isOpenMenu) return;
        _isOpenMenu = true;
        OpenMenu();
        OnClickBOpenMenuEvent?.Invoke(); 
    }
    private void OnCoinChanged(int _value) => currencyText.text = $"{_value}";
    
    
    private void OpenMenu()
    {
        GUI_Manager.UpdateGUIData();
        menuPanel.SetActive(true);
        _player.playerData.PlayerRenderTexture.OpenRenderUI(PlayerRenderTexture.RenderType.Character);
        HandleMenuOpen();
    }
    public void CloseMenu()
    {
        OnCloseMenuEvent?.Invoke();
        menuPanel.SetActive(false);
        _player.playerData.PlayerRenderTexture.CloseRenderUI();
        _isOpenMenu = false;
        HandleMenuClose();
    }

    public void HandleMenuOpen()
    {
        _playerHUD.CloseHUD();
        Time.timeScale = 0;
        Blur.Instance.Enable();
        CursorHandle.NoneLocked();
        AudioManager.PlayOneShot(FMOD_Events.Instance.menuOpen, transform.position);
    }
    public void HandleMenuClose()
    {
        _playerHUD.OpenHUD();
        Time.timeScale = 1;
        Blur.Instance.Disable();
        CursorHandle.Locked();
        AudioManager.PlayOneShot(FMOD_Events.Instance.menuClose, transform.position);
    }
    public void ExitToLoginScene(string _sceneName)
    {
        Time.timeScale = 1;
        DOTween.Clear();
        
        if (PlayFabController.Instance) PlayFabController.Instance.ClearAccountTemp();
        if (LoadSceneManager.Instance) LoadSceneManager.Instance.LoadScene(_sceneName);
    }
}
