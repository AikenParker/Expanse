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

        void Reset()
        {
            attachedMonoBehaviour = this;

            timerSettings = TimerSettings.GetDefault(1f);
            timerSettings.autoPlay = true;
        }

        void Awake()
        {
            if (!this.enabled)
                return;

            callBackRelay = useGlobalCBR ? CallBackRelay.GlobalCBR : callBackRelay;

            if (callBackRelay != null)
            {
                timer = Timer.Create(this, callBackRelay, timerSettings);
            }
            else
            {
                timer = Timer.Create(this, timerSettings);
            }

            if (enableCompletedEvent)
                timer.Completed += completed.Invoke;

            if (enableReturnedEvent)
                timer.Returned += returned.Invoke;

            if (enableCompletedOrReturnedEvent)
                timer.CompletedOrReturned += completedOrReturned.Invoke;

            timer.Deactivated += OnDeactivated;
        }

        void Start() { }

        public void Play()
        {
            timer.Play();
        }

        public void Stop()
        {
            timer.Stop();
        }

        public void Deactivate()
        {
            timer.Deactivate();
        }

        private void OnDeactivated()
        {
            Destroy(this);
        }
    }
}
