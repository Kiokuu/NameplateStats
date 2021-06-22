namespace NameplateStats
{
    using System;
    using UnhollowerBaseLib.Attributes;
    using UnityEngine;

    public class NameplateStatsManager : MonoBehaviour
    {
        public NameplateStatsManager(IntPtr ptr) : base(ptr) {}

        private void LateUpdate()
        {
            
        }

        [HideFromIl2Cpp]
        private void OnQMOpen()
        {
            
        }

        [HideFromIl2Cpp]
        private void OnQMClose()
        {
            
        }
        
        [HideFromIl2Cpp]
        public bool QuickMenuOpen
        {
            set
            {
                if (value) OnQMOpen();
                else OnQMClose();
            }
            // set what happens after QM is open
        }
    }
}