using System.Collections.Generic;
using MelonLoader;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NameplateStats
{
    public class Nameplate
    {
        public static GameObject PingFPSObjectRef;

        public static readonly Dictionary<VRCPlayer, GameObject> PlayerText = new();
        public static readonly HashSet<VRCPlayer> EntriesToRemove = new();
        public static bool Locked;
        public static void Start()
        {
            MelonLogger.Msg("Starting 'Nameplates'");
            Managers.Start();
        }
        
        public static bool AdditionalChecks(VRCPlayer player) => !(player == null || player.field_Public_PlayerNameplate_0 == null);
        
        public static void OnAvatarReady(VRCPlayer player)
        {
            if (!AdditionalChecks(player)) return;
            PlayerNameplate playerNameplate = player.field_Public_PlayerNameplate_0;
            if (!playerNameplate.isActiveAndEnabled) return;
            if (!PingFPSObjectRef)
            {
                GameObject temp = playerNameplate.transform.Find("Contents/Quick Stats").gameObject;

                PingFPSObjectRef = Object.Instantiate(temp);
                Object.DestroyImmediate(PingFPSObjectRef.transform.Find("Trust Icon").gameObject);
                Object.DestroyImmediate(PingFPSObjectRef.transform.Find("Performance Icon").gameObject);
                Object.DestroyImmediate(PingFPSObjectRef.transform.Find("Friend Anchor Stats").gameObject);

                PingFPSObjectRef.transform.Find("Trust Text").name = "FPS Text";
                PingFPSObjectRef.transform.Find("Performance Text").name = "Ping Text";
                    
                PingFPSObjectRef.name = "FPSPingReference";
                PingFPSObjectRef.SetActive(false);
                Object.DontDestroyOnLoad(PingFPSObjectRef);
            }

            if (playerNameplate.transform.Find("Contents/FPSPing")) return;
            var newStuff = Object.Instantiate(PingFPSObjectRef,
                playerNameplate.transform.Find("Contents/").transform);
            newStuff.name = "FPSPing";
                    
            newStuff.transform.localPosition = new Vector3(0, 30, 0);
                    
            var text = newStuff.transform.Find("FPS Text").GetComponent<TextMeshProUGUI>();
            text.text = "FPS:000";
            text.color = Color.green;

            text = newStuff.transform.Find("Ping Text").GetComponent<TextMeshProUGUI>();
            text.text = "Ping:0000";
            text.color = Color.green;
                    
            newStuff.SetActive(true);
            PlayerText.Add(player, newStuff);
            MelonLogger.Msg($"Added {player.prop_Player_0.prop_String_0}");

        }

        public static void NameplateBlanketPatch(PlayerNameplate instance)
        {
            if (instance.field_Private_VRCPlayer_0 != null &&
                !PlayerText.ContainsKey(instance.field_Private_VRCPlayer_0))
            {
                OnAvatarReady(instance.field_Private_VRCPlayer_0);
            }
        }
        public static void OnSceneChanged()
        {
            Locked = true;
            PlayerText.Clear();
            Locked = false;
        }
    }
}