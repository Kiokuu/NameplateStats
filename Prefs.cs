namespace NameplateStats
{
    using System;
    using MelonLoader;
    using UnhollowerRuntimeLib;

    public static class Prefs
    {
        public static void OnStart()
        {
            ClassInjector.RegisterTypeInIl2Cpp<NameplateStatsManager>();
            ClassInjector.RegisterTypeInIl2Cpp<ObjectListener>();
            RegisterPreferences();
        }

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
        }

        private static MelonPreferences_Entry<bool> _Enabled;
        private static MelonPreferences_Entry<bool> _Ping;
        private static MelonPreferences_Entry<bool> _FPS;
        private static MelonPreferences_Entry<bool> _DynamicResizing;
        private static MelonPreferences_Entry<bool> _DynamicColour;
        private static MelonPreferences_Entry<string> _Colour;
        private static MelonPreferences_Entry<int> _UpdateTime;

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