using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// A collection of streaming assets related utility functionality.
    /// </summary>
    public static class StreamingAssetsUtil
    {
        /// <summary>
        /// Gets the full asset path of a file in streamingAssets.
        /// </summary>
        public static string GetFullStreamingAssetFilepath(string filepath)
        {
            return string.Concat(Application.streamingAssetsPath, filepath);
        }
    }
}
