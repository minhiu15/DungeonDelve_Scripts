using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using UnityEngine;
using UnityEngine.InputSystem;

public class RewardManager : Singleton<RewardManager>
{
    [SerializeField] private SO_GameItemData gameItemData;
    [Space]
    [SerializeField] private Coin coinPrefab;
    [SerializeField] private ItemDrop _itemDropCommonPrefab;
    [SerializeField] private ItemDrop _itemDropUnCommonPrefab;
    [SerializeField] private ItemDrop _itemDropRarePrefab;
    [SerializeField] private ItemDrop _itemDropEpicPrefab;
    [SerializeField] private ItemDrop _itemDropLegendaryPrefab;
    [Space]
    [SerializeField] private EventReference collectItemAudio;
    
    private UserData _userData;
    private PlayerController _player;
    private ObjectPooler<Coin> _poolCoin;
    private ObjectPooler<ItemDrop> _poolItemCommon;
    private ObjectPooler<ItemDrop> _poolItemUnCommon;
    private ObjectPooler<ItemDrop> _poolItemRare;
    private ObjectPooler<ItemDrop> _poolItemEpic;
    private ObjectPooler<ItemDrop> _poolItemLegendary;

    
    // tạo 1 danh sách phẩn thưởng gồm các vật phẩm đang ở gần user và chờ user nhấn input để nhận
    private readonly Dictionary<ItemDrop, ItemReward> _itemRewardsData = new(); 
    
    // tạo hàng chờ phần thưởng của coin, và coin di chuyển tới vị trí player sẽ tự động nhận thưởng
    private readonly Queue<ItemReward> _coinRewardData = new();
    
    
    private void OnEnable()
    {
        GUI_Inputs.InputAction.UI.CollectItem.performed += OnCollectInput;
    }
    private void Start()
    {
        Init();
        GetRef();
    }
    private void OnDisable()
    {
        GUI_Inputs.InputAction.UI.CollectItem.performed -= OnCollectInput;
    }
    
    private void Init()
    {
        _poolCoin = new ObjectPooler<Coin>(coinPrefab, transform, 30);
        _poolCoin.List.ForEach(coin => coin.SetPlayer(_player));

        _poolItemCommon = new ObjectPooler<ItemDrop>(_itemDropCommonPrefab, transform, 10);
        _poolItemUnCommon = new ObjectPooler<ItemDrop>(_itemDropUnCommonPrefab, transform, 10);
        _poolItemRare = new ObjectPooler<ItemDrop>(_itemDropRarePrefab, transform, 10);
        _poolItemEpic = new ObjectPooler<ItemDrop>(_itemDropEpicPrefab, transform, 10);
        _poolItemLegendary = new ObjectPooler<ItemDrop>(_itemDropLegendaryPrefab, transform, 1);
    }
    private void GetRef()
    {
        var _gameManager = GameManager.Instance; if (!_gameManager) return;
        _userData = _gameManager.UserData;
        _player = _gameManager.Player;
    }
    private void OnCollectInput(InputAction.CallbackContext _context)
    {
        if (!_itemRewardsData.Any()) return;
        
        var _keyValuePair = _itemRewardsData.FirstOrDefault();
        RemoveNoticeReward(_keyValuePair.Key);
        SetReward(_keyValuePair.Value);
        _keyValuePair.Key.Release();
        AudioManager.PlayOneShot(collectItemAudio, _keyValuePair.Key.transform.position);
    }
    
    
    /// <summary>
    /// Tạo phần thưởng là các ItemDrop dựa trên danh sách đã Setup trước đó
    /// </summary>
    /// <param name="_rewardSetup"> Instance chứa danh sách phần thưởng. </param>
    public void CreateReward(RewardSetup _rewardSetup)
    {
        Coin _coin = null;
        var position = _rewardSetup.transform.position + Vector3.up;
        var _rewards = _rewardSetup.GetRewardData();
        
        foreach (var VARIABLE in _rewards)
        {
            var itemCustom = gameItemData.GetItemCustom(VARIABLE.GetNameCode());
            if (itemCustom.type != ItemType.Currency)
            {
                CreateItemDrop(VARIABLE, itemCustom, position );
                continue;
            }
            
            _coinRewardData.Enqueue(VARIABLE);
            for (var i = 0; i < 8; i++)
            {
                _coin = _poolCoin.Get(position);
                if(_coin.IsPlayer) 
                    continue;
                _coin.SetPlayer(_player);
            }
        
            if (_coin != null) 
                _coin.OnMoveCompleteEvent += CoinMoveCompleted;
        }
    }
    public void CreateReward(ItemReward _itemReward, Vector3 _itemPosition)
    {
        var _itemCustom = gameItemData.GetItemCustom(_itemReward.GetNameCode());
        CreateItemDrop(_itemReward, _itemCustom, _itemPosition);
    }
    private void CoinMoveCompleted(Coin _coin)
    {
        _coin.PlayAudio();
        _coin.OnMoveCompleteEvent -= CoinMoveCompleted;
        SetReward(_coinRewardData.Dequeue());
    }
    private void CreateItemDrop(ItemReward _itemReward, ItemCustom _itemCustom, Vector3 _itemPosition)
    {
        if (_itemReward.GetValue() <= 0) return;
        var _itemDrop = _itemCustom.ratity switch
        {
            ItemRarity.Common => _poolItemCommon.Get(_itemPosition),
            ItemRarity.Uncommon => _poolItemUnCommon.Get(_itemPosition),
            ItemRarity.Rare => _poolItemRare.Get(_itemPosition),
            ItemRarity.Epic => _poolItemEpic.Get(_itemPosition),
            ItemRarity.Legendary => _poolItemLegendary.Get(_itemPosition),
            _ => null
        };
        _itemDrop!.SetItemDrop(_itemCustom.sprite, _itemReward);
    }
    
    
    
