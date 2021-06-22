namespace NameplateStats
{
    using UnityEngine;

    public class Managers
    {
        public static void Start()
        {
            var objectListener = GameObject.Find("UserInterface/QuickMenu/QuickModeTabs").AddComponent<ObjectListener>();
            var managerObject = new GameObject("NameplateStatsManager");
            Object.DontDestroyOnLoad(managerObject);
            var managerComponent = managerObject.AddComponent<NameplateStatsManager>();

            objectListener.OnEnableEvent += () => { managerComponent.QuickMenuOpen = true; };
            objectListener.OnDisableEvent += () => { managerComponent.QuickMenuOpen = false; };
        }
    }
}