using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Text;

namespace Expanse
{
    /// <summary>
    /// A collection of benchmark related utility functionality.
    /// </summary>
    public static class BenchmarkUtil
    {
        /// <summary>
        /// Times how long it takes to perform an action once.
        /// </summary>
        /// <returns>Length in milliseconds it took</returns>
        public static long Benchmark(Action action)
        {
            Stopwatch Timer = new Stopwatch();

            Timer.Start();

            action.Invoke();

            Timer.Stop();

            return Timer.ElapsedMilliseconds;
        }

        /// <summary>
        /// Times how long it takes to perform an action in a specified amount of iterations.
        /// </summary>
        /// <returns>Length in milliseconds it took</returns>
        public static long Benchmark(Action action, int iterations)
        {
            Stopwatch Timer = new Stopwatch();

            Timer.Start();

            for (int i = 0; i < iterations; i++)
            {
                action.Invoke();
            }

            Timer.Stop();

            return Timer.ElapsedMilliseconds;
        }
    }
}
