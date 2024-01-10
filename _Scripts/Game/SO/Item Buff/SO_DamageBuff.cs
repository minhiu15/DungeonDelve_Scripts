using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Buff Effect/Damage Buff", fileName = "DMG_BU")]
public class SO_DamageBuff : SO_BuffEffect
{
    [field: Tooltip("Thời gian hủy Buff (s)"), SerializeField] public float buffTimeOut { get; private set; }

    private Coroutine _buffCoroutine;
    private int _currentDMG;
    
    public override void Apply(PlayerController _player)
    {
        if (_buffCoroutine != null)
        {
            return;
        }
        _currentDMG = _player.PlayerConfig.GetATK();
        var _valueBonus = _currentDMG * Value;
        _player.PlayerConfig.SetATK(Mathf.CeilToInt(_currentDMG + _valueBonus));
        _buffCoroutine = _player.StartCoroutine(CooldownDeBuff(_player));
    }

    private IEnumerator CooldownDeBuff(PlayerStateMachine _player)
    {
        yield return new WaitForSeconds(buffTimeOut);
        _player.PlayerConfig.SetATK(_currentDMG);
        _buffCoroutine = null;
    }
    
}
