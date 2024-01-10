using System;
using System.Collections.Generic;
using UnityEngine;
using DungeonDelve.Project;
using NaughtyAttributes;
using Newtonsoft.Json;

namespace DungeonDelve.Project
{
    [Serializable]
    public class Task
    {
        public Task() { }
        public Task(bool _isCompleted, bool _isLocked, bool _isReceived)
        {
            isCompleted = _isCompleted;
            isLocked = _isLocked;
            isReceived = _isReceived;
        }
        
        [SerializeField, JsonProperty] private bool isCompleted;
        [SerializeField, JsonProperty] private bool isLocked;
        [SerializeField, JsonProperty] private bool isReceived;
        
        public bool IsCompleted() => isCompleted;
        public bool IsLocked() => isLocked;
        public bool IsReceived() => isReceived;
        public void SetCompleted(bool _value) => isCompleted = _value;
        public bool SetTaskLocked(bool _value) =>  isLocked = _value;
        public bool SetReceived(bool _value) =>  isReceived = _value;
    } 

    [Serializable]
    public class TaskRequirement
    {
        [SerializeField, Tooltip("Vật phẩm yêu cầu")]
        private ItemNameCode code;
        
        [SerializeField, Tooltip("Số lượng vật phẩm cần")] 
        private int value;

        public ItemNameCode GetNameCode() => code;
        public void SetNameCode(ItemNameCode _value) => code = _value;
            
        public int GetValue() => value;
        public int SetValue(int _value) => value = _value;
        
    }
}


[Serializable]
[CreateAssetMenu(menuName = "Create Quest", fileName = "Quest_")]
public class QuestSetup : ScriptableObject
{
    [SerializeField, ReadOnly] private Task task;
    [SerializeField] private string title;
    [SerializeField] private string description;
    [SerializeField] private TaskRequirement requirement;
    [SerializeField] private List<ItemReward> rewards;
    
    
    #region Getter
    /// <summary> Thông tin Task </summary>
    public Task GetTask() => task;
    
    /// <summary> Tiêu đề Task </summary>
    public string GetTitle() => title;
    
    /// <summary> Mô tả Task </summary>
    public string GetDescription() => description;
    
    /// <summary> Yêu cầu của Task </summary>
    public TaskRequirement GetRequirement() => requirement;
    
    /// <summary> Phần thưởng của Task </summary>
    public List<ItemReward> GetRewards() => rewards;
    #endregion
    
    #region Setter
    public void SetTask(Task _value) => task = _value;
    public void SetTitle(string _value) => title = _value;
    public void SetDescription(string _value) => description = _value;
    public void SetReward(List<ItemReward> _value) => rewards = _value;
    #endregion

}
