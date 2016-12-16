using System.Collections.Generic;

namespace Expanse
{
    public interface IPriority : IUnity
    {
        int Priority { get; }
    }

    public abstract class PriorityComparer<T> : IComparer<T> where T : IPriority
    {
        int IComparer<T>.Compare(T x, T y)
        {
            if (x.MonoBehaviour && y.MonoBehaviour)
                return x.Priority.CompareTo(y.Priority);
            else if (x.MonoBehaviour)
                return 1;
            else if (y.MonoBehaviour)
                return -1;
            else return 0;
        }
    }
}