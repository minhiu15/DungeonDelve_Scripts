using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DungeonDelve.Project;

public class QuestManager : Singleton<QuestManager>
{
    [SerializeField] private MonoBehaviourID behaviourID;
    [field: SerializeField] public InteractiveUI interactiveUI { get; private set; }
    public static List<QuestSetup> QuestLists { get; private set; } = new();
    
    public static int currentQuest { get; private set; } // Số lượng quest đang nhận.
    public static int maxQuest => 3; // Số lượng tối đa quest được nhận/1 ngày. 
    
    private void Start()
    {
        if (PlayFabHandleUserData.Instance && PlayFabHandleUserData.Instance.IsLogin)
        {
            QuestLists = PlayFabHandleUserData.Instance.Quests;
        }
        else
        {
            var _quests = Resources.LoadAll<QuestSetup>("Quest Custom");
            QuestLists.Clear();
            foreach (var questSetup in _quests)
            {
                QuestLists.Add(Instantiate(questSetup));
            }
        }
        
        currentQuest = 0;
        var _tasks = QuestLists.Select(x => x.GetTask());
        var _lastDay = DateTime.Parse(PlayerPrefs.GetString(behaviourID.GetID, DateTime.MinValue.ToString()));
        if (_lastDay < DateTime.Today)
        {
            LoadNewQuest(_tasks);
            StartCoroutine(NoticeCoroutine());
            PlayerPrefs.SetString(behaviourID.GetID, DateTime.Now.ToString());
        }
        else
        {
            LoadOldQuest(_tasks);
        }
        
        QuestLists.ForEach(x => SordItemReward(x.GetRewards()));
    }
    private static void LoadNewQuest(IEnumerable<Task> _tasks)
    {
        foreach (var _task in _tasks)
        {
            _task.SetCompleted(false);
            _task.SetReceived(false);
            _task.SetTaskLocked(false);
        }   
    }
    private static void LoadOldQuest(IEnumerable<Task> _tasks)
    {
        foreach (var _task in _tasks)
        {
            if (_task.IsCompleted() || _task.IsLocked() || _task.IsReceived())
                currentQuest++;
        }
    }
    private static IEnumerator NoticeCoroutine()
    {
        yield return new WaitForSeconds(.5f);
        NoticeManager.Instance.OpenNewQuestNoticePanelT4();
    }
    
    public static void OnStartedQuest(QuestSetup _quest)
    {
        currentQuest = Mathf.Clamp(currentQuest + 1, 0, maxQuest);
        
        var _task = _quest.GetTask();
        _task.SetCompleted(false);
        _task.SetTaskLocked(false);
        _task.SetReceived(true);
    }
    public void OnCompletedQuest(QuestSetup _quest)
    {
        interactiveUI.ClosePanel(default);
        _quest.GetRewards().ForEach(x => RewardManager.Instance.SetReward(x));
        var _task = _quest.GetTask();
        _task.SetCompleted(true);
        _task.SetTaskLocked(false);
        _task.SetReceived(true);
        AudioManager.PlayOneShot(FMOD_Events.Instance.rewards_01, Vector3.zero);
        
        if (PlayFabHandleUserData.Instance) 
            PlayFabHandleUserData.Instance.UpdateData(PlayFabHandleUserData.PF_Key.QuestData_Key);
    }
    public static void OnCancelQuest(QuestSetup _quest)
    {
        var _task = _quest.GetTask();
        _task.SetCompleted(false);
        _task.SetTaskLocked(true);
        _task.SetReceived(true);
    }
    private static void SordItemReward(List<ItemReward> _rewards)
    {
        _rewards.Sort((x1, x2) => x1.GetRarity().CompareTo(x2.GetRarity()));
    }
    

    
}
