using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUI_CharacterStats : MonoBehaviour, IGUI
{
    
    [SerializeField] private RawImage rawMainMesh;
    [SerializeField] private RawImage rawShadowMesh;
    
    [Header("Infor")] 
    [SerializeField] private Image charChapterIcon;
    [SerializeField] private TextMeshProUGUI charNameText;
    [SerializeField] private TextMeshProUGUI charLevelText;
    [SerializeField] private TextMeshProUGUI charCurrentEXPText;
    [Header("BaseStats")]
    [SerializeField] private TextBar maxHPText;
    [SerializeField] private TextBar maxSTText;
    [SerializeField] private TextBar runSpeedText;
    [SerializeField] private TextBar elementalSkillText;
    [SerializeField] private TextBar elementalBurstText;
    [Header("Attack")] 
    [SerializeField] private  TextBar atkText;
    [Header("Defense")] 
    [SerializeField] private  TextBar defText;

    // Variables
    private SO_CharacterUpgradeData _upgradeData;
    private SO_PlayerConfiguration _playerConfig;
    private PlayerRenderTexture _playerRender;
    private int _characterLevelMax;
    
    private void OnEnable() => GUI_Manager.Add(this);
    private void OnDisable() => GUI_Manager.Remove(this);
    
    
    public void GetRef(GameManager _gameManager)
    {
        _playerConfig = _gameManager.Player.PlayerConfig;
        _playerRender = _gameManager.Player.playerData.PlayerRenderTexture;
        _upgradeData = _gameManager.CharacterUpgradeData;
        rawMainMesh.texture = _playerRender.renderTexture;
        rawShadowMesh.texture = rawMainMesh.texture;
        _characterLevelMax = SO_CharacterUpgradeData.levelMax;
        
        UpdateData();
    }
    public void UpdateData()
    {
        UpdateStatsText();
    }
    

    private void UpdateStatsText()
    {
        if(!_playerConfig) return;
        
        charNameText.text = $"{_playerConfig.GetName()}";
        charChapterIcon.sprite = _playerConfig.ChapterIcon;

        var _currentLv = _playerConfig.GetLevel();
        charLevelText.text = $"Lv. {_currentLv}";
        charCurrentEXPText.text = _currentLv >= _characterLevelMax ? $"~ / {_upgradeData.GetNextEXP(_currentLv)}": $"{_playerConfig.GetCurrentEXP()} / {_upgradeData.GetNextEXP(_currentLv)}";
        maxHPText.SetValueText($"{_playerConfig.GetHP()}");
        maxSTText.SetValueText($"{_playerConfig.GetST()}");
        runSpeedText.SetValueText($"{_playerConfig.GetRunSpeed()}");
        elementalSkillText.SetValueText($"{_playerConfig.GetElementalSkillCD()}");
        elementalBurstText.SetValueText($"{_playerConfig.GetElementalBurstCD()}");
        
        atkText.SetValueText($"{_playerConfig.GetATK()}");
        defText.SetValueText($"{_playerConfig.GetDEF()}");
    }
    public void OpenRenderTexture()
    {
        if (!_playerRender) return;
        
        _playerRender.OpenRenderUI(PlayerRenderTexture.RenderType.Character);
    }


}
