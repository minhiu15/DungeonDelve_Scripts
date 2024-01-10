using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    
    public static float masterVolume { get; set; }
    public static float musicVolume { get; set; }
    public static float sfxVolume { get; set; }
    public static float dialogueVolume { get; set; }
    public Bus masterBus { get; private set; }
    public Bus musicBus { get; private set; }
    public Bus sfxBus { get; private set; }
    public Bus dialogueBus { get; private set; }
    
    // Volume Key PlayerPrefs
    public const string PP_VolumeIndex = "VolumeIndex";
    public const string PP_MusicVolumeIndex = "MusicVolumeIndex";
    public const string PP_SFXVolumeIndex = "SFXVolumeIndex";
    public const string PP_DialogueVolumeIndex = "DialogueVolumeIndex";
    
    
    protected override void Awake()
    {
        base.Awake();
        masterBus = RuntimeManager.GetBus("bus:/");
        musicBus = RuntimeManager.GetBus("bus:/MUSIC");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");
        dialogueBus = RuntimeManager.GetBus("bus:/DIALOGUE");
    }
    private void Start()
    {
        LoadOldVolume();
    }
    private void OnApplicationQuit()
    {
        PlayerPrefs.SetFloat(PP_VolumeIndex, masterVolume);
        PlayerPrefs.SetFloat(PP_MusicVolumeIndex, musicVolume);
        PlayerPrefs.SetFloat(PP_SFXVolumeIndex, sfxVolume);
        PlayerPrefs.SetFloat(PP_DialogueVolumeIndex, dialogueVolume);
    }
    private void LoadOldVolume()
    {
        masterVolume = PlayerPrefs.GetFloat(PP_VolumeIndex, .5f);
        musicVolume = PlayerPrefs.GetFloat(PP_MusicVolumeIndex, .4f);
        dialogueVolume = PlayerPrefs.GetFloat(PP_DialogueVolumeIndex, .5f);
        sfxVolume = PlayerPrefs.GetFloat(PP_SFXVolumeIndex, .5f);

        masterBus.setVolume(masterVolume);
        musicBus.setVolume(musicVolume);
        dialogueBus.setVolume(dialogueVolume);
        sfxBus.setVolume(sfxVolume);
    }
    

    // ReSharper disable Unity.PerformanceAnalysis
    /// <summary>
    /// Phát Audio theo tham chiếu của Ref truyền vào.
    /// </summary>
    /// <param name="_eventReference"> REF Source cần phát. </param>
    /// <param name="_worldPos"> Vị trí phát trong không gian 3D. </param>
    public static void PlayOneShot(EventReference _eventReference, Vector3 _worldPos) => RuntimeManager.PlayOneShot(_eventReference, _worldPos);
    public static EventInstance CreateInstance(EventReference _eventReference) => RuntimeManager.CreateInstance(_eventReference);
}


