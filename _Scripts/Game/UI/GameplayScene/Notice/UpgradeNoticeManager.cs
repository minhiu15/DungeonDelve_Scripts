using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeNoticeManager : Singleton<UpgradeNoticeManager>
{
    [SerializeField] private Animator animator;
    [SerializeField] private Button confirmBtt;
    [SerializeField] private TextMeshProUGUI levelText;
    [Space]
    [SerializeField] private TextBar_2 textBarPrefab;
    [SerializeField] private Transform textContent;

    private static ObjectPooler<TextBar_2> _poolTextBar;

    
    private void Start()
    {
        _poolTextBar = new ObjectPooler<TextBar_2>(textBarPrefab, textContent, 10);
        confirmBtt.onClick.AddListener(DisableNotice);
    }
    private void OnDestroy()
    {
        confirmBtt.onClick.RemoveListener(DisableNotice);
    }
    


    /// <summary>
    /// Set Title Level của type đang upgrade: Level Character, Level Weapon ?
    /// </summary>
    /// <param name="_value"></param>
    public void SetLevelText(string _value) => levelText.text = _value;
    
    /// <summary>
    /// Tạo 1 text thông báo với tiêu đề và 2 giá trị value tương ứng giá trị cũ và mới của stats sau khi upgrade
    /// </summary>
    /// <param name="_title"> Tiêu đề textBar </param>
    /// <param name="_value1"> Giá trị cũ </param>
    /// <param name="_value2"> Giá trị mới </param>
    public static void CreateNoticeBar(string _title, string _value1, string _value2)
    {
        var textBar = _poolTextBar.Get();
        textBar.SetTitleText(_title);
        textBar.SetValueText(_value1);
        textBar.SetValueText2(_value2);
    }
    
    
    public void EnableNotice() => animator.Play("OnEnableUpgradeSuccess");
    private void DisableNotice()
    {
        animator.Play("OnDisableUpgradeSuccess");
        foreach (var textBar in _poolTextBar.List)
        {
            textBar.Release();
        }
    }


}
