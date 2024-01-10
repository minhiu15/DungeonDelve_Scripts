using UnityEngine;

[CreateAssetMenu(menuName = "Buff Effect/Health Buff", fileName = "Health_BU")]
public class SO_HealthBuff : SO_BuffEffect
{
    
    public override void Apply(PlayerController _player)
    {
        _player.Health.Increases(Mathf.CeilToInt(Value));    
    }
}