    /// <summary>
    /// Thiết lập phần thưởng và lưu vào dữ liệu của người dùng, đồng thời gửi thông báo tới GUI để hiển thị thông tin ra UI
    /// </summary>
    /// <param name="_itemReward"> Thông tin phần thưởng. </param>
    public void SetReward(ItemReward _itemReward)
    {
        var _val = _itemReward.GetValue();
        var _nameCode = _itemReward.GetNameCode();
        var _itemCustom = gameItemData.GetItemCustom(_nameCode);
        var _sprite = _itemCustom.sprite;
        var _des = _itemCustom.nameItem + " <size=12><color=#ABABAB>x</size></color> " + _val;
        
        NoticeManager.Instance.EnableTitleNoticeT1();
        NoticeManager.CreateNoticeT1(_des, _sprite);
        if (_itemCustom.type == ItemType.Currency)
        {
            _userData.IncreaseCoin(_val);
            return;
        }
        _userData.IncreaseItemValue(_nameCode, _val);
    }
    
    /// <summary>
    /// Thông báo có phần thưởng trên UI, khi người dùng nhấn Input(được cho trước) mới thiết lập phần thưởng cho người dùng
    /// </summary>
    /// <param name="_itemDrop"> Item đang giữ phần thưởng. </param>
    /// <param name="_itemReward"> Phần thưởng được thêm vào. </param>
    public void AddNoticeReward(ItemDrop _itemDrop, ItemReward _itemReward)
    {
        _itemRewardsData.TryAdd(_itemDrop, _itemReward);
        var _newItemNotice = new Dictionary<string, Sprite>();
        foreach (var (key, value) in _itemRewardsData)
        {
            var _itemCustom = gameItemData.GetItemCustom(value.GetNameCode());
            _newItemNotice.TryAdd(_itemCustom.nameItem, _itemCustom.sprite);
        }
        NoticeManager.UpdateNoticeT2(_newItemNotice);
    }
    
    /// <summary>
    /// Xóa thông báo phần thưởng trên UI khi người dùng không còn đứng gần vật phẩm
    /// </summary>
    /// <param name="_itemReward"> Phần thưởng sẽ bị xóa. </param>
    public void RemoveNoticeReward(ItemDrop _itemDrop)
    {
        if(!_itemRewardsData.ContainsKey(_itemDrop)) return;
        
        _itemRewardsData.Remove(_itemDrop);
        NoticeManager.ReleaseAllNoticeT2();
        
        foreach (var keyValuePair in _itemRewardsData)
        {
            var _nameCode = keyValuePair.Value.GetNameCode();
            var _itemCustom = gameItemData.GetItemCustom(_nameCode);
            var _des = _itemCustom.nameItem;
            var _sprite = _itemCustom.sprite;
            NoticeManager.CreateNoticeT2(_des, _sprite);
        }
    }



}
