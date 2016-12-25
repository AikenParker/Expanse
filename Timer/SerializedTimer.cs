using System;
using UnityEngine;
using UnityEngine.Events;

namespace Expanse
{
    [Serializable]
    public class SerializedTimer : MonoBehaviour
    {
        protected Timer timer;

        [SerializeField]
        private bool useGlobalCBR = true;

        [SerializeField]
        private CallBackRelay callBackRelay;
        [SerializeField]
        private MonoBehaviour attachedMonoBehaviour;
        [SerializeField]
        private TimerSettings timerSettings;

        [SerializeField]
        private UnityEvent completed = new UnityEvent();
        [SerializeField]
        private UnityEvent returned = new UnityEvent();
        [SerializeField]
        private UnityEvent completedOrReturned = new UnityEvent();

        [SerializeField, HideInInspector]
        private bool enableCompletedEvent = true;
        [SerializeField, HideInInspector]
        private bool enableReturnedEvent = false;
        [SerializeField, HideInInspector]
        private bool enableCompletedOrReturnedEvent = false;

        private bool isInitialized;

        void Reset()
        {
            attachedMonoBehaviour = this;

            timerSettings = TimerSettings.Default.WithDuration(1f);
            timerSettings.autoPlay = true;
        }

        void Start()
        {
            Initialize();
        }

        protected void Initialize()
        {
            if (isInitialized)
                return;

            isInitialized = true;

            //

            callBackRelay = useGlobalCBR ? CallBackRelay.GlobalCBR : callBackRelay;

            if (callBackRelay != null)
            {
                timer = Timer.Create(attachedMonoBehaviour, callBackRelay, timerSettings);
            }
            else
            {
                timer = Timer.Create(attachedMonoBehaviour, timerSettings);
            }

            if (enableCompletedEvent)
                timer.Completed += completed.Invoke;

            if (enableReturnedEvent)
                timer.Returned += returned.Invoke;

            if (enableCompletedOrReturnedEvent)
                timer.CompletedOrReturned += completedOrReturned.Invoke;

            timer.Deactivated += OnDeactivated;
        }

        public Timer Timer
        {
            get
            {
                if (!isInitialized)
                    Initialize();

                return this.timer;
            }
        }

        private void OnDeactivated()
        {
            Destroy(this);
        }
    }
}
