using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;


[Serializable]
public class UserData
{
    [SerializeField, JsonProperty]
    private string _username;
    [SerializeField, JsonProperty]
    private int _coin;
    //
    [JsonIgnore] public string Username => _username;
    [JsonIgnore] public int Coin => _coin;

    
    [Tooltip("Dữ liệu item mà người dùng sở hữu"), SerializeField, JsonProperty] 
    private Dictionary<ItemNameCode, int> _itemInventory;
    public event Action<int> OnCoinChangedEvent;
    
    
    public UserData() { }
    public UserData(string _username, int _coin)
    {
        this._username = _username;
        this._coin = _coin;
        _itemInventory = new Dictionary<ItemNameCode, int>()
        {
            { ItemNameCode.POHealth , 5},
            { ItemNameCode.PODamage, 5},
            { ItemNameCode.POStamina, 3},
            { ItemNameCode.PODefense, 3},
            { ItemNameCode.EXPSmall, 5},
            { ItemNameCode.EXPMedium, 5},
            { ItemNameCode.JASliver1, 5},
            { ItemNameCode.JASliver2, 5},
            { ItemNameCode.JASliver3, 5},
            { ItemNameCode.UPSpearhead1, 5},
            { ItemNameCode.UPSpearhead2, 5},
            { ItemNameCode.UPSpearhead3, 5}
        };
    }
    
    /// <summary> Danh sách các Item mà người dùng đang có </summary>
    public Dictionary<ItemNameCode, int> GetItemInInventory() => _itemInventory;
    
    
    /// <summary>
    /// Check trong inventory của User có vật phẩm theo nameCode không, nếu có trả về số lượng của vật phẩm
    /// </summary>
    /// <param name="_itemCode"> NameCode cần tìm </param>
    /// <param name="_value"> Giá trị trả về </param>
    /// <returns></returns>
    public int HasItemValue(ItemNameCode _itemCode) => _itemInventory.TryGetValue(_itemCode, out var value) ? value : 0;
    
    /// <summary>
    /// Tăng/Giảm Coin, nếu giá trị truyền vào là âm(-) sẽ DecreaseCoin
    /// </summary>
    /// <param name="_amount"> Số lượng tăng/giảm Coin</param>
    public void IncreaseCoin(int _amount)
    {
        _coin = Mathf.Clamp(_coin + _amount, 0, _coin + _amount);
        SendEventCoinChaged();
    }

    /// <summary>
    /// Tăng/Giảm value của Item, nếu giá trị truyền vào là âm(-) sẽ Decrease value của Item
    /// </summary>
    /// <param name="_amount"> Số lượng tăng/giảm Coin</param>
    public void IncreaseItemValue(ItemNameCode _itemCode, int _amount)
    {
        if (!_itemInventory.ContainsKey(_itemCode))
        {
            _itemInventory.Add(_itemCode, _amount);
            return;
        }
        
        _itemInventory[_itemCode] += _amount;
        if (_itemInventory[_itemCode] > 0) return;
        _itemInventory.Remove(_itemCode);
    }
    
    /// <summary>
    /// Gọi Event để gửi giá trị Coin đi
    /// </summary>
    public void SendEventCoinChaged() => OnCoinChangedEvent?.Invoke(_coin);
    
}
