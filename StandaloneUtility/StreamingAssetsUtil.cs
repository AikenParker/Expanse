using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Expanse
{
    public static class StreamingAssetsUtil
    {
        public static string GetFullStreamingAssetFilepath(string filepath)
        {
            return string.Concat(Application.streamingAssetsPath, filepath);
        }
    }
}
