using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class NoticeManager : Singleton<NoticeManager>
{
    //
    [SerializeField, BoxGroup("Notice type 1")] private TextMeshProUGUI titleText;
    [SerializeField, BoxGroup("Notice type 1")] private TextBar_3 textBar1Prefab;
    [SerializeField, BoxGroup("Notice type 1")] private Transform content1;
    //
    [SerializeField, BoxGroup("Notice type 2")] private TextBar_3 textBar2Prefab;
    [SerializeField, BoxGroup("Notice type 2")] private Transform content2;
    //
    [SerializeField, BoxGroup("Notice type 3")] private GameObject noticeT3Panel;
    [SerializeField, BoxGroup("Notice type 3")] private TextMeshProUGUI noticeT3Text;
    //
    [SerializeField, BoxGroup("Notice type 4")] private Animator newQuestNotice;
    [SerializeField, BoxGroup("Notice type 4")] private Animator successfulChallengeNotice;
    //
    [SerializeField, BoxGroup("Notice type 5")] private Animator bossClearNotice;
    
    
    private static ObjectPooler<TextBar_3> _pooltextBar1;
    private static ObjectPooler<TextBar_3> _pooltextBar2;
    private Tween _titleTween;
    private readonly float _tweenDuration = .2f;
    private Coroutine _disableNoticeCoroutine;
    private readonly YieldInstruction _yieldInstruction = new WaitForSeconds(2f);
    
    private void Start()
    {
        _pooltextBar1 = new ObjectPooler<TextBar_3>(textBar1Prefab, content1, 15);
        _pooltextBar2 = new ObjectPooler<TextBar_3>(textBar2Prefab, content2, 15);
        titleText.color = new Color(1, 1, 1, 0);
    }

    

    #region Notice Type 1
    /// <summary>
    /// Tạo 1 text thông báo với giá trị value item nhận được và icon hiển thị tương ứng item đó
    /// Thông báo này sẽ xuất hiện khi đã thu thập vật phẩm
    /// </summary>
    /// <param name="_value"> Giá trị Text trong TextBar </param>
    /// <param name="_spriteIcon"> Icon hiển thị trên TextBar </param>
    public static void CreateNoticeT1(string _value, Sprite _spriteIcon)
    {
        var textBar = _pooltextBar1.Get();
        textBar.SetTextBar(_value, _spriteIcon);
    }
    
    /// <summary>
    /// Bật tiêu đề trên bản thông báo khi nhận vật phẩm
    /// </summary>
    public void EnableTitleNoticeT1()
    {
        _titleTween?.Kill();
        _titleTween = titleText.DOColor(new Color(1, 1, 1, 1), _tweenDuration);
        if(_disableNoticeCoroutine != null) 
            StopCoroutine(_disableNoticeCoroutine);
        _disableNoticeCoroutine = StartCoroutine(DisableNoticeCoroutine());
    }
    private IEnumerator DisableNoticeCoroutine()
    {
        yield return _yieldInstruction;
        _titleTween?.Kill();
        _titleTween = titleText.DOColor(new Color(1, 1, 1, 0), _tweenDuration);
        
        // Khi tiêu đề được tắt sẽ bắt đầu save dữ liệu mới vào dữ liệu của User
        if(PlayFabHandleUserData.Instance) PlayFabHandleUserData.Instance.UpdateAllData();
    }
    #endregion

    
    #region  Notice Type 2
    /// <summary>
    /// Tạo 1 text thông báo với giá trị value item nhận được và icon hiển thị tương ứng item đó
    /// Thông báo này sẽ xuất hiện để thu thập vật phẩm khi player đứng gần 1 item
    /// </summary>
    /// <param name="_titleText"> Giá trị Text trong TextBar </param>
    /// <param name="_spriteIcon"> Icon hiển thị trên TextBar </param>
    public static void CreateNoticeT2(string _titleText, Sprite _spriteIcon)
    {
        var count = _pooltextBar2.List.Count(textBar3 => textBar3.gameObject.activeSelf);
        
        var textBar = _pooltextBar2.Get();
        textBar.SetTextBar(_titleText, _spriteIcon);
        textBar.animator.Play(count == 0 ? "TextBar02_IN" : "TextBar02_WAIT");
    }

    /// <summary>
    /// Cập nhật lại danh sách các Notice của item khi người dùng đứng gần các item.
    /// </summary>
    /// <param name="_newItemNotice"> Danh sách item mới. </param>
    public static void UpdateNoticeT2(Dictionary<string, Sprite> _newItemNotice)
    {
        var _currenNotice = _pooltextBar2.List.Where(item => item.gameObject.activeSelf).ToList();
        var _currentNoticeCount = _currenNotice.Count;
        var _newNoticeCount = _newItemNotice.Count;
        
        if (_currentNoticeCount < _newNoticeCount)
        {
            var _needCount = _newNoticeCount - _currentNoticeCount;
            for (var i = 0; i < _needCount; i++)
            {
                var _newNotice = _pooltextBar2.Get();
                _currenNotice.Add(_newNotice);
            }
        }
        else if (_currentNoticeCount > _newNoticeCount)
        {
            var _needCount = _currentNoticeCount - _newNoticeCount;
            for (var i = 0; i < _needCount; i++)
            {
                _currenNotice[i].Release();
            }
        }
        
        var count = 0;
        var _animIDPlay = "TextBar02_IN";
        foreach (var (key, value) in _newItemNotice)
        {
            _currenNotice[count].SetTextBar(key, value);
            _currenNotice[count].animator.Play(_animIDPlay);
            _animIDPlay = "TextBar02_WAIT";
            count++;
        }
    }
    
    /// <summary> Giải phóng tất cả text thông báo nhận Item về Pool </summary>
    public static void ReleaseAllNoticeT2() => _pooltextBar2.List.ForEach(t => t.Release());
    #endregion


    #region Notice Type 3
    /// <summary>
    /// Tạo 1 text thông báo khi Trigger với 1 đối tượng trong game.
    /// Vd: Đứng gần rương thì hiện text [F] Open Chest, ....
    /// </summary>
    /// <param name="_titleNotice"> Text sẽ hiện trên UI thông báo </param>
    public void CreateNoticeT3(string _titleNotice) 
    {
        noticeT3Panel.SetActive(true);
        noticeT3Text.text = _titleNotice;
    }
    
    /// <summary>
    /// Đóng text thông báo.
    /// </summary>
    public void CloseNoticeT3() => noticeT3Panel.SetActive(false);
    #endregion

    
    #region Notice Type 4
    /// <summary> Mở thông báo nhiệm vụ mới mỗi ngày </summary>
    public void OpenNewQuestNoticePanelT4() => newQuestNotice.Play("Panel_IN");
    /// <summary> Mở thông báo hoàn thành 1 thử thách bất kì </summary>
    public void OpenSuccessfulChallengeNoticePanelT4() => successfulChallengeNotice.Play("Panel_IN");
    #endregion

    
    #region Notice Type 5
    /// <summary> Mở thông báo tiêu diệt thành công BOSS </summary>
    public void OpenBossConqueredNoticeT5() => bossClearNotice.Play("BossConquered_IN");


    #endregion
}
