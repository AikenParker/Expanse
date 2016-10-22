using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Expanse
{
    public abstract class ExpanseException : Exception
    {
        public ExpanseException() : base() { }

        public ExpanseException(string message) : base(message) { }

        public ExpanseException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class InactiveException : ExpanseException
    {
        public InactiveException() : base() { }

        public InactiveException(string message) : base(message) { }

        public InactiveException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class UnexpectedException : ExpanseException
    {
        public UnexpectedException() : base() { }

        public UnexpectedException(string message) : base(message) { }

        public UnexpectedException(string message, Exception innerException) : base(message, innerException) { }
    }
}
