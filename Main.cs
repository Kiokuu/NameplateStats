using MelonLoader;
using NameplateStats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[assembly: MelonInfo(typeof(NameplateStats.Main), ModInfo.InternalName, ModInfo.Version, ModInfo.Authors)]
[assembly: MelonGame("VRChat", "VRChat")]
[assembly: MelonColor(System.ConsoleColor.DarkCyan)]
[assembly: MelonPriority(62)]

namespace NameplateStats
{
    using MelonLoader;

    public static class ModInfo {
        public const string
            Title = "NameplateStats",
            Copyright = "Copyright © 2022",
            Version = "1.1.0",
            Authors = "Yato#4499, Dawn, lil-fluff",
            DownloadLink = "https://www.github.com/lil-fluff/NameplateStats",
            InternalName = "Nameplate Stats";

        public static readonly string[] OptionalDependencies = { "UIExpansionKit" };
    }

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
            while (VRCUiManager.field_Private_Static_VRCUiManager_0 == null) yield return null;
            while (Object.FindObjectOfType<VRC.UI.Elements.QuickMenu>() == null) yield return null;   
            Nameplate.Start();
            if(Prefs.IsBTKSANameplateModPresent) Prefs.OnLateStart();
        }
        
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (buildIndex!=-1)
                Nameplate.OnSceneChanged();
        }
        
    }
}