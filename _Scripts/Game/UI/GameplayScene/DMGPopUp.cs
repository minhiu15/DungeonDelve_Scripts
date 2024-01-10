using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class DMGPopUp : MonoBehaviour, IPooled<DMGPopUp>
{
   public TextMeshProUGUI textPopUp;
   
   [Space]
   public AnimationCurve opacityCurve;
   public AnimationCurve heightCurve;
   
   [Space]
   public AnimationCurve DMGScaleCurve;
   public AnimationCurve CRDScaleCurve;

   [Space]
   [Tooltip("Màu text khi nhận sát thương bạo kích")]
   public Color CRDColor;
   [Tooltip("Màu text khi Enemy nhận sát thương thường")]
   public Color EnemyDMGColor;
   [Tooltip("Màu text khi Player nhận sát thương thường")]
   public Color PlayerDMGColor = Color.white;
   
   public Camera mainCamera { get; set; }
   private bool _isCrit;
   private float _evaluateTime;
   private Color _currentColor;
   private Vector3 _originPosition;
   private Vector3 _originScale;
   private Coroutine _updateCoroutine;


   public void SetMainCamera(Camera _camera) => mainCamera = _camera;
   public void Show(int _damage, bool isApplyCRIT,bool _isEnemy)
   {
      _isCrit = isApplyCRIT;
      _currentColor = isApplyCRIT ? CRDColor : _isEnemy ? EnemyDMGColor : PlayerDMGColor;
      textPopUp.color = _currentColor;
      textPopUp.text = $"{_damage}";
      
      if(_updateCoroutine != null) 
         StopCoroutine(_updateCoroutine);
      _updateCoroutine = StartCoroutine(UpdateCoroutine());
   }
   private IEnumerator UpdateCoroutine()
   {
      _evaluateTime = 0;
      _originPosition = transform.position;
      _originScale = new Vector3(.01f, .01f, .01f);
      
      while (_evaluateTime <= 1)
      {
         transform.forward = mainCamera.transform.forward;

         _currentColor.a = opacityCurve.Evaluate(_evaluateTime);
         textPopUp.color = _currentColor;

         transform.position = _originPosition + new Vector3(0, 1 + heightCurve.Evaluate(_evaluateTime), 0);
         transform.localScale = _isCrit ? 
                              _originScale * CRDScaleCurve.Evaluate(_evaluateTime) : 
                              _originScale * DMGScaleCurve.Evaluate(_evaluateTime) ;

         _evaluateTime += Time.deltaTime;
         yield return null;
      }
      Release();
   }
   
   

   
   public void Release() => ReleaseCallback?.Invoke(this);
   public Action<DMGPopUp> ReleaseCallback { get; set; }
}
