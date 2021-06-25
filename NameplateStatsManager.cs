namespace NameplateStats
{
    using System;
    using System.Collections.Generic;
    using MelonLoader;
    using TMPro;
    using UnhollowerBaseLib.Attributes;
    using UnityEngine;
    using static Nameplate;

    public class NameplateStatsManager : MonoBehaviour
    {
        public NameplateStatsManager(IntPtr ptr) : base(ptr) {}
        private readonly Vector3 quickMenuOpenPosition = new(0, 60, 0);
        private readonly Vector3 quickMenuClosePosition = new(0, 30, 0);
        
        private bool needToMoveNameplates;

        private Gradient dynamicFPSColourGradient;
        private Gradient dynamicPingColourGradient;
        
        private void Start()
        {
            
            var colKey = new GradientColorKey[3];
            colKey[0].color = Color.red;
            colKey[0].time = 0.0f;
            colKey[1].color = Color.yellow;
            colKey[1].time = 0.5f;
            colKey[2].color = Color.green;
            colKey[2].time = 1f;

            var alphaKey = new GradientAlphaKey[2];
            alphaKey[0].alpha = 1.0f;
            alphaKey[0].time = 0f;
            alphaKey[1].alpha = 1.0f;
            alphaKey[1].time = 1f;
            
            dynamicFPSColourGradient = new Gradient {colorKeys = colKey, alphaKeys = alphaKey, mode = GradientMode.Blend};

            var colKeyPing = new GradientColorKey[4];
            colKeyPing[0].color = Color.green;
            colKeyPing[0].time = 0.0f;
            colKeyPing[1].color = Color.green;
            colKeyPing[1].time = 0.25f;
            colKeyPing[2].color = Color.yellow;
            colKeyPing[2].time = 0.5f;
            colKeyPing[3].color = Color.red;
            colKeyPing[3].time = 1f;
            
            dynamicPingColourGradient = new Gradient {colorKeys = colKeyPing, alphaKeys = alphaKey, mode = GradientMode.Blend};
            MelonLogger.Msg("Starting 'NameplateStatsManager'");
        }

        private DateTime intervalCheck;
        private void LateUpdate()
        {
            if (DateTime.UtcNow < intervalCheck) return;
            intervalCheck = DateTime.UtcNow.AddMilliseconds(Prefs.UpdateTime);
            NameplateUpdate();
        }

        private void OnDisable()
        {
            if (EntriesToRemove.Count > 0)
            {
                foreach (VRCPlayer player in EntriesToRemove)
                {
                    PlayerText.Remove(player);
                }
            }
        }

        [HideFromIl2Cpp]
        private void NameplateUpdate()
        {
            if (EntriesToRemove.Count > 0)
            {
                foreach (VRCPlayer player in EntriesToRemove)
                {
                    PlayerText.Remove(player);
                }
            }

            if (Locked) return;
 
                
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
                var cacheFPSText = keyPair.Value.transform.GetChild(0).gameObject;
                var cachePingText = keyPair.Value.transform.GetChild(1).gameObject;

                if (!cacheFPSText.active || !cachePingText.active) continue;
                
                var cacheNet = keyPair.Key._playerNet;
                
                if (needToMoveNameplates && keyPair.Value.transform.localPosition==quickMenuClosePosition)
                {
                    keyPair.Value.transform.localPosition = quickMenuOpenPosition;
                }
                else if (!needToMoveNameplates && keyPair.Value.transform.localPosition == quickMenuOpenPosition)
                {
                    keyPair.Value.transform.localPosition = quickMenuClosePosition;
                }
                
                var fps = MelonUtils.Clamp((int) (1000f / cacheNet.field_Private_Byte_0), -999, 9999);
                var ping = MelonUtils.Clamp(cacheNet.prop_Int16_0, -999, 9999);

                var cacheFPSTextComponent = cacheFPSText.GetComponent<TextMeshProUGUI>();
                var cachePingTextComponent = cachePingText.GetComponent<TextMeshProUGUI>();
                // ty louky ily
                cacheFPSTextComponent.text =
                    $"FPS:{fps}";

                cachePingTextComponent.text =
                    $"PING:{ping}";

                if (Prefs.DynamicColour)
                {
                    cacheFPSTextComponent.color = dynamicFPSColourGradient.Evaluate((float)fps/60); // will change to prefs
                    cachePingTextComponent.color = dynamicPingColourGradient.Evaluate((float)ping/300);
                }
                //else if(!Prefs.DynamicColour && )
            }
        }
        
        [HideFromIl2Cpp]
        private void OnQMOpen()
        {
            needToMoveNameplates = true;
            NameplateUpdate();
        }

        [HideFromIl2Cpp]
        private void OnQMClose()
        {
            needToMoveNameplates = false;
            NameplateUpdate();
        }
        
        [HideFromIl2Cpp]
        public bool QuickMenuOpen
        {
            [HideFromIl2Cpp]
            set
            {
                if (value) OnQMOpen();
                else OnQMClose();
            }
            // set what happens after QM is open
        }
    }
}