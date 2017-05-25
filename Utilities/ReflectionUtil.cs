using System;
using System.Reflection;

namespace Expanse.Utilities
{
    /// <summary>
    /// A collection of Reflection related utility functionality.
    /// </summary>
    public static class ReflectionUtil
    {
        /// <summary>
        /// Creates a delegate that invokes a method.
        /// </summary>
        /// <typeparam name="TDelegate">Type of the delegate.</typeparam>
        /// <param name="methodInfo">Method info to create the delegate from.</param>
        public static TDelegate CreateMethodInvokerDelegate<TDelegate>(MethodInfo methodInfo) where TDelegate : class
        {
            Type tDelegate = typeof(TDelegate);

            if (!tDelegate.IsSubclassOf(typeof(Delegate)))
                throw new InvalidTypeException("Type TDelegate must be a subclass of Delegate");

            return Delegate.CreateDelegate(tDelegate, methodInfo, true) as TDelegate;
        }

        /// <summary>
        /// Creates a delegate that gets the value of a static property.
        /// </summary>
        /// <typeparam name="TValue">Property type of the property info.</typeparam>
        /// <param name="propertyInfo">Property info to create the delegate from.</param>
        public static Func<TValue> CreateStaticPropertyGetterDelegate<TValue>(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            if (!propertyInfo.CanRead)
                throw new InvalidArgumentException("Cannot read from propertyInfo");

            if (typeof(TValue) != propertyInfo.PropertyType)
                throw new InvalidArgumentException("Type TValue must equal the propertyInfo property type");

            MethodInfo getterMethod = propertyInfo.GetGetMethod(true);

            if (!getterMethod.IsStatic)
                throw new InvalidArgumentException("propertyInfo is not static use CreatePropertyGetterDelegate() instead");

            return CreateMethodInvokerDelegate<Func<TValue>>(getterMethod);
        }

        /// <summary>
        /// Creates a delegate that gets the value of a property.
        /// </summary>
        /// <typeparam name="TSource">Declaring type that the property belongs to.</typeparam>
        /// <typeparam name="TValue">Property type of the property info.</typeparam>
        /// <param name="propertyInfo">Property info to create the delegate from.</param>
        public static Func<TSource, TValue> CreatePropertyGetterDelegate<TSource, TValue>(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            if (!propertyInfo.CanRead)
                throw new InvalidArgumentException("Cannot read from propertyInfo");

            if (typeof(TSource) != propertyInfo.DeclaringType)
                throw new InvalidArgumentException("Type TSource must equal the propertyInfo declaring type");

            if (typeof(TValue) != propertyInfo.PropertyType)
                throw new InvalidArgumentException("Type TValue must equal the propertyInfo property type");

            MethodInfo getterMethod = propertyInfo.GetGetMethod(true);

            if (getterMethod.IsStatic)
                throw new InvalidArgumentException("propertyInfo is static use CreateStaticPropertyGetterDelegate() instead");

            return CreateMethodInvokerDelegate<Func<TSource, TValue>>(getterMethod);
        }

        /// <summary>
        /// Creates a delegate that sets the value of a static property.
        /// </summary>
        /// <typeparam name="TValue">Property type of the property info.</typeparam>
        /// <param name="propertyInfo">Property info to create the delegate from.</param>
        public static Action<TValue> CreateStaticPropertySetterDelegate<TValue>(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            if (!propertyInfo.CanWrite)
                throw new InvalidArgumentException("Cannot write to propertyInfo");

            if (typeof(TValue) != propertyInfo.PropertyType)
                throw new InvalidArgumentException("Type TValue must equal the propertyInfo property type");

            MethodInfo setterMethod = propertyInfo.GetSetMethod(true);

            if (!setterMethod.IsStatic)
                throw new InvalidArgumentException("propertyInfo is not static use CreatePropertySetterDelegate() instead");

            return CreateMethodInvokerDelegate<Action<TValue>>(setterMethod);
        }

        /// <summary>
        /// Creates a delegate that sets the value of a property.
        /// </summary>
        /// <typeparam name="TSource">Declaring type that the property belongs to.</typeparam>
        /// <typeparam name="TValue">Property type of the property info.</typeparam>
        /// <param name="propertyInfo">Property info to create the delegate from.</param>
        public static Action<TSource, TValue> CreatePropertySetterDelegate<TSource, TValue>(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            if (!propertyInfo.CanWrite)
                throw new InvalidArgumentException("Cannot write to propertyInfo");

            if (typeof(TSource) != propertyInfo.DeclaringType)
                throw new InvalidArgumentException("Type TSource must equal the propertyInfo declaring type");

            if (typeof(TValue) != propertyInfo.PropertyType)
                throw new InvalidArgumentException("Type TValue must equal the propertyInfo property type");

            MethodInfo setterMethod = propertyInfo.GetSetMethod(true);

            if (setterMethod.IsStatic)
                throw new InvalidArgumentException("propertyInfo is static use CreateStaticPropertySetterDelegate() instead");

            return CreateMethodInvokerDelegate<Action<TSource, TValue>>(setterMethod);
        }
    }
}
