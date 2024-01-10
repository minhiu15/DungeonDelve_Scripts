using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class Coin : MonoBehaviour, IPooled<Coin>
{
    [SerializeField] private new Audio audio;
    [SerializeField] private TrailRenderer coinTrail;
    [SerializeField] private AnimationCurve moveCurve;
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float rorationSpeed = 360;
    public event Action<Coin> OnMoveCompleteEvent;
    public bool IsPlayer => _player != null;
    
    private PlayerController _player;
    private Tween _moveTween;
    private Tween _rotateTween;
    private float _duration;
    private bool _canMove;
    
    
    private void OnEnable()
    {
        MoveCoin();
    }
    private void MoveCoin()
    {
        _canMove = false;
        coinTrail.Clear();
        _duration = Random.Range(1f, 1.5f);
        transform.rotation = Quaternion.Euler(Vector3.zero);
        
        _rotateTween?.Kill();
        _rotateTween = transform.DORotate(new Vector3(0, 360, 0), rorationSpeed, RotateMode.FastBeyond360)
            .SetSpeedBased(true)
            .SetEase(Ease.Linear)
            .SetLoops(-1);
        
        _moveTween ?.Kill();
        _moveTween = transform.DOMove(GetRandomPos_1(transform.position), _duration).SetEase(moveCurve).OnComplete(() => { _canMove = true; });
    }
    private void LateUpdate()
    {
        if(!_canMove) return;
        if (Vector3.Distance(transform.position, _player.transform.position) <= 1f)
        {
            Release();
            return;
        }
        transform.position = Vector3.MoveTowards(transform.position, GetRandomPos_2(_player.transform.position), moveSpeed * Time.deltaTime);
    }


    public void PlayAudio() => audio.Play();
    private static Vector3 GetRandomPos_1(Vector3 _currentPos)
    {
        var posRand = Random.insideUnitSphere * 1.25f;
        return _currentPos + new Vector3(posRand.x, posRand.y, posRand.z);
    }
    private static Vector3 GetRandomPos_2(Vector3 _currentPos) => _currentPos +  new Vector3(0f, Random.Range(0.5f, 1f), 0f); 
    public void SetPlayer(PlayerController player) => _player = player;
    public void Release()
    {
        OnMoveCompleteEvent?.Invoke(this);
        _canMove = false;
        coinTrail.Clear();
        _moveTween?.Kill();
        _rotateTween?.Kill();
        ReleaseCallback?.Invoke(this);
    }
    public Action<Coin> ReleaseCallback { get; set; }
}
