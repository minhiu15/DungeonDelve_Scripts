using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GUI_AccountManager : MonoBehaviour
{
    [SerializeField] private Button logoutBtt;
    [SerializeField] private Button accountBtt;
    [Space] 
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject panelAnimatedLoading;
    [SerializeField] private LoadSceneButton startGameBtt;
    [Space]
    [SerializeField] private GUI_Login guiLogin;
    [SerializeField] private GUI_Register guiRegister;
    [SerializeField] private GUI_ForgotPW guiForgotPw;
    [Space] 
    [SerializeField] private RectTransform noticeFrame;
    [SerializeField] private TextMeshProUGUI noticeText;
    [Space] 
    [SerializeField] private TextMeshProUGUI accountIDText;
    [SerializeField] private TextMeshProUGUI mailText;
    
    private Coroutine _handleCoroutine;
    private Coroutine _noticeCoroutine;

    
    private void OnEnable()
    {
        Init();
        RegisterEvent();
    }
    private void OnDisable()
    {
        UnRegisterEvent();
    }

    private void Init()
    {
        accountIDText.text = "";
        mailText.text = "";
        panelAnimatedLoading.SetActive(true);
        accountBtt.gameObject.SetActive(true);
        logoutBtt.gameObject.SetActive(false);
        startGameBtt.gameObject.SetActive(false);
    }
    private void RegisterEvent()
    {
        logoutBtt.onClick.AddListener(LogoutAccount);
        accountBtt.onClick.AddListener(OpenPanelLogin);
        
        if(PlayFabHandleUserData.Instance)
        {
            #region Loading Data
            PlayFabHandleUserData.Instance.OnLoadUserDataSuccessEvent.AddListener(OnLoadUserDataSuccess);
            PlayFabHandleUserData.Instance.OnLoadUserDataFailureEvent.AddListener(OnLoadUserDataFailure);
            #endregion
        }
        
        if(!PlayFabController.Instance) return;
        #region Login
        guiLogin.startgameBtt.onClick.AddListener(PlayFabController.Instance.OnLogin);
        guiLogin.emailField.onValueChanged.AddListener(PlayFabController.Instance.SetUserEmail);
        guiLogin.passwordField.onValueChanged.AddListener(PlayFabController.Instance.SetUserPassword);      
        PlayFabController.Instance.OnLoginSuccessEvent += HandleLoginSuccess;
        PlayFabController.Instance.OnLoginFailureEvent += HandleLoginFailure;
        PlayFabController.Instance.OnLoginFailureEvent += guiLogin.SetErrorText;
        #endregion

        #region Register
        guiRegister.registerBtt.onClick.AddListener(PlayFabController.Instance.OnRegister);
        guiRegister.usernameField.onValueChanged.AddListener(PlayFabController.Instance.SetUserName);
        guiRegister.emailField.onValueChanged.AddListener(PlayFabController.Instance.SetUserEmail);
        guiRegister.passwordField.onValueChanged.AddListener(PlayFabController.Instance.SetUserPassword);
        PlayFabController.Instance.OnRegisterSuccessEvent += HandleRegisterSuccess;
        PlayFabController.Instance.OnRegisterFailureEvent += guiRegister.SetErrorText;
        #endregion

        #region Forgot password
        guiForgotPw.verificationBtt.onClick.AddListener(PlayFabController.Instance.OnForgotPW);
        guiForgotPw.emailField.onValueChanged.AddListener(PlayFabController.Instance.SetUserEmail);
        PlayFabController.Instance.OnMailSendForgotPWSuccessEvent += HandleForgotPWSuccess;
        PlayFabController.Instance.OnMailSendForgotPWFailureEvent += guiForgotPw.SetErrorText;
        #endregion

        PlayFabController.Instance.OnLogin();
    }
    private void UnRegisterEvent()
    {
        logoutBtt.onClick.RemoveListener(LogoutAccount);
        accountBtt.onClick.RemoveListener(OpenPanelLogin);
        
        if (PlayFabHandleUserData.Instance)
        {
            #region Loading Data
            PlayFabHandleUserData.Instance.OnLoadUserDataSuccessEvent.RemoveListener(OnLoadUserDataSuccess);
            PlayFabHandleUserData.Instance.OnLoadUserDataFailureEvent.RemoveListener(OnLoadUserDataFailure);
            #endregion
        }
        
        if(!PlayFabController.Instance) return;
        
        #region Login
        guiLogin.startgameBtt.onClick.RemoveListener(PlayFabController.Instance.OnLogin);
        guiLogin.emailField.onValueChanged.RemoveListener(PlayFabController.Instance.SetUserEmail);
        guiLogin.passwordField.onValueChanged.RemoveListener(PlayFabController.Instance.SetUserPassword);      
        PlayFabController.Instance.OnLoginSuccessEvent -= HandleLoginSuccess;
        PlayFabController.Instance.OnLoginFailureEvent -= HandleLoginFailure;
        PlayFabController.Instance.OnLoginFailureEvent -= guiLogin.SetErrorText;
        #endregion

        #region Register
        guiRegister.registerBtt.onClick.RemoveListener(PlayFabController.Instance.OnRegister);
        guiRegister.usernameField.onValueChanged.RemoveListener(PlayFabController.Instance.SetUserName);
        guiRegister.emailField.onValueChanged.RemoveListener(PlayFabController.Instance.SetUserEmail);
        guiRegister.passwordField.onValueChanged.RemoveListener(PlayFabController.Instance.SetUserPassword);
        PlayFabController.Instance.OnRegisterSuccessEvent -= HandleRegisterSuccess;
        PlayFabController.Instance.OnRegisterFailureEvent -= guiRegister.SetErrorText;
        #endregion

        #region Forgot password
        guiForgotPw.verificationBtt.onClick.RemoveListener(PlayFabController.Instance.OnForgotPW);
        guiForgotPw.emailField.onValueChanged.RemoveListener(PlayFabController.Instance.SetUserEmail);
        PlayFabController.Instance.OnMailSendForgotPWSuccessEvent -= HandleForgotPWSuccess;
        PlayFabController.Instance.OnMailSendForgotPWFailureEvent -= guiForgotPw.SetErrorText;
        #endregion
    }


    public void OpenPanelLogin()
    {
        animator.Play("Enable");
    }
    public void LogoutAccount()
    {
        ClearAccountTemp();
        accountIDText.text = "";
        mailText.text = "";
        accountBtt.gameObject.SetActive(true);
        logoutBtt.gameObject.SetActive(false);
        startGameBtt.gameObject.SetActive(false);
    }
    
    
    private void HandleLoginFailure(string _value)
    {
        panelAnimatedLoading.SetActive(false);
        OpenPanelLogin();
    }
    private void HandleLoginSuccess()
    {
        if(_handleCoroutine != null) StopCoroutine(_handleCoroutine);
        _handleCoroutine = StartCoroutine(LoginSuccessCoroutine());
    }
    private IEnumerator LoginSuccessCoroutine()
    {
        panelAnimatedLoading.SetActive(true);
        yield return new WaitForSeconds(Random.Range(1.25f, 1.85f));
        animator.Play("Disable");
        panelAnimatedLoading.SetActive(false);
        logoutBtt.gameObject.SetActive(true);
        accountBtt.gameObject.SetActive(false);
        startGameBtt.gameObject.SetActive(true);
        GetUserInfor();
        if (_noticeCoroutine != null) 
            StopCoroutine(_noticeCoroutine);
        _noticeCoroutine = StartCoroutine(ShowNoticeText("Login successful"));
    }
    private void HandleRegisterSuccess()
    {
        if(_handleCoroutine != null) StopCoroutine(_handleCoroutine);
        _handleCoroutine = StartCoroutine(RegisterSuccessCoroutine());
    }
    private IEnumerator RegisterSuccessCoroutine()
    {
        panelAnimatedLoading.SetActive(true);
        yield return new WaitForSeconds(Random.Range(0.85f, 1.1f));
        animator.Play("OpenLoginPanel");
        panelAnimatedLoading.SetActive(false);
        
        if (_noticeCoroutine != null) StopCoroutine(_noticeCoroutine);
        _noticeCoroutine = StartCoroutine(ShowNoticeText("Registration successful"));
    }
    private void HandleForgotPWSuccess()
    {
        if(_handleCoroutine != null) StopCoroutine(_handleCoroutine);
        _handleCoroutine = StartCoroutine(ForgotPWSuccessCoroutine());
    }
    private IEnumerator ForgotPWSuccessCoroutine()
    {
        panelAnimatedLoading.SetActive(true);
        yield return new WaitForSeconds(Random.Range(0.85f, 1.1f));
        animator.Play("OpenLoginPanel");
        panelAnimatedLoading.SetActive(false);
        
        if (_noticeCoroutine != null) StopCoroutine(_noticeCoroutine);
        _noticeCoroutine = StartCoroutine(ShowNoticeText("Password change request successful"));
    }
    
    private IEnumerator ShowNoticeText(string _text)
    {
        noticeFrame.anchoredPosition = new Vector2(0, 400f);
        noticeFrame.DOAnchorPosY(350f, .35f);
        noticeFrame.gameObject.SetActive(true);
        noticeText.text = _text;
        yield return new WaitForSeconds(3f);
        noticeFrame.gameObject.SetActive(false);
    }
    private void GetUserInfor()
    {
        if (!PlayFabController.Instance) 
            return;
        
        accountIDText.text = PlayFabController.Instance.userID;
        var _mail = PlayFabController.Instance.userEmail;
        var _mailTemp = $"{_mail[0]}{_mail[1]}";
        var _lastId = 0;
        for (var i = 2; i < _mail.Length; i++)
        {
            if (_mail[i] == '@')
            {
                _lastId = i;
                break;
            }
            _mailTemp += '*';
        }

        _mailTemp += _mail.Substring(_lastId);
        mailText.text = $"<color=#10C7FF>User</color> <color=#FFCD10>{_mailTemp}</color>";
    }

    
    /// <summary> Nếu có dữ liệu người chơi trên PlayFab -> Load scene gameplay. </summary>
    private void OnLoadUserDataSuccess() => startGameBtt.SetSceneName("DungeonScene");
    /// <summary> Nếu chưa có dữ liệu -> Load scene chọn nhân vật mới. </summary>
    private void OnLoadUserDataFailure() => startGameBtt.SetSceneName("NewSelectCharacterScene");
    
    public void ClearAccountTemp()
    {
        if(PlayFabController.Instance) PlayFabController.Instance.ClearAccountTemp();
    }
    
}
