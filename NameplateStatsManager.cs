﻿namespace NameplateStats
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
        private void Start()
        {
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
                var cacheFPSText = keyPair.Value.transform.Find("FPS Text").gameObject;
                var cachePingText = keyPair.Value.transform.Find("Ping Text").gameObject;

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
                // ty louky ily
                cacheFPSText.GetComponent<TextMeshProUGUI>().text =
                    $"FPS:{MelonUtils.Clamp((int) (1000f / cacheNet.field_Private_Byte_0), -99, 999)}";

                cachePingText.GetComponent<TextMeshProUGUI>().text =
                    $"PING:{cacheNet.prop_Int16_0}";
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