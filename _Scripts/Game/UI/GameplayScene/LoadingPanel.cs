using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LoadingPanel : Singleton<LoadingPanel>
{
    [SerializeField] private Animator animator;
    [SerializeField] private Image background;
    [SerializeField] private Sprite[] spriteBG;

    
    private readonly int IDSceneLoading_IN = Animator.StringToHash("SceneLoading_IN");
    private readonly int IDSceneLoading_OUT = Animator.StringToHash("SceneLoading_OUT");
    private Coroutine _coroutine;
    
    public void Active()
    {
        animator.SetTrigger(IDSceneLoading_IN);
        background.sprite = spriteBG[Random.Range(0, spriteBG.Length)];
    }
    public void Active(float _deactiveTime)
    {
        animator.SetTrigger(IDSceneLoading_IN);
        background.sprite = spriteBG[Random.Range(0, spriteBG.Length)];

        if (_coroutine != null) StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(DeactiveCoroutine(_deactiveTime));
    }
    public void Deactive()
    {
        animator.SetTrigger(IDSceneLoading_OUT);
    }
    private IEnumerator DeactiveCoroutine(float _deactiveTime)
    {
        yield return new WaitForSecondsRealtime(_deactiveTime);
        Deactive();
    }
    
}
