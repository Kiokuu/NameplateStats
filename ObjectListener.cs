namespace NameplateStats
{
    using System;
    using UnityEngine;

    public class ObjectListener : MonoBehaviour
    {
        public ObjectListener(IntPtr ptr) : base(ptr) {}

        public event Action OnEnableEvent;
        public event Action OnDisableEvent;
        private void OnEnable() => OnEnableEvent?.Invoke();
        private void OnDisable() => OnDisableEvent?.Invoke();
    }
}