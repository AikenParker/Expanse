using UnityEngine;
using System.Collections.Generic;
using System;

namespace Expanse.Utilities
{
    /// <summary>
    /// Single-point vector 3 distance comparer.
    /// </summary>
    public class DistanceComparer : IComparer<Vector3>
    {
        public readonly Vector3 targetPosition;

        public bool OrderByDescending { get; set; }

        public DistanceComparer(Vector3 targetPosition)
        {
            this.targetPosition = targetPosition;
        }

        int IComparer<Vector3>.Compare(Vector3 x, Vector3 y)
        {
            if (OrderByDescending == false)
                return (targetPosition - x).sqrMagnitude.CompareTo((targetPosition - y).sqrMagnitude);
            else
                return (targetPosition - y).sqrMagnitude.CompareTo((targetPosition - x).sqrMagnitude);
        }
    }

    /// <summary>
    /// Single-point vector 3 distance comparer.
    /// </summary>
    public class DistanceComparer<T> : IComparer<T>
    {
        public readonly Vector3 targetPosition;
        public readonly Func<T, Vector3> selector;

        public bool OrderByDescending { get; set; }

        public DistanceComparer(Vector3 targetPosition, Func<T, Vector3> selector)
        {
            this.targetPosition = targetPosition;
            this.selector = selector;
        }

        public DistanceComparer(T targetObj, Func<T, Vector3> selector)
        {
            this.targetPosition = selector(targetObj);
            this.selector = selector;
        }

        int IComparer<T>.Compare(T x, T y)
        {
            if (OrderByDescending == false)
                return (targetPosition - selector(x)).sqrMagnitude.CompareTo((targetPosition - selector(y)).sqrMagnitude);
            else
                return (targetPosition - selector(y)).sqrMagnitude.CompareTo((targetPosition - selector(x)).sqrMagnitude);
        }
    }

    /// <summary>
    /// Single-point vector 2 distance comparer.
    /// </summary>
    public class DistanceComparer2D : IComparer<Vector2>
    {
        public readonly Vector2 targetPosition;

        public bool OrderByDescending { get; set; }

        public DistanceComparer2D(Vector2 targetPosition)
        {
            this.targetPosition = targetPosition;
        }

        int IComparer<Vector2>.Compare(Vector2 x, Vector2 y)
        {
            if (OrderByDescending == false)
                return (targetPosition - x).sqrMagnitude.CompareTo((targetPosition - y).sqrMagnitude);
            else
                return (targetPosition - y).sqrMagnitude.CompareTo((targetPosition - x).sqrMagnitude);
        }
    }

    /// <summary>
    /// Single-point vector 2 distance comparer.
    /// </summary>
    public class DistanceComparer2D<T> : IComparer<T>
    {
        public readonly Vector2 targetPosition;
        public readonly Func<T, Vector2> selector;

        public bool OrderByDescending { get; set; }

        public DistanceComparer2D(Vector2 targetPosition, Func<T, Vector2> selector)
        {
            this.targetPosition = targetPosition;
            this.selector = selector;
        }

        public DistanceComparer2D(T targetObj, Func<T, Vector2> selector)
        {
            this.targetPosition = selector(targetObj);
            this.selector = selector;
        }

        int IComparer<T>.Compare(T x, T y)
        {
            if (OrderByDescending == false)
                return (targetPosition - selector(x)).sqrMagnitude.CompareTo((targetPosition - selector(y)).sqrMagnitude);
            else
                return (targetPosition - selector(y)).sqrMagnitude.CompareTo((targetPosition - selector(x)).sqrMagnitude);
        }
    }

    /// <summary>
    /// Single-point vector 4 distance comparer.
    /// </summary>
    public class DistanceComparer4D : IComparer<Vector4>
    {
        public readonly Vector4 targetPosition;

        public bool OrderByDescending { get; set; }

        public DistanceComparer4D(Vector4 targetPosition)
        {
            this.targetPosition = targetPosition;
        }

        int IComparer<Vector4>.Compare(Vector4 x, Vector4 y)
        {
            if (OrderByDescending == false)
                return (targetPosition - x).sqrMagnitude.CompareTo((targetPosition - y).sqrMagnitude);
            else
                return (targetPosition - y).sqrMagnitude.CompareTo((targetPosition - x).sqrMagnitude);
        }
    }

    /// <summary>
    /// Single-point vector 4 distance comparer.
    /// </summary>
    public class DistanceComparer4D<T> : IComparer<T>
    {
        public readonly Vector4 targetPosition;
        public readonly Func<T, Vector4> selector;

        public bool OrderByDescending { get; set; }

        public DistanceComparer4D(Vector4 targetPosition, Func<T, Vector4> selector)
        {
            this.targetPosition = targetPosition;
            this.selector = selector;
        }

        public DistanceComparer4D(T targetObj, Func<T, Vector4> selector)
        {
            this.targetPosition = selector(targetObj);
            this.selector = selector;
        }

        int IComparer<T>.Compare(T x, T y)
        {
            if (OrderByDescending == false)
                return (targetPosition - selector(x)).sqrMagnitude.CompareTo((targetPosition - selector(y)).sqrMagnitude);
            else
                return (targetPosition - selector(y)).sqrMagnitude.CompareTo((targetPosition - selector(x)).sqrMagnitude);
        }
    }
}
