using UnityEngine;
using UnityEngine.UI;

public class LoadSceneButton : MonoBehaviour
{
    [field: SerializeField] public Button btt { get; private set; }
    [SerializeField, Tooltip("Tên Scene cần Load")] private string SceneName;

    private bool _nullBtt;
    public string SetSceneName(string _sceneNameSet) => SceneName = _sceneNameSet;
    private void OnEnable()
    {
        _nullBtt = btt != null;
        
        if(!_nullBtt) return;
        btt.onClick.AddListener(LoadScene);
    }
    private void OnDisable()
    {
        if(!_nullBtt) return;
        btt.onClick.RemoveListener(LoadScene);
    }
    private void LoadScene()
    {
        if (LoadSceneManager.Instance)
        {
            LoadSceneManager.Instance.LoadScene(SceneName);
        }
    }
    
}
