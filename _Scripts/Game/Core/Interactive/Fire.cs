using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Fire : MonoBehaviour
{
    [SerializeField] private int fireDmg = 10;
    [SerializeField] private float waitTime = 1;

    private Coroutine _takeDMGCoroutine;
    private YieldInstruction _yieldTime;
    private PlayerController _player;
    private void Start()
    {
        _player = GameManager.Instance.Player;
        _yieldTime = new WaitForSeconds(waitTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || !_player) return;
        
        if (_takeDMGCoroutine != null)
            StopCoroutine(_takeDMGCoroutine);
        _takeDMGCoroutine = StartCoroutine(TakeDMGCoroutine());
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player") || !_player) return;
        
        if (_takeDMGCoroutine != null)
            StopCoroutine(_takeDMGCoroutine);
    }
    
    private IEnumerator TakeDMGCoroutine()
    {
        while (true)
        {
            _player.Health.Decreases(fireDmg);
            if (Random.value <= .4f)
                _player.voice.PlayLightHit();
            DMGPopUpGenerator.Instance.Create(_player.transform.position, fireDmg, false, false);
            
            yield return _yieldTime;
        }
    } 
    
}
