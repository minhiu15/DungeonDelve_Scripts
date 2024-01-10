using UnityEngine;

public class PlayerRenderTexture : MonoBehaviour
{
    public RenderTexture renderTexture;
    [SerializeField] private Animator animator;
    [SerializeField] private Camera camCharRender;
    [SerializeField] private Camera camWeaponRender;
    
    
    public enum RenderType
    {
        Character,
        Weapon
    }
    
    public void OpenRenderUI(RenderType _renderType)
    {
        gameObject.SetActive(true);
        camCharRender.gameObject.SetActive(_renderType == RenderType.Character);
        camWeaponRender.gameObject.SetActive(_renderType == RenderType.Weapon);
        animator.updateMode = _renderType == RenderType.Character ? AnimatorUpdateMode.UnscaledTime : AnimatorUpdateMode.Normal;
    }
    public void CloseRenderUI()
    {
        camCharRender.gameObject.SetActive(false);
        camWeaponRender.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
    


}
