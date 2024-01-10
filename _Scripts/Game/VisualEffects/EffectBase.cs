using System;
using NaughtyAttributes;
using UnityEngine;

public abstract class EffectBase : MonoBehaviour, IPooled<EffectBase>
{
    [Header("Ref"), Required] 
    public DetectionBase detectionType;
    
    [Header("Effect Prefab")]
    public ParticleSystem flash;
    public ParticleSystem projectile;
    public ParticleSystem hit;

    
    protected enum ActiveType
    {
        Enable,
        Disable
    }
    

    /// <summary>
    /// Set Active của Effect Flash theo Parameter
    /// </summary>
    protected void Flash(ActiveType _type)
    {
        if(flash == null) 
            return;
        
        flash.transform.position = transform.position;
        switch (_type)
        {
            case ActiveType.Enable:
                flash.transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
                flash.transform.SetParent(null);
                flash.gameObject.SetActive(true);
                flash.Play();
                break;
            
            case ActiveType.Disable:
                flash.transform.SetParent(transform);
                flash.gameObject.SetActive(false);
                flash.Stop();
                break;
        }
    }
    
    /// <summary>
    /// Set Active của Effect Projectile theo Parameter
    /// </summary>
    protected void Projectile(ActiveType _type)
    {
        if(projectile == null) 
            return;
        
        switch (_type)
        {
            case ActiveType.Enable:
                projectile.gameObject.SetActive(true);
                projectile.Play();
                break;
            
            case ActiveType.Disable:
                projectile.gameObject.SetActive(false);
                projectile.Stop();
                break;
        }
    }
    
    /// <summary>
    /// Set Active của Effect Hit theo Parameter tại vị trí Pos
    /// </summary>
    protected void Hit(ActiveType _type, Vector3 _pos)
    {
        if(hit == null) 
            return;
        
        hit.transform.position = _pos;
        switch (_type)
        {
            case ActiveType.Enable:
                hit.transform.SetParent(null);
                hit.gameObject.SetActive(true);
                hit.Play();
                break;
            
            case ActiveType.Disable:
                hit.transform.SetParent(transform);
                hit.gameObject.SetActive(false);
                hit.Stop();
                break;
        }
    }


    public abstract void FIRE();


    


    public void Release()
    {
        Flash(ActiveType.Disable);
        Hit(ActiveType.Disable, Vector3.zero);
        
        ReleaseCallback?.Invoke(this);
    }
    public Action<EffectBase> ReleaseCallback { get; set; }
}
