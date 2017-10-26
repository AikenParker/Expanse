using System;
using UnityEngine;

namespace Expanse.Tools
{
    /// <summary>
    /// Persistent data remains after assembly reload and application shutdown.
    /// </summary>
    [Serializable]
    public sealed class ConstantsGeneratorPersistentData
    {
        public string filePath;
        public string @namespace;
    }

    /// <summary>
    /// Session data remains after assembly reload but NOT after application shutdown.
    /// </summary>
    [Serializable]
    public sealed class ConstantsGeneratorSessionData
    {
        public Vector2 scrollPosition;
        public string filePathMessage;
    }

    /// <summary>
    /// Volatile data does NOT remain after assembly reload or application shutdown.
    /// </summary>
    public sealed class ConstantsGeneratorVolatileData
    {
        public GUIStyle filePathMessageStyle;
    }
}
