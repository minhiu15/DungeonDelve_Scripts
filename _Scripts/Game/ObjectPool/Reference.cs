using System;
using UnityEngine;

/// <summary>
/// Để tạo 1 pool Object mà không cần tham chiếu đến các thành phần của Object, thì gán Script này vào Objetc cần tạo Pool để lấy Function giải phóng về Pool
/// </summary>
public class Reference : MonoBehaviour, IPooled<Reference>
{
    public void Release() => ReleaseCallback?.Invoke(this);
    public Action<Reference> ReleaseCallback { get; set; }
}
