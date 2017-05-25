using System;
using System.Diagnostics;

namespace Expanse.Utilities
{
    /// <summary>
    /// A collection of benchmark related utility functionality.
    /// </summary>
    public static class BenchmarkUtil
    {
        /// <summary>
        /// Times how long it takes to perform an action.
        /// </summary>
        /// <param name="action">Action to be benchmarked.</param>
        /// <returns>Time taken to invoke the action in milliseconds.</returns>
        public static double Benchmark(Action action)
        {
            if (action == null)
                throw new NullReferenceException("action can not be null");

            Stopwatch benchmarker = new Stopwatch();

            benchmarker.Start();
            action.Invoke();
            benchmarker.Stop();

            return benchmarker.Elapsed.TotalMilliseconds;
        }

        /// <summary>
        /// Times how long it takes to perform an action a specified amount of times.
        /// </summary>
        /// <param name="action">Action to be benchmarked.</param>
        /// <param name="iterations">Amount of times to invoke the action.</param>
        /// <returns>Time taken to invoke the action an amount of times in milliseconds.</returns>
        public static double Benchmark(Action action, int iterations)
        {
            if (action == null)
                throw new NullReferenceException("action can not be null");

            if (iterations <= 0)
                throw new InvalidArgumentException("iterations must be greater than zero");

            Stopwatch benchmarker = new Stopwatch();

            benchmarker.Start();

            for (int i = iterations - 1; i >= 0; i--)
            {
                action.Invoke();
            }

            benchmarker.Stop();

            return benchmarker.Elapsed.TotalMilliseconds;
        }
    }
}
