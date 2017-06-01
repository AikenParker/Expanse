#region AOT_ONLY_DEFINE
#define AOT_ONLY
#if (UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID) && !ENABLE_IL2CPP
#undef AOT_ONLY
#endif
#endregion

#if !AOT_ONLY
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Expanse.Utilities
{
    /// <summary>
    /// A collection of Reflection.Emit related utility functionality.
    /// </summary>
    public static class EmitUtil
    {
        /// <summary>
        /// Will generated methods be named meaningfully using string concatenation.
        /// </summary>
        public static bool UseMeaningfulNames
        {
            get { return useMeaningfulNames; }
            set { useMeaningfulNames = value; }
        }

        private static bool useMeaningfulNames = true;
        private const string GENERATED_NAME = "EmitUtilGenerated";

        // Type array caches used when emitting methods
        private static Type[] emptyTypeArray = new Type[0];
        private static Type[] singleTypeArray = new Type[1];
        private static Type[] doubleTypeArray = new Type[2];

        /// <summary>
        /// Generates a delegate that constructs an instance of type TSource using the default constructor.
        /// </summary>
        /// <typeparam name="TSource">Declaring type that the constructor belongs to.</typeparam>
        /// <returns>Returns the delegate that invokes the default constructor of a type.</returns>
        public static Func<TSource> GenerateDefaultConstructorDelegate<TSource>()
        {
            Type tSource = typeof(TSource);
            ConstructorInfo defaultConstructorInfo = tSource.GetConstructor(emptyTypeArray);

            if (defaultConstructorInfo == null)
                throw new InvalidTypeException("TSource must have a default constructor");

            string dynamicMethodName = useMeaningfulNames ? tSource.FullName + ".ctor" : GENERATED_NAME;
            DynamicMethod constructorMethod = new DynamicMethod(dynamicMethodName, tSource, emptyTypeArray, tSource);
            ILGenerator gen = constructorMethod.GetILGenerator();

            gen.Emit(OpCodes.Newobj, defaultConstructorInfo);
            gen.Emit(OpCodes.Ret);

            return (Func<TSource>)constructorMethod.CreateDelegate(typeof(Func<TSource>));
        }

        /// <summary>
        /// Creates a delegate that gets the value of a static field.
        /// </summary>
        /// <typeparam name="TValue">Field type of the field info.</typeparam>
        /// <param name="fieldInfo">Field info to generate getter delegate from.</param>
        /// <returns>Returns the delegate that invokes a generated field getter for the field.</returns>
        public static Func<TValue> GenerateStaticFieldGetterDelegate<TValue>(FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
                throw new ArgumentNullException("field");

            if (!fieldInfo.IsStatic)
                throw new InvalidArgumentException("fieldInfo is not static use GenerateFieldGetterDelegate() instead");

            Type tValue = typeof(TValue);

            if (tValue != fieldInfo.FieldType)
                throw new InvalidArgumentException("Type TValue must equal the fieldInfo field type");

            string dynamicMethodName = useMeaningfulNames ? fieldInfo.ReflectedType.FullName + ".get_" + fieldInfo.Name : GENERATED_NAME;
            DynamicMethod getterMethod = new DynamicMethod(dynamicMethodName, tValue, emptyTypeArray, fieldInfo.DeclaringType);

            ILGenerator gen = getterMethod.GetILGenerator();
            gen.Emit(OpCodes.Ldsfld, fieldInfo);
            gen.Emit(OpCodes.Ret);

            return (Func<TValue>)getterMethod.CreateDelegate(typeof(Func<TValue>));
        }

        /// <summary>
        /// Creates a delegate that gets the value of a field.
        /// </summary>
        /// <typeparam name="TSource">Declaring type that the field belongs to.</typeparam>
        /// <typeparam name="TValue">Field type of the field info.</typeparam>
        /// <param name="fieldInfo">Field info to generate getter delegate from.</param>
        /// <returns>Returns the delegate that invokes a generated field getter for the field.</returns>
        public static Func<TSource, TValue> GenerateFieldGetterDelegate<TSource, TValue>(FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
                throw new ArgumentNullException("field");

            if (fieldInfo.IsStatic)
                throw new InvalidArgumentException("fieldInfo is static use GenerateStaticFieldGetterDelegate() instead");

            Type tSource = typeof(TSource);
            Type tValue = typeof(TValue);

            if (tSource != fieldInfo.DeclaringType)
                throw new InvalidArgumentException("Type TSource must equal the fieldInfo declaring type");

            if (tValue != fieldInfo.FieldType)
                throw new InvalidArgumentException("Type TValue must equal the fieldInfo field type");

            singleTypeArray[0] = tSource;
            string dynamicMethodName = useMeaningfulNames ? fieldInfo.ReflectedType.FullName + ".get_" + fieldInfo.Name : GENERATED_NAME;
            DynamicMethod getterMethod = new DynamicMethod(dynamicMethodName, tValue, singleTypeArray, tSource);

            ILGenerator gen = getterMethod.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld, fieldInfo);
            gen.Emit(OpCodes.Ret);

            return (Func<TSource, TValue>)getterMethod.CreateDelegate(typeof(Func<TSource, TValue>));
        }

        /// <summary>
        /// Creates a delegate that sets the value of a static field.
        /// </summary>
        /// <typeparam name="TValue">Field type of the field info.</typeparam>
        /// <param name="fieldInfo">Field info to generate setter delegate from.</param>
        /// <returns>Returns the delegate that invokes a generated field setter for the field.</returns>
        public static Action<TValue> GenerateStaticFieldSetterDelegate<TValue>(FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
                throw new ArgumentNullException("field");

            if (!fieldInfo.IsStatic)
                throw new InvalidArgumentException("fieldInfo is not static use GenerateFieldSetterDelegate() instead");

            Type tValue = typeof(TValue);

            if (tValue != fieldInfo.FieldType)
                throw new InvalidArgumentException("Type TValue must equal the fieldInfo field type");

            singleTypeArray[0] = tValue;
            string dynamicMethodName = useMeaningfulNames ? fieldInfo.ReflectedType.FullName + ".set_" + fieldInfo.Name : GENERATED_NAME;
            DynamicMethod setterMethod = new DynamicMethod(dynamicMethodName, null, singleTypeArray, fieldInfo.DeclaringType);

            ILGenerator gen = setterMethod.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Stsfld, fieldInfo);
            gen.Emit(OpCodes.Ret);

            return (Action<TValue>)setterMethod.CreateDelegate(typeof(Action<TValue>));
        }

        /// <summary>
        /// Creates a delegate that sets the value of a field.
        /// </summary>
        /// <typeparam name="TSource">Declaring type that the field belongs to.</typeparam>
        /// <typeparam name="TValue">Field type of the field info.</typeparam>
        /// <param name="fieldInfo">Field info to generate setter delegate from.</param>
        /// <returns>Returns the delegate that invokes a generated field setter for the field.</returns>
        public static Action<TSource, TValue> GenerateFieldSetterDelegate<TSource, TValue>(FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
                throw new ArgumentNullException("field");

            if (fieldInfo.IsStatic)
                throw new InvalidArgumentException("fieldInfo is static use GenerateStaticFieldSetterDelegate() instead");

            Type tSource = typeof(TSource);
            Type tValue = typeof(TValue);

            if (tSource != fieldInfo.DeclaringType)
                throw new InvalidArgumentException("Type TSource must equal the fieldInfo declaring type");

            if (tValue != fieldInfo.FieldType)
                throw new InvalidArgumentException("Type TValue must equal the fieldInfo field type");

            doubleTypeArray[0] = tSource;
            doubleTypeArray[1] = tValue;
            string dynamicMethodName = useMeaningfulNames ? fieldInfo.ReflectedType.FullName + ".set_" + fieldInfo.Name : GENERATED_NAME;
            DynamicMethod setterMethod = new DynamicMethod(dynamicMethodName, null, doubleTypeArray, tSource);

            ILGenerator gen = setterMethod.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Stfld, fieldInfo);
            gen.Emit(OpCodes.Ret);

            return (Action<TSource, TValue>)setterMethod.CreateDelegate(typeof(Action<TSource, TValue>));
        }

        #region NON_GENERIC

        /// <summary>
        /// Generates a delegate that constructs an instance of type tSource using the default constructor. Slower and less safe than generic overload.
        /// </summary>
        /// <param name="tSource">The type to generate the default constructor invoker from.</param>
        /// <returns>Returns the delegate that invokes the default constructor for a given type.</returns>
        public static Func<object> GenerateDefaultConstructorDelegate(Type tSource)
        {
            ConstructorInfo defaultConstructorInfo = tSource.GetConstructor(emptyTypeArray);

            if (defaultConstructorInfo == null)
                throw new InvalidTypeException("tSource must have a default constructor");

            string dynamicMethodName = useMeaningfulNames ? tSource.FullName + ".ctor" : GENERATED_NAME;
            DynamicMethod constructorMethod = new DynamicMethod(dynamicMethodName, tSource, emptyTypeArray, tSource);
            ILGenerator gen = constructorMethod.GetILGenerator();

            gen.Emit(OpCodes.Newobj, defaultConstructorInfo);
            if (tSource.IsValueType)
                gen.Emit(OpCodes.Box, tSource);
            else if (tSource != typeof(object))
                gen.Emit(OpCodes.Castclass, tSource);
            gen.Emit(OpCodes.Ret);

            return (Func<object>)constructorMethod.CreateDelegate(typeof(Func<object>));
        }

        /// <summary>
        /// Creates a delegate that gets the value of a field. Slower and less safe than generic overload.
        /// Warning: Boxes/Unboxes value types
        /// </summary>
        /// <param name="fieldInfo">Field info to generate getter delegate from.</param>
        /// <returns>Returns the delegate that invokes a generated field getter for the field.</returns>
        public static Func<object, object> GenerateFieldGetterDelegate(FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
                throw new ArgumentNullException("field");

            if (fieldInfo.IsStatic)
                throw new InvalidArgumentException("fieldInfo is static use GenerateStaticFieldGetterDelegate() instead");

            Type tSource = fieldInfo.DeclaringType;
            Type tValue = fieldInfo.FieldType;

            Type tObject = typeof(object);
            singleTypeArray[0] = tObject;
            string dynamicMethodName = useMeaningfulNames ? fieldInfo.ReflectedType.FullName + ".get_" + fieldInfo.Name : GENERATED_NAME;
            DynamicMethod getterMethod = new DynamicMethod(dynamicMethodName, tObject, singleTypeArray, tSource);

            ILGenerator gen = getterMethod.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            if (tSource.IsValueType)
                gen.Emit(OpCodes.Unbox_Any, tSource);
            else if (tSource != tObject)
                gen.Emit(OpCodes.Castclass, tSource);
            gen.Emit(OpCodes.Ldfld, fieldInfo);
            if (tValue.IsValueType)
                gen.Emit(OpCodes.Box, tValue);
            else if (tValue != tObject)
                gen.Emit(OpCodes.Castclass, tValue);
            gen.Emit(OpCodes.Ret);

            return (Func<object, object>)getterMethod.CreateDelegate(typeof(Func<object, object>));
        }

        /// <summary>
        /// Creates a delegate that gets the value of a static field. Slower and less safe than generic overload.
        /// Warning: Boxes/Unboxes value types
        /// </summary>
        /// <param name="fieldInfo">Field info to generate getter delegate from.</param>
        /// <returns>Returns the delegate that invokes a generated field getter for the field.</returns>
        public static Func<object> GenerateStaticFieldGetterDelegate(FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
                throw new ArgumentNullException("field");

            if (!fieldInfo.IsStatic)
                throw new InvalidArgumentException("fieldInfo is not static use GenerateFieldGetterDelegate() instead");

            Type tSource = fieldInfo.DeclaringType;
            Type tValue = fieldInfo.FieldType;

            Type tObject = typeof(object);
            string dynamicMethodName = useMeaningfulNames ? fieldInfo.ReflectedType.FullName + ".get_" + fieldInfo.Name : GENERATED_NAME;
            DynamicMethod getterMethod = new DynamicMethod(dynamicMethodName, tObject, emptyTypeArray, tSource);

            ILGenerator gen = getterMethod.GetILGenerator();
            gen.Emit(OpCodes.Ldsfld, fieldInfo);
            if (tValue.IsValueType)
                gen.Emit(OpCodes.Box, tValue);
            else if (tValue != tObject)
                gen.Emit(OpCodes.Castclass, tValue);
            gen.Emit(OpCodes.Ret);

            return (Func<object>)getterMethod.CreateDelegate(typeof(Func<object>));
        }

        /// <summary>
        /// Creates a delegate that sets the value of a field. Slower and less safe than generic overload.
        /// Warning: Unboxes value types
        /// </summary>
        /// <param name="fieldInfo">Field info to generate setter delegate from.</param>
        /// <returns>Returns the delegate that invokes a generated field setter for the field.</returns>
        public static Action<object, object> GenerateFieldSetterDelegate(FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
                throw new ArgumentNullException("field");

            if (fieldInfo.IsStatic)
                throw new InvalidArgumentException("fieldInfo is static use GenerateStaticFieldSetterDelegate() instead");

            Type tSource = fieldInfo.DeclaringType;
            Type tValue = fieldInfo.FieldType;

            Type tObject = typeof(object);
            doubleTypeArray[0] = tObject;
            doubleTypeArray[1] = tObject;
            string dynamicMethodName = useMeaningfulNames ? fieldInfo.ReflectedType.FullName + ".set_" + fieldInfo.Name : GENERATED_NAME;
            DynamicMethod setterMethod = new DynamicMethod(dynamicMethodName, null, doubleTypeArray, tSource);

            ILGenerator gen = setterMethod.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            if (tSource.IsValueType)
                gen.Emit(OpCodes.Unbox_Any, tSource);
            else if (tSource != tObject)
                gen.Emit(OpCodes.Castclass, tSource);
            gen.Emit(OpCodes.Ldarg_1);
            if (tValue.IsValueType)
                gen.Emit(OpCodes.Unbox_Any, tValue);
            else if (tValue != tObject)
                gen.Emit(OpCodes.Castclass, tValue);
            gen.Emit(OpCodes.Stfld, fieldInfo);
            gen.Emit(OpCodes.Ret);

            return (Action<object, object>)setterMethod.CreateDelegate(typeof(Action<object, object>));
        }

        /// <summary>
        /// Creates a delegate that sets the value of a static field. Slower and less safe than generic overload.
        /// Warning: Unboxes value types
        /// </summary>
        /// <param name="fieldInfo">Field info to generate setter delegate from.</param>
        /// <returns>Returns the delegate that invokes a generated field setter for the field.</returns>
        public static Action<object> GenerateStaticFieldSetterDelegate(FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
                throw new ArgumentNullException("field");

            if (!fieldInfo.IsStatic)
                throw new InvalidArgumentException("fieldInfo is not static use GenerateFieldSetterDelegate() instead");

            Type tSource = fieldInfo.DeclaringType;
            Type tValue = fieldInfo.FieldType;

            Type tObject = typeof(object);
            singleTypeArray[0] = tObject;
            string dynamicMethodName = useMeaningfulNames ? fieldInfo.ReflectedType.FullName + ".set_" + fieldInfo.Name : GENERATED_NAME;
            DynamicMethod setterMethod = new DynamicMethod(dynamicMethodName, null, singleTypeArray, tSource);

            ILGenerator gen = setterMethod.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            if (tValue.IsValueType)
                gen.Emit(OpCodes.Unbox_Any, tValue);
            else if (tValue != tObject)
                gen.Emit(OpCodes.Castclass, tValue);
            gen.Emit(OpCodes.Stsfld, fieldInfo);
            gen.Emit(OpCodes.Ret);

            return (Action<object>)setterMethod.CreateDelegate(typeof(Action<object>));
        }

        /// <summary>
        /// Creates a delegate that gets the value of a property. Slower and less safe than generic overload.
        /// Warning: Boxes/Unboxes value types
        /// </summary>
        /// <param name="propertyInfo">Property info to generate getter delegate from.</param>
        /// <returns>Returns the delegate that invokes a generated property getter for the property.</returns>
        public static Func<object, object> GeneratePropertyGetterDelegate(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            if (!propertyInfo.CanRead)
                throw new InvalidArgumentException("Cannot read from propertyInfo");

            MethodInfo getterMethodInfo = propertyInfo.GetGetMethod(true);

            if (getterMethodInfo.IsStatic)
                throw new InvalidArgumentException("propertyInfo is static use GenerateStaticPropertyGetterDelegate() instead");

            Type tSource = propertyInfo.DeclaringType;
            Type tValue = propertyInfo.PropertyType;

            Type tObject = typeof(object);
            singleTypeArray[0] = tObject;
            string dynamicMethodName = useMeaningfulNames ? propertyInfo.ReflectedType.FullName + ".get_" + propertyInfo.Name : GENERATED_NAME;
            DynamicMethod getterMethod = new DynamicMethod(dynamicMethodName, tObject, singleTypeArray, tSource);

            ILGenerator gen = getterMethod.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            if (tSource.IsValueType)
                gen.Emit(OpCodes.Unbox_Any, tSource);
            else if (tSource != tObject)
                gen.Emit(OpCodes.Castclass, tSource);
            if (getterMethodInfo.IsAbstract || getterMethodInfo.IsVirtual)
                gen.Emit(OpCodes.Callvirt, getterMethodInfo);
            else
                gen.Emit(OpCodes.Call, getterMethodInfo);
            if (tValue.IsValueType)
                gen.Emit(OpCodes.Box, tValue);
            else if (tValue != tObject)
                gen.Emit(OpCodes.Castclass, tValue);
            gen.Emit(OpCodes.Ret);

            return (Func<object, object>)getterMethod.CreateDelegate(typeof(Func<object, object>));
        }

        /// <summary>
        /// Creates a delegate that gets the value of a static property. Slower and less safe than generic overload.
        /// Warning: Boxes/Unboxes value types
        /// </summary>
        /// <param name="propertyInfo">Property info to generate getter delegate from.</param>
        /// <returns>Returns the delegate that invokes a generated property getter for the property.</returns>
        public static Func<object> GenerateStaticPropertyGetterDelegate(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            if (!propertyInfo.CanRead)
                throw new InvalidArgumentException("Cannot read from propertyInfo");

            MethodInfo getterMethodInfo = propertyInfo.GetGetMethod(true);

            if (!getterMethodInfo.IsStatic)
                throw new InvalidArgumentException("propertyInfo is not static use GeneratePropertyGetterDelegate() instead");

            Type tSource = propertyInfo.DeclaringType;
            Type tValue = propertyInfo.PropertyType;

            Type tObject = typeof(object);
            string dynamicMethodName = useMeaningfulNames ? propertyInfo.ReflectedType.FullName + ".get_" + propertyInfo.Name : GENERATED_NAME;
            DynamicMethod getterMethod = new DynamicMethod(dynamicMethodName, tObject, emptyTypeArray, tSource);

            ILGenerator gen = getterMethod.GetILGenerator();
            if (getterMethodInfo.IsAbstract || getterMethodInfo.IsVirtual)
                gen.Emit(OpCodes.Callvirt, getterMethodInfo);
            else
                gen.Emit(OpCodes.Call, getterMethodInfo);
            if (tValue.IsValueType)
                gen.Emit(OpCodes.Box, tValue);
            else if (tValue != tObject)
                gen.Emit(OpCodes.Castclass, tValue);
            gen.Emit(OpCodes.Ret);

            return (Func<object>)getterMethod.CreateDelegate(typeof(Func<object>));
        }

        /// <summary>
        /// Creates a delegate that sets the value of a property. Slower and less safe than generic overload.
        /// Warning: Unboxes value types
        /// </summary>
        /// <param name="propertyInfo">Property info to generate setter delegate from.</param>
        /// <returns>Returns the delegate that invokes a generated property setter for the property.</returns>
        public static Action<object, object> GeneratePropertySetterDelegate(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            if (!propertyInfo.CanWrite)
                throw new InvalidArgumentException("Cannot write to propertyInfo");

            MethodInfo setterMethodInfo = propertyInfo.GetSetMethod(true);

            if (setterMethodInfo.IsStatic)
                throw new InvalidArgumentException("fieldInfo is static use GenerateStaticFieldPropertyDelegate() instead");

            Type tSource = propertyInfo.DeclaringType;
            Type tValue = propertyInfo.PropertyType;

            Type tObject = typeof(object);
            doubleTypeArray[0] = tObject;
            doubleTypeArray[1] = tObject;
            string dynamicMethodName = useMeaningfulNames ? propertyInfo.ReflectedType.FullName + ".set_" + propertyInfo.Name : GENERATED_NAME;
            DynamicMethod setterMethod = new DynamicMethod(dynamicMethodName, null, doubleTypeArray, tSource);

            ILGenerator gen = setterMethod.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            if (tSource.IsValueType)
                gen.Emit(OpCodes.Unbox_Any, tSource);
            else if (tSource != tObject)
                gen.Emit(OpCodes.Castclass, tSource);
            gen.Emit(OpCodes.Ldarg_1);
            if (tValue.IsValueType)
                gen.Emit(OpCodes.Unbox_Any, tValue);
            else if (tValue != tObject)
                gen.Emit(OpCodes.Castclass, tValue);
            if (setterMethodInfo.IsAbstract || setterMethodInfo.IsVirtual)
                gen.Emit(OpCodes.Callvirt, setterMethodInfo);
            else
                gen.Emit(OpCodes.Call, setterMethodInfo);
            gen.Emit(OpCodes.Ret);

            return (Action<object, object>)setterMethod.CreateDelegate(typeof(Action<object, object>));
        }

        /// <summary>
        /// Creates a delegate that sets the value of a static property. Slower and less safe than generic overload.
        /// Warning: Unboxes value types
        /// </summary>
        /// <param name="propertyInfo">Property info to generate setter delegate from.</param>
        /// <returns>Returns the delegate that invokes a generated property setter for the property.</returns>
        public static Action<object> GenerateStaticPropertySetterDelegate(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            if (!propertyInfo.CanWrite)
                throw new InvalidArgumentException("Cannot write to propertyInfo");

            MethodInfo setterMethodInfo = propertyInfo.GetSetMethod(true);

            if (!setterMethodInfo.IsStatic)
                throw new InvalidArgumentException("fieldInfo is not static use GenerateFieldPropertyDelegate() instead");

            Type tSource = propertyInfo.DeclaringType;
            Type tValue = propertyInfo.PropertyType;

            Type tObject = typeof(object);
            singleTypeArray[0] = tObject;
            string dynamicMethodName = useMeaningfulNames ? propertyInfo.ReflectedType.FullName + ".set_" + propertyInfo.Name : GENERATED_NAME;
            DynamicMethod setterMethod = new DynamicMethod(dynamicMethodName, null, singleTypeArray, tSource);

            ILGenerator gen = setterMethod.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            if (tSource.IsValueType)
                gen.Emit(OpCodes.Unbox_Any, tSource);
            else if (tSource != tObject)
                gen.Emit(OpCodes.Castclass, tSource);
            if (setterMethodInfo.IsAbstract || setterMethodInfo.IsVirtual)
                gen.Emit(OpCodes.Callvirt, setterMethodInfo);
            else
                gen.Emit(OpCodes.Call, setterMethodInfo);
            gen.Emit(OpCodes.Ret);

            return (Action<object>)setterMethod.CreateDelegate(typeof(Action<object>));
        }

        #endregion
    }
}
#endif
