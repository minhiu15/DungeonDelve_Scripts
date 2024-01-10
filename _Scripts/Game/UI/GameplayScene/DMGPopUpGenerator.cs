using NaughtyAttributes;
using UnityEngine;

public class DMGPopUpGenerator : Singleton<DMGPopUpGenerator>
{
    [SerializeField] private DMGPopUp dmgPopUpPrefab;
    
    private Camera _mainCamera;
    private ObjectPooler<DMGPopUp> _poolDMGPopUp;
    
    private void Start()
    {
        _mainCamera = Camera.main;
        _poolDMGPopUp = new ObjectPooler<DMGPopUp>(dmgPopUpPrefab, transform, 25);
        foreach (var VARIABLE in _poolDMGPopUp.List)
        {
            VARIABLE.mainCamera = _mainCamera;
        }
    }


    /// <summary>
    /// Tạo 1 DMG PopUp
    /// </summary>
    /// <param name="_position"> Vị trí xuất hiện </param>
    /// <param name="_damage"> Sát thương cập nhật cho Text </param>
    /// <param name="_isCRIT"> Sát thượng có kích bạo không ? </param>
    /// <param name="_isEnemy"> Char vừa nhận sát thương có phải Enemy ? </param>
    public void Create(Vector3 _position, int _damage, bool _isCRIT, bool _isEnemy)
    {
        var randPos = Random.insideUnitCircle * 0.5f;
        _position += new Vector3(randPos.x, randPos.y, 0);
        
        var dmgPopUp = _poolDMGPopUp.Get(_position);
        dmgPopUp.Show(_damage, _isCRIT, _isEnemy);
    }
    
}
