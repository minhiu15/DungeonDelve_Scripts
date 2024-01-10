using UnityEngine;

public interface IAttack
{
    /// <summary>
    /// Phát hiện va chạm của Normal Attack
    /// </summary>  
    public void Detection_NA(GameObject _gameObject);

    /// <summary>
    /// Phát hiện va chạm của Charged Attack
    /// </summary>  
    public void Detection_CA(GameObject _gameObject);

    /// <summary>
    /// Phát hiện va chạm của Elemental Skill
    /// </summary>  
    public void Detection_ES(GameObject _gameObject);

    /// <summary>
    /// Phát hiện va chạm của Elemental Burst
    /// </summary>  
    public void Detection_EB(GameObject _gameObject);

}
