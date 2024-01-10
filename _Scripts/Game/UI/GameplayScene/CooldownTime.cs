using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CooldownTime : MonoBehaviour
{
    [SerializeField] private Image fill;
    [SerializeField] private TextMeshProUGUI valueText;

    /// <summary> Kết thúc việc đếm ngược thời gian chưa ? </summary>
    public bool IsEndCD { get; private set; } = true;
    
    public float DurationTotalTemp { get; private set; }
    public float LastDurationTemp { get; private set; }
    //
    private bool _valueTextNotNull;
    private Coroutine _cooldownCoroutine;

    private void Start()
    {
        _valueTextNotNull = valueText != null;
        ActiveFill(false);
        IsEndCD = true;
    }

    public void SetDuration(float _durationTotal, float _lastDuration)
    {
        DurationTotalTemp = _durationTotal;
        LastDurationTemp = _lastDuration;
    }
    public void StartCooldownTime(float duration)
    {
        SetDuration(duration, duration);
        if(_cooldownCoroutine != null) StopCoroutine(_cooldownCoroutine);
        _cooldownCoroutine = StartCoroutine(CooldownCoroutine());
    }
    public void ContinueCooldownTime()
    {
        if (LastDurationTemp == 0)
            return;
        if(_cooldownCoroutine != null) StopCoroutine(_cooldownCoroutine);
        _cooldownCoroutine = StartCoroutine(CooldownCoroutine());
    }
    public void StopCooldownTime()
    {
        IsEndCD = true;
        LastDurationTemp = 0;
        ActiveFill(false);
        if(_cooldownCoroutine != null) StopCoroutine(_cooldownCoroutine);
    }
    
    private IEnumerator CooldownCoroutine()
    {
        ActiveFill(true);
        IsEndCD = false;
        while (LastDurationTemp >= 0)
        {
            var _currentTime = LastDurationTemp / DurationTotalTemp;
            SetFill(_currentTime);
            SetValueText();
            LastDurationTemp -= Time.deltaTime;
            yield return null;
        }
        IsEndCD = true;
        LastDurationTemp = 0;
        ActiveFill(false);
    }
    private void ActiveFill(bool _canActive)
    {
        if (fill) 
            fill.gameObject.SetActive(_canActive);
        if (_valueTextNotNull)
            valueText.gameObject.SetActive(_canActive);
    }
    private void SetFill(float _amount)
    {
        if (fill) fill.fillAmount = _amount;
    }
    private void SetValueText()
    {
        if (_valueTextNotNull) valueText.text = LastDurationTemp.ToString("F1");
    }
    
}
