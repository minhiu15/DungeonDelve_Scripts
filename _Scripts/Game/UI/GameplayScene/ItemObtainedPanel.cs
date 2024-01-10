using System.Collections;
using UnityEngine;

public class ItemObtainedPanel : Singleton<ItemObtainedPanel>
{
    [SerializeField] private Animator animator;
    [SerializeField] private UI_Item uiItem;
    [SerializeField] private float disableTime;
    
    private Coroutine _disableCoroutine;
    private WaitForSecondsRealtime _yieldInstruction;
    private bool _isActive;
        
    private void Start()
    {
        animator.Play("Panel_Disable");
        _yieldInstruction = new WaitForSecondsRealtime(disableTime);
    }
    public void ClosePanel()
    {
        animator.Play("Panel_OUT");
        _isActive = false;
    }
    public void OpenPanel(ItemCustom _itemCustom, int _value)
    {
        animator.Play("Panel_IN");
        uiItem.SetItem(_itemCustom, _value);
        uiItem.SetValueText($"{_value}");
        _isActive = true;
        
        if (_disableCoroutine != null) StopCoroutine(_disableCoroutine);
        _disableCoroutine = StartCoroutine(DisableCoroutine());
    }

    private IEnumerator DisableCoroutine()
    {
        yield return _yieldInstruction;
        if (!_isActive) yield break;
        ClosePanel();
    }
    
}
