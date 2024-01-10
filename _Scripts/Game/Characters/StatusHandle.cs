using System;
using UnityEngine;

[Serializable]
public class StatusHandle
{
   public event Action<int> OnCurrentValueChangeEvent;
   public event Action<int> OnMaxValueChangeEvent;
   public event Action<int, int> OnInitValueEvent;
   public event Action OnDieEvent;
   
   public int MaxValue { get; private set; }
   public int CurrentValue { get; private set; }

   /// <summary>
   /// Khởi tạo 1 Status với các giá trị ban đầu.
   /// </summary>
   /// <param name="_currentValue"> Giá trị hiện tại. </param>
   /// <param name="_maxValue"> Giá trị tối đa </param>
   public void InitValue(int _currentValue, int _maxValue)
   {
      MaxValue = _maxValue;
      CurrentValue = _currentValue;
      OnInitValueEvent?.Invoke(CurrentValue, MaxValue);
   }
   
   /// <summary>
   /// Cập nhật lại giá trị tối đa
   /// </summary>
   /// <param name="_maxValue"></param>
   public void UpdateMaxValue(int _maxValue)
   {
      MaxValue = _maxValue;
      OnMaxValueChangeEvent?.Invoke(MaxValue);
   }
   
   public void Increases(int _amount)
   {
      CurrentValue = Mathf.Clamp(CurrentValue + _amount, 0, MaxValue);
      OnCurrentValueChangeEvent?.Invoke(CurrentValue);
   }
   public void Decreases(int _amount)
   {
      CurrentValue = Mathf.Clamp(CurrentValue - _amount, 0, MaxValue);
      if (CurrentValue <= 0)
      {
         OnDieEvent?.Invoke();
      }
      OnCurrentValueChangeEvent?.Invoke(CurrentValue);
   }
   
}
