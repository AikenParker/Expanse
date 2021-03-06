﻿using System;

namespace Expanse.Utilities
{
    /// <summary>
    /// Base Expanse exception.
    /// </summary>
    public abstract class ExpanseException : Exception
    {
        public ExpanseException() : base() { }

        public ExpanseException(string message) : base(message) { }

        public ExpanseException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Attempted to access inaccessible functionality on an inactive object.
    /// </summary>
    public class InactiveException : ExpanseException
    {
        public InactiveException() : base() { }

        public InactiveException(string message) : base(message) { }

        public InactiveException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Attempted to access/modify object while active when it can only happen while the object is inactive.
    /// </summary>
    public class ActiveException : ExpanseException
    {
        public ActiveException() : base() { }

        public ActiveException(string message) : base(message) { }

        public ActiveException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// As unexpected code path was reached.
    /// </summary>
    public class UnexpectedException : ExpanseException
    {
        public UnexpectedException() : base() { }

        public UnexpectedException(string message) : base(message) { }

        public UnexpectedException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// A currently unsupported code path was reached.
    /// </summary>
    public class UnsupportedException : ExpanseException
    {
        public UnsupportedException() : base() { }

        public UnsupportedException(string message) : base(message) { }

        public UnsupportedException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// A passed in argument is invalid in some way.
    /// </summary>
    public class InvalidArgumentException : ExpanseException
    {
        public InvalidArgumentException() : base() { }

        public InvalidArgumentException(string message) : base(message) { }

        public InvalidArgumentException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Generic type is invalid in some way.
    /// </summary>
    public class InvalidTypeException : ExpanseException
    {
        public InvalidTypeException() : base() { }

        public InvalidTypeException(string message) : base(message) { }

        public InvalidTypeException(string message, Exception innerException) : base(message, innerException) { }
    }
}
