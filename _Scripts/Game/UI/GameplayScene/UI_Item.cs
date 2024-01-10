using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Item : MonoBehaviour, IPooled<UI_Item>
{
    [SerializeField] private Image rarityFrame;
    [SerializeField] private Image iconItem;
    [SerializeField] private TextMeshProUGUI valueText;
    [Space]
    [SerializeField] private Sprite rarityFrameCommon;
    [SerializeField] private Sprite rarityFrameUnCommon;
    [SerializeField] private Sprite rarityFrameRare;
    [SerializeField] private Sprite rarityFrameEpic;
    [SerializeField] private Sprite rarityFrameLegendary;

    public ItemCustom GetItemCustom { get; private set; }
    public int GetItemValue { get; private set; }

    
    /// <summary>
    /// Set Item ra UI
    /// </summary>
    /// <param name="_itemCustom"> Thông tin Item </param>
    /// <param name="_value"> Số lượng Item </param>
    public void SetItem(ItemCustom _itemCustom, int _itemValue)
    {
        GetItemCustom = _itemCustom;
        iconItem.sprite = _itemCustom.sprite;
        GetItemValue = _itemValue;
        SetRarityFrame(_itemCustom.ratity);
    }
    
    private void SetRarityFrame(ItemRarity _itemRarity)
    {
        rarityFrame.sprite = _itemRarity switch
        {
            ItemRarity.Common => rarityFrameCommon,
            ItemRarity.Uncommon => rarityFrameUnCommon,
            ItemRarity.Rare => rarityFrameRare,
            ItemRarity.Epic => rarityFrameEpic,
            ItemRarity.Legendary => rarityFrameLegendary,
            _ => rarityFrame.sprite
        };
    }
    public void SetValueText(string _textValue) => valueText.text = _textValue;
    
        
    public void Release() => ReleaseCallback?.Invoke(this);
    public Action<UI_Item> ReleaseCallback { get; set; }
}
