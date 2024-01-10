using UnityEngine;

[CreateAssetMenu(menuName = "Buff Effect/Stamina Buff", fileName = "Stamina_BU")]
public class SO_StaminaBuff : SO_BuffEffect
{
    public override void Apply(PlayerController _player)
    {   
        _player.Stamina.Increases(Mathf.CeilToInt(Value));
    }
    
}
