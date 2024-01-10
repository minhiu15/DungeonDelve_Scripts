using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class GameplayEditorWindow : EditorWindow
{

    [MenuItem("Tools/DUNGEON DELVE - CUSTOM")]
    public static void ShowCustom()
    {
        var editor = GetWindow<GameplayEditorWindow>();
        editor.titleContent = new GUIContent("DUNGEON DELVE - GAMECUSTOM");
    }

    private readonly string[] _toolTitles = { "PLAYERS", "ENEMIES", "GAME CUSTOM" };
    private int _selectedTool = -1;
    private Vector2 scrollView;
    
    
    private void OnGUI()
    {
        _selectedTool = GUILayout.Toolbar(_selectedTool, _toolTitles, Width(500), Height(50));
        switch (_selectedTool)
        {
            case 0:
                HandlePanelPlayers();
                break;
            
            case 1:
                HandlePanelEnemies();
                break;
            
            case 2:
                HandlePanelGameCustom();
                break;
        }
    }
    
    
    #region PLAYERS
    private int _selectedType = -1;
    private readonly string[] _type = { "STATS CONFIG", "CHARACTER UPGRADE DATA" };
    private int _selectedPlayer = -1;
    private readonly string[] _playerNames = { "Arlan", "Lynx" };
    
    private SO_PlayerConfiguration _arlanConfig;
    private SO_PlayerConfiguration _lynxConfig;
    private SO_CharacterUpgradeData _characterUpgradeData;
    private SO_RequiresWeaponUpgradeConfiguration _requiresWeaponUpgrade;

    
    private void HandlePanelPlayers()
    {
        Space(15);
        _selectedType = GUILayout.Toolbar(_selectedType, _type, Width(250), Height(35));
        switch (_selectedType)
        {
            case 0:
                _selectedPlayer = GUILayout.Toolbar(_selectedPlayer, _playerNames, Width(200), Height(30));
                switch (_selectedPlayer)
                {
                    case 0:
                        _arlanConfig = EditorGUIUtility.Load("Assets/Dungeon Delve - PROJECT/Player/1. Arlan/Prefab/Arlan Config.asset") as SO_PlayerConfiguration;
                        _requiresWeaponUpgrade = EditorGUIUtility.Load("Assets/Dungeon Delve - PROJECT/Player/1. Arlan/Prefab/Weapon Upgrade Config.asset") as SO_RequiresWeaponUpgradeConfiguration;
                        ShowPlayerConfig(_arlanConfig);
                        break;
                    
                    case 1:
                        _lynxConfig = EditorGUIUtility.Load("Assets/Dungeon Delve - PROJECT/Player/2. Lynx/Prefab/Lynx Config.asset") as SO_PlayerConfiguration;
                        _requiresWeaponUpgrade = EditorGUIUtility.Load("Assets/Dungeon Delve - PROJECT/Player/2. Lynx/Prefab/Weapon Upgrade Config.asset") as SO_RequiresWeaponUpgradeConfiguration;
                        ShowPlayerConfig(_lynxConfig);
                        break;
                }
                break;
            case 1:
                _characterUpgradeData = EditorGUIUtility.Load("Assets/Dungeon Delve - PROJECT/GameData/Character Upgrade Data.asset") as SO_CharacterUpgradeData;
                ShowUpgradeDetails(_characterUpgradeData);
                break;

        }
    }

    private void ShowPlayerConfig(SO_PlayerConfiguration _playerConfig) 
    {
        if (_playerConfig == null)
        {
            EditorGUILayout.HelpBox("Assign a PlayerConfig.", MessageType.Warning);
            return;
        }

        scrollView = GUILayout.BeginScrollView(scrollView);
        EditorGUI.BeginChangeCheck();

        #region Stats
        Space(25);
        GUILayout.Label("NAME CODE -----------------------------", EditorStyles.boldLabel);
        _playerConfig.NameCode = (CharacterNameCode)EditorGUILayout.EnumPopup("Code", _playerConfig.NameCode, Width(500));
        
        Space(30);
        GUILayout.Label("INFORMATION ------------------------", EditorStyles.boldLabel);
        _playerConfig.SetName(EditorGUILayout.TextField("Name", $"{_playerConfig.NameCode}", Width(500)));
        _playerConfig.SetLevel(EditorGUILayout.IntSlider("Character Level", _playerConfig.GetLevel(), 1, SO_CharacterUpgradeData.levelMax, Width(500)));
        _playerConfig.SetCurrentEXP(EditorGUILayout.IntField("Current EXP", _playerConfig.GetCurrentEXP(), Width(500)));
        _playerConfig.SetInfor(EditorGUILayout.TextField("Infor", _playerConfig.GetInfor(), Width(500)));
        GUILayout.BeginHorizontal();
        GUILayout.Label("Chapter Icon", Width(148));
        _playerConfig.ChapterIcon = (Sprite)EditorGUILayout.ObjectField(_playerConfig.ChapterIcon, typeof(Sprite), false, Width(50), Height(50));
        GUILayout.EndHorizontal();
        
        Space(30);
        GUILayout.Label("CHARACTER STATS ----------------------", EditorStyles.boldLabel);
        _playerConfig.SetHP(EditorGUILayout.IntField("Max HP", _playerConfig.GetHP(), Width(500)));
        _playerConfig.SetST(EditorGUILayout.IntField("Max ST", _playerConfig.GetST(), Width(500)));
        _playerConfig.SetATK(EditorGUILayout.IntField("ATK", _playerConfig.GetATK(), Width(500)));
        
        _playerConfig.SetCRITRate(EditorGUILayout.FloatField("CRIT Rate(%)", _playerConfig.GetCRITRate(), Width(500)));
        _playerConfig.SetCRITDMG(EditorGUILayout.IntField("CRIT DMG(%)", _playerConfig.GetCRITDMG(), Width(500)));
        _playerConfig.SetDEF(EditorGUILayout.IntField("DEF", _playerConfig.GetDEF(), Width(500)));
        _playerConfig.SetChargedAttackSTCost(EditorGUILayout.IntField("Charged Attack ST Cost", _playerConfig.GetChargedAttackSTCost(), Width(500)));
        _playerConfig.SetWalkSpeed(EditorGUILayout.FloatField("Walk Speed", _playerConfig.GetWalkSpeed(), Width(500)));
        _playerConfig.SetRunSpeed(EditorGUILayout.FloatField("Walk Speed", _playerConfig.GetRunSpeed(), Width(500)));
        _playerConfig.SetRunFastSpeed(EditorGUILayout.FloatField("Run Fast Speed", _playerConfig.GetRunFastSpeed(), Width(500)));
        _playerConfig.SetDashSTCost(EditorGUILayout.IntField("Dash ST Cost", _playerConfig.GetDashSTCost(), Width(500)));
        _playerConfig.SetJumpHeight(EditorGUILayout.FloatField("Jump Height", _playerConfig.GetJumpHeight(), Width(500)));

        Space(30);
        GUILayout.Label("WEAPON -------------------------", EditorStyles.boldLabel);
        _playerConfig.SetWeaponName(EditorGUILayout.TextField("Weapon Name", _playerConfig.GetWeaponName(), Width(500)));
        _playerConfig.SetWeaponInfo(EditorGUILayout.TextField("Weapon Info", _playerConfig.GetWeaponInfo(), Width(500)));
        _playerConfig.SetWeaponLevel(EditorGUILayout.IntSlider("Weapon Level", _playerConfig.GetWeaponLevel(), 1, 10, Width(500)));
        
        Space(30);
        GUILayout.Label("COOLDOWN -------------------------", EditorStyles.boldLabel);
        _playerConfig.SetElementalSkillCD(EditorGUILayout.FloatField("Elemental Skill CD(s)", _playerConfig.GetElementalSkillCD(), Width(500)));
        _playerConfig.SetElementalBurstCD(EditorGUILayout.FloatField("Elemental Burst CD(s)", _playerConfig.GetElementalBurstCD(), Width(500)));
        #endregion

        Space(30);
        GUILayout.Label("MULTIPLIER -------------------------", EditorStyles.boldLabel);
        #region Normal Attack
        Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Label("NORMAL ATTACK MULTIPLIER", Width(200), Height(25));
        for (var i = 1; i <= 10; i++)
        {
            GUILayout.Box($"Wea Lv{i}", Width(65), Height(25));
        }
        GUILayout.EndHorizontal();
        for (var j = 0; j < _playerConfig.GetNormalAttackMultiplier().Count; j++)
        {
            GUILayout.BeginHorizontal();
            _playerConfig.GetNormalAttackMultiplier()[j].MultiplierTypeName = EditorGUILayout.TextField($"", _playerConfig.GetNormalAttackMultiplier()[j].MultiplierTypeName ,Width(202), Height(27));
            for (var i = 0; i < _playerConfig.GetNormalAttackMultiplier()[j].GetMultiplier().Count; i++)
            {
                _playerConfig.GetNormalAttackMultiplier()[j].GetMultiplier()[i] = EditorGUILayout.FloatField("", _playerConfig.GetNormalAttackMultiplier()[j].GetMultiplier()[i], EditorStyles.numberField, Width(65), Height(27));
            }
            GUILayout.EndHorizontal();
        }
        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("+", Width(45), Height(25)))
        {
            _playerConfig.AddNormalAttackMultiplier();
        }
        if(GUILayout.Button("-", Width(45), Height(25)) && _playerConfig.GetNormalAttackMultiplier().Count != 0)
        {
            _playerConfig.RemoveNormalAttackMultiplier();
        }
        GUILayout.EndHorizontal();
        #endregion
        
        #region Charged Attack
        Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Label("CHARGED ATTACK MULTIPLIER", Width(200), Height(25));
        for (var i = 1; i <= 10; i++)
        {
            GUILayout.Box($"Wea Lv{i}", Width(65), Height(25));
        }
        GUILayout.EndHorizontal();
        for (var j = 0; j < _playerConfig.GetChargedAttackMultiplier().Count; j++)
        {
            GUILayout.BeginHorizontal();
            _playerConfig.GetChargedAttackMultiplier()[j].MultiplierTypeName = EditorGUILayout.TextField($"", _playerConfig.GetChargedAttackMultiplier()[j].MultiplierTypeName ,Width(202), Height(27));
            for (var i = 0; i < _playerConfig.GetChargedAttackMultiplier()[j].GetMultiplier().Count; i++)
            {
                _playerConfig.GetChargedAttackMultiplier()[j].GetMultiplier()[i] = EditorGUILayout.FloatField("", _playerConfig.GetChargedAttackMultiplier()[j].GetMultiplier()[i], EditorStyles.numberField, Width(65), Height(27));
            }
            GUILayout.EndHorizontal();
        }
        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("+", Width(45), Height(25)))
        {
            _playerConfig.AddChargedAttackMultiplier();
        }
        if(GUILayout.Button("-", Width(45), Height(25)) && _playerConfig.GetChargedAttackMultiplier().Count != 0)
        {
            _playerConfig.RemoveChargedAttackMultiplier();
        }
        GUILayout.EndHorizontal();
        #endregion
        
        #region Elemental Skill
        Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Label("ELEMENTAL SKILL MULTIPLIER", Width(200), Height(25));
        for (var i = 1; i <= 10; i++)
        {
            GUILayout.Box($"Wea Lv{i}", Width(65), Height(25));
        }
        GUILayout.EndHorizontal();
        for (var j = 0; j < _playerConfig.GetElementalSkillMultiplier().Count; j++)
        {
            GUILayout.BeginHorizontal();
            _playerConfig.GetElementalSkillMultiplier()[j].MultiplierTypeName = EditorGUILayout.TextField($"", _playerConfig.GetElementalSkillMultiplier()[j].MultiplierTypeName ,Width(202), Height(27));
            for (var i = 0; i < _playerConfig.GetElementalSkillMultiplier()[j].GetMultiplier().Count; i++)
            {
                _playerConfig.GetElementalSkillMultiplier()[j].GetMultiplier()[i] = EditorGUILayout.FloatField("", _playerConfig.GetElementalSkillMultiplier()[j].GetMultiplier()[i], EditorStyles.numberField, Width(65), Height(27));
            }
            GUILayout.EndHorizontal();
        }
        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("+", Width(45), Height(25)))
        {
            _playerConfig.AddElementalSkillMultiplier();
        }
        if(GUILayout.Button("-", Width(45), Height(25)) && _playerConfig.GetElementalSkillMultiplier().Count != 0)
        {
            _playerConfig.RemoveElementalSkillMultiplier();
        }
        GUILayout.EndHorizontal();
        #endregion
        
        #region Elemental Burst
        Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Label("ELEMENTAL BURST MULTIPLIER", Width(200), Height(25));
        for (var i = 1; i <= 10; i++)
        {
            GUILayout.Box($"Wea Lv{i}", Width(65), Height(25));
        }
        GUILayout.EndHorizontal();
        for (var j = 0; j < _playerConfig.GetElementalBurstMultiplier().Count; j++)
        {
            GUILayout.BeginHorizontal();
            _playerConfig.GetElementalBurstMultiplier()[j].MultiplierTypeName = EditorGUILayout.TextField($"", _playerConfig.GetElementalBurstMultiplier()[j].MultiplierTypeName ,Width(202), Height(27));
            for (var i = 0; i < _playerConfig.GetElementalBurstMultiplier()[j].GetMultiplier().Count; i++)
            {
                _playerConfig.GetElementalBurstMultiplier()[j].GetMultiplier()[i] = 
                    EditorGUILayout.FloatField("", _playerConfig.GetElementalBurstMultiplier()[j].GetMultiplier()[i],
                    EditorStyles.numberField, Width(65), Height(27));
            }
            GUILayout.EndHorizontal();
        }
        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("+", Width(45), Height(25)))
        {
            _playerConfig.AddElementalBurstMultiplier();
        }
        if(GUILayout.Button("-", Width(45), Height(25)) && _playerConfig.GetElementalBurstMultiplier().Count != 0)
        {
            _playerConfig.RemoveElementalBurstMultiplier();
        }
        GUILayout.EndHorizontal();
        #endregion
        
        ShowRequiesWeaponUpgrade(_requiresWeaponUpgrade);

        if(EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(_playerConfig);
        GUILayout.EndScrollView();
    }
    private void ShowRequiesWeaponUpgrade(SO_RequiresWeaponUpgradeConfiguration _requiresData)
    {
        Space(30);
        GUILayout.Label("REQUIRES WEAPON UPGRADE -----------", EditorStyles.boldLabel);
        
        if(_requiresData == null) return;
        
        Space(8);
        GUILayout.BeginHorizontal();
        GUILayout.Box("Level Upgrade", Width(100));
        GUILayout.Box("Coin Upgrade Cost", Width(120));
        GUILayout.Box("Item Code", Width(150));
        GUILayout.Box("Item Value", Width(75));
        GUILayout.Label("", Width(50));
        _requiresData.maxLevelUpgrade = EditorGUILayout.IntField("Max Level Upgrade", _requiresData.maxLevelUpgrade, Width(200));
        GUILayout.EndHorizontal();
        
        EditorGUI.BeginChangeCheck();
        for (var i = 0; i < _requiresData.RequiresDatas.Count; i++)
        {
            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.BeginHorizontal();
            
            var _data = _requiresData.RequiresDatas[i];
            GUILayout.Box($"{i + 1} -> {i + 2}",BoxColorText(Color.red) , Width(100));
            _data.coinCost = EditorGUILayout.IntField("", _data.coinCost, TextFieldColorText(Color.cyan) , Width(120));
            
            GUILayout.BeginVertical();
            foreach (var _item in _data.requiresItem)
            {
                GUILayout.BeginHorizontal();
                _item.code = (ItemNameCode)EditorGUILayout.EnumPopup("", _item.code, Width(150));
                _item.value = EditorGUILayout.IntField("", _item.value, Width(75));
                GUILayout.EndHorizontal();
            }
            
            #region Button
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+", Width(45), Height(20)) && _data.requiresItem.Count < 9)
            {
                _data.requiresItem.Add(new SO_RequiresWeaponUpgradeConfiguration.UpgradeItem());
            }
            if(GUILayout.Button("-", Width(45), Height(20)) && _data.requiresItem.Count != 0)
            {
                _data.requiresItem.Remove(_data.requiresItem[^1]);
            }
            GUILayout.EndHorizontal();
            Space(10);
            #endregion
            GUILayout.EndVertical();
            GUILayout.EndVertical();
            
            GUILayout.EndHorizontal();
        }
        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("+", Width(45), Height(25)) && _requiresData.RequiresDatas.Count < _requiresData.maxLevelUpgrade - 1)
        {
            _requiresData.RequiresDatas.Add(new SO_RequiresWeaponUpgradeConfiguration.RequiresData());
        }
        if(GUILayout.Button("-", Width(45), Height(25)) && _requiresData.RequiresDatas.Count != 0)
        {
            _requiresData.RequiresDatas.Remove(_requiresData.RequiresDatas[^1]);
        }
        GUILayout.EndHorizontal();
        Space(30);        
        
        if(EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(_requiresData);
    }
    private void ShowUpgradeDetails(SO_CharacterUpgradeData _upgradeData)
    {
        if(_upgradeData == null) 
            return;
       
        Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Box("Level", Width(150));
        GUILayout.Box("Level Next Upgrade", Width(140));
        GUILayout.Box("EXP Needed", Width(150));
        GUILayout.Label("", Width(30));
        GUILayout.Box("Total EXP Cost", Width(150));
        GUILayout.Label("", Width(50));
        if(GUILayout.Button("Renew Value",ButtonColorText(Color.white), Width(120), Height(20)))
        {
            _upgradeData.RenewValue();
            Debug.Log("Reset Value Success!");
        }
        if (GUILayout.Button("Show ConsoleLog", ButtonColorText(Color.white), Width(120), Height(20)))
        {
            foreach (var upgradeCustom in _upgradeData.Data)
            {
                Debug.Log($"Level: {upgradeCustom.Level}/EXP: {upgradeCustom.EXP}/Total EXP Cost: {upgradeCustom.TotalExp}");
            }
        }
        GUILayout.EndHorizontal();
        
        scrollView = GUILayout.BeginScrollView(scrollView);
        for (var i = 0; i < _upgradeData.Data.Count; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Box($"{ _upgradeData.Data[i].Level}", Width(150));
            GUILayout.Box(i + 1 >= SO_CharacterUpgradeData.levelMax ? "~" : $"{_upgradeData.Data[i + 1].Level}", BoxColorText(Color.red), Width(140));
            GUILayout.Box($"{ _upgradeData.Data[i].EXP}", BoxColorText(Color.cyan),Width(150));
            GUILayout.Label("  ->  ", Width(30));
            GUILayout.Box($"{_upgradeData.Data[i].TotalExp}" , BoxColorText(Color.magenta), Width(150));
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
    }
    #endregion

    
    #region ENEMIES
    private readonly string[] _enemiesNames = { "Goblin", "BOSS: Reaper" };
    private int _selectedEnemy = -1;
    private readonly string[] _goblinType = { "Goblin: Sword", "Goblin: Slingshot", "Goblin: Daggers" };
    private int _selectedTypeEnemy = -1;
    
    private SO_EnemyConfiguration goblin_SwordConfig;
    private SO_EnemyConfiguration goblin_SlingshotConfig;
    private SO_EnemyConfiguration goblin_DaggersConfig;
    private SO_EnemyConfiguration BOReaperCongfig;
    
    private void HandlePanelEnemies()
    {
        Space(10);
        _selectedEnemy = GUILayout.Toolbar(_selectedEnemy, _enemiesNames, Width(250), Height(35));
        switch (_selectedEnemy)
        {
            case 0:
                Space(10);
                _selectedTypeEnemy = GUILayout.Toolbar(_selectedTypeEnemy, _goblinType,Width(500), Height(50));
                switch (_selectedTypeEnemy)
                {
                    case 0:
                        goblin_SwordConfig = EditorGUIUtility.Load("Assets/Dungeon Delve - PROJECT/Enemies/Goblin/Sword/Prefab/Goblin_Sword Config.asset") as SO_EnemyConfiguration;
                        ShowEnemyConfig(goblin_SwordConfig);
                        break;
                    case 1: 
                        goblin_SlingshotConfig = EditorGUIUtility.Load("Assets/Dungeon Delve - PROJECT/Enemies/Goblin/Slingshot/Prefab/Goblin_Slingshot Config.asset") as SO_EnemyConfiguration;
                        ShowEnemyConfig(goblin_SlingshotConfig);
                        break;
                    case 2: 
                        goblin_DaggersConfig = EditorGUIUtility.Load("Assets/Dungeon Delve - PROJECT/Enemies/Goblin/Daggers/Prefab/Goblin_Daggers Config.asset") as SO_EnemyConfiguration;
                        ShowEnemyConfig(goblin_DaggersConfig);
                        break;
                }
                break;
            
            case 1: 
                BOReaperCongfig = EditorGUIUtility.Load("Assets/Dungeon Delve - PROJECT/Enemies/Reaper/Prefab/BOReaper Config.asset") as SO_EnemyConfiguration;
                ShowEnemyConfig(BOReaperCongfig);
                break;
        }
    }
    private void ShowEnemyConfig(SO_EnemyConfiguration _enemyConfig)
    {
        if (_enemyConfig == null)
        {
            EditorGUILayout.HelpBox("Assign a EnemyConfig.", MessageType.Warning);
            return;
        }

        scrollView = GUILayout.BeginScrollView(scrollView);
        EditorGUI.BeginChangeCheck();

        #region Stats
        Space(30);
        GUILayout.Label("INFORMATION ------------------------", EditorStyles.boldLabel);
        _enemyConfig.SetName(EditorGUILayout.TextField("Name", _enemyConfig.GetName(), Width(500)));
        _enemyConfig.SetInfor(EditorGUILayout.TextField("Infor", _enemyConfig.GetInfor(), Width(500)));
        
        Space(30);
        GUILayout.Label("STATS -------------------------------", EditorStyles.boldLabel);
        _enemyConfig.SetHPRatio(EditorGUILayout.FloatField("Enemy HP Ratio", _enemyConfig.GetHPRatio(), Width(500)));
        _enemyConfig.SetDEFRatio(EditorGUILayout.FloatField("Enemy DEF Ratio", _enemyConfig.GetDEFRatio(), Width(500)));
        _enemyConfig.SetLevelRatio(EditorGUILayout.FloatField("Enemy Level Ratio", _enemyConfig.GetLevelRatio(), Width(500)));
        _enemyConfig.SetATK(EditorGUILayout.IntField("ATK", _enemyConfig.GetATK(), Width(500)));
        _enemyConfig.SetCRITRate(EditorGUILayout.FloatField("CRIT Rate(%)", _enemyConfig.GetCRITRate(), Width(500)));
        _enemyConfig.SetCRITDMG(EditorGUILayout.IntField("CRIT DMG(%)", _enemyConfig.GetCRITDMG(), Width(500)));
        _enemyConfig.SetDEF(EditorGUILayout.IntField("DEF", _enemyConfig.GetDEF(), Width(500)));
        _enemyConfig.SetWalkSpeed(EditorGUILayout.FloatField("Walk Speed", _enemyConfig.GetWalkSpeed(), Width(500)));
        _enemyConfig.SetRunSpeed(EditorGUILayout.FloatField("Run Speed", _enemyConfig.GetRunSpeed(), Width(500)));
        
        Space(30);
        GUILayout.Label("COOLDOWN -------------------------", EditorStyles.boldLabel);
        _enemyConfig.SetNormalAttackCD(EditorGUILayout.FloatField("Normal Attack CD(s)", _enemyConfig.GetNormalAttackCD(), Width(500)));
        _enemyConfig.SetSkillAttackCD(EditorGUILayout.FloatField("Skill Attack CD(s)", _enemyConfig.GetSkillAttackCD(), Width(500)));
        _enemyConfig.SetSpecialAttackCD(EditorGUILayout.FloatField("Special Attack CD(s)", _enemyConfig.GetSpecialAttackCD(), Width(500)));
        #endregion

        Space(30);
        GUILayout.Label("MULTIPLIER -------------------------", EditorStyles.boldLabel);
        
        #region Normal Attack
        Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Label("NORMAL ATTACK MULTIPLIER", Width(200), Height(25));
        for (var i = 1; i <= 100; i++)
        {
            GUILayout.Box($"Lv {i} - {i + 9}", Width(80), Height(25));
            i += 9;
        }
        GUILayout.EndHorizontal();
        for (var j = 0; j < _enemyConfig.GetNormalAttackMultiplier().Count; j++)
        {
            GUILayout.BeginHorizontal();
            _enemyConfig.GetNormalAttackMultiplier()[j].MultiplierTypeName = EditorGUILayout.TextField($"", _enemyConfig.GetNormalAttackMultiplier()[j].MultiplierTypeName ,Width(202), Height(27));
            for (var i = 0; i < _enemyConfig.GetNormalAttackMultiplier()[j].GetMultiplier().Count; i++)
            {
                _enemyConfig.GetNormalAttackMultiplier()[j].GetMultiplier()[i] = 
                    EditorGUILayout.FloatField("", _enemyConfig.GetNormalAttackMultiplier()[j].GetMultiplier()[i],
                        EditorStyles.numberField, Width(80), Height(27));
                Space(1);
            }
            GUILayout.EndHorizontal();
        }
        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("+", Width(45), Height(25)))
        {
            _enemyConfig.AddNormalAttackMultiplier();
        }
        if(GUILayout.Button("-", Width(45), Height(25)) && _enemyConfig.GetNormalAttackMultiplier().Count != 0)
        {
            _enemyConfig.RemoveNormalAttackMultiplier();
        }
        GUILayout.EndHorizontal();
        #endregion
        
        #region Elemental Skill
        Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Label("ELEMENTAL SKILL MULTIPLIER", Width(200), Height(25));
        for (var i = 1; i <= 100; i++)
        {
            GUILayout.Box($"Lv {i} - {i + 9}", Width(80), Height(25));
            i += 9;
        }
        GUILayout.EndHorizontal();
        for (var j = 0; j < _enemyConfig.GetElementalSkillMultiplier().Count; j++)
        {
            GUILayout.BeginHorizontal();
            _enemyConfig.GetElementalSkillMultiplier()[j].MultiplierTypeName = EditorGUILayout.TextField($"", _enemyConfig.GetElementalSkillMultiplier()[j].MultiplierTypeName ,Width(202), Height(27));
            for (var i = 0; i < _enemyConfig.GetElementalSkillMultiplier()[j].GetMultiplier().Count; i++)
            {
                _enemyConfig.GetElementalSkillMultiplier()[j].GetMultiplier()[i] = 
                    EditorGUILayout.FloatField("", _enemyConfig.GetElementalSkillMultiplier()[j].GetMultiplier()[i],
                        EditorStyles.numberField, Width(80), Height(27));
                Space(1);
            }
            GUILayout.EndHorizontal();
        }
        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("+", Width(45), Height(25)))
        {
            _enemyConfig.AddElementalSkillMultiplier();
        }
        if(GUILayout.Button("-", Width(45), Height(25)) && _enemyConfig.GetElementalSkillMultiplier().Count != 0)
        {
            _enemyConfig.RemoveElementalSkillMultiplier();
        }
        GUILayout.EndHorizontal();
        #endregion
        
        #region Elemental Burst
        Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Label("ELEMENTAL BURST MULTIPLIER", Width(200), Height(25));
        for (var i = 1; i <= 100; i++)
        {
            GUILayout.Box($"Lv {i} - {i + 9}", Width(80), Height(25));
            i += 9;
        }
        GUILayout.EndHorizontal();
        for (var j = 0; j < _enemyConfig.GetElementalBurstMultiplier().Count; j++)
        {
            GUILayout.BeginHorizontal();
            _enemyConfig.GetElementalBurstMultiplier()[j].MultiplierTypeName = EditorGUILayout.TextField($"", _enemyConfig.GetElementalBurstMultiplier()[j].MultiplierTypeName ,Width(202), Height(27));
            for (var i = 0; i < _enemyConfig.GetElementalBurstMultiplier()[j].GetMultiplier().Count; i++)
            {
                _enemyConfig.GetElementalBurstMultiplier()[j].GetMultiplier()[i] = 
                    EditorGUILayout.FloatField("", _enemyConfig.GetElementalBurstMultiplier()[j].GetMultiplier()[i],
                        EditorStyles.numberField, Width(80), Height(27));
                Space(1);
            }
            GUILayout.EndHorizontal();
        }
        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("+", Width(45), Height(25)))
        {
            _enemyConfig.AddElementalBurstMultiplier();
        }
        if(GUILayout.Button("-", Width(45), Height(25)) && _enemyConfig.GetElementalBurstMultiplier().Count != 0)
        {
            _enemyConfig.RemoveElementalBurstMultiplier();
        }
        GUILayout.EndHorizontal();
        #endregion
        
        if(EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(_enemyConfig);
        GUILayout.EndScrollView();
    }
    #endregion


    #region GAMECUSTOM
    private int _selectedPanelGameCustomType = -1;
    private readonly string[] _gameCustomtype = { "ITEM DATA" , "CHARACTER DATA", "QUESTS", "SHOP"};
    
    private SO_GameItemData _itemData;
    private SO_CharacterData _characterData;
    private QuestSetup[] _questSetups;
    private ShopItemSetup[] _shopItemSetups;
    
    private void HandlePanelGameCustom()
    {
        Space(15);
        _selectedPanelGameCustomType = GUILayout.Toolbar(_selectedPanelGameCustomType, _gameCustomtype, Width(350), Height(35));
        _itemData= EditorGUIUtility.Load("Assets/Dungeon Delve - PROJECT/GameData/Game Item Data.asset") as SO_GameItemData;
        _characterData= EditorGUIUtility.Load("Assets/Dungeon Delve - PROJECT/GameData/Character Data.asset") as SO_CharacterData;
        switch (_selectedPanelGameCustomType)
        {
            case 0:
                ShowItemsDetails(_itemData);
                break;
            case 1:
                ShowCharacterData(_characterData);
                break;
            case 2:
                CustomQuest();
                break;
            case 3:
                CustomItemPurchase();
                break;
        }
    }
    private void ShowItemsDetails(SO_GameItemData _gameItemData)
    {
        if(_gameItemData == null) return;
        
        EditorGUI.BeginChangeCheck();
        
        Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Label( "",  Width(100));
        GUILayout.Box( "Name Code", BoxColorText(Color.red), Width(150));
        GUILayout.Box( "Name Item", BoxColorText(Color.red), Width(150));
        GUILayout.Box( "Description", BoxColorText(Color.red), Width(150));
        GUILayout.Box( "Sprite", BoxColorText(Color.red), Width(150));
        GUILayout.Box( "Rarity", BoxColorText(Color.red), Width(150));
        GUILayout.Box( "Type", BoxColorText(Color.red), Width(150));
        GUILayout.EndHorizontal();
        
        scrollView = GUILayout.BeginScrollView(scrollView);
        GUILayout.BeginVertical();
        foreach (var _itemData in _gameItemData.GameItemDatas)
        {
            GUILayout.BeginHorizontal(GUI.skin.box);
            
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField( "CUSTOM", LabelColorText(Color.yellow), Width(100));
            _itemData.code = (ItemNameCode)EditorGUILayout.EnumPopup("", _itemData.code, Width(150));
            _itemData.nameItem = EditorGUILayout.TextField("", _itemData.nameItem,Width(150));
            _itemData.description = EditorGUILayout.TextField("", _itemData.description,Width(150));
            _itemData.sprite = (Sprite)EditorGUILayout.ObjectField(_itemData.sprite, typeof(Sprite), false, Width(150), Height(150));
            _itemData.ratity = (ItemRarity)EditorGUILayout.EnumPopup("", _itemData.ratity, Width(150));
            _itemData.type = (ItemType)EditorGUILayout.EnumPopup("", _itemData.type, Width(150));
            GUILayout.EndHorizontal();
            
            GUILayout.BeginVertical();
            EditorGUILayout.LabelField( "   STATS", LabelColorText(Color.cyan));
            EditorGUILayout.LabelField($"   NameCode: {_itemData.code}", Width(550));
            EditorGUILayout.LabelField($"   Name: {_itemData.nameItem}", Width(550));
            EditorGUILayout.LabelField($"   Rarity: {_itemData.ratity}", Width(150));
            EditorGUILayout.LabelField($"   Type: {_itemData.type}", Width(150));
            var _desToStr = " Description: " + _itemData.description;
            var lines = _desToStr.Split('.');
            foreach (var line in lines)
            {
                EditorGUILayout.LabelField("  " + line);
            }
            GUILayout.EndVertical();
            
            GUILayout.EndHorizontal();
            Space(10);
        }
        
        GUILayout.EndVertical();
        GUILayout.EndScrollView();
        
        Space(20);
        EditorGUI.DrawRect(GUILayoutUtility.GetRect(1, 2), Color.white);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("+", Width(45), Height(25)))
        {
            _gameItemData.GameItemDatas.Add(new ItemCustom());
        }
        GUILayout.Box("Add new Item");
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("-", Width(45), Height(25)) && _gameItemData.GameItemDatas.Count != 0)
        {
            _gameItemData.GameItemDatas.RemoveAt(_gameItemData.GameItemDatas.Count - 1);
        }
        GUILayout.Box("Remove Item");
        GUILayout.EndHorizontal();
        
        if(EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(_gameItemData);
    }
    private void ShowCharacterData(SO_CharacterData _characterData)
    {
        if(_characterData == null) return;
        
        EditorGUI.BeginChangeCheck();
        
        Space(10);
        
        GUILayout.BeginHorizontal();
        GUILayout.Label("NameCode", Width(150));
        GUILayout.Label("", Width(30));
        GUILayout.Label("Player Prefab", Width(150));
        GUILayout.EndHorizontal();
        
        scrollView = GUILayout.BeginScrollView(scrollView);
        foreach (var characterCustom in _characterData.CharactersData)
        {
            GUILayout.BeginHorizontal();
            characterCustom.nameCode = (CharacterNameCode)EditorGUILayout.EnumPopup("", characterCustom.nameCode, Width(150));
            GUILayout.Label("  ->  ", Width(30));
            characterCustom.prefab = (PlayerController)EditorGUILayout.ObjectField(characterCustom.prefab, typeof(PlayerController), false, Width(200));
            GUILayout.EndHorizontal();
            Space(10);
        }
        GUILayout.EndScrollView();
        
        Space(20);
        EditorGUI.DrawRect(GUILayoutUtility.GetRect(1, 2), Color.white);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("+", Width(45), Height(25)))
        {
            _characterData.CharactersData.Add(new CharacterCustom());
        }
        GUILayout.Box("Add new Character");
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("-", Width(45), Height(25)) && _characterData.CharactersData.Count != 0)
        {
            _characterData.CharactersData.RemoveAt(_characterData.CharactersData.Count - 1);
        }
        GUILayout.Box("Remove Character");
        GUILayout.EndHorizontal();
        
        if(EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(_characterData);
    }
    private void CustomQuest()
    {
        _questSetups = Resources.LoadAll<QuestSetup>("Quest Custom");
        
        var _count = 0;
        if (_questSetups != null && _questSetups.Any())
        {
            Space(25);
            GUILayout.Label( "CUSTOM QUESTS", BoxColorText(Color.red), Width(150));
            Space(5);

            scrollView = GUILayout.BeginScrollView(scrollView);
            foreach (var questSetup in _questSetups)
            {
                GUILayout.BeginVertical(GUI.skin.box);
                EditorGUI.BeginChangeCheck();
                questSetup.SetTitle(EditorGUILayout.TextField("Title", questSetup.GetTitle(), Width(1000)));
                questSetup.SetDescription(EditorGUILayout.TextField("Description", questSetup.GetDescription(), Width(1000)));

                #region Quest Requirement
                Space(15);
                GUILayout.BeginHorizontal();
                GUILayout.Label( "Quest Requirement", LabelColorText(Color.yellow), Width(150));
                GUILayout.Label( "Item Needed", BoxColorText(Color.cyan), Width(150));
                GUILayout.Label( "Item Value", BoxColorText(Color.cyan), Width(150));
                GUILayout.EndVertical();
                
                GUILayout.BeginHorizontal();
                GUILayout.Label( "", Width(150));
                var _taskRequired = questSetup.GetRequirement();
                _taskRequired.SetNameCode((ItemNameCode)EditorGUILayout.EnumPopup("", _taskRequired.GetNameCode(), Width(150)));
                _taskRequired.SetValue(EditorGUILayout.IntField("", _taskRequired.GetValue(), Width(150)));
                GUILayout.EndHorizontal();
                #endregion
                
                #region Reward Field
                Space(20);
                GUILayout.BeginHorizontal();
                GUILayout.Label( "Rewards Setup", LabelColorText(Color.yellow), Width(150));
                GUILayout.Label( "Item Reward", BoxColorText(Color.cyan), Width(150));
                GUILayout.Label( "Item Rarity", BoxColorText(Color.cyan), Width(150));
                GUILayout.Label( "Item Value", BoxColorText(Color.cyan), Width(150));
                GUILayout.EndVertical();
                var _currentReward = questSetup.GetRewards();
                for (var i = 0; i < _currentReward.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label( "", Width(150));
                    _currentReward[i].SetNameCode((ItemNameCode)EditorGUILayout.EnumPopup("", _currentReward[i].GetNameCode(), Width(150)));
                    GUILayout.Box($"{ _currentReward[i].GetRarity()}", Width(150));   
                    _currentReward[i].SetValue(EditorGUILayout.IntField("", _currentReward[i].GetValue(), Width(150)));
                    if (GUILayout.Button("-", Width(30), Height(18)))
                    {
                        if (_currentReward.Contains(_currentReward[i]))
                        {
                            _currentReward.Remove(_currentReward[i]);
                            i--;
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                
                foreach (var itemReward in _currentReward)
                {
                    var _itemCustom = _itemData.GetItemCustom(itemReward.GetNameCode());
                    itemReward.SetRarity(_itemCustom.ratity);
                    
                }
                
                #region Button Add/Remove
                Space(15);
                if (GUILayout.Button("Add Reward", Width(120), Height(25)))
                {
                    _currentReward.Add(new ItemReward());
                }
                if (GUILayout.Button("Remove Reward", Width(120), Height(25)))
                {
                    if (_currentReward.Any())
                    {
                        _currentReward.Remove(_currentReward[^1]);
                    }
                }
                #endregion
                #endregion

                if(EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(questSetup);
                GUILayout.EndVertical();
                EditorGUI.DrawRect(GUILayoutUtility.GetRect(1, 1), Color.gray);
                _count++;
            }
            GUILayout.EndScrollView();
            Space(20);
        }
        else
        {
            EditorGUILayout.HelpBox("Assign a object.", MessageType.Info);
        }
        
        #region Create/Remove Quest
        Space(20);
        EditorGUI.DrawRect(GUILayoutUtility.GetRect(1, 2), Color.white);
        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("+", Width(45), Height(25)))
        {
            var _path = $"Assets/Resources/Quest Custom/Quest_{_count + 1}.asset";
            var _newQuest = CreateInstance<QuestSetup>();
            _newQuest.SetReward(new List<ItemReward>());
            AssetDatabase.CreateAsset(_newQuest, _path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        GUILayout.Box("New Quest");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("-", Width(45), Height(25)))
        {
            var _path = $"Assets/Resources/Quest Custom/Quest_{_count}.asset";
            var _instanceDel = AssetDatabase.LoadAssetAtPath<QuestSetup>(_path);
            if (_instanceDel == null) return;
            AssetDatabase.DeleteAsset(_path);
            AssetDatabase.Refresh();
        }
        GUILayout.Box("Remove Quest");
        GUILayout.EndHorizontal();
        #endregion
    }
    private void CustomItemPurchase()
    {
        _shopItemSetups = Resources.LoadAll<ShopItemSetup>("Shop Custom");
        var _count = 0;
        if (_itemData == null) return;
        if (_shopItemSetups != null && _shopItemSetups.Any())
        {
            Space(25);
            GUILayout.Label( "CUSTOM ITEM PURCHASE", BoxColorText(Color.red), Width(250));
            Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Label( "Index", BoxColorText(Color.yellow), Width(70));
            GUILayout.Label( "Name Code", BoxColorText(Color.yellow), Width(150));
            GUILayout.Label( "Rarity", BoxColorText(Color.yellow), Width(150));
            GUILayout.Label( "Price", BoxColorText(Color.yellow), Width(135));
            GUILayout.Label( "Quantity Receive", BoxColorText(Color.yellow), Width(135));
            GUILayout.Label( "Purchase Max in Day", BoxColorText(Color.yellow), Width(135));
            GUILayout.EndHorizontal();
            Space(7);
            scrollView = GUILayout.BeginScrollView(scrollView);
            foreach (var _itemSetup in _shopItemSetups)
            {
                EditorGUI.BeginChangeCheck();
                
                GUILayout.BeginHorizontal();
                GUILayout.Box( $"{_count}", Width(70));
                _itemSetup.SetItemCode((ItemNameCode)EditorGUILayout.EnumPopup("", _itemSetup.GetItemCode(), Width(150)));
                var _itemCustom = _itemData.GetItemCustom(_itemSetup.GetItemCode());
                _itemSetup.SetRarity(_itemCustom.ratity);
                _itemSetup.SetPurchaseCurrent(0);
                GUILayout.Box( $"{_itemSetup.GetRarity()}", Width(150));
                _itemSetup.SetPrice(EditorGUILayout.IntField("", _itemSetup.GetPrice(), Width(135)));
                _itemSetup.SetQuantityReceive(EditorGUILayout.IntField("", _itemSetup.GetQuantityReceive(), Width(135)));
                _itemSetup.SetPurchaseMax(EditorGUILayout.IntField("", _itemSetup.GetPurchaseMax(), Width(135)));
               
                GUILayout.EndHorizontal();
                if(EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(_itemSetup);
                Space(2);
                _count++;
            }
            GUILayout.EndScrollView();
        }
        
        #region Create/Remove Quest
        Space(20);
        EditorGUI.DrawRect(GUILayoutUtility.GetRect(1, 2), Color.white);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("+", Width(45), Height(25)))
        {
            var _path = $"Assets/Resources/Shop Custom/Purchase_{_count + 1}.asset";
            var _newItemPurchase = CreateInstance<ShopItemSetup>();
            AssetDatabase.CreateAsset(_newItemPurchase, _path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        GUILayout.Box("New Item Purchase");
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("-", Width(45), Height(25)))
        {
            var _path = $"Assets/Resources/Shop Custom/Purchase_{_count}.asset";
            var _instanceDel = AssetDatabase.LoadAssetAtPath<ShopItemSetup>(_path);
            if (_instanceDel == null) return;
            AssetDatabase.DeleteAsset(_path);
            AssetDatabase.Refresh();
        }
        GUILayout.Box("Remove Item Purchase");
        GUILayout.EndHorizontal();
        #endregion
    }
    #endregion



    private static GUIStyle BoxColorText(Color _color) => new(GUI.skin.box) { normal = { textColor = _color } };
    private static GUIStyle LabelColorText(Color _color) => new(GUI.skin.label) { normal = { textColor = _color } };
    private static GUIStyle ButtonColorText(Color _color) => new(GUI.skin.button) { normal = { textColor = _color } };
    private static GUIStyle TextFieldColorText(Color _color) => new(EditorStyles.textField) { normal = { textColor = _color } };
    
    
    private static void Space(float space) => GUILayout.Space(space);
    private static GUILayoutOption Width(float width) => GUILayout.Width(width);
    private static GUILayoutOption Height(float height) => GUILayout.Height(height);

}
