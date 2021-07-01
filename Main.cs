using HarmonyLib;

namespace NameplateStats
{
    using MelonLoader;

    public class Main : MelonMod
    {
        public static HarmonyLib.Harmony Instance { get; private set; }

        private byte? _scenesLoaded = 0;
        
        public override void OnApplicationStart()
        {
            Instance = new HarmonyLib.Harmony("NameplateStats");
            Prefs.OnStart();
            //Patches.DoPatches();
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (_scenesLoaded is <= 2)
            {
                _scenesLoaded++;
                if (_scenesLoaded == 2) // UiManagerInit
                {
                    Nameplate.Start();
                    Prefs.ForceUpdateListeners();
                    if(Prefs.IsBTKSANameplateModPresent) Prefs.OnLateStart();
                    _scenesLoaded=null;
                }
            }
            else Nameplate.OnSceneChanged(); //Wipe playerlist because changing worlds
        }

        public override void OnPreferencesSaved() => Prefs.UpdateListeners();
    }
}