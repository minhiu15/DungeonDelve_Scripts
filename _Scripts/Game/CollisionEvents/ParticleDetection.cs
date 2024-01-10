using System.Collections.Generic;
using UnityEngine;

public class ParticleDetection : DetectionBase
{
    [Space]
    public ParticleSystem particle;
    
    private readonly List<ParticleCollisionEvent> _particleEvent = new();
    
    private void OnParticleCollision(GameObject other)
    {
        CollisionEnterEvent?.Invoke(other);
        
        _particleEvent.Clear();
        var nums = particle.GetCollisionEvents(other, _particleEvent);
        if(nums <= 0) 
            return;
        
        PositionEnterEvent?.Invoke(_particleEvent[0].intersection);
    }

    
    
    
}
