using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expanse
{
    [Serializable]
    public class ExtList<T> : List<T>
    {
        public ExtList() : base() { }

        public ExtList(int capacity) : base(capacity) { }

        public ExtList(IEnumerable<T> collection) : base(collection) { }

        public void Push(T obj)
        {
            this.Insert(0, obj);
        }

        public T Pop()
        {
            T obj = this[0];
            this.RemoveAt(0);
            return obj;
        }

        public T Peek()
        {
            return this[0];
        }

        public void Enqeue(T obj)
        {
            this.Add(obj);
        }

        public T Dequeue()
        {
            T obj = this[0];
            this.RemoveAt(0);
            return obj;
        }

        public T Requeue()
        {
            T obj = this.Dequeue();
            this.Enqeue(obj);
            return obj;
        }

        public void Requeue(T obj)
        {
            this.Remove(obj);
            this.Enqeue(obj);
        }

        public bool Contains<U>(U obj, Func<T, U> selector)
        {
            var selected = this.Select(selector);

            return selected.Contains(obj);
        }

        public bool RemoveFirst<U>(U obj, Func<T, U> selector)
        {
            object matchElem = null;

            foreach (T elem in this)
            {
                if (selector(elem).Equals(obj))
                {
                    matchElem = elem;
                    break;
                }
            }

            if (matchElem != null)
                return this.Remove((T)matchElem);
            else return false;
        }

        public bool HasAny
        {
            get
            {
                if (this.Count <= 0)
                    return false;
                else return this.Any();
            }
        }
    }
}
