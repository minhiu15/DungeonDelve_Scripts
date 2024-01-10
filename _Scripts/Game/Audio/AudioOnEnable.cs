using System.Collections;
using System.Linq;
using FMODUnity;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioOnEnable : MonoBehaviour
{
    [SerializeField, Tooltip("Chọn option TRUE nếu Object này được khởi tạo bằng Pool")]
    private bool _isFromPool = true;
    [SerializeField, Tooltip("Thời gian chờ khi Object được Enable")] 
    private float waitTime;
    [Space(15)]
    [SerializeField] private new EventReference audio;
    [SerializeField, ShowIf("isRandomAudio")] private EventReference[] audios;
    [SerializeField, Tooltip("Có Random Audio ?")] private bool isRandomAudio;
    
    private EventReference _audioTemp;
    private Coroutine _delayCoroutine;
    
    private void OnEnable()
    {
        if (_isFromPool)
        {
            _isFromPool = false;
            return;
        }
        
        _audioTemp = audio;
        if (isRandomAudio && audios.Any())
        {
            _audioTemp = audios[Random.Range(0, audios.Length)];
        }

        if (_delayCoroutine != null) StopCoroutine(_delayCoroutine);
        _delayCoroutine = StartCoroutine(DelayCoroutine());
    }

    private IEnumerator DelayCoroutine()
    {
        yield return new WaitForSeconds(waitTime);
        AudioManager.PlayOneShot(_audioTemp, transform.position);   
    }
    
}
