using System;
using System.Threading.Tasks;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour, IGUI
{
    [SerializeField, Required] private PlayerController player;
    [Space]
    [SerializeField] private Animator hudAnimator;
    [Space]
    [SerializeField] private ProgressBar healthBar;
    [SerializeField] private ProgressBar staminaBar;
    [SerializeField] private CooldownTime elementalSkillCD;
    [SerializeField] private CooldownTime elementalBurstCD;
    [SerializeField] private CooldownTime revivalTimeCD;
    [Space] 
    [SerializeField] private Image chapterIcon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Slider expProgress;
    //
    [SerializeField, BoxGroup("UI")] private Transform slotContent;
    
    private SO_CharacterUpgradeData _characterUpgradeData;
    public Slot[] slots { get; private set; } = Array.Empty<Slot>();
    private bool _isEventRegistered;
    private bool _isRevival;
    
    
    private void OnEnable()
    {
        RegisterEvent();
        OpenHUD();
    }
    private void Start()
    {
        nameText.text = player.PlayerConfig.GetName();
        chapterIcon.sprite = player.PlayerConfig.ChapterIcon;
        expProgress.minValue = 0;
        revivalTimeCD.gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        UnRegisterEvent();
    }


    private void RegisterEvent()
    {
        GUI_Manager.Add(this);
        GUI_Bag.OnInitSlotEvent += CreateSlot;
        GUI_Bag.OnItemChangedSlotEvent += UpdateItemSlot;

        player.OnRevivalTimeEvent += ActiveRevivalPanel;
        player.OnElementalSkillCDEvent += elementalSkillCD.StartCooldownTime;
        player.OnElementalBurstCDEvent += elementalBurstCD.StartCooldownTime;

        player.Health.OnInitValueEvent += healthBar.Init;
        player.Health.OnCurrentValueChangeEvent += healthBar.OnCurrentValueChange;
        player.Health.OnMaxValueChangeEvent += healthBar.OnMaxValueChange;

        player.Stamina.OnInitValueEvent += staminaBar.Init;
        player.Stamina.OnCurrentValueChangeEvent += staminaBar.OnCurrentValueChange;
        player.Stamina.OnMaxValueChangeEvent += staminaBar.OnMaxValueChange;
    }
    private void UnRegisterEvent()
    {
        GUI_Manager.Remove(this);
        GUI_Bag.OnInitSlotEvent -= CreateSlot;
        GUI_Bag.OnItemChangedSlotEvent -= UpdateItemSlot;
        
        player.OnRevivalTimeEvent -= ActiveRevivalPanel; 
        player.OnElementalSkillCDEvent -= elementalSkillCD.StartCooldownTime;
        player.OnElementalBurstCDEvent -= elementalBurstCD.StartCooldownTime;
        
        player.Health.OnInitValueEvent -= healthBar.Init;
        player.Health.OnCurrentValueChangeEvent -= healthBar.OnCurrentValueChange;
        player.Health.OnMaxValueChangeEvent -= healthBar.OnMaxValueChange;
        
        player.Stamina.OnInitValueEvent -= staminaBar.Init;
        player.Stamina.OnCurrentValueChangeEvent -= staminaBar.OnCurrentValueChange;
        player.Stamina.OnMaxValueChangeEvent -= staminaBar.OnMaxValueChange;
    }

    private void CreateSlot(Slot[] _slots)
    {
        slots = new Slot[_slots.Length];
        for (var i = 0; i < _slots.Length; i++)
        {
            slots[i] = Instantiate(_slots[i], slotContent);
        }
    }
    private void UpdateItemSlot(Slot[] _slots)
    {
        for (var i = 0; i < _slots.Length; i++)
        {
            slots[i].SetSlot(_slots[i].Item);
        }
    }

    
    public void GetRef(GameManager _gameManager)
    {
        _characterUpgradeData = _gameManager.CharacterUpgradeData;
        UpdateData();
    }
    public void UpdateData()
    {
        var _currentLevel = player.PlayerConfig.GetLevel();
        levelText.text = $"Lv. {_currentLevel}";
        expProgress.maxValue = _characterUpgradeData.GetNextEXP(_currentLevel);
        expProgress.value = player.PlayerConfig.GetCurrentEXP();
    }

    private void ActiveRevivalPanel(float _time)
    {
        if (_time == 0)
        {
            revivalTimeCD.StartCooldownTime(0);
            revivalTimeCD.gameObject.SetActive(false);
            return;
        }
        revivalTimeCD.gameObject.SetActive(true);
        revivalTimeCD.StartCooldownTime(_time);
    }
    public async void OpenHUD()
    {
        hudAnimator.Play("Panel_IN");
        await Task.Delay(100);
        elementalSkillCD.ContinueCooldownTime();
        elementalBurstCD.ContinueCooldownTime();
         foreach (var slot in slots)
         {
             slot.cooldownTime.ContinueCooldownTime();
         }
    }
    public void CloseHUD()
    {
        hudAnimator.Play("Panel_OUT");
    }
}
