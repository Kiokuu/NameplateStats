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

            EnabledListener = new PreferencesStateListener(Enabled, () => { }, () => { });
            PingListener = new PreferencesStateListener(Ping, () => { }, () => { });
            FPSListener = new PreferencesStateListener(FPS, () => { }, () => { });
            DynamicResizingListener = new PreferencesStateListener(DynamicResizing, () => { }, () => { });
            DynamicColourListener = new PreferencesStateListener(DynamicColour, () => { }, () => { });
        }

        public static void ForceUpdateListeners() // On VRCUiManagerInit
        {
            EnabledListener.ForceUpdate(Enabled);
            PingListener.ForceUpdate(Ping);
            FPSListener.ForceUpdate(FPS);
            DynamicResizingListener.ForceUpdate(DynamicResizing);
            DynamicColourListener.ForceUpdate(DynamicColour);
        }
        public static void UpdateListeners()
        {
            EnabledListener.Update(Enabled);
            PingListener.Update(Ping);
            FPSListener.Update(FPS);
            DynamicResizingListener.Update(DynamicResizing);
            DynamicColourListener.Update(DynamicColour);
        }

        private static PreferencesStateListener EnabledListener;
        private static PreferencesStateListener PingListener;
        private static PreferencesStateListener FPSListener;
        private static PreferencesStateListener DynamicResizingListener;
        private static PreferencesStateListener DynamicColourListener;
        

        private static void RegisterPreferences()
        {
            var cat = MelonPreferences.CreateCategory("NameplateStats");
            _Enabled = cat.CreateEntry("Enabled", true);
            _Ping = cat.CreateEntry("Ping", true);
            _FPS = cat.CreateEntry("FPS", true);
            _DynamicResizing = cat.CreateEntry("DynamicResizing", true, "Dyamic Resizing");
            _DynamicColour = cat.CreateEntry("DynamicColour", true, "Dynamic Colour");
            _Colour = cat.CreateEntry("Colour", "Green");
            _UpdateTime = cat.CreateEntry("NameplateUpdateTime", 1000, "Nameplate Update Time (ms)");
            
            ColourR = cat.CreateEntry("ColourR", (byte)255, "Static Colour(R)", "Colour(R) of the text when DynamicColour is turned off.");
            ColourG = cat.CreateEntry("ColourR", (byte)255, "Static Colour(G)", "Colour(G) of the text when DynamicColour is turned off.");
            ColourB = cat.CreateEntry("ColourR", (byte)255, "Static Colour(B)", "Colour(B) of the text when DynamicColour is turned off.");
            
            DynamicColouringGoodFPS = cat.CreateEntry("DynColour_GoodFPS", (short)60, "Dynamic Colouring - Good FPS", "If Dynamic Colouring Enabled: The Value For Good FPS");
            DynamicColouringBadFPS = cat.CreateEntry("DynColour_BadFPS", (short)20, "Dynamic Colouring - Bad FPS", "If Dynamic Colouring Enabled: The Value For Bad FPS");
            
            DynamicColouringGoodPing = cat.CreateEntry("DynColour_GoodPing", (short)69, "Dynamic Colouring - Good Ping", "If Dynamic Colouring Enabled: The Value For Good Ping");
            DynamicColouringBadPing = cat.CreateEntry("DynColour_BadPing", (short)300, "Dynamic Colouring - Bad Ping", "If Dynamic Colouring Enabled: The Value For Bad Ping");
            
            
        }

        private static MelonPreferences_Entry<bool> _Enabled;
        private static MelonPreferences_Entry<bool> _Ping;
        private static MelonPreferences_Entry<bool> _FPS;
        private static MelonPreferences_Entry<bool> _DynamicResizing;
        private static MelonPreferences_Entry<bool> _DynamicColour;
        private static MelonPreferences_Entry<string> _Colour;
        private static MelonPreferences_Entry<int> _UpdateTime;
        //Colours
        private static MelonPreferences_Entry<byte> ColourR;
        private static MelonPreferences_Entry<byte> ColourG;
        private static MelonPreferences_Entry<byte> ColourB;

        private static MelonPreferences_Entry<short> DynamicColouringGoodFPS;
        private static MelonPreferences_Entry<short> DynamicColouringBadFPS;
        
        private static MelonPreferences_Entry<short> DynamicColouringGoodPing;
        private static MelonPreferences_Entry<short> DynamicColouringBadPing;

        

        public static bool Enabled => _Enabled.Value;
        public static bool Ping => _Ping.Value;
        public static bool FPS => _FPS.Value;
        public static bool DynamicResizing => _DynamicResizing.Value;
        public static bool DynamicColour => _DynamicColour.Value;
        public static PresetColours Colour => (PresetColours) Enum.Parse(typeof(PresetColours), _Colour.Value);
        public static int UpdateTime => _UpdateTime.Value;

        public enum PresetColours
        {
            Green,
            something,
            somethingagain,
        }
    }
}