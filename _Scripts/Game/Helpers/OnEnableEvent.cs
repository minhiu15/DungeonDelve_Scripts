using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary> Mỗi lần GameObject này được bật, thì sau ?(timer) giây sẽ gọi sự kiện 1 lần duy nhất </summary>
public class OnEnableEvent : MonoBehaviour
{
    [Tooltip("Thời gian chờ để gọi sự kiện khi object được active")]
    public float timer;

    [Space]
    public UnityEvent OnEndTimerEvent;
    
    
    private Coroutine _eventCoroutine;
    
    private void OnEnable()
    {
        if(_eventCoroutine != null)
            StopCoroutine(_eventCoroutine);
        _eventCoroutine = StartCoroutine(CountingTimeCoroutine());
    }
    private IEnumerator CountingTimeCoroutine()
    {
        yield return new WaitForSeconds(timer);
        OnEndTimerEvent?.Invoke();
    }
    
}
