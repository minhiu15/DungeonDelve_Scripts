using System.Collections;
using DG.Tweening;
using FMODUnity;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class OnSelectCharacterManager : MonoBehaviour
{
    [BoxGroup("SCENE"), SerializeField] private Button loadSceneBtt;
    [BoxGroup("SCENE"), SerializeField] private string sceneName;
    [Space]
    [BoxGroup("CHARACTER 01"), SerializeField] private GameObject nameChar_01;
    [BoxGroup("CHARACTER 01"), SerializeField] private RawImage char_01RawImage;
    [BoxGroup("CHARACTER 01"), SerializeField] private Animator char_01Animator;
    [BoxGroup("CHARACTER 01"), SerializeField] private SO_PlayerConfiguration char_01Config;
    [Space]
    [BoxGroup("CHARACTER 02"), SerializeField] private GameObject nameChar_02;
    [BoxGroup("CHARACTER 02"), SerializeField] private RawImage char_02RawImage;
    [BoxGroup("CHARACTER 02"), SerializeField] private Animator char_02Animator;
    [BoxGroup("CHARACTER 02"), SerializeField] private SO_PlayerConfiguration char_02Config;
    [Space] 
    [SerializeField] private GameObject animatedLoadPanel;
    [SerializeField] private EventReference escOnClickSound;
    [SerializeField] private BackgroundAudio backgroundAudio;
    //
    [SerializeField, ReadOnly] private SO_PlayerConfiguration _playerConfig;
    private Tween colorChar_01Tween;
    private Tween colorChar_02Tween;
    private Tween textChar_01Tween;
    private Tween textChar_02Tween;
    private readonly float _durationTween = .2f;
    private readonly Vector3 _selectScale = new(1.15f, 1.15f, 1f);
    private readonly Color _unSelectColor = new(0.8f, 0.8f, 0.8f, 1);
    private readonly int IDTalkingSelect = Animator.StringToHash("TalkingSelect");
    private readonly int IDTalkingDeSelect = Animator.StringToHash("TalkingDeSelect");
    private Coroutine _loadPanelCoroutine;
    private bool _canLoadscene;

    private void Start()
    {
        OpenPanelLoad(.55f);
        char_01RawImage.color = _unSelectColor;
        char_02RawImage.color = _unSelectColor;
        textChar_01Tween = nameChar_02.transform.DOScale(Vector3.one, _durationTween);
        textChar_02Tween = nameChar_02.transform.DOScale(Vector3.one, _durationTween);
        
        _canLoadscene = false;
        loadSceneBtt.interactable = false;
        loadSceneBtt.onClick.AddListener(OnClickSelectedCharButton);
    }
    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;

        loadSceneBtt.interactable = false;
        AudioManager.PlayOneShot(escOnClickSound, transform.position);
        
        if (char_01Animator.IsTag("select"))   char_01Animator.SetTrigger(IDTalkingDeSelect);
        if (char_02Animator.IsTag("select"))   char_02Animator.SetTrigger(IDTalkingDeSelect);
        
        ClearDotTween();
        char_01RawImage.DOColor(_unSelectColor, _durationTween);
        char_02RawImage.DOColor(_unSelectColor, _durationTween);
        textChar_01Tween = nameChar_01.transform.DOScale(Vector3.one, _durationTween);
        textChar_02Tween = nameChar_02.transform.DOScale(Vector3.one, _durationTween);
    }
    private void OnDestroy()
    {
        loadSceneBtt.onClick.RemoveListener(OnClickSelectedCharButton);
    }

    public void SelectChar(RawImage _charRawImage)
    {
        ClearDotTween();
        
        loadSceneBtt.interactable = true;
        if (char_01RawImage == _charRawImage)
        {
            _playerConfig = char_01Config;
            colorChar_01Tween = char_01RawImage.DOColor(Color.white, _durationTween);
            colorChar_02Tween = char_02RawImage.DOColor(_unSelectColor, _durationTween);
            textChar_01Tween = nameChar_01.transform.DOScale(_selectScale, _durationTween);
            textChar_02Tween = nameChar_02.transform.DOScale(Vector3.one, _durationTween);

            if (!char_01Animator.IsTag("select"))   char_01Animator.SetTrigger(IDTalkingSelect);
            if (!char_02Animator.IsTag("deselect")) char_02Animator.SetTrigger(IDTalkingDeSelect);
        }
        else
        {
            _playerConfig = char_02Config;
            colorChar_01Tween = char_01RawImage.DOColor(_unSelectColor, _durationTween);
            colorChar_02Tween = char_02RawImage.DOColor(Color.white, _durationTween);
            textChar_01Tween = nameChar_01.transform.DOScale(Vector3.one, _durationTween);
            textChar_02Tween = nameChar_02.transform.DOScale(_selectScale, _durationTween);
            
            if (!char_01Animator.IsTag("deselect")) char_01Animator.SetTrigger(IDTalkingDeSelect);
            if (!char_02Animator.IsTag("select"))   char_02Animator.SetTrigger(IDTalkingSelect);
        }
    }
    private void ClearDotTween()
    {
        colorChar_01Tween ?.Kill();
        colorChar_02Tween ?.Kill();
        textChar_01Tween ?.Kill();
        textChar_02Tween ?.Kill();
    }
    private void OnClickSelectedCharButton()
    {
        if (!PlayFabHandleUserData.Instance)
            return;

        _canLoadscene = true;
        PlayFabHandleUserData.Instance.PlayerConfig = Instantiate(_playerConfig);
        PlayFabHandleUserData.Instance.UpdateData(PlayFabHandleUserData.PF_Key.PlayerConfigData_Key);
        OpenPanelLoad(Random.Range(2f, 2.8f));
    }


    private void OpenPanelLoad(float _disableTime)
    {
        if(_loadPanelCoroutine != null)
            StopCoroutine(_loadPanelCoroutine);
        _loadPanelCoroutine = StartCoroutine(AnimatedPanelCoroutine(_disableTime));
    }
    private IEnumerator AnimatedPanelCoroutine(float _disableTime)
    {
        animatedLoadPanel.gameObject.SetActive(true);
        yield return new WaitForSeconds(_disableTime);
        animatedLoadPanel.gameObject.SetActive(false);
        
        if (!_canLoadscene || !PlayFabHandleUserData.Instance || !LoadSceneManager.Instance)
            yield break;
        
        yield return new WaitForSeconds(Random.Range(0.5f, 1.2f));
       LoadSceneManager.Instance.LoadScene(sceneName);
       backgroundAudio.Stop();
    }

}
