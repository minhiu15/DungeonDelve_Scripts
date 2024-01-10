using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Buff Effect/Defense Buff", fileName = "DEF_BU")]
public class SO_DefenseBuff : SO_BuffEffect
{
    [field: Tooltip("Thời gian hủy Buff (s)"), SerializeField] public float buffTimeOut { get; private set; }

    private Coroutine _buffCoroutine;
    private int _currentDEF;
    
    public override void Apply(PlayerController _player)
    {
        if (_buffCoroutine != null)
        {
            return;
        }
        _currentDEF = _player.PlayerConfig.GetDEF();
        var _valueBonus = _currentDEF * Value;
        _player.PlayerConfig.SetDEF(Mathf.CeilToInt(_currentDEF + _valueBonus));
        _buffCoroutine = _player.StartCoroutine(CooldownDeBuff(_player));
    }
    
    private IEnumerator CooldownDeBuff(PlayerStateMachine _player)
    {
        yield return new WaitForSeconds(buffTimeOut);
        _player.PlayerConfig.SetDEF(_currentDEF);
        _buffCoroutine = null;
    }
    
}
