using System.Collections;
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
        public static bool Locked = false;
        public static void Start()
        {
            Managers.Start();
            
            MelonLogger.Msg("Starting Module Nameplates");
            MelonCoroutines.Start(UpdateNameplateText());
        }
        
        private static bool AdditionalChecks(VRCPlayer player)
        {
            return !(player == null || player.field_Public_PlayerNameplate_0 == null);
        }
        
        public static void OnAvatarReady(VRCPlayer player)
        {
            if (AdditionalChecks(player))
            {
                PlayerNameplate playerNameplate = player.field_Public_PlayerNameplate_0;
                if (!playerNameplate.isActiveAndEnabled) return;
                if (!PingFPSObjectRef)
                {
                    GameObject temp = playerNameplate.transform.Find("Contents/Quick Stats").gameObject;
                    //MelonLogger.Msg($"{temp.name}");

                    PingFPSObjectRef = UnityEngine.Object.Instantiate(temp);
                    Object.DestroyImmediate(PingFPSObjectRef.transform.Find("Trust Icon").gameObject);
                    Object.DestroyImmediate(PingFPSObjectRef.transform.Find("Performance Icon").gameObject);
                    Object.DestroyImmediate(PingFPSObjectRef.transform.Find("Friend Anchor Stats").gameObject);

                    PingFPSObjectRef.name = "FPSPingReference";
                    PingFPSObjectRef.SetActive(false);
                }
                
                if (!playerNameplate.transform.Find("Contents/FPSPing"))
                {

                    var newStuff = Object.Instantiate(PingFPSObjectRef,
                        playerNameplate.transform.Find("Contents/").transform);
                    newStuff.name = "FPSPing";
                    
                    newStuff.transform.localPosition = new Vector3(0, 60, 0);
                    
                    var text = newStuff.transform.Find("Trust Text").GetComponent<TextMeshProUGUI>();
                    text.text = "FPS:000";
                    text.color = Color.green;

                    text = newStuff.transform.Find("Performance Text").GetComponent<TextMeshProUGUI>();
                    text.text = "Ping:0000";
                    text.color = Color.green;
                    
                    newStuff.SetActive(true);
                    PlayerText.Add(player, newStuff);
                    //MelonLogger.Msg($"Added {player._player.name}");
                }
            }

        }

        public static IEnumerator UpdateNameplateText()
        {
            while (true)
            {
                if (EntriesToRemove.Count > 0)
                {
                    foreach (VRCPlayer player in EntriesToRemove)
                    {
                        PlayerText.Remove(player);
                    }
                }
                if (Locked) yield return new WaitForSeconds(1);
 
                
                foreach (KeyValuePair<VRCPlayer, GameObject> keyPair in PlayerText)
                {
                    if (!AdditionalChecks(keyPair.Key))
                    {
                        if (keyPair.Key._player == null)
                        {
                            //MelonLogger.Msg("Removing player");
                            EntriesToRemove.Add(keyPair.Key);
                        }

                        continue;
                    }


                    //var cacheValue = keyPair.Value.transform.Find("Text").gameObject;
                    var cacheFPSText = keyPair.Value.transform.Find("Trust Text").gameObject;
                    var cachePingText = keyPair.Value.transform.Find("Performance Text").gameObject;
                    var cacheNet = keyPair.Key._playerNet;
                    if (cacheFPSText.active && cachePingText.active)
                    {
                        // ty louky ily
                        cacheFPSText.GetComponent<TextMeshProUGUI>().text =
                            $"FPS:{MelonUtils.Clamp((int) (1000f / cacheNet.field_Private_Byte_0), -99, 999)}";

                        cachePingText.GetComponent<TextMeshProUGUI>().text =
                            $"PING:{cacheNet.prop_Int16_0}";
                    }
                }

                yield return new WaitForSeconds(1f);
            }
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