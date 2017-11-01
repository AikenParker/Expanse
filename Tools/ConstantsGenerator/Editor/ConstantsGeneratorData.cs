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
        public string filePath = string.Empty;
        public string @namespace = string.Empty;
        public ConstantsGeneratorSceneSettings sceneSettings = new ConstantsGeneratorSceneSettings();
        public ConstantsGeneratorLayerSettings layerSettings = new ConstantsGeneratorLayerSettings();
        public ConstantsGeneratorTagSettings tagSettings = new ConstantsGeneratorTagSettings();
        public ConstantsGeneratorSortingLayerSettings sortingLayerSettings = new ConstantsGeneratorSortingLayerSettings();
    }

    /// <summary>
    /// Session data remains after assembly reload but NOT after application shutdown.
    /// </summary>
    [Serializable]
    public sealed class ConstantsGeneratorSessionData
    {
        public Vector2 scrollPosition;
        public string filePathMessage;

        public bool sceneSettingsOpen;
        public bool layerSettingsOpen;
        public bool tagSettingsOpen;
        public bool sortingLayerSettingOpen;

        public float sceneSettingsFade = 1f;
        public float layerSettingsFade = 1f;
        public float tagSettingsFade = 1f;
        public float sortingLayerSettingsFade = 1f;
    }

    /// <summary>
    /// Volatile data does NOT remain after assembly reload or application shutdown.
    /// </summary>
    public sealed class ConstantsGeneratorVolatileData
    {
        public GUIStyle filePathMessageStyle;
        public string[] namingConventionNames = new string[]
        {
            "camelCase",
            "PascalCase",
            "CAPITAL_CASE",
            "lower_case",
            "_camelCase",
            "_PascalCase",
            "_CAPITAL_CASE",
            "_lower_case"
        };
    }

    [Serializable]
    public sealed class ConstantsGeneratorSceneSettings
    {
        public bool enabled = true;
        public bool typesEnabled = true;
        public string typesEnum = "SceneType";
        public NamingConvention typesNamingConvention = NamingConvention.PascalCase;
        public bool namesEnabled = true;
        public string namesClass = "SceneNames";
        public NamingConvention namesNamingConvention = NamingConvention.CAPITAL_CASE;
        public bool idsEnabled = true;
        public string idsClass = "SceneIds";
        public NamingConvention idsNamingConvention = NamingConvention.CAPITAL_CASE;
    }

    [Serializable]
    public sealed class ConstantsGeneratorLayerSettings
    {
        public bool enabled = true;
        public bool includeBuiltIn = true;
        public bool typesEnabled = true;
        public string typesEnum = "LayerType";
        public NamingConvention typesNamingConvention = NamingConvention.PascalCase;
        public bool namesEnabled = true;
        public string namesClass = "LayerNames";
        public NamingConvention namesNamingConvention = NamingConvention.CAPITAL_CASE;
        public bool idsEnabled = true;
        public string idsClass = "LayerIds";
        public NamingConvention idsNamingConvention = NamingConvention.CAPITAL_CASE;
        public bool masksEnabled = true;
        public string masksClass = "LayerMasks";
        public NamingConvention masksNamingConvention = NamingConvention.PascalCase;
    }

    [Serializable]
    public sealed class ConstantsGeneratorTagSettings
    {
        public bool enabled = true;
        public bool includeBuiltIn = true;
        public string @class = "Tags";
        public NamingConvention namingConvention = NamingConvention.CAPITAL_CASE;
    }

    [Serializable]
    public sealed class ConstantsGeneratorSortingLayerSettings
    {
        public bool enabled = true;
        public bool includeBuiltIn = true;
        public bool typesEnabled = true;
        public string typesEnum = "SortingLayerType";
        public NamingConvention typesNamingConvention = NamingConvention.PascalCase;
        public bool namesEnabled = true;
        public string namesClass = "SortingLayerNames";
        public NamingConvention namesNamingConvention = NamingConvention.CAPITAL_CASE;
        public bool idsEnabled = true;
        public string idsClass = "SortingLayerIds";
        public NamingConvention idsNamingConvention = NamingConvention.CAPITAL_CASE;
    }

    public enum NamingConvention : byte
    {
        camelCase,
        PascalCase,
        CAPITAL_CASE,
        lower_case,
        _camelCase,
        _Pascal_Case,
        _CAPITAL_CASE,
        _lower_case
    }
}
