using UnityEngine;
using System;
using System.Collections.Generic;


/// <summary>
/// Cấu hình về các yêu cầu khi nâng cấp vũ khí của nhân vật: gồm số lượng/type của item, và chi phí cần khi nâng cấp
/// </summary>
[Serializable, CreateAssetMenu(fileName = "Weapon Upgrade Config", menuName = "Characters Configuration/Player/Weapon Config")]
public class SO_RequiresWeaponUpgradeConfiguration : ScriptableObject
{
    [Serializable]
    public class UpgradeItem
    {
        public ItemNameCode code;
        public int value;
    }
    
    [Serializable]
    public class RequiresData
    {
        public int coinCost;
        public List<UpgradeItem> requiresItem = new();
    }
    
    [Tooltip("Giá trị nâng cấp tối đa")]
    public int maxLevelUpgrade = 10;
    
    [Tooltip("Danh sách các Item cần trên từng mốc khi nâng cấp vũ khí")]
    public List<RequiresData> RequiresDatas;

    /// <summary>
    /// Trả về các dữ liệu cần khi upgrade weapon lên level tiếp theo từ level hiện tại như ItemValue, Coin, ....
    /// </summary>
    /// <param name="_level"> Level hiện tại </param>
    /// <returns></returns>
    public RequiresData GetRequires(int _level) => RequiresDatas[_level - 1];
}
