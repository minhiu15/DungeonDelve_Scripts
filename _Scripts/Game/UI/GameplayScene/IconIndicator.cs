using System;
using UnityEngine;
public class IconIndicator : MonoBehaviour, IPooled<IconIndicator>
{
    public RectTransform iconIndicator;
    public RectTransform arrowIndicator;

    
    public void Release() => ReleaseCallback?.Invoke(this);
    public Action<IconIndicator> ReleaseCallback { get; set; }
}
