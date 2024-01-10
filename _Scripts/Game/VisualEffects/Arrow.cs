using System.Collections;
using UnityEngine;

public class Arrow : EffectBase
{

    [Space]
    public Rigidbody rb;
    public SphereCollider sphereCollider;
    
    [Tooltip("Lực bắn")] 
    public float force;

    [Tooltip("Trì hoãn trọng lực")]
    public float delayGravity;
    
    private Coroutine _gravityCoroutine;
    
    
    
    public override void FIRE()
    {
        Flash(ActiveType.Enable);
        Projectile(ActiveType.Enable);
        ResetRigidbody();
    }

    
    private void ResetRigidbody() // đặt lại các giá trị ban đầu của Rb
    {
        if(rb == null) return;
        
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.None;
        
        rb.AddForce(transform.forward * force, ForceMode.Impulse);
        HandleGravity();
    }
    private void HandleGravity()
    {
        if (_gravityCoroutine != null) 
            StopCoroutine(_gravityCoroutine);
        _gravityCoroutine = StartCoroutine(GravityCoroutine());
    }
    private IEnumerator GravityCoroutine()
    {
        yield return new WaitForSeconds(delayGravity);
        rb.useGravity = true;

        while (true)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(rb.velocity), 100 * Time.deltaTime);
            yield return new WaitForFixedUpdate();
        }
    }


    
    // Xử lí khi va chạm
    public void HandleCollision()
    {
        Projectile(ActiveType.Disable);
        Hit(ActiveType.Enable, sphereCollider.ClosestPoint(transform.position));
        
        if(rb) rb.constraints = RigidbodyConstraints.FreezeAll;
    }
    public void HandleCollision(Vector3 _collisionPosition)
    {
        Projectile(ActiveType.Disable);
        Hit(ActiveType.Enable, _collisionPosition);
    }
    
    
}
