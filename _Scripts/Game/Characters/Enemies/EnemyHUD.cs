using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class EnemyHUD : MonoBehaviour
{
    [SerializeField, Required] private EnemyController enemy;
    [SerializeField] private ProgressBar healthBar;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    
    private void Start()
    {
        if(nameText) nameText.text = enemy.EnemyConfig.GetName();
        
        enemy.Health.OnInitValueEvent += InitEnemyState;
        enemy.Health.OnCurrentValueChangeEvent += healthBar.OnCurrentValueChange;
    }
    private void OnDestroy()
    {
        enemy.Health.OnInitValueEvent -= InitEnemyState;
        enemy.Health.OnCurrentValueChangeEvent -= healthBar.OnCurrentValueChange;
    }
    private void InitEnemyState(int _currentValue, int _maxValue)
    {
        healthBar.Init(_currentValue, _maxValue);
        levelText.text = $"Lv. {enemy.EnemyConfig.GetLevel()}";
    }

    
}
