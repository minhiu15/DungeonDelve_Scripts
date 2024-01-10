using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class BackgroundAudio : MonoBehaviour
{
    [SerializeField] private new EventReference audio;
    [SerializeField] private string volumeParameterName;
    [SerializeField, Range(0, 1)] private float volumeStart;
    [SerializeField, Tooltip("Có Play BGMusic khi bắt đầu không ?")] private bool isPlayStart = true;
   
    private EventInstance _instance;
    private void Start()
    {
        _instance = AudioManager.CreateInstance(audio);
        SetVolume(volumeStart);
        if (!isPlayStart) return;
        Play();
    }
    
    public void Play() => _instance.start();
    public void Stop() => _instance.stop(STOP_MODE.ALLOWFADEOUT);
    public void SetVolume(float _valume) =>  _instance.setParameterByName(volumeParameterName, _valume);
    
}
