using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(RewardSetup))]
public class Chest : MonoBehaviour
{
    [SerializeField, Required] private RewardSetup rewardSetup;
    [SerializeField, Tooltip("Có tạo rương khi bắt đầu game ?")]
    private bool isCreateStart;
    [Space]
    [SerializeField] private Animator chestAnimator;
    [SerializeField] private BoxCollider chestCollider;
    [SerializeField] private GameObject detection;
    [SerializeField] private ParticleSystem chestVFX;
    [SerializeField] private M_SetFloat setDissolve;
    
    public UnityEvent OnCreateChestEvent;
    public UnityEvent OnOpenChestEvent;
    public IconIndicator Indicator { get; set; }
    
    private static readonly int OpenChestID = Animator.StringToHash("OpenChest");
    private static readonly int ActiveChestID = Animator.StringToHash("ActiveChest");

    private Coroutine _activeCoroutine;
    private Coroutine _openCoroutine;
    private Coroutine _closeCoroutine;
    private NoticeManager _notice;
    [SerializeField, ReadOnly] private bool _detectPlayer; // Có trigger với Player ?
    [SerializeField, ReadOnly] private bool _canReceived;  // Có thể nhận thưởng ?
    
    
    private void OnEnable()
    {
        GUI_Inputs.InputAction.UI.CollectItem.performed += OnClickOpenChest;
        OnOpenChestEvent.AddListener(rewardSetup.SendRewardData);
    }
    private void Start()
    {
        chestCollider.enabled = false;
        chestVFX.gameObject.SetActive(false);
        detection.SetActive(false);
        _notice = NoticeManager.Instance;
        SetDissolve(0, 1, 0);
        
        if (isCreateStart)
        {
            CreateChest();
        }
    }
    private void OnDisable()
    {
        GUI_Inputs.InputAction.UI.CollectItem.performed -= OnClickOpenChest;
        OnOpenChestEvent.RemoveListener(rewardSetup.SendRewardData);
    }


    /// <summary>
    /// Khi nhấn (Input) để mở rương
    /// </summary>
    /// <param name="_context"></param>
    private void OnClickOpenChest(InputAction.CallbackContext _context)
    {
        if(!_detectPlayer || !_canReceived) return;
        _canReceived = false;
        OpenChest();
        CloseChest();
        _notice.CloseNoticeT3();
    }
    
    
    public void CreateChest()
    {
        if(_activeCoroutine != null) 
            StopCoroutine(_activeCoroutine);
        _activeCoroutine = StartCoroutine(ActiveChestCoroutine());
    }
    private IEnumerator ActiveChestCoroutine()
    {
        OnCreateChestEvent?.Invoke();
        detection.SetActive(true);
        _canReceived = true;
        chestCollider.enabled = true;
        chestAnimator.Rebind();
        SetDissolve(0, 1, 0);
        chestAnimator.SetTrigger(ActiveChestID);
        yield return new WaitForSeconds(.1f);
        SetDissolve(1, 0, 2f);
    }
    
    private void OpenChest()
    {
        if(_openCoroutine != null) 
            StopCoroutine(_openCoroutine);
        _openCoroutine = StartCoroutine(OpenChestCoroutine());
    }
    private IEnumerator OpenChestCoroutine()
    {
        CloseIndicator();
        chestAnimator.SetBool(OpenChestID, true);
        
        yield return new WaitForSeconds(.8f);
        AudioManager.PlayOneShot(FMOD_Events.Instance.chestOpen, transform.position);
        chestVFX.gameObject.SetActive(true);
        chestVFX.Play();
        OnOpenChestEvent?.Invoke();
    }
    
    private void CloseChest()
    {
        if(_closeCoroutine != null) 
            StopCoroutine(_closeCoroutine);
        _closeCoroutine = StartCoroutine(CloseChestCoroutine());
    }
    private IEnumerator CloseChestCoroutine()
    {
        detection.SetActive(false);
        yield return new WaitForSeconds(3f);
        SetDissolve(0, 1, 1.5f);
        
        yield return new WaitForSeconds(.5f);
        chestVFX.gameObject.SetActive(false);
        chestVFX.Stop();
        
        yield return new WaitForSeconds(.4f);
        chestCollider.enabled = false;
        chestAnimator.SetBool(OpenChestID, false);
        _detectPlayer = false;
    }
    
    
    private void SetDissolve(float _currentValue, float _setValue, float _duration)
    {
        setDissolve.ChangeCurrentValue(_currentValue);
        setDissolve.ChangeValueSet(_setValue);
        setDissolve.ChangeDurationApply(_duration);
        setDissolve.Apply();
    }

    
    #region Trigger Event
    /// <summary> Mở thông báo mở rương </summary>
    public void OnEnterPlayerCollision(GameObject _playerObject)
    {
        _detectPlayer = true;
        if(!_canReceived) 
            _notice.CloseNoticeT3();
        else 
            _notice.CreateNoticeT3("[F] Open Chest.");
    }
    
    /// <summary> Đóng thông báo mở rương </summary>
    public void OnExitPlayerCollision() 
    {
        _detectPlayer = false;
        _notice.CloseNoticeT3();
    }

    /// <summary> Mở chỉ dẫn vị trí rương </summary>
    public void OpenIndicator()
    {
        if (!_canReceived) return;
        ChestNoticeManager.AddChest(this);
        AudioManager.PlayOneShot(FMOD_Events.Instance.notice_01, transform.position);
    }

    /// <summary> Đóng chỉ dẫn </summary>
    public void CloseIndicator()
    {
        ChestNoticeManager.RemoveChest(this);
    }
    #endregion

}
