namespace NameplateStats
{
    using System;
    using UnhollowerBaseLib.Attributes;
    using UnityEngine;

    public class NameplateStatsManager : MonoBehaviour
    {
        public NameplateStatsManager(IntPtr ptr) : base(ptr) {}

        private DateTime intervalCheck;
        private void LateUpdate()
        {
            if (DateTime.UtcNow < intervalCheck) return;
            intervalCheck = DateTime.UtcNow.AddMilliseconds(Prefs.UpdateTime);
            NameplateUpdate();
        }

        private void NameplateUpdate()
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