namespace NameplateStats
{
    using MelonLoader;

    public class Main : MelonMod
    {
        public static Main Instance { get; private set; }

        private byte? _scenesLoaded;
        
        public override void OnApplicationStart()
        {
            Instance = this;
            Prefs.OnStart();
            Patches.DoPatches();
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
                    _scenesLoaded=null;
                }
            }
            else Nameplate.OnSceneChanged(); //Wipe playerlist because changing worlds
        }

        public override void OnPreferencesSaved() => Prefs.UpdateListeners();
    }
}