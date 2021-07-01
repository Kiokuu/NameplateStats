using System.Linq;
using UnityEngine;

namespace NameplateStats
{
    using System;
    using Dawn.Utilities;
    using MelonLoader;
    using UnhollowerRuntimeLib;

    public static class Prefs
    {
        public static void OnStart()
        {
            ClassInjector.RegisterTypeInIl2Cpp<NameplateStatsManager>();
            ClassInjector.RegisterTypeInIl2Cpp<ObjectListener>();
            RegisterPreferences();

            EnabledListener = new PreferencesStateListener(Enabled, () =>
            {
                MelonLogger.Msg("Enabling NameplateStats! If you are in lobby, nameplates may take some time to appear!");
                Patches.DoPatches();
                Managers.NameplateStatsManager.enabled = true;
            }, () =>
            {
                MelonLogger.Msg("Disabling NameplateStats!");
                Patches.Unpatch();
                Managers.NameplateStatsManager.enabled = false;
            });
            //TODO Implement Ping/FPS Listeners, enable/disable the visibility individually in nameplate stats
            //PingListener = new PreferencesStateListener(Ping, () => { }, () => { });
            //FPSListener = new PreferencesStateListener(FPS, () => { }, () => { });

            if (IsBTKSANameplateModPresent = MelonHandler.Mods.Any(m => m.Info.Name == "BTKSANameplateMod" || m.Info.Name == "BTKCompanionLoader")) // untested companionloader
            {
                MelonLogger.Msg("BTKSANameplateMod/BTKCompanion detected! enabling colour matching functionality!");
            };
            _CachedColour = new Color32(255, 255, 255, 255);
        }

        public static void OnLateStart()
        {
            if (!IsBTKSANameplateModPresent) return;
            _IsAlwaysShowQuickInfoOn = MelonPreferences.GetCategory("BTKSANameplateFix")
                .GetEntry<bool>("nmAlwaysShowQuickInfo");
            IsAlwaysShowQuickInfoOnListener = new PreferencesStateListener(IsAlwaysShowQuickInfoOn, () =>
            {
                Managers.NameplateStatsManager.AlwaysShowQuickMenuStats = true;
            }, () =>
            {
                Managers.NameplateStatsManager.AlwaysShowQuickMenuStats = false;
            }, true);
        }
        public static void ForceUpdateListeners() // On VRCUiManagerInit
        {
            EnabledListener.ForceUpdate(Enabled);
            //PingListener.ForceUpdate(Ping);
            //FPSListener.ForceUpdate(FPS);
        }

        public static void UpdateListeners()
        {
            EnabledListener.Update(Enabled);
            if (_IsAlwaysShowQuickInfoOn != null) IsAlwaysShowQuickInfoOnListener.Update(IsAlwaysShowQuickInfoOn);
            //PingListener.Update(Ping);
            //FPSListener.Update(FPS);
        }

        private static PreferencesStateListener EnabledListener;

        private static PreferencesStateListener IsAlwaysShowQuickInfoOnListener;
        //private static PreferencesStateListener PingListener;
        //private static PreferencesStateListener FPSListener;


