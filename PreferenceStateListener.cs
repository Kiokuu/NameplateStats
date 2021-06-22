using System;

namespace Dawn.Utilities
{
    public class PreferencesStateListener
    {
        /// <summary>
        /// Typically used OnAppStart
        /// </summary>
        public PreferencesStateListener(bool Preference, Action OnTrue, Action OnFalse, bool InitialInvoke = false)
        {
            PreviousResult = Preference;
            this.OnTrue = OnTrue;
            this.OnFalse = OnFalse;
            if (Preference && InitialInvoke) OnTrue?.Invoke();
        }
        /// <summary>
        /// Typically used OnPreferencesSaved
        /// </summary>
        public void Update(bool UpdateState)
        {
            if (UpdateState)
            {
                if (!PreviousResult)
                {
                    OnTrue?.Invoke();
                }
            }
            else
            {
                if (PreviousResult)
                {
                    OnFalse?.Invoke();
                }
            }
            PreviousResult = UpdateState;
        }

        public void ForceUpdate(bool UpdateState)
        {
            if (UpdateState) OnTrue?.Invoke();
            else OnFalse?.Invoke();
            PreviousResult = UpdateState;
        }
        private readonly Action OnTrue;
        private readonly Action OnFalse;
        private bool PreviousResult;
    }
}