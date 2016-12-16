using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace Expanse
{
    [Serializable]
    public class SerializedTimer : MonoBehaviour
    {
        protected Timer timer;

        public CallBackRelay callBackRelay;
        public MonoBehaviour attachedMonoBehaviour;
        public TimerSettings timerSettings;

        public UnityEvent completed = new UnityEvent();
        public UnityEvent returned = new UnityEvent();
        public UnityEvent completedOrReturned = new UnityEvent();

        void Reset()
        {
            attachedMonoBehaviour = this;

            timerSettings = TimerSettings.GetDefault(1f);
        }

        void Awake()
        {
            callBackRelay = callBackRelay ?? CallBackRelay.GlobalCBR;

            timer = Timer.Create(this, callBackRelay, timerSettings);

            timer.Completed += completed.Invoke;
            timer.Returned += returned.Invoke;
            timer.CompletedOrReturned += completedOrReturned.Invoke;

            timer.Deactivated += OnDeactivated;
        }

        private void OnDeactivated()
        {
            Destroy(this);
        }
    }
}
