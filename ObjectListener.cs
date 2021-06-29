using UnhollowerBaseLib.Attributes;

namespace NameplateStats
{
    using System;
    using UnityEngine;

    public class ObjectListener : MonoBehaviour
    {
        public ObjectListener(IntPtr ptr) : base(ptr) {}
        [method: HideFromIl2Cpp]
        public event Action OnEnableEvent;
        [method: HideFromIl2Cpp]
        public event Action OnDisableEvent;
        private void OnEnable() => OnEnableEvent?.Invoke();
        private void OnDisable() => OnDisableEvent?.Invoke();
    }
}