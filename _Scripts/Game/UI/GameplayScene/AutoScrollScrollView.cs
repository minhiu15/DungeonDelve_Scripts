using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AutoScrollScrollView : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField, Tooltip("Tốc độ cuộn")]
    private float speedScroll = 15f;
    private Coroutine _scrollCoroutine;
    private readonly WaitForSecondsRealtime _waitNull = new(0f);
    
    
    public void Scroll()
    {
        if (_scrollCoroutine != null) StopCoroutine(_scrollCoroutine); 
            _scrollCoroutine = StartCoroutine(ScrollCoroutine());
    }
    private IEnumerator ScrollCoroutine()
    {
        while (scrollRect.verticalNormalizedPosition > 0.01f)
        {
            scrollRect.verticalNormalizedPosition = Mathf.Lerp(scrollRect.verticalNormalizedPosition, 0f, Time.unscaledTime * speedScroll);
            yield return _waitNull;
        }
        scrollRect.verticalNormalizedPosition = 0f;
    }

}
