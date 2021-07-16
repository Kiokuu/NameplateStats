namespace NameplateStats
{
    using System.Linq;
    using System.Reflection;
    using HarmonyLib;
    using MelonLoader;

    public class Patches
    {
        private static readonly System.Collections.Generic.IEnumerable<MethodInfo> NameplateMethods = typeof(PlayerNameplate).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(x => x.Name.StartsWith("Method_Public_Void_") && x.Name.Length == 20);
        private static readonly MethodInfo NameplateMethodsPatch = typeof(Patches).GetMethod(nameof(NameplateBlanketPatch));
        private static readonly MethodInfo VRCPlayerAwake = typeof(VRCPlayer).GetMethod("Awake");
        private static readonly MethodInfo VRCPlayerPatch = typeof(Patches).GetMethod(nameof(OnVRCPlayerAwake));
        public static bool Patched = false;
        
        public static void TogglePatches(bool toggle)
        {
            if (toggle)
            {
                Main.Instance.Patch(VRCPlayerAwake,
                    postfix: new HarmonyMethod(VRCPlayerPatch)
                    );
                foreach (var method in NameplateMethods)
                {
                    Main.Instance.Patch(method,
                        postfix: new HarmonyMethod(typeof(Patches).GetMethod(nameof(NameplateBlanketPatch))));
                }

                Patched = true;
            }
            else if (Patched == false) return;
            else
            {
                Main.Instance.Unpatch(VRCPlayerAwake,VRCPlayerPatch);
                foreach (var method in NameplateMethods)
                {
                    Main.Instance.Unpatch(method, NameplateMethodsPatch);
                }

                Patched = false;
            }

            var word = (toggle ? "Patched" : "Unpatched");
            MelonLogger.Msg($"{word} VRCPlayer.Awake()");
            MelonLogger.Msg($"{word} Nameplate Methods: {string.Join(", ",NameplateMethods.Select(methodName=>methodName.Name))}");
        }

        //"borrowed" from nameplate king https://github.com/ddakebono/BTKSANameplateFix/blob/6a150d520e6a49e2e1ea3484ec673899380f9ccb/BTKSANameplateMod.cs#L715
        public static void OnVRCPlayerAwake(VRCPlayer __instance)
        {
            __instance.Method_Public_add_Void_MulticastDelegateNPublicSealedVoUnique_0(new System.Action(() =>
            {
                if (__instance != null && __instance._player != null && __instance._player.prop_APIUser_0 != null)
                {
                    Nameplate.OnAvatarReady(__instance);
                }
            }));
        }
        
        // "borrowed" with love from https://github.com/ddakebono/BTKSANameplateFix/blob/6a150d520e6a49e2e1ea3484ec673899380f9ccb/BTKSANameplateMod.cs#L646
        public static void NameplateBlanketPatch(PlayerNameplate __instance)
        {
            Nameplate.NameplateBlanketPatch(__instance);
        }
    }
}