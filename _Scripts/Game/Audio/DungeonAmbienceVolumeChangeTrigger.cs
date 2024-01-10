using UnityEngine;

public class DungeonAmbienceVolumeChangeTrigger : MonoBehaviour
{
    [Tooltip("Layer kiểm tra va chạm"), SerializeField] private LayerMask layerTrigger;
    [Header("Parameter Change")]
    [SerializeField] private string parameterName;
    [SerializeField, Range(0, 1)] private float parameterValue;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!layerTrigger.Contains(other.gameObject)) return;
        
        SetVolume(parameterValue);
    }
    public void SetVolume(float _volume) => DungeonAmbienceAudio.Instance.SetVolume(parameterName, _volume);

}
