using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using System;
using Random = UnityEngine.Random;


/// <summary> Custom Item làm phần thưởng </summary>
[Serializable]
public class ItemReward
{
    [SerializeField, Tooltip("Loại phần thưởng, được phân loại bởi Item Namecode")]
    private ItemNameCode code;

    [SerializeField, ReadOnly, HideInInspector] 
    private ItemRarity rarity;
    
    [SerializeField, Tooltip("Giá trị phần thưởng")]
    private int value;
    
    [SerializeField, Tooltip("Có Random giá trị phần thưởng ?")] 
    private bool isRandom;
    
    [SerializeField, ShowIf("isRandom"), MinMaxSlider(0, 999), Tooltip("Giá trị tối thiểu và tối đa của phần thưởng sẽ Random nếu biến isRandom = TRUE")]
    private Vector2 valueRandom;
    
    
    public void SetNameCode(ItemNameCode _value) => code = _value;
    public void SetValue(int _value) => value = _value;
    public void SetIsRandom(bool _value) => isRandom = _value;
    public void SetValueRandom(Vector2 _value) => valueRandom = _value;
    public void SetRarity(ItemRarity _value) => rarity = _value;
    //
    public bool GetIsRandom() => isRandom;
    public Vector2 GetValueRandom() => valueRandom;
    public ItemNameCode GetNameCode() => code;
    public int GetValue() => isRandom ? (int)Random.Range(valueRandom.x, valueRandom.y) : value;
    public ItemRarity GetRarity() => rarity;
}

public class RewardSetup : MonoBehaviour
{
    [Tooltip("Thiết lập 1 danh sách dữ liệu phần thưởng và gọi SendRewardData để class RewardManager nhận dữ liệu và xử lí.")]
    [SerializeField] private List<ItemReward> rewardsData;
    public List<ItemReward> GetRewardData() => rewardsData;
    
    
    /// <summary>
    /// Gửi dữ liệu phần thưởng tới class quản lí.
    /// </summary>
    public void SendRewardData() => RewardManager.Instance.CreateReward(this);
    
}
