using System.Diagnostics;

namespace Expanse.Extensions
{
    /// <summary>
    /// A collection of System.Diagnostics.Stopwatch related extension methods.
    /// </summary>
    public static class StopwatchExt
    {
        public static double GetElapsedMilliseconds(this Stopwatch stopwatch)
        {
            return stopwatch.Elapsed.TotalMilliseconds;
        }
    }
}
