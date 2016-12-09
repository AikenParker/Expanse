using UnityEngine;
using System.Collections.Generic;
using System;

namespace Expanse
{
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
                return Vector3.Distance(targetPosition, selector(x)).CompareTo((Vector3.Distance(targetPosition, selector(y))));
            else
                return Vector3.Distance(targetPosition, selector(y)).CompareTo((Vector3.Distance(targetPosition, selector(x))));
        }
    }
}
