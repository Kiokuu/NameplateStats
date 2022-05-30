using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NameplateStats
{
    using MelonLoader;

    public class Main : MelonMod
    {
        public static HarmonyLib.Harmony Instance { get; private set; }
        //private byte? _scenesLoaded = 0;
        
        public override void OnApplicationStart()
        {
            Instance = HarmonyInstance;
            Prefs.OnStart();
            MelonCoroutines.Start(WaitForUiManager());
        }

        private IEnumerator WaitForUiManager()
        {
            while (VRCUiManager.field_Private_Static_VRCUiManager_0 == null) yield return new UnityEngine.WaitForSeconds(1f);

            while (Object.FindObjectOfType<VRC.UI.Elements.QuickMenu>() == null) yield return new UnityEngine.WaitForSeconds(0.5f);
            Nameplate.Start();
            Prefs.OnLateStart();
        }
        
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (buildIndex!=-1)
                Nameplate.OnSceneChanged();
        }
        
    }
}
