namespace NameplateStats
{
    using UnityEngine;

    public class Managers
    {
        public static NameplateStatsManager NameplateStatsManager;
        public static void Start()
        {
            var objectListener = GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/").AddComponent<ObjectListener>();
            
            
            var managerObject = new GameObject("NameplateStatsManager");
            Object.DontDestroyOnLoad(managerObject);
            NameplateStatsManager = managerObject.AddComponent<NameplateStatsManager>();
            Prefs.ToggleEnable(Prefs.Enabled);
            objectListener.OnEnableEvent += () => { NameplateStatsManager.QuickMenuOpen = true; };
            objectListener.OnDisableEvent += () => { NameplateStatsManager.QuickMenuOpen = false; };
        }
    }
}