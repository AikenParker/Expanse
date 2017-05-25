using Expanse.Utilities;
using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// Provides time related properties. Useful to reduce engine calls through UnityEngine.Time.
    /// </summary>
    [DefaultExecutionOrder(ExecutionOrderConstants.TIME_MANAGER)]
    public sealed class TimeManager : Singleton<TimeManager>
    {
        private static int frameCount;
        private static float time;
        private static float deltaTime;
        private static float unscaledTime;
        private static float unscaledDeltaTime;
        private static float fixedDeltaTime;
        private static float timeScale;

        static TimeManager()
        {
            // Creates a hidden instance of a TimeManager if there is not one already

            TimeManager instance = FindObjectOfType<TimeManager>();

            if (instance == null)
            {
                GameObject timeManagerGameObject = new GameObject(typeof(TimeManager).Name);
                //timeManagerGameObject.hideFlags = HideFlags.HideInHierarchy;
                instance = timeManagerGameObject.AddComponent<TimeManager>();
            }

            TimeManager.Instance = instance;

            fixedDeltaTime = UnityEngine.Time.fixedDeltaTime;
            timeScale = UnityEngine.Time.timeScale;
        }

        void Update()
        {
            frameCount++;
            time = UnityEngine.Time.time;
            deltaTime = UnityEngine.Time.deltaTime;
            unscaledTime = UnityEngine.Time.unscaledTime;
            unscaledDeltaTime = UnityEngine.Time.unscaledDeltaTime;
        }

        public static float Time
        {
            get { return time; }
        }

        public static float DeltaTime
        {
            get { return deltaTime; }
        }

        public static float UnscaledTime
        {
            get { return unscaledTime; }
        }

        public static float UnscaledDeltaTime
        {
            get { return unscaledDeltaTime; }
        }

        public static float FixedDeltaTime
        {
            get { return fixedDeltaTime; }
            set { UnityEngine.Time.fixedDeltaTime = fixedDeltaTime = value; }
        }

        public static float TimeScale
        {
            get { return timeScale; }
            set { UnityEngine.Time.timeScale = timeScale = value; }
        }
    }
}
