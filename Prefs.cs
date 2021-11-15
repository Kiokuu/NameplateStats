namespace NameplateStats
{
    using System.Diagnostics.CodeAnalysis;
    using MelonLoader;
    using UnhollowerRuntimeLib;
    using VRC;
    using System.Linq;
    using UnityEngine;

    public class Prefs
    {
        [SuppressMessage("ReSharper", "AssignmentInConditionalExpression")]
        public static void OnStart()
        {
            ClassInjector.RegisterTypeInIl2Cpp<NameplateStatsManager>();
            ClassInjector.RegisterTypeInIl2Cpp<ObjectListener>();
            RegisterPreferences();

            _Enabled.OnValueChanged += (_, b1) => ToggleEnable(b1);

            //TODO Implement Ping/FPS Listeners, enable/disable the visibility individually in nameplate stats
            
            var BonoMod = MelonHandler.Mods.Where(m => m.Info.Name is "BTKSANameplateMod" or "BTKCompanionLoader").ToArray();
            if (IsBTKSANameplateModPresent = BonoMod.Any())
            {
                if (BonoMod.Any(m => m.Info.Name == "BTKSANameplateMod"))
                {
                    MelonLogger.Msg("BTKSANameplateMod detected! enabling colour matching functionality!");
                    isNameplateFixSAPresent = true;
                }

                if (BonoMod.Any(m => m.Info.Name == "BTKCompanionLoader"))
                {
                    MelonLogger.Msg("BTKCompanion detected! enabling colour matching functionality!");
                    isCompanionPresent = true;
                }
            }
            _CachedColour = new Color32(255, 255, 255, 255);
        }
        
        private static bool? isNameplateFixSAPresent = false;
        private static bool? isCompanionPresent = false;
        public static void OnLateStart()
        {
            if (!IsBTKSANameplateModPresent) return;
            if (isCompanionPresent!.Value)
            {
                _IsAlwaysShowQuickInfoOn = MelonPreferences.GetCategory("BTKCompanionNP")
                    .GetEntry<bool>("nmAlwaysShowQuickInfo");
            }
            else if (isNameplateFixSAPresent!.Value)
            {
                _IsAlwaysShowQuickInfoOn = MelonPreferences.GetCategory("BTKSANameplateFix")
                    .GetEntry<bool>("nmAlwaysShowQuickInfo");
            }
            isNameplateFixSAPresent = null;
            isCompanionPresent = null; // Yeeted out since we don't need this lurking in memory.

            _IsAlwaysShowQuickInfoOn.OnValueChanged +=
                (_, b1) => Managers.NameplateStatsManager.AlwaysShowQuickMenuStats = b1;
        }

        public static void ToggleEnable(bool enable)
        {
            MelonLogger.Msg($"{(enable ? "Enabling" : "Disabling")} NameplateStats.");
            Patches.TogglePatches(enable);
            Managers.NameplateStatsManager.enabled = enable;
        }

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
        
        public static bool QMInfoShow => MonoBehaviourPublicMaObMaSiLi1CoObBoTeUnique.prop_Boolean_0;
        
        //public static bool IconsOnlyMode =>
        //    NameplateManager.prop_NameplateMode_0 == NameplateManager.NameplateMode.Icons;

        public static bool IconsOnlyMode =>
            MonoBehaviourPublicMaObMaSiLi1CoObBoTeUnique.field_Private_Static_EnumNPublicSealedvaStIcHiMA5vUnique_0 ==
                MonoBehaviourPublicMaObMaSiLi1CoObBoTeUnique.EnumNPublicSealedvaStIcHiMA5vUnique.Icons;

    }
}