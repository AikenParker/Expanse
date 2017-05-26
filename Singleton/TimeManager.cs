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
        // Make fields public for faster access times
        private static int frameCount;
        private static float realtimeSinceStartup;
        private static float time;
        private static float deltaTime;
        private static float unscaledTime;
        private static float unscaledDeltaTime;
        private static float fixedDeltaTime;
        private static float timeScale;
        private static float timeSinceLevelLoad;

        static TimeManager()
        {
            // Creates a hidden instance of a TimeManager if there is not one already

            TimeManager instance = FindObjectOfType<TimeManager>();

            if (instance == null)
            {
                GameObject timeManagerGameObject = new GameObject(typeof(TimeManager).Name);
                timeManagerGameObject.hideFlags = HideFlags.HideInHierarchy;
                instance = timeManagerGameObject.AddComponent<TimeManager>();
            }

            DontDestroyOnLoad(instance.gameObject);

            TimeManager.Instance = instance;

            fixedDeltaTime = UnityEngine.Time.fixedDeltaTime;
            timeScale = UnityEngine.Time.timeScale;
            frameCount = UnityEngine.Time.frameCount;
            realtimeSinceStartup = UnityEngine.Time.realtimeSinceStartup;
            time = UnityEngine.Time.time;
            deltaTime = UnityEngine.Time.deltaTime;
            unscaledTime = UnityEngine.Time.unscaledTime;
            unscaledDeltaTime = UnityEngine.Time.unscaledDeltaTime;
            timeSinceLevelLoad = UnityEngine.Time.timeSinceLevelLoad;
        }

        void Update()
        {
            frameCount++;
            realtimeSinceStartup = UnityEngine.Time.realtimeSinceStartup;
            time = UnityEngine.Time.time;
            deltaTime = UnityEngine.Time.deltaTime;
            unscaledTime = UnityEngine.Time.unscaledTime;
            unscaledDeltaTime = UnityEngine.Time.unscaledDeltaTime;
            timeSinceLevelLoad = UnityEngine.Time.timeSinceLevelLoad;
        }

        /// <summary>
        /// The total number of frames that have passed (Read Only).
        /// </summary>
        public static int FrameCount
        {
            get { return frameCount; }
        }

        /// <summary>
        /// The real time in seconds since the game started (Read Only).
        /// </summary>
        public static float RealtimeSinceStartup
        {
            get { return realtimeSinceStartup; }
        }

        /// <summary>
        /// The time at the beginning of this frame (Read Only). This is the time in seconds
        /// since the start of the game.
        /// </summary>
        public static float Time
        {
            get { return time; }
        }

        /// <summary>
        /// The time in seconds it took to complete the last frame (Read Only).
        /// </summary>
        public static float DeltaTime
        {
            get { return deltaTime; }
        }

        /// <summary>
        /// The timeScale-independent time for this frame (Read Only). This is the time in
        /// seconds since the start of the game.
        /// </summary>
        public static float UnscaledTime
        {
            get { return unscaledTime; }
        }

        /// <summary>
        /// The timeScale-independent interval in seconds from the last frame to the current
        /// one (Read Only).
        /// </summary>
        public static float UnscaledDeltaTime
        {
            get { return unscaledDeltaTime; }
        }

        /// <summary>
        /// The interval in seconds at which physics and other fixed frame rate updates (like
        /// MonoBehaviour's MonoBehaviour.FixedUpdate) are performed.
        /// </summary>
        public static float FixedDeltaTime
        {
            get { return fixedDeltaTime; }
            set { UnityEngine.Time.fixedDeltaTime = fixedDeltaTime = value; }
        }

        /// <summary>
        /// The scale at which the time is passing. This can be used for slow motion effects.
        /// </summary>
        public static float TimeScale
        {
            get { return timeScale; }
            set { UnityEngine.Time.timeScale = timeScale = value; }
        }

        /// <summary>
        /// The time this frame has started (Read Only). This is the time in seconds since
        /// the last level has been loaded.
        /// </summary>
        public static float TimeSinceLevelLoaded
        {
            get { return timeSinceLevelLoad; }
        }
    }
}
