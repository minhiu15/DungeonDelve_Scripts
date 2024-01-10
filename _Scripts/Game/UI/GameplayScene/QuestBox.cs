using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestBox : MonoBehaviour, IPooled<QuestBox>
{
    public Animator animator;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Image iconAccept;
    [SerializeField] private Image iconReport;
    public event Action<QuestBox> OnQuestSelectedEvent;
    
    
    /// <summary> Thông tin quest mà box đang giữ </summary>
    public QuestSetup questSetup { get; private set; }
    
    /// <summary> Có đang nhận Task này ? </summary>
    public bool IsReceived { get; private set; }
    
    /// <summary> Task đã hoàn thành chưa ? </summary>
    public bool IsCompleted { get; private set; }
    
    /// <summary> Task có đang bị khóa ? </summary>
    public bool IsLocked { get; private set; }
    
    
    public void SetQuestBox(QuestSetup _questSetup)
    {
        questSetup = _questSetup;
        titleText.text = questSetup.GetTitle();
        iconReport.enabled = false;
        
        var _task = _questSetup.GetTask();
        IsLocked = _task.IsLocked();
        IsReceived = _task.IsReceived();
        IsCompleted = _task.IsCompleted();

        SetReceivedQuestBox(IsReceived && !IsLocked);
    }
    
    public void SelectQuest() =>  OnQuestSelectedEvent?.Invoke(this);
    public void TriggerBoxQuest() => GUI_Quest.IsTriggerQuestBox(this, true);
    public void NonTriggerBoxQuest() =>  GUI_Quest.IsTriggerQuestBox(this, false);

    public void SetReceivedQuestBox(bool _value)
    {
        iconAccept.enabled = _value;
        IsReceived = _value;
    }
    public void SetReportQuest(bool _canCompleted)
    {
        iconReport.enabled = _canCompleted && IsReceived;
    }
    public void LockTask() => IsLocked = true;
    
    public void Release() => ReleaseCallback?.Invoke(this);
    public Action<QuestBox> ReleaseCallback { get; set; }
}
