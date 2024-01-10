using System;

public interface IPooled<T>
{
    /// <summary>
    /// Giải phóng Object về lại Pool
    /// </summary>
    public void Release();
    
    Action<T> ReleaseCallback { get; set; }
}
