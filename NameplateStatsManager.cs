﻿using System.Linq;
using VRC;

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
            MelonLogger.Msg("Starting 'NameplateStatsManager'");
            
            //TODO allow users to add custom gradient levels and maybe colours too.
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
        }

        private DateTime intervalCheck;
        private void LateUpdate()
        {
            if (DateTime.UtcNow < intervalCheck) return;
            intervalCheck = DateTime.UtcNow.AddMilliseconds(Prefs.UpdateTime);
            NameplateUpdate();
        }

        private void CleanupDict()
        {
            if (EntriesToRemove.Count <= 0) return;
            foreach (var player in EntriesToRemove)
            {
                PlayerText.Remove(player);
            }
        }
        private void OnDisable()
        {
            
            foreach (KeyValuePair<VRCPlayer,GameObject> keyPair in PlayerText)
            {
                DestroyImmediate(keyPair.Value);
            }
            PlayerText.Clear();
            
        }

        private void OnEnable()
        {
            foreach (VRCPlayer player in PlayerManager.prop_PlayerManager_0.prop_ArrayOf_Player_0.Select(ply => ply.prop_VRCPlayer_0))
            {
                Patches.OnVRCPlayerAwake(player);
            }
        }

        [HideFromIl2Cpp]
        private void NameplateUpdate()
        {
            CleanupDict();
            if (Locked) return;

            foreach (KeyValuePair<VRCPlayer, GameObject> keyPair in PlayerText)
            {
                if (!AdditionalChecks(keyPair.Key))
                {
                    if (keyPair.Key._player == null)
                    {
                        EntriesToRemove.Add(keyPair.Key);
                    }
                    continue;
                }

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
                
                //from https://github.com/loukylor/VRC-Mods/blob/c3a9b723a1ddb3cf17ae38737648720034e12c6e/PlayerList/Entries/PlayerEntry.cs#L164+L165
                var fps = MelonUtils.Clamp((int) (1000f / cacheNet.field_Private_Byte_0), -999, 9999);
                var ping = MelonUtils.Clamp(cacheNet.prop_Int16_0, -999, 9999);

                var cacheFPSTextComponent = cacheFPSText.GetComponent<TextMeshProUGUI>();
                var cachePingTextComponent = cachePingText.GetComponent<TextMeshProUGUI>();
                
                cacheFPSTextComponent.text = $"FPS:{fps}";
                cachePingTextComponent.text = $"PING:{ping}";

                switch (Prefs.DynamicColour)
                {
                    case true:
                        cacheFPSTextComponent.color = dynamicFPSColourGradient.Evaluate((float)fps/Prefs.GoodFPS);
                        cachePingTextComponent.color = dynamicPingColourGradient.Evaluate((float)ping/Prefs.BadPing);
                        break;
                    case false when cacheFPSTextComponent.color != Prefs.StaticColour:
                        cacheFPSTextComponent.color = Prefs.StaticColour;
                        cachePingTextComponent.color = Prefs.StaticColour;
                        break;
                }
            }
        }
        
        [HideFromIl2Cpp]
        public bool QuickMenuOpen
        {
            [HideFromIl2Cpp]
            set
            {
                needToMoveNameplates = value;
                NameplateUpdate();
            }
        }
    }
}