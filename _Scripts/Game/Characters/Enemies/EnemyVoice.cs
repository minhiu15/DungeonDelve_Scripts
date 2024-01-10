using FMODUnity;
using UnityEngine;

public class EnemyVoice : MonoBehaviour
{
    [SerializeField] private EventReference[] battleStartEvent;
    [SerializeField] private EventReference[] normalAttackEvent;
    [SerializeField] private EventReference[] elementalSkillEvent;
    [SerializeField] private EventReference[] elementalBurstEvent;
    [SerializeField] private EventReference[] hitEvent;
    [SerializeField] private EventReference[] dieEvent;
    
    private void Play(EventReference _eventReference) => AudioManager.PlayOneShot(_eventReference, transform.position);


    public void PlayBattleStart()
    {
        if (battleStartEvent.Length == 0) return;
        Play(battleStartEvent[Random.Range(0, battleStartEvent.Length)]);
    }
    public void PlayNormalAttack(int _percentSoundCanPlay)
    {
        if (normalAttackEvent.Length == 0 || Random.Range(0, 100) >= _percentSoundCanPlay) return;
        Play(normalAttackEvent[Random.Range(0, normalAttackEvent.Length)]);
    }
    public void PlayElementalSkill()
    {
        if (elementalSkillEvent.Length == 0) return;
        Play(elementalSkillEvent[Random.Range(0, elementalSkillEvent.Length)]);
    }
    public void PlayElementalBurst()
    {
        if (elementalBurstEvent.Length == 0) return;
        Play(elementalBurstEvent[Random.Range(0, elementalBurstEvent.Length)]);
    }
    public void PlayHit()
    {
        if (hitEvent.Length == 0) return;
        Play(hitEvent[Random.Range(0, hitEvent.Length)]);
    }
    public void PlayDie()
    {
        if (dieEvent.Length == 0) return;
        Play(dieEvent[Random.Range(0, dieEvent.Length)]);
    }
    
}
