namespace NameplateStats
{
    using UnityEngine;

    public class Managers
    {
        public static NameplateStatsManager NameplateStatsManager;
        public static void Start()
        {
            var objectListener = GameObject.Find("UserInterface/QuickMenu/QuickMenu_NewElements").AddComponent<ObjectListener>();
            var managerObject = new GameObject("NameplateStatsManager");
            Object.DontDestroyOnLoad(managerObject);
            NameplateStatsManager = managerObject.AddComponent<NameplateStatsManager>();

            objectListener.OnEnableEvent += () => { NameplateStatsManager.QuickMenuOpen = true; };
            objectListener.OnDisableEvent += () => { NameplateStatsManager.QuickMenuOpen = false; };
        }
    }
}