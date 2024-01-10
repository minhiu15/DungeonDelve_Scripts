using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class SwitchPanelControl : MonoBehaviour
{
    
    public List<Animator> Animators;
    public List<GameObject> Panels;
    
    public static readonly int NameHashID_Selected = Animator.StringToHash("Selected");
    public static readonly int NameHashID_DeSelected = Animator.StringToHash("DeSelected");
    public static readonly int NameHashID_Trigger = Animator.StringToHash("Trigger");
    public static readonly int NameHashID_NonTrigger = Animator.StringToHash("NonTrigger");
    
    public void SetActivePanel(GameObject _panelObject)
    { 
        Panels.ForEach(panel => panel.SetActive(panel == _panelObject));
    }
    public void DeActiveAllPanel() => Panels.ForEach(panel => panel.SetActive(false));
    
    public void SelectButton(Animator _animatorCheck)
    {
        foreach (var animator in Animators)
        {
            if(animator == _animatorCheck)
                animator.Play(NameHashID_Selected);
            else if(animator.IsTag("Selected"))
                animator.Play(NameHashID_NonTrigger);
        }
    }
    public void DeSelectButton(Animator _animatorCheck)
    {
        foreach (var animator in Animators.Where(animator => animator == _animatorCheck && animator.IsTag("Selected")))
        {
            animator.Play(NameHashID_DeSelected);
        }
    }
    
    public void TriggerButton(Animator _animatorCheck)
    {
        var _animatorsCheck = Animators.Where(animator => animator == _animatorCheck && !animator.IsTag("Selected"));
        foreach (var animator in _animatorsCheck)
        {
            animator.Play(NameHashID_Trigger);
        }
    }
    public void NonTriggerButton(Animator _animatorCheck)
    {
        var _animatorsCheck = Animators.Where(animator => animator == _animatorCheck && !animator.IsTag("Selected"));
        foreach (var animator in _animatorsCheck)
        {
            animator.Play(NameHashID_NonTrigger);
        }
    }
}
