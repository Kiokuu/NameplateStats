using MelonLoader;

namespace NameplateStats
{
    public class Main : MelonMod
    {
        public static Main Instance { get; private set; }

        private int _scenesLoaded;
        
        public override void OnApplicationStart()
        {
            Instance = this;
            Patches.DoPatches();
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (_scenesLoaded > 2) return;
            _scenesLoaded++;
            if (_scenesLoaded != 2)
            {
                Nameplate.OnSceneChanged(); 
                return;
            }
            
            Nameplate.Start();
        }
    }
}