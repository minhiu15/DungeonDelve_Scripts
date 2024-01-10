using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CheatsManager : MonoBehaviour, IGUI
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_InputField inputField;
    [Space]
    [SerializeField] private TextBar textBarPrefab;
    [SerializeField] private Transform textContent;
    [Space]
    [SerializeField] private AutoScrollScrollView scrollView;
    [SerializeField] private RectTransform frameScale;

    //
    private readonly Vector2 posScale = new(294f, 433.0027f);
    private readonly Vector2 sizeScale = new(550f, 701.0052f);
    private readonly Vector2 posNotScale = new(294f, 265f);
    private readonly Vector2 sizeNotScale = new(550f, 365f);
    [HideInInspector] private UserData _userData;
    [HideInInspector] private PlayerController _player;
    [HideInInspector] private Inputs _input;
    [HideInInspector] private ItemNameCode[] _itemCodes;
    private readonly Vector3[] _waypoints = {
        Vector3.zero, 
        new (2.22f, 11f, 55f),
        new (24.37f, 12.8f, 165f),
        new (14.3f, -30.3f, 176.5f),
    };
    //
    private readonly WaitForSecondsRealtime _wait_100ms = new(.1f);
    private readonly WaitForSecondsRealtime _wait_500ms = new(.5f);
    private readonly WaitForSecondsRealtime _waitNull = new(0f);
    private Coroutine _itemHandleCoroutine;
    private Coroutine _characterHandleCoroutine;
    private Coroutine _positionHandleCoroutine;
    private Coroutine _playerPrefsCoroutine;
    //
    [HideInInspector] private bool _isOpen;
    private readonly int _maxLimit = 50;
    private bool _enter;
    private bool _isStarted;
    private bool _checkType;
    // Default String Code
    private readonly string _pattern_ENTER = "enter";
    private readonly string _pattern_RESET = "reset";
    private readonly string _pattern_RETURN = "/b";
    private readonly string _pattern_SET_ITEM = @"\[KEY:(\d+),VAL:(\d+)]";
    
    
    private void OnEnable()
    {
        GUI_Manager.Add(this);
        _input = new Inputs();
        _input.Enable();
        _input.TESTER.Enter.performed += OnEnterChat;
    }
    private void Start()
    {
        _enter = false;
        panel.SetActive(false);
        _itemCodes = (ItemNameCode[])Enum.GetValues(typeof(ItemNameCode));
    }
    private void OnDisable()
    {
        GUI_Manager.Remove(this);
        _input.TESTER.Enter.performed -= OnEnterChat;
        _input.Disable();
    }
    public void GetRef(GameManager _gameManager)
    {
        _userData = _gameManager.UserData;
        _player = _gameManager.Player;
    }
    public void UpdateData() { }
    
    
    private void OnScalePanel(InputAction.CallbackContext _context)
    {
        if (inputField.text.Length != 0) return;
        if (_context.ReadValueAsButton())
        {
            frameScale.sizeDelta = sizeScale;
            frameScale.anchoredPosition = posScale;
            inputField.DeactivateInputField();
        }
        else
        {
            inputField.ActivateInputField();
            frameScale.sizeDelta = sizeNotScale;
            frameScale.anchoredPosition = posNotScale;
            scrollView.Scroll();
        }
    }
    private void OnEnterChat(InputAction.CallbackContext _context)
    {
        _isOpen = !_isOpen;
        if (!_isOpen && string.IsNullOrEmpty(inputField.text))
        {
            _enter = false;
            ClosePanel();
            return;
        }
        
        _enter = inputField.text.Length != 0;
        OpenPanel();
        HandlerCheats();
    }
    private void OpenPanel()
    {          
        if (panel.activeSelf) return;
        panel.SetActive(true);
        CursorHandle.NoneLocked();
        GUI_Inputs.InputAction.UI.Disable();
        _player.input.PlayerInput.Disable();
        _input.TESTER.ScalePanel.started += OnScalePanel;
        _input.TESTER.ScalePanel.canceled += OnScalePanel;
        
        //
        _isStarted = false;
        _checkType = false;
        frameScale.sizeDelta = sizeNotScale;
        frameScale.anchoredPosition = posNotScale;
        SpawnText("<color=#00ECFF>------- CHEATS PANEL -------</color>");
        Time.timeScale = 0;
        ResetAllTypeCoroutine();
    }
    private void ClosePanel()
    {
        Time.timeScale = 1;
        panel.SetActive(false);
        CursorHandle.Locked();
        GUI_Inputs.InputAction.UI.Enable();
        _player.input.PlayerInput.Enable();
        _input.TESTER.ScalePanel.started -= OnScalePanel;
        _input.TESTER.ScalePanel.canceled -= OnScalePanel;
        inputField.text = "";
        ResetAllTypeCoroutine();
    }
    private void HandlerCheats()
    {
        inputField.ActivateInputField();
        if (string.IsNullOrEmpty(inputField.text)) return;
        if (CheckPattern(inputField.text, _pattern_RESET))
        {
            _isStarted = false;
            _checkType = false;
            ReleaseTextBox();
            SpawnText("<color=green>Reset Success!!!</color>");
            return;
        }
        
        if (!_isStarted)
        {
            if (!CheckPattern(inputField.text, _pattern_ENTER))
            {
                SpawnText("<color=#FF3434>Unable to start. Please try again. !</color>");
                return;
            } 
            ShowTypeStart();
            return;
        }
        OptionSelect();
        CheckLimit();
    }
    private async void ShowTypeStart()
    {
        _isStarted = true;
        _checkType = false;
        SpawnText("<color=#0BFF7D>Started....</color>");
        await Task.Delay(100);
        SpawnText("Please select the option you wish to execute.");
        await Task.Delay(100);
        SpawnText("1. Characters");
        await Task.Delay(100);
        SpawnText("2. Item");
        await Task.Delay(100);
        SpawnText("3. Transform Position");
        await Task.Delay(100);
        SpawnText("4. PlayerPrefs");
        await Task.Delay(100);
        scrollView.Scroll();
    }
    
    // Selected Option Handle
    private void OptionSelect()
    {
        if (!int.TryParse(inputField.text, out var result) && !_checkType)
        {
            SpawnText($"The syntax is invalid. Error option: <color=#FF3434>{inputField.text}</color>");
            return;
        }
        
        if (_checkType) return;
        _checkType = true;
        switch (result)
        {
            case 1:
                _characterHandleCoroutine ??= StartCoroutine(PlayerConfigCoroutine());
                return;
                
            case 2:
                _itemHandleCoroutine ??= StartCoroutine(ItemCoroutine());
                return;
            
            case 3:
                _positionHandleCoroutine ??= StartCoroutine(PositionHandleCoroutine());
                return;
            
            case 4:
                _playerPrefsCoroutine ??= StartCoroutine(PlayerPrefsCoroutine());
                return;
            default:
                SpawnText($"Option not found. Error option: <color=#FF3434>{inputField.text}</color>");
                _checkType = false;
                scrollView.Scroll();
                return;
        }
    }

    private IEnumerator PlayerConfigCoroutine()
    {
        GETPlayerConfig();
        while (true)
        {
            if (_enter && inputField.text.Length != 0)
            {
                if (CheckPattern(inputField.text, _pattern_RETURN))
                {
                    SpawnText($"{inputField.text} <color=#0BFF7D>Return</color>");
                    ShowTypeStart();
                    ResetAllTypeCoroutine();
                    yield break;
                }
                switch (inputField.text)
                {
                    case "a" or "A":
                        GETPlayerConfig();
                        break;
                    case "b" or "B":
                        SpawnText($"Please enter in the following format: [KEY:x,VAL:x] to SET.");
                        yield return _wait_100ms;
                        scrollView.Scroll();
                        while (true)
                        {
                            if (_enter)
                            {
                                if (Regex.IsMatch(inputField.text, _pattern_SET_ITEM, RegexOptions.IgnoreCase))
                                {
                                    var _match = Regex.Match(inputField.text, _pattern_SET_ITEM, RegexOptions.IgnoreCase);
                                    var x = Convert.ToInt32(_match.Groups[1].Value);
                                    var y = Convert.ToInt32(_match.Groups[2].Value);
                                    SpawnText("<color=#0BFF7D>Wait...</color>");
                                    yield return _wait_500ms;
                                    if (x is < 1 or > 11)
                                    {
                                        SpawnText($"Index not found. Error code: [KEY:<color=#FF3434>{x}</color>,VAL:{y}]");
                                        yield return _wait_100ms;
                                        scrollView.Scroll();
                                        continue;
                                    }
                                    SETValuePlayerConfig(x, y);
                                    yield return _wait_500ms;
                                    SpawnText("<color=#0BFF7D>SET Data Success...</color>");
                                    GUI_Manager.UpdateGUIData();
                                    SpawnText($"{inputField.text} <color=#0BFF7D>Return</color>");
                                    OperationMethod();
                                    yield return _wait_100ms;
                                    scrollView.Scroll();
                                    break;
                                }
                                SpawnText($"<color=#FF3434>The syntax is invalid!!!</color>");
                                SpawnText($"<color=#0BFF7D>Recommended syntax: [KEY:x,VAL:x]</color>");
                                yield return _wait_100ms;
                                SpawnText($"{inputField.text} <color=#0BFF7D>Return</color>");
                                OperationMethod();
                                yield return _wait_100ms;
                                scrollView.Scroll();
                                break;
                            }
                            yield return _waitNull;
                        }
                        break;
                    case "c" or "C":
                        SpawnText($"Please enter in the following format: [KEY:x,VAL:x] to ADD.");
                        yield return _wait_100ms;
                        scrollView.Scroll();
                        while (true)
                        {
                            if (_enter)
                            {
                                if (Regex.IsMatch(inputField.text, _pattern_SET_ITEM, RegexOptions.IgnoreCase))
                                {
                                    var _match = Regex.Match(inputField.text, _pattern_SET_ITEM, RegexOptions.IgnoreCase);
                                    var x = Convert.ToInt32(_match.Groups[1].Value);
                                    var y = Convert.ToInt32(_match.Groups[2].Value);
                                    SpawnText("<color=#0BFF7D>Wait...</color>");
                                    yield return _wait_500ms;
                                    if (x is < 1 or > 11)
                                    {
                                        SpawnText($"Index not found. Error code: [KEY:<color=#FF3434>{x}</color>,VAL:{y}]");
                                        yield return _wait_100ms;
                                        scrollView.Scroll();
                                        continue;
                                    }
                                    ADDValuePlayerConfig(x, y);
                                    yield return _wait_500ms;
                                    SpawnText("<color=#0BFF7D>SET Data Success...</color>");
                                    GUI_Manager.UpdateGUIData();
                                    SpawnText($"{inputField.text} <color=#0BFF7D>Return</color>");
                                    OperationMethod();
                                    yield return _wait_100ms;
                                    scrollView.Scroll();
                                    break;
                                }
                                SpawnText($"<color=#FF3434>The syntax is invalid!!!</color>");
                                SpawnText($"<color=#0BFF7D>Recommended syntax: [KEY:x,VAL:x]</color>");
                                yield return _wait_100ms;
                                SpawnText($"{inputField.text} <color=#0BFF7D>Return</color>");
                                OperationMethod();
                                yield return _wait_100ms;
                                scrollView.Scroll();
                                break;
                            }
                            yield return _waitNull;
                        }
                        break;
                    case "d" or "D":
                        SpawnText($"Please enter in the following format: [KEY:x,VAL:x] to SUBTRACT.");
                        yield return _wait_100ms;
                        scrollView.Scroll();
                        while (true)
                        {
                            if (_enter)
                            {
                                if (Regex.IsMatch(inputField.text, _pattern_SET_ITEM, RegexOptions.IgnoreCase))
                                {
                                    var _match = Regex.Match(inputField.text, _pattern_SET_ITEM, RegexOptions.IgnoreCase);
                                    var x = Convert.ToInt32(_match.Groups[1].Value);
                                    var y = Convert.ToInt32(_match.Groups[2].Value);
                                    SpawnText("<color=#0BFF7D>Wait...</color>");
                                    yield return _wait_500ms;
                                    if (x is < 1 or > 11)
                                    {
                                        SpawnText($"Index not found. Error code: [KEY:<color=#FF3434>{x}</color>,VAL:{y}]");
                                        yield return _wait_100ms;
                                        scrollView.Scroll();
                                        continue;
                                    }
                                    SUBTRACTValuePlayerConfig(x, y);
                                    yield return _wait_500ms;
                                    SpawnText("<color=#0BFF7D>SET Data Success...</color>");
                                    GUI_Manager.UpdateGUIData();
                                    SpawnText($"{inputField.text} <color=#0BFF7D>Return</color>");
                                    OperationMethod();
                                    yield return _wait_100ms;
                                    scrollView.Scroll();
                                    break;
                                }
                                SpawnText($"<color=#FF3434>The syntax is invalid!!!</color>");
                                SpawnText($"<color=#0BFF7D>Recommended syntax: [KEY:x,VAL:x]</color>");
                                yield return _wait_100ms;
                                SpawnText($"{inputField.text} <color=#0BFF7D>Return</color>");
                                OperationMethod();
                                yield return _wait_100ms;
                                scrollView.Scroll();
                                break;
                            }                            
                            yield return _waitNull;
                        }
                        break;
                    case "e" or "E":
                        SpawnText($"Please enter in the following format: [KEY:x,VAL:x] to MULTIPLY.");
                        yield return _wait_100ms;
                        scrollView.Scroll();
                        while (true)
                        {
                            if (_enter)
                            {
                                if (Regex.IsMatch(inputField.text, _pattern_SET_ITEM, RegexOptions.IgnoreCase))
                                {
                                    var _match = Regex.Match(inputField.text, _pattern_SET_ITEM, RegexOptions.IgnoreCase);
                                    var x = Convert.ToInt32(_match.Groups[1].Value);
                                    var y = Convert.ToInt32(_match.Groups[2].Value);
                                    SpawnText("<color=#0BFF7D>Wait...</color>");
                                    yield return _wait_500ms;
                                    if (x is < 1 or > 11)
                                    {
                                        SpawnText($"Index not found. Error code: [KEY:<color=#FF3434>{x}</color>,VAL:{y}]");
                                        yield return _wait_100ms;
                                        scrollView.Scroll();
                                        continue;
                                    }
                                    MULTIPLYValuePlayerConfig(x, y);
                                    yield return _wait_500ms;
                                    SpawnText("<color=#0BFF7D>SET Data Success...</color>");
                                    GUI_Manager.UpdateGUIData();
                                    SpawnText($"{inputField.text} <color=#0BFF7D>Return</color>");
                                    OperationMethod();
                                    yield return _wait_100ms;
                                    scrollView.Scroll();
                                    break;
                                }
                                SpawnText($"<color=#FF3434>The syntax is invalid!!!</color>");
                                SpawnText($"<color=#0BFF7D>Recommended syntax: [KEY:x,VAL:x]</color>");
                                yield return _wait_100ms;
                                SpawnText($"{inputField.text} <color=#0BFF7D>Return</color>");
                                OperationMethod();
                                yield return _wait_100ms;
                                scrollView.Scroll();
                                break;
                            }
                            yield return _waitNull;
                        }
                        break;
                    default:
                        SpawnText($"Error option: <color=#FF3434>{inputField.text}</color>"); 
                        break;
                }
            }
            yield return _waitNull;
        }
    }
    private IEnumerator ItemCoroutine()
    {
        SpawnText("<color=#0BFF7D>--------- ITEM ---------</color>");
        yield return _wait_100ms;
        SpawnText("Would you like to get information or set information?");
        yield return _wait_100ms;
        SpawnText("1. GETTER");
        SpawnText("2. SETTER");
        yield return _wait_100ms;
        scrollView.Scroll();
        while (true)
        {
            if (_enter && inputField.text.Length != 0)
            {
                if (CheckPattern(inputField.text, _pattern_RETURN))
                {
                    ShowTypeStart();
                    ResetAllTypeCoroutine();
                    yield break;
                }
                if (!int.TryParse(inputField.text, out var _result))
                {
                    SpawnText($"The syntax is invalid. Error option: <color=#FF3434>{inputField.text}</color>");
                    yield return _wait_100ms;
                    scrollView.Scroll();
                    continue;
                }
                switch (_result)
                {
                    case 1:
                        SpawnText("<color=#2F6CFF>NAMECODE");
                        var _count = 0;
                        foreach (var itemCode in _itemCodes)
                        {
                            SpawnText($"{_count}. {itemCode}");
                            yield return _wait_100ms;
                            _count++;
                        }
                        SpawnText("<color=#0BFF7D>--------- ITEM ---------</color>");
                        SpawnText("1. GETTER");
                        SpawnText("2. SETTER");
                        yield return _wait_100ms;
                        scrollView.Scroll();
                        continue;
                        
                    case 2:
                        SpawnText($"Please enter in the following format: [KEY:x,VAL:x] to SET.");
                        yield return _wait_100ms;
                        scrollView.Scroll();
                        while (true)
                        {
                            if (_enter)
                            {
                                if (CheckPattern(inputField.text, _pattern_RETURN))
                                {
                                    SpawnText($"{inputField.text} <color=#0BFF7D>Return</color>");
                                    SpawnText("1. GETTER");
                                    SpawnText("2. SETTER");
                                    break;
                                }
                                
                                if (Regex.IsMatch(inputField.text, _pattern_SET_ITEM, RegexOptions.IgnoreCase))
                                {
                                    var _match = Regex.Match(inputField.text, _pattern_SET_ITEM, RegexOptions.IgnoreCase);
                                    var x = Convert.ToInt32(_match.Groups[1].Value);
                                    var y = Convert.ToInt32(_match.Groups[2].Value);
                                    
                                    SpawnText("<color=#0BFF7D>Wait...</color>");
                                    yield return _wait_500ms;
                                    if (x >= _itemCodes.Length)
                                    {
                                        SpawnText($"Item not found. Error code: [KEY:<color=#FF3434>{x}</color>,VAL:{y}]");
                                        yield return _wait_100ms;
                                        scrollView.Scroll();
                                    }
                                    else
                                    {
                                        SpawnText($"NAMECODE: {_itemCodes[x]}");
                                        SpawnText($"VALUE: {y}");
                                        if (_itemCodes[x] == ItemNameCode.COCoin) 
                                            _userData.IncreaseCoin(y);
                                        else 
                                            _userData.IncreaseItemValue(_itemCodes[x], y);
                                        yield return _wait_500ms;
                                        SpawnText("<color=#0BFF7D>SET Data Success...</color>");
                                        yield return _wait_100ms;
                                        scrollView.Scroll();
                                    }
                                    yield return _wait_100ms;
                                    SpawnText($"{inputField.text} <color=#0BFF7D>Return</color>");
                                    SpawnText("1. GETTER");
                                    SpawnText("2. SETTER");
                                    yield return _wait_100ms;
                                    scrollView.Scroll();
                                    break;
                                }
                                SpawnText($"<color=#FF3434>The syntax is invalid!!!</color>");
                                SpawnText($"<color=#0BFF7D>Recommended syntax: [KEY:x,VAL:x]</color>");
                                yield return _wait_100ms;
                                scrollView.Scroll();
                            }
                            yield return _waitNull;
                        }
                        continue;
                    
                    default:
                        SpawnText($"Error option: <color=#FF3434>{inputField.text}</color>"); 
                        continue;
                }
            }
            yield return _waitNull;
        }
    }
    private IEnumerator PositionHandleCoroutine()
    {
        SpawnText("<color=#0BFF7D>--------- POSITION ---------</color>");
        yield return _wait_100ms;
        SpawnText("Would you like to get information or set information?");
        yield return _wait_100ms;
        SpawnText("1. GETTER");
        SpawnText("2. SETTER");
        yield return _wait_100ms;
        scrollView.Scroll();
        while (true)
        {
            if (_enter && inputField.text.Length != 0)
            {
                if (CheckPattern(inputField.text, _pattern_RETURN))
                {
                    ShowTypeStart();
                    ResetAllTypeCoroutine();
                    yield break;
                }
                
                if (!int.TryParse(inputField.text, out var _result))
                {
                    SpawnText($"The syntax is invalid. Error option: <color=#FF3434>{inputField.text}</color>");
                    continue;
                }

                switch (_result)
                {
                    case 1:
                        SpawnText($"Current Position: {_player.transform.position.ToString("F1")}");
                        SpawnText($"/b <color=#0BFF7D>Return</color>");
                        yield return _wait_100ms;
                        SpawnText("Would you like to get information or set information?");
                        yield return _wait_100ms;
                        SpawnText("1. GETTER");
                        SpawnText("2. SETTER");
                        yield return _wait_100ms;
                        scrollView.Scroll();
                        break;
                    case 2:
                        SpawnText("Choose an available position");
                        yield return _wait_100ms;
                        for (var i = 0; i < _waypoints.Length; i++)
                        {
                            SpawnText($"Point {i + 1}: [x:{_waypoints[i].x}, y:{_waypoints[i].y}, z:{_waypoints[i].z}]");
                            yield return _wait_100ms;
                        }
                        yield return _wait_100ms;
                        SpawnText("Please select a position.");
                        while (true)
                        {
                            if (_enter && inputField.text.Length != 0)
                            {
                                if (CheckPattern(inputField.text, _pattern_RETURN))
                                {
                                    SpawnText($"{inputField.text} <color=#0BFF7D>Return</color>");
                                    yield return _wait_100ms;
                                    SpawnText("Would you like to get information or set information?");
                                    yield return _wait_100ms;
                                    SpawnText("1. GETTER");
                                    SpawnText("2. SETTER");
                                    yield return _wait_100ms;
                                    scrollView.Scroll();
                                    break;
                                }
                                
                                if (!int.TryParse(inputField.text, out var result))
                                {
                                    SpawnText($"The syntax is invalid. Error option: <color=#FF3434>{inputField.text}</color>");
                                    continue;
                                } 
                                var _check = result is >= 1 and <= 4;
                                if (_check)
                                {
                                    Time.timeScale = 1;
                                    LoadingPanel.Instance.Active(1.85f);
                                    var _target = _waypoints[result - 1];
                                    SpawnText($"Position SET {_target}");
                                    SpawnText("<color=#0BFF7D>SET Position Success...</color>");
                                    panel.SetActive(false);
                                    _player.input.PlayerInput.Disable();
                                    yield return new WaitForSecondsRealtime(2.15f);
                                    _player.characterController.enabled = false;  
                                    _player.transform.position = _target;
                                    _player.characterController.enabled = true;  
                                    _player.input.PlayerInput.Enable();
                                    _enter = false;
                                    _isOpen = false;
                                    ClosePanel();
                                    break;
                                }
                                SpawnText($"Error option: <color=#FF3434>{inputField.text}</color>");
                            }
                            yield return _waitNull;
                        }
                        break;
                    default:
                        SpawnText($"Error option: <color=#FF3434>{inputField.text}</color>"); 
                        yield return _wait_100ms;
                        scrollView.Scroll();
                        break;
                }
            }
            yield return _waitNull;
        }
    }
    private IEnumerator PlayerPrefsCoroutine()
    {
        yield return _wait_100ms;
        SpawnText($"<color=#F377FF>Delete All PlayerPrefs Key.</color>");
        yield return _wait_100ms;
        SpawnText($"The application will restart when you delete all keys in PlayerPrefs. Press <color=#0BFF7D>Y</color> to continue and <color=#FF3434>N</color> to exit.");
        yield return _wait_100ms;
        scrollView.Scroll();
        
        while (true)
        {
            if (_enter && inputField.text.Length != 0)
            {
                if (CheckPattern(inputField.text, _pattern_RETURN))
                {
                    SpawnText($"{inputField.text} <color=#0BFF7D>Return</color>");
                    ShowTypeStart();
                    ResetAllTypeCoroutine();
                    yield break;
                }

                switch (inputField.text)
                {
                    case "y" or "Y":
#if !UNITY_EDITOR
                        PlayerPrefs.DeleteAll();
                        Application.Quit();
                        yield break;
#endif
                        break;
                    
                    case "n" or "N":
                        SpawnText($"<color=#0BFF7D>Return</color>");
                        ShowTypeStart();
                        ResetAllTypeCoroutine();
                        break;
                    
                    default:
                        SpawnText($"Error option: <color=#FF3434>{inputField.text}</color>"); 
                        break;
                }
            }
            yield return _waitNull;
        }
    }
    private async void GETPlayerConfig()
    {
        SpawnText($"<color=#2F6CFF>===== PLAYER STATS =====</color>");
        await Task.Delay(100);
        SpawnText($"1. HP: {_player.PlayerConfig.GetHP()}");
        await Task.Delay(100);
        SpawnText($"2. ST: {_player.PlayerConfig.GetST()}");
        await Task.Delay(100);
        SpawnText($"3. ATK: {_player.PlayerConfig.GetATK()}");
        await Task.Delay(100);
        SpawnText($"4. DEF: {_player.PlayerConfig.GetDEF()}");
        await Task.Delay(100);
        SpawnText($"5. CRIT Rate: {_player.PlayerConfig.GetCRITRate()}%");
        await Task.Delay(100);
        SpawnText($"6. CRIT DMG: {_player.PlayerConfig.GetCRITDMG()}%");
        await Task.Delay(100);
        SpawnText($"7. WALKSPEED: {_player.PlayerConfig.GetWalkSpeed()}");
        await Task.Delay(100);
        SpawnText($"8. RUNSPEED: {_player.PlayerConfig.GetRunSpeed()}");
        await Task.Delay(100);
        SpawnText($"9. RUNFASTSPEED: {_player.PlayerConfig.GetRunFastSpeed()}");
        await Task.Delay(100);
        SpawnText($"10. ELEMENTAL SKILL CD: {_player.PlayerConfig.GetElementalSkillCD()}s");
        await Task.Delay(100);
        SpawnText($"11. ELEMENTAL BURST CD: {_player.PlayerConfig.GetElementalBurstCD()}s");
        await Task.Delay(100);
        SpawnText("Which action would you like to perform?");
        await Task.Delay(100);
        OperationMethod();
        await Task.Delay(150);
        scrollView.Scroll();
    }
    private void OperationMethod()
    {
        SpawnText("<color=#FF7D00>a. GET</color>");
        SpawnText("<color=#FF7D00>b. SET</color>");
        SpawnText("<color=#FF7D00>c. ADD</color>");
        SpawnText("<color=#FF7D00>d. SUBTRACT</color>");
        SpawnText("<color=#FF7D00>e. MULTIPLY</color>");
        scrollView.Scroll();
    }
    private void ADDValuePlayerConfig(int _index, float _value)
    {
        SpawnText($"INDEX SET: {_index}");
        switch (_index)
        {
            case 1:
                var _hp = _player.PlayerConfig.GetHP();
                _player.PlayerConfig.SetHP(_hp + (int)_value);
                _hp = _player.PlayerConfig.GetHP();
                _player.Health.InitValue(_hp, _hp);
                SpawnText($"VALUE SETTO: {_player.PlayerConfig.GetHP()}");
                break;
            case 2:
                var _st = _player.PlayerConfig.GetST();
                _player.PlayerConfig.SetST(_st + (int)_value);
                _st = _player.PlayerConfig.GetST();
                _player.Stamina.InitValue(_st, _st);
                SpawnText($"VALUE SETTO: {_player.PlayerConfig.GetST()}");
                break;
            case 3:
                _player.PlayerConfig.SetATK(_player.PlayerConfig.GetATK() + (int)_value);
                SpawnText($"VALUE SETTO: {_player.PlayerConfig.GetATK()}");
                break;
            case 4:
                _player.PlayerConfig.SetDEF(_player.PlayerConfig.GetDEF() + (int)_value);
                SpawnText($"VALUE SETTO: {_player.PlayerConfig.GetDEF()}");
                break;
            case 5:
                _player.PlayerConfig.SetCRITRate(_player.PlayerConfig.GetCRITRate() + _value);
                SpawnText($"VALUE SETTO: {_player.PlayerConfig.GetCRITRate()}");
                break;
            case 6:
                _player.PlayerConfig.SetCRITDMG(_player.PlayerConfig.GetCRITDMG() + (int)_value);
                SpawnText($"VALUE SETTO: {_player.PlayerConfig.GetCRITDMG()}");
                break;
            case 7:
                _player.PlayerConfig.SetWalkSpeed(_player.PlayerConfig.GetWalkSpeed() + _value);
                SpawnText($"VALUE SETTO: {_player.PlayerConfig.GetWalkSpeed()}");
                break;
            case 8:
                _player.PlayerConfig.SetRunSpeed(_player.PlayerConfig.GetRunSpeed() + _value);
                SpawnText($"VALUE SETTO: {_player.PlayerConfig.GetRunSpeed()}");
                break;
            case 9:
                _player.PlayerConfig.SetRunFastSpeed(_player.PlayerConfig.GetRunFastSpeed() + _value);
                SpawnText($"VALUE SETTO: {_player.PlayerConfig.GetRunFastSpeed()}");
                break;
            case 10:
                _player.PlayerConfig.SetElementalSkillCD(_player.PlayerConfig.GetElementalSkillCD() + _value);
                SpawnText($"VALUE SETTO: {_player.PlayerConfig.GetElementalSkillCD()}");
                break;
            case 11:
                _player.PlayerConfig.SetElementalBurstCD(_player.PlayerConfig.GetElementalBurstCD() + _value);
                SpawnText($"VALUE SETTO: {_player.PlayerConfig.GetElementalBurstCD()}");
                break;
        }
    }
    private void SETValuePlayerConfig(int _index, float _value)
    {
        SpawnText($"INDEX SET: {_index}");
        switch (_index)
        {
            case 1:
                _player.PlayerConfig.SetHP((int)_value);
                SpawnText($"VALUE SETTO: {_player.PlayerConfig.GetHP()}");
                var _hp = _player.PlayerConfig.GetHP();
                _player.Health.InitValue(_hp, _hp);
                break;
            case 2:
                _player.PlayerConfig.SetST((int)_value);
                SpawnText($"VALUE SETTO: {_player.PlayerConfig.GetST()}");
                var _st = _player.PlayerConfig.GetST();
                _player.Stamina.InitValue(_st, _st);
                break;
            case 3:
                _player.PlayerConfig.SetATK((int)_value);
                SpawnText($"VALUE SETTO: {_player.PlayerConfig.GetATK()}");
                break;
            case 4:
                _player.PlayerConfig.SetDEF((int)_value);
                SpawnText($"VALUE SETTO: {_player.PlayerConfig.GetDEF()}");
                break;
            case 5:
                _player.PlayerConfig.SetCRITRate(_value);
                SpawnText($"VALUE SETTO: {_player.PlayerConfig.GetCRITRate()}");
                break;
            case 6:
                _player.PlayerConfig.SetCRITDMG((int)_value);
                SpawnText($"VALUE SETTO: {_player.PlayerConfig.GetCRITDMG()}");
                break;
            case 7:
                _player.PlayerConfig.SetWalkSpeed(_value);
                SpawnText($"VALUE SETTO: {_player.PlayerConfig.GetWalkSpeed()}");
                break;
            case 8:
                _player.PlayerConfig.SetRunSpeed(_value);
                SpawnText($"VALUE SETTO: {_player.PlayerConfig.GetRunSpeed()}");
                break;
            case 9:
                _player.PlayerConfig.SetRunFastSpeed(_value);
                SpawnText($"VALUE SETTO: {_player.PlayerConfig.GetRunFastSpeed()}");
                break;
            case 10:
                _player.PlayerConfig.SetElementalSkillCD(_value);
                SpawnText($"VALUE SETTO: {_player.PlayerConfig.GetElementalSkillCD()}");
                break;
            case 11:
                _player.PlayerConfig.SetElementalBurstCD(_value);
                SpawnText($"VALUE SETTO: {_player.PlayerConfig.GetElementalBurstCD()}");
                break;
        }
    }
    private void SUBTRACTValuePlayerConfig(int _index, float _value)
    {
        SpawnText($"INDEX SET: {_index}");
        switch (_index)
        {
            case 1:
                var _hp = _player.PlayerConfig.GetHP();
                _player.PlayerConfig.SetHP(_hp - (int)_value);
                _hp = _player.PlayerConfig.GetHP();
                _player.Health.InitValue(_hp, _hp);
                SpawnText($"VALUE SETTO: {_player.PlayerConfig.GetHP()}");
                
                break;
            case 2:
                var _st = _player.PlayerConfig.GetST();
                _player.PlayerConfig.SetST(_st - (int)_value);
                _st = _player.PlayerConfig.GetST();
                _player.Stamina.InitValue(_st, _st);
                SpawnText($"VALUE SETTO: {_player.PlayerConfig.GetST()}");
                break;
            case 3:
                _player.PlayerConfig.SetATK(_player.PlayerConfig.GetATK() - (int)_value);
                SpawnText($"VALUE SETTO: {_player.PlayerConfig.GetATK()}");
                break;
            case 4:
                _player.PlayerConfig.SetDEF(_player.PlayerConfig.GetDEF() - (int)_value);
                SpawnText($"VALUE SETTO: {_player.PlayerConfig.GetDEF()}");
                break;
            case 5:
                _player.PlayerConfig.SetCRITRate(_player.PlayerConfig.GetCRITRate() - _value);
                SpawnText($"VALUE SETTO: {_player.PlayerConfig.GetCRITRate()}");
                break;
            case 6:
                _player.PlayerConfig.SetCRITDMG(_player.PlayerConfig.GetCRITDMG() - (int)_value);
                SpawnText($"VALUE SETTO: {_player.PlayerConfig.GetCRITDMG()}");
                break;
            case 7:
                _player.PlayerConfig.SetWalkSpeed(_player.PlayerConfig.GetWalkSpeed() - _value);
                SpawnText($"VALUE SETTO: {_player.PlayerConfig.GetWalkSpeed()}");
                break;
            case 8:
                _player.PlayerConfig.SetRunSpeed(_player.PlayerConfig.GetRunSpeed() - _value);
                SpawnText($"VALUE SETTO: {_player.PlayerConfig.GetRunSpeed()}");
                break;
            case 9:
                _player.PlayerConfig.SetRunFastSpeed(_player.PlayerConfig.GetRunFastSpeed() - _value);
                SpawnText($"VALUE SETTO: {_player.PlayerConfig.GetRunFastSpeed()}");
                break;
            case 10:
                _player.PlayerConfig.SetElementalSkillCD(_player.PlayerConfig.GetElementalSkillCD() - _value);
                SpawnText($"VALUE SETTO: {_player.PlayerConfig.GetElementalSkillCD()}");
                break;
            case 11:
                _player.PlayerConfig.SetElementalBurstCD(_player.PlayerConfig.GetElementalBurstCD() - _value);
                SpawnText($"VALUE SETTO: {_player.PlayerConfig.GetElementalBurstCD()}");
                break;
        }
    }
    private void MULTIPLYValuePlayerConfig(int _index, float _value)
    {
        SpawnText($"INDEX SET: {_index}");
        switch (_index)
        {
            case 1:
                var _hp = _player.PlayerConfig.GetHP();
                _player.PlayerConfig.SetHP(_hp * (int)_value);
                _hp = _player.PlayerConfig.GetHP();
                _player.Health.InitValue(_hp, _hp);
                SpawnText($"VALUE SETTO: {_player.PlayerConfig.GetHP()}");
                break;
            case 2:
                var _st = _player.PlayerConfig.GetST();
                _player.PlayerConfig.SetST(_st * (int)_value);
                _st = _player.PlayerConfig.GetST();
                _player.Stamina.InitValue(_st, _st);
                SpawnText($"VALUE SETTO: {_player.PlayerConfig.GetST()}");
                break;
            case 3:
                _player.PlayerConfig.SetATK(_player.PlayerConfig.GetATK() * (int)_value);
                SpawnText($"VALUE SETTO: {_player.PlayerConfig.GetATK()}");
                break;
            case 4:
                _player.PlayerConfig.SetDEF(_player.PlayerConfig.GetDEF() * (int)_value);
                SpawnText($"VALUE SETTO: {_player.PlayerConfig.GetDEF()}");
                break;
            case 5:
                _player.PlayerConfig.SetCRITRate(_player.PlayerConfig.GetCRITRate() * _value);
                SpawnText($"VALUE SETTO: {_player.PlayerConfig.GetCRITRate()}");
                break;
            case 6:
                _player.PlayerConfig.SetCRITDMG(_player.PlayerConfig.GetCRITDMG() * (int)_value);
                SpawnText($"VALUE SETTO: {_player.PlayerConfig.GetCRITDMG()}");
                break;
            case 7:
                _player.PlayerConfig.SetWalkSpeed(_player.PlayerConfig.GetWalkSpeed() * _value);
                SpawnText($"VALUE SETTO: {_player.PlayerConfig.GetWalkSpeed()}");
                break;
            case 8:
                _player.PlayerConfig.SetRunSpeed(_player.PlayerConfig.GetRunSpeed() * _value);
                SpawnText($"VALUE SETTO: {_player.PlayerConfig.GetRunSpeed()}");
                break;
            case 9:
                _player.PlayerConfig.SetRunFastSpeed(_player.PlayerConfig.GetRunFastSpeed() * _value);
                SpawnText($"VALUE SETTO: {_player.PlayerConfig.GetRunFastSpeed()}");
                break;
            case 10:
                _player.PlayerConfig.SetElementalSkillCD(_player.PlayerConfig.GetElementalSkillCD() * _value);
                SpawnText($"VALUE SETTO: {_player.PlayerConfig.GetElementalSkillCD()}");
                break;
            case 11:
                _player.PlayerConfig.SetElementalBurstCD(_player.PlayerConfig.GetElementalBurstCD() * _value);
                SpawnText($"VALUE SETTO: {_player.PlayerConfig.GetElementalBurstCD()}");
                break;
        }
    }
    private void ResetAllTypeCoroutine()
    {
        if (_itemHandleCoroutine != null) StopCoroutine(_itemHandleCoroutine);
        _itemHandleCoroutine = null;
        if (_characterHandleCoroutine != null) StopCoroutine(_characterHandleCoroutine);
        _characterHandleCoroutine = null;
        if (_positionHandleCoroutine != null) StopCoroutine(_positionHandleCoroutine);
        _positionHandleCoroutine = null;
        if (_playerPrefsCoroutine != null) StopCoroutine(_playerPrefsCoroutine);
        _playerPrefsCoroutine = null;
    }

 
    private void ReleaseTextBox()
    {
        inputField.text = "";
        for (var i = 0; i < textContent.childCount; i++)
        {
            Destroy(textContent.GetChild(i).gameObject);
        }
    }
    private void SpawnText(string _value)
    {
        var _textBar = Instantiate(textBarPrefab, textContent);
        _textBar.SetTitleText(FormatTitle(DateTime.Now));
        _textBar.SetValueText(_value);
        inputField.text = "";
        _enter = false;
        scrollView.Scroll();
    }
    private void CheckLimit()
    {
        var _count = textContent.childCount;
        if (_count <= _maxLimit) return;
        for (var i = 0; i < _count - _maxLimit; i++)
        {
            Destroy(textContent.GetChild(i).gameObject);
        }
    }
    private static bool CheckPattern(string _valueCheck, string _pattern) => string.Equals(_valueCheck, _pattern, StringComparison.OrdinalIgnoreCase);
    private static string FormatTitle(DateTime _time)
    {
        var _hour = _time.Hour;
        var _minute = _time.Minute;
        var _second = _time.Second;
        var _day = _time.Day;
        var _month = _time.Month;
        return "[" + _hour.ToString("00") + ":" + _minute.ToString("00") + ":" + _second.ToString("00") + $" {_day}/{_month}]:";
    }
}
