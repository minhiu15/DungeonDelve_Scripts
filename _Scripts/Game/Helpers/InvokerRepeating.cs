using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Gọi Event mỗi giây
/// </summary>
public class InvokerRepeating : MonoBehaviour
{
    [Tooltip("Thời gian chờ trước khi bắt đầu")]
    public float startTime;
    [Tooltip("Thời gian chờ cho mỗi lần gọi tiếp theo")] 
    public float timeRepeat;
    
    [Space] public UnityEvent OnCallEvent;
    
    private Coroutine _invokerCoroutine;
    private YieldInstruction _yieldInstruction;
    
    private void OnEnable()
    {
        _yieldInstruction = new WaitForSeconds(timeRepeat);
        StartInvoke();
    }
    
    public void StartInvoke()
    {
        if(_invokerCoroutine != null) 
            StopCoroutine(_invokerCoroutine);
        _invokerCoroutine = StartCoroutine(InvokeCoroutine());
    }
    public void StopInvoke()
    {
        if(_invokerCoroutine != null) 
            StopCoroutine(_invokerCoroutine);
    }

    
    private IEnumerator InvokeCoroutine()
    {
        yield return new WaitForSeconds(startTime);
        while (true)
        {
            OnCallEvent?.Invoke();
            yield return _yieldInstruction;
        }
    }
    
    
}
