using System;
using System.Linq;
using FMODUnity;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerVoice : MonoBehaviour
{

    [BoxGroup("STORY"), SerializeField] private EventReference GoodMorningEvent;
    [BoxGroup("STORY"), SerializeField] private EventReference GoodAfternoonEvent;
    [BoxGroup("STORY"), SerializeField] private EventReference GoodEveningEvent;
    [BoxGroup("STORY"), SerializeField] private EventReference GoodNightEvent;
    [BoxGroup("STORY"), SerializeField] private EventReference[] ChatEvent;
    [BoxGroup("STORY"), SerializeField] private EventReference[] OpenChestEvent;
    [BoxGroup("STORY"), SerializeField] private EventReference[] RelaxEvent;

    //
    [BoxGroup("COMBAT"), SerializeField] private EventReference[] JumpEvent;
    [BoxGroup("COMBAT"), SerializeField] private EventReference[] DashEvent;
    [BoxGroup("COMBAT"), SerializeField] private EventReference[] LightAttackEvent;
    [BoxGroup("COMBAT"), SerializeField] private EventReference[] MidAttackEvent;
    [BoxGroup("COMBAT"), SerializeField] private EventReference[] HeavyAttackEvent;
    [BoxGroup("COMBAT"), SerializeField] private EventReference[] ElementalSkillEvent;
    [BoxGroup("COMBAT"), SerializeField] private EventReference[] ElementalBurstEvent;
    [BoxGroup("COMBAT"), SerializeField] private EventReference[] LightHitEvent;
    [BoxGroup("COMBAT"), SerializeField] private EventReference[] HeavyHitEvent;
    [BoxGroup("COMBAT"), SerializeField] private EventReference[] DieEvent;
    
    private void OnEnable()
    {
        var _currentHour = DateTime.Now.Hour;
        switch (_currentHour)
        {
            case >= 0 and <= 12:
                AudioManager.PlayOneShot(GoodMorningEvent, transform.position);
                break;
            case > 12 and <= 18:
                AudioManager.PlayOneShot(GoodAfternoonEvent, transform.position);
                break;
            default:
                AudioManager.PlayOneShot(GoodEveningEvent, transform.position);
                break;
        }
    }

    
    private void Play(EventReference _eventReference) => AudioManager.PlayOneShot(_eventReference, transform.position);
    
    #region STORY
    public void PlayChat()
    {
        if (!ChatEvent.Any()) return;
        Play(ChatEvent[Random.Range(0, ChatEvent.Length)]);
    }
    public void PlayOpenChest()
    {
        if (!OpenChestEvent.Any()) return;
        Play(OpenChestEvent[Random.Range(0, OpenChestEvent.Length)]);
    }
    public void PlayRelax()
    {
        if (!RelaxEvent.Any()) return;
        Play(RelaxEvent[Random.Range(0, RelaxEvent.Length)]);
    }
    #endregion
    
    #region COMBAT
    public void PlayLightAttack()
    {
        if (!LightHitEvent.Any()) return;
        Play(LightAttackEvent[Random.Range(0, LightAttackEvent.Length)]);
    }
    public void PlayMidAttack()
    {
        if (!MidAttackEvent.Any()) return;
        Play(MidAttackEvent[Random.Range(0, MidAttackEvent.Length)]);
    }
    public void PlayHeavyAttack()
    {
        if (!HeavyAttackEvent.Any()) return;
        Play(HeavyAttackEvent[Random.Range(0, HeavyAttackEvent.Length)]);
    }
    public void PlayElementalSkill()
    { 
        if (!ElementalSkillEvent.Any()) return;
        Play(ElementalSkillEvent[Random.Range(0, ElementalSkillEvent.Length)]);
    }
    public void PlayElementalBurstEvent()
    {
        if (!ElementalBurstEvent.Any()) return;
        Play(ElementalBurstEvent[Random.Range(0, ElementalBurstEvent.Length)]);
    }
    //
    public void PlayJumping()
    {
        if (!JumpEvent.Any()) return;
        Play(JumpEvent[Random.Range(0, JumpEvent.Length)]);
    }
    public void PlayDash()
    {
        if (!DashEvent.Any()) return;
        Play(DashEvent[Random.Range(0, DashEvent.Length)]);
    }
    public void PlayLightHit()
    {
        if (!LightHitEvent.Any()) return;
        Play(LightHitEvent[Random.Range(0, LightHitEvent.Length)]);
    }
    public void PlayHeavyHit()
    {
        if (!HeavyHitEvent.Any()) return;
        Play(HeavyHitEvent[Random.Range(0, HeavyHitEvent.Length)]);
    }
    public void PlayDie()
    {
        if (!DieEvent.Any()) return;
        Play(DieEvent[Random.Range(0, DieEvent.Length)]);
    }
    #endregion
    
}
