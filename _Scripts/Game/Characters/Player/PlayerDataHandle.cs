using Unity.Mathematics;
using UnityEngine;

public class PlayerDataHandle : MonoBehaviour
{
    
    /// <summary> Dữ liệu khi nâng cấp vũ khí: số lượng/type item, coin,... cần để nâng cấp </summary>
    [field: SerializeField] public SO_RequiresWeaponUpgradeConfiguration WeaponUpgradeConfig { get; private set; }
    
    
    /// <summary> Toàn bộ cấu hình của nhân vật: HP, ST, ATK, ..... </summary>
    [field: SerializeField] public SO_PlayerConfiguration PlayerConfig { get; private set; }


    /// <summary> RenderTexture(RawImage) -> Render các model của nhân vật lên UI </summary>
    public PlayerRenderTexture PlayerRenderTexture { get; private set; }

    [SerializeField] private PlayerRenderTexture playerRenderTexturePrefab;


    private void OnEnable()
    {
        PlayerRenderTexture = Instantiate(playerRenderTexturePrefab, null);
        PlayerRenderTexture.CloseRenderUI();
    }
    
    
    /// <summary>
    /// Cập nhật DataConfig vào nhân vật
    /// </summary>
    public void SetConfiguration(SO_PlayerConfiguration _playerConfig)
    {
        PlayerConfig = _playerConfig;
    }
    
}
