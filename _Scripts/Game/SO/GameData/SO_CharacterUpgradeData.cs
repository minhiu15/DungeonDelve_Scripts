using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class UpgradeCustom
{
    [SerializeField] private int _level;
    [SerializeField] private int _exp;
    [SerializeField] private int _totalExp;

    public int Level { get => _level; private set => _level = value; }
    public int EXP { get => _exp; private set => _exp = value; }
    public int TotalExp { get => _totalExp; private set => _totalExp = value; }
    
    public UpgradeCustom() { }
    public UpgradeCustom(int _level, int _exp, int _totalExp)
    {
        Level = _level;
        EXP = _exp;
        TotalExp = _totalExp;
    }
}

/// <summary>
/// Dự liệu EXP nâng cấp của tất cả Player trong game (Lv 1 -> Lv 90)
/// </summary>
[CreateAssetMenu(fileName = "Upgrade Data", menuName = "Game Configuration/Upgrade Data")]
public class SO_CharacterUpgradeData : ScriptableObject
{
    [SerializeField] private TextAsset LevelingTextAsset;
    public List<UpgradeCustom> Data;
    public const int levelMax = 90;
    private string _strData;
    

    /// <summary>
    /// Hàm này chỉ lấy dữ liệu 1 lần duy nhất trên EDITOR để cập nhật dự liệu vào list Data để tham chiếu và check trong game
    /// </summary>
    public void SetData()
    {
        _strData = LevelingTextAsset.text;
        if(string.IsNullOrEmpty(_strData)) return;
        
        var files = _strData.Split('\n');
        var _lastTotalExp = 0;
        foreach (var _line in files)
        {
            var part = _line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (!part[0].StartsWith("LV:")  || !int.TryParse(part[0][3..], out var _lv) || 
                !part[1].StartsWith("EXP:") || !int.TryParse(part[1][4..], out var _exp) ) 
                continue;
            var _upgradeData = new UpgradeCustom(_lv, _exp, _exp + _lastTotalExp);
            Data.Add(_upgradeData);
            _lastTotalExp = _upgradeData.TotalExp;
        }
    }
    public void RenewValue()
    {
        Data.Clear();
        SetData();
    }

    
    /// <summary>
    /// Trả về điểm kinh nghiệm của level tiếp theo từ level hiện tại
    /// </summary>
    /// <param name="_level"> Level hiện tại của nhân vật </param>
    /// <returns></returns>
    public int GetNextEXP(int _level)
    {
        if(_level >= levelMax) 
            return Data[^1].EXP;

        return _level <= 1 ? Data[0].EXP : Data[_level - 1].EXP;
    }

    
    /// <summary>
    /// Trả về tổng số điểm kinh nghiệm từ Lv1 -> đến (_currentLevel - 1)
    /// Ví dụ _currentLevel = 3, sẽ trả về tổng điểm kinh nghiệm (Lv1 + Lv2)
    /// </summary>
    /// <param name="_currentLevel"> Level hiện tại. </param>
    /// <returns></returns>
    public int GetTotalEXP(int _currentLevel) => _currentLevel <= 1 ? 0 : Data[_currentLevel - 2].TotalExp;
    
}