        private static void RegisterPreferences()
        {
            var cat = MelonPreferences.CreateCategory("NameplateStats");
            
            _Enabled = cat.CreateEntry("Enabled", true);
            //_Ping = cat.CreateEntry("Ping", true);
            //_FPS = cat.CreateEntry("FPS", true);
            _DynamicColour = cat.CreateEntry("DynamicColour", true, "Dynamic Colour",
                "Enable colouring of the stats dependant on value (green = good, yellow = ok, red = bad");
            _UpdateTime = cat.CreateEntry("NameplateUpdateTime", (short) 1000, "Nameplate Update Time (ms)",
                "The lower this is, the quicker the stats will refresh at the cost of performance");

            _ColourR = cat.CreateEntry("ColourR", (byte) 255, "Static Colour(R)",
                "Colour(R) of the text when DynamicColour is turned off.");
            _ColourG = cat.CreateEntry("ColourG", (byte) 255, "Static Colour(G)",
                "Colour(G) of the text when DynamicColour is turned off.");
            _ColourB = cat.CreateEntry("ColourB", (byte) 255, "Static Colour(B)",
                "Colour(B) of the text when DynamicColour is turned off.");

            _DynamicColouringGoodFPS = cat.CreateEntry("DynColour_GoodFPS", (short) 60, "Dynamic Colouring - Good FPS",
                "If Dynamic Colouring Enabled: The Value For Good FPS");
            /*            
            _DynamicColouringBadFPS = cat.CreateEntry("DynColour_BadFPS", (short) 20, "Dynamic Colouring - Bad FPS",
                "If Dynamic Colouring Enabled: The Value For Bad FPS");
            _DynamicColouringGoodPing = cat.CreateEntry("DynColour_GoodPing", (short) 69,
                "Dynamic Colouring - Good Ping", "If Dynamic Colouring Enabled: The Value For Good Ping");
            */
            _DynamicColouringBadPing = cat.CreateEntry("DynColour_BadPing", (short) 300, "Dynamic Colouring - Bad Ping",
                "If Dynamic Colouring Enabled: The Value For Bad Ping");
        }

        private static MelonPreferences_Entry<bool> _Enabled;
        //private static MelonPreferences_Entry<bool> _Ping;
        //private static MelonPreferences_Entry<bool> _FPS;
        private static MelonPreferences_Entry<bool> _DynamicColour;
        private static MelonPreferences_Entry<short> _UpdateTime;

        //Colours
        private static MelonPreferences_Entry<byte> _ColourR;
        private static MelonPreferences_Entry<byte> _ColourG;
        private static MelonPreferences_Entry<byte> _ColourB;
        private static Color32 _CachedColour;

        private static MelonPreferences_Entry<short> _DynamicColouringGoodFPS;
        private static MelonPreferences_Entry<short> _DynamicColouringBadPing;
        private static MelonPreferences_Entry<bool> _IsAlwaysShowQuickInfoOn;
        //private static MelonPreferences_Entry<short> _DynamicColouringBadFPS;
        //private static MelonPreferences_Entry<short> _DynamicColouringGoodPing;

        public static bool IsBTKSANameplateModPresent;
        public static bool IsAlwaysShowQuickInfoOn => _IsAlwaysShowQuickInfoOn.Value;
        
        public static short GoodFPS => MelonUtils.Clamp<short>(_DynamicColouringGoodFPS.Value, 0, 9999);
        public static short BadPing => MelonUtils.Clamp<short>(_DynamicColouringBadPing.Value, 0, 9999);
        //TODO Implement proper scaling for dynamic colours
        //public static short BadFPS => MelonUtils.Clamp<short>(_DynamicColouringBadFPS.Value, 0, 9999);
        //public static short GoodPing => MelonUtils.Clamp<short>(_DynamicColouringGoodPing.Value, 0, 9999);
        
        public static bool Enabled => _Enabled.Value;
        //public static bool Ping => _Ping.Value;
        //public static bool FPS => _FPS.Value;
        public static bool DynamicColour => _DynamicColour.Value;
        public static short UpdateTime =>  MelonUtils.Clamp<short>(_UpdateTime.Value,50,30000);

        public static Color StaticColour
        {
            get
            {
                if (_CachedColour.r != _ColourR.Value || _CachedColour.g != _ColourG.Value ||
                    _CachedColour.b != _ColourB.Value)
                {
                    _CachedColour = new Color32(_ColourR.Value, _ColourG.Value, _ColourB.Value, 255);
                }

                return _CachedColour;
            }
        }

        public static bool IconsOnlyMode =>
            MonoBehaviourPublicObSiLi1CoObBoStBoSiUnique.field_Private_Static_EnumNPublicSealedvaStIcHiMA5vUnique_0 ==
            MonoBehaviourPublicObSiLi1CoObBoStBoSiUnique.EnumNPublicSealedvaStIcHiMA5vUnique.Icons;
    }
}