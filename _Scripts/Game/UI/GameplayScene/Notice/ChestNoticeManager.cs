using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChestNoticeManager : MonoBehaviour
{
    [SerializeField] private IconIndicator indicatorPrefab;
    [SerializeField, Tooltip("Giới hạn cạnh Left và Right mà Indicator được hiển thị")] 
    private float borderWidthSize;
    [SerializeField, Tooltip("Giới hạn cạnh Top và Bottom mà Indicator được hiển thị")]
    private float borderHeightSize;
    
    private Camera _mainCam;
    private Vector3 _chestScreenPoint;
    private Vector3 _chestScreenPointNoOffset;
    private readonly Vector3 _offsetIndicator = new(0, 1.75f, 0);
    private static Dictionary<Chest, IconIndicator> _chests;
    private static ObjectPooler<IconIndicator> _poolIndicator;
    
    private void Awake()
    {
        _mainCam = Camera.main;
        _chests = new Dictionary<Chest, IconIndicator>();
    }
    private void Start()
    {
        _poolIndicator = new ObjectPooler<IconIndicator>(indicatorPrefab, transform, 5);
    }
    private void LateUpdate()
    {
        if(!_chests.Any()) return;

        foreach (var (key, value) in _chests)
        {
            _chestScreenPoint = _mainCam.WorldToScreenPoint(key.transform.position + _offsetIndicator);
            _chestScreenPointNoOffset = _mainCam.WorldToScreenPoint(key.transform.position);
            
            _chestScreenPoint.z = 0;
            var isOffScreen = _chestScreenPoint.x - borderWidthSize <= 0 || _chestScreenPoint.x + borderWidthSize >= Screen.width ||
                              _chestScreenPoint.y - borderHeightSize <= 0 || _chestScreenPoint.y + borderHeightSize >= Screen.height ;

            if (isOffScreen) // Ra khỏi màn hình
            {
                OffScreenIndicator(value);
            }
            else
            {
                OnScreenIndicator(value);
            }
        }
    }
    

    private void OffScreenIndicator(IconIndicator _indicator) 
    {
        var screenBound = new Vector2(Screen.width - borderWidthSize, Screen.height - borderHeightSize);
        var clampScreenPoinnt = new Vector3(Mathf.Clamp(_chestScreenPoint.x, borderWidthSize, screenBound.x),
                                            Mathf.Clamp(_chestScreenPoint.y, borderHeightSize, screenBound.y),
                                            0f);
        
        _indicator.iconIndicator.position = clampScreenPoinnt;
        _indicator.arrowIndicator.gameObject.SetActive(true);

        var directionToChest = _chestScreenPointNoOffset - clampScreenPoinnt;
        directionToChest.Normalize();

        _indicator.arrowIndicator.position = clampScreenPoinnt + directionToChest * 50f;
        _indicator.arrowIndicator.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(directionToChest.y, directionToChest.x) * Mathf.Rad2Deg);
    }
    private void OnScreenIndicator(IconIndicator _indicator)
    {
       _indicator.iconIndicator.position = _chestScreenPoint;
       _indicator.arrowIndicator.gameObject.SetActive(false);
    }
    
    //
    public static void AddChest(Chest _chest)
    {
        if (_chests.ContainsKey(_chest)) 
            return;
        
        var _indi = _poolIndicator.Get();
        _chest.Indicator = _indi;
        _chests.Add(_chest, _indi);
    }
    public static void RemoveChest(Chest _chest)
    {
        if (!_chests.ContainsKey(_chest)) 
            return;
        
        _chest.Indicator.Release();
        _chests.Remove(_chest);
    }

}
