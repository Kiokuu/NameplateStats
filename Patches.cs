using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MelonLoader;

namespace NameplateStats
{
    public class Patches
    {
        public static void DoPatches()
        {
            var harmonyInstance = Main.Instance.HarmonyInstance;
            harmonyInstance.Patch(typeof(VRCPlayer).GetMethod("Awake"),
                postfix: new HarmonyMethod(typeof(Patches).GetMethod(nameof(OnVRCPlayerAwake)))
            );
            MelonLogger.Msg($"Patched VRCPlayer.Awake()");

            var nameplateMethods = typeof(PlayerNameplate).GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.Name.StartsWith("Method_Public_Void_") && x.Name.Length == 20);

            foreach (var method in nameplateMethods)
            {
                harmonyInstance.Patch(method,
                    postfix: new HarmonyMethod(typeof(Patches).GetMethod(nameof(NameplateBlanketPatch))));
            }
            
            MelonLogger.Msg($"Patched Nameplate Methods: {String.Join(", ",nameplateMethods.Select(methodName=>methodName.Name))}");
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