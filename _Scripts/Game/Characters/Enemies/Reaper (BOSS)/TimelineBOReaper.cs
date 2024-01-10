using System;
using System.Collections;
using FMODUnity;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TimelineBOReaper : MonoBehaviour
{
    [SerializeField] private MonoBehaviourID behaviourID;
    [Space] 
    [SerializeField] private InteractiveUI interactiveUI;
    [SerializeField] private EnemyController reaperBOSS;
    [SerializeField] private Chest reaperChestReward;
    [SerializeField] private PlayableDirector playableDirector;

    [Space]
    [SerializeField] private M_SetEmission setEmission;
    [SerializeField] private EventReference onClickSoundRef;
    
    [BoxGroup("VOLUME CHANGE"), SerializeField] private DungeonAmbienceVolumeChangeTrigger dungeonAmbienceVolumeChange;
    [BoxGroup("VOLUME CHANGE"), SerializeField] private BackgroundAudio reaperBattleAudio;
    
    private bool _canTrigger;      // có được active BO lên k ?
    private bool _isTriggerPlayer; // có đang TriggerPlayer ?
    // Ref
    private PlayerController _player;
    private PlayerHUD _playerHUD;
    private CameraFOV _cameraFOV;
    private DateTime _lastDay;
    private TimelineAsset _timeline;
    // 
    private Coroutine _enableTimelineCoroutine;
    private Coroutine _bossDieCoroutine;
    private Coroutine _timeCoroutine;

    
    private void Start()
    {
        interactiveUI.OnPanelOpenEvent += OnEnterPlayer;
        _player = GameManager.Instance.Player;
        _playerHUD =  GameManager.Instance.PlayerHUD;
        _player.OnRevivalTimeEvent += HandlePlayerDie;
        _cameraFOV = _player.cameraFOV;
        reaperBOSS.OnDieEvent.AddListener(HandleBossDie);

        _timeline = (TimelineAsset)playableDirector.playableAsset;
        MuteGroupTrack(0, false);
        MuteGroupTrack(1, true);
    }
    private void OnDestroy()
    {
        interactiveUI.OnPanelOpenEvent -= OnEnterPlayer;
        _player.OnRevivalTimeEvent -= HandlePlayerDie;
        reaperBOSS.OnDieEvent.RemoveListener(HandleBossDie);
    }
    
    
    private void HandlePlayerDie(float _timeRevival)
    {
        if (_timeRevival != 0) return;
        HandleCommon();
        reaperBOSS.gameObject.SetActive(false);
    }
    private void HandleBossDie(EnemyController _enemy)
    {
        if (_bossDieCoroutine != null) StopCoroutine(_bossDieCoroutine);
        _bossDieCoroutine = StartCoroutine(HandleBODieCoroutine());
    }
    private IEnumerator HandleBODieCoroutine()
    {
        PlayerPrefs.SetString(behaviourID.GetID, DateTime.Now.ToString("O"));
        HandleCommon();
        
        yield return new WaitForSeconds(1f);
        NoticeManager.Instance.OpenSuccessfulChallengeNoticePanelT4();
        
        yield return new WaitForSeconds(2f);
        BlackBoard.Instance.Enable(1.5f);
        GUI_Inputs.DisableInput();
        _player.input.PlayerInput.Disable();
        
        yield return new WaitForSeconds(.9f);
        _player.FreeLookCamera.m_YAxis.Value = .5f;
        var _currentAng = _player.model.eulerAngles.y;
        var angle = _currentAng >= 180 ? Mathf.Abs(180 - _currentAng) : -Mathf.Abs(180 - _currentAng);
        _player.FreeLookCamera.m_XAxis.Value = angle;
        _player.FreeLookCamera.m_Lens.FieldOfView = 35f;
        
        yield return new WaitForSeconds(.15f);
        _player.FreeLookCamera.enabled = false;
        
        yield return new WaitForSeconds(.8f);
        NoticeManager.Instance.OpenBossConqueredNoticeT5();
        
        yield return new WaitForSeconds(3.5f);
        MuteGroupTrack(0, true);
        MuteGroupTrack(1, false);
        playableDirector.Play();
        
        yield return new WaitForSeconds(2f);
        reaperChestReward.CreateChest();
    }
    
    
    private void HandleCommon()
    {
        reaperBattleAudio.Stop();
        dungeonAmbienceVolumeChange.SetVolume(1f);
        ApplyEmission(15, 0);
    }

    private IEnumerator TimeCoroutine()
    {
        while (true)
        {
            var _currentTime = DateTime.Now;
            var _nextMidnight = _currentTime.Date.AddDays(1);
            var _time = _nextMidnight - _currentTime;
            interactiveUI.SetNoticeText($"Can start after {_time.Hours} hour, {_time.Minutes} minute.");
            yield return new WaitForSecondsRealtime(10f);
        }
    }
    public void OnTriggerEnterPlayer()
    {
        _canTrigger = false;
        _lastDay = DateTime.Parse(PlayerPrefs.GetString(behaviourID.GetID, DateTime.MinValue.ToString()));
        if (_lastDay >= DateTime.Today || reaperBOSS.gameObject.activeSelf)
        {
            if (_timeCoroutine != null) StopCoroutine(_timeCoroutine);
            _timeCoroutine = StartCoroutine(TimeCoroutine());
           return;
        }
        
        _canTrigger = true;
        interactiveUI.SetNoticeText("[F] Start.");
    }
    public void OnTriggerExitPlayer()
    {
        _isTriggerPlayer = false;
        if (_timeCoroutine != null) StopCoroutine(_timeCoroutine);
    }
    public void OnEnterPlayer()
    {
        _isTriggerPlayer = true;
        if (_enableTimelineCoroutine != null) 
            StopCoroutine(EnableTimelineCoroutine());
        _enableTimelineCoroutine = StartCoroutine(EnableTimelineCoroutine());
    }
    public void ExitBossActivationArea(bool _isExit) // Khi Player ra khỏi khu vực BossBattle 
    {
        if (!_isExit) return;
        
        // Check BO có đang được active?
        if (!reaperBOSS.gameObject.activeSelf) return;
        
        _canTrigger = false;
        dungeonAmbienceVolumeChange.SetVolume(1f);
        reaperBattleAudio.Stop();
        ApplyEmission(15, 0);
        reaperBOSS.gameObject.SetActive(false);
    }
    
    private void ApplyEmission(float _currentVal, float SetVal)
    {
        setEmission.ChangeCurrentIntensity(_currentVal);
        setEmission.ChangeIntensitySet(SetVal); 
        setEmission.Apply();
    }
    private IEnumerator EnableTimelineCoroutine()
    {
        if(!_canTrigger) yield break;
        
        DeactiveControl();
        interactiveUI.OnExitPlayer();
        ApplyEmission(0, 15);
        MuteGroupTrack(0, false);
        MuteGroupTrack(1, true);
        AudioManager.PlayOneShot(onClickSoundRef, transform.position);

        yield return new WaitForSeconds(2f);
        if (_isTriggerPlayer)
        {
            playableDirector.Play();
            _canTrigger = false;
            dungeonAmbienceVolumeChange.SetVolume(.05f);
            reaperBattleAudio.Play();
        }
        else
        {
            yield return new WaitForSeconds(.7f);
            playableDirector.Stop();
            ApplyEmission(15, 0);
        }
    }

    // Animation Event
    public void SetCamFOV()
    {
        _cameraFOV.SetCurrentFOV(_player.FreeLookCamera.m_Lens.FieldOfView);
        _player.FreeLookCamera.enabled = true;
    }
    public void ActiveControl()
    {
        GUI_Inputs.EnableInput();

        if (!_player) return;
        _player.input.PlayerInput.Enable();
        _playerHUD.OpenHUD();
    }
    public void DeactiveControl()
    {
        GUI_Inputs.DisableInput();
        
        if (!_player) return;
        _player.input.PlayerInput.Disable();
        _playerHUD.CloseHUD();
    }
  
    
    private void MuteGroupTrack(int _trackIndex, bool _isMute)
    {
        var _groupTrack = _timeline.GetRootTrack(_trackIndex);
        _groupTrack.muted = _isMute;
    
        var t = playableDirector.time;
        playableDirector.RebuildGraph();
        playableDirector.time = t;
    }
}
