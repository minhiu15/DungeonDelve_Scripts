using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackBoard : Singleton<BlackBoard>
{
    [SerializeField] private Animator panel;
    
    private Coroutine _disableCoroutine;
    private float _waitTime;
    
    public void Enable(float _disableTime)
    {
        _waitTime = _disableTime;
        panel.Play("Panel_IN");
        
        if (_disableCoroutine != null) StopCoroutine(_disableCoroutine);
        _disableCoroutine = StartCoroutine(DisableCoroutine());
    }
    private IEnumerator DisableCoroutine()
    {
        yield return new WaitForSecondsRealtime(_waitTime);
        panel.Play("Panel_OUT");
    }


}
