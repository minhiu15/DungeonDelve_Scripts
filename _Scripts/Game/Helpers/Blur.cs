using UnityEngine;

namespace DungeonDelve.Project
{
    public class Blur : Singleton<Blur>
    {
        [SerializeField] private GameObject blurPanel;

        public void Enable() => blurPanel.SetActive(true);
        public void Disable() => blurPanel.SetActive(false);
    }

}
