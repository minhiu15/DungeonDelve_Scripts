using UnityEngine;

public class DeadZone : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayerMask;
    private PlayerController _player;

    private void Start()
    {
        _player = GameManager.Instance.Player;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(!playerLayerMask.Contains(other.gameObject)) return;
        _player.Health.Decreases(int.MaxValue);
    }
}
