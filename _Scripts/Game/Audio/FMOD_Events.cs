using FMODUnity;
using UnityEngine;

public class FMOD_Events : Singleton<FMOD_Events>
{
    [field: Header("GAME AMBIENCE")]
    [field: SerializeField] public EventReference dungeonAmbience { get; private set; }
    
    //
    [field: Header("GAME SFX")]
    [field: SerializeField] public EventReference menuOpen { get; private set; }
    [field: SerializeField] public EventReference menuClose { get; private set; }
    [field: SerializeField] public EventReference chestOpen { get; private set; }
    [field: SerializeField] public EventReference rewards_01 { get; private set; }
    [field: SerializeField] public EventReference rewards_02 { get; private set; }
    [field: SerializeField] public EventReference buffEffect { get; private set; }
    [field: SerializeField] public EventReference notice_01 { get; private set; }
    
    //
    [field: Header("PLAYER SFX")]
    [field: SerializeField] public EventReference walkFootsteps { get; private set; }
    [field: SerializeField] public EventReference runFootsteps { get; private set; }
    [field: SerializeField] public EventReference runfastFootsteps { get; private set; }
    
    
}
