using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManager : Singleton<LoadSceneManager>
{
    private float _progressLoad;
    private Coroutine _loadCoroutine;
    
    public void LoadScene(string _sceneName)
    {
        if(_loadCoroutine != null) 
            StopCoroutine(_loadCoroutine);
        _loadCoroutine = StartCoroutine(LoadCoroutine(_sceneName));
    }
    private IEnumerator LoadCoroutine(string _sceneName)
    {
        LoadingPanel.Instance.Active();
        var scene = SceneManager.LoadSceneAsync(_sceneName);
        scene.allowSceneActivation = false;
        _progressLoad = 0;
        
        while (true)
        {
            if(_progressLoad > .9f) break;
            _progressLoad = Mathf.Clamp01(scene.progress / 0.9f);
            yield return null;
        }
        yield return new WaitForSeconds(Random.Range(3.5f,5.5f));
        scene.allowSceneActivation = true;
        LoadingPanel.Instance.Deactive();
    }
    
    
}
