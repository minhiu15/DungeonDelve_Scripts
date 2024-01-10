using FMOD.Studio;

public class DungeonAmbienceAudio : Singleton<DungeonAmbienceAudio>
{
    private EventInstance _audioIns;
    private bool _isPlay;
    
    private void Start()
    {
        _audioIns = AudioManager.CreateInstance(FMOD_Events.Instance.dungeonAmbience);
        Play();
    }
    private void OnDestroy()
    {
        Stop();   
    }

    private void Play()
    {
        if (_isPlay) return;
        _isPlay = true;
        
        _audioIns.start();
    }
    private void Stop()
    {
        if (!_isPlay) return;
        _isPlay = false;
        
        _audioIns.stop(STOP_MODE.ALLOWFADEOUT);
        _audioIns.release();
    }
    public void SetVolume(string _volumeParameterName, float _volume) => _audioIns.setParameterByName(_volumeParameterName, _volume);

}
