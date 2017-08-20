#region AOT_ONLY_DEFINE
#define AOT_ONLY
#if (UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID) && !ENABLE_IL2CPP
#undef AOT_ONLY
#endif
#endregion

#if !AOT_ONLY
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Expanse.Extensions;

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

        public static MethodInfo Info { get; private set; }

        private static bool useMeaningfulNames = true;
        private const string GENERATED_NAME = "EmitUtilGenerated";

        // Type array caches used when emitting methods
        private static readonly Type tObject = typeof(object);
        private static readonly Type tTypedRef = typeof(TypedReference);
        private static readonly Type tArray = typeof(Array);
        private static readonly Type tInt = typeof(int);
        private static readonly Type[] emptyTypeArray = new Type[0];
        private static readonly Type[] singleTypeArray = new Type[1];
        private static readonly Type[] doubleTypeArray = new Type[2];

        /// <summary>
        /// Delegate used to cast an object of one type to another.
        /// </summary>
        /// <typeparam name="TSource">Type of the source object.</typeparam>
        /// <typeparam name="TTarget">Type of the target object.</typeparam>
        /// <param name="source">Source object to be casted.</param>
        /// <returns>Returns a casted object of Target type.</returns>
        public delegate TTarget TypeCastDelegate<TSource, TTarget>(TSource source);
        /// <summary>
        /// Delegate used to create new objects using the default constructor.
        /// </summary>
        /// <typeparam name="TSource">Type of the object being constructed.</typeparam>
        /// <returns>Returns a new instance of a newly constructed type.</returns>
        public delegate TSource DefaultConstructorDelegate<TSource>() where TSource : new();
        /// <summary>
        /// Delegate used to create new objects using any constructor.
        /// </summary>
        /// <typeparam name="TSource">Type of the object being constructed.</typeparam>
        /// <param name="params">Ordered type parameters of the constructor.</param>
        /// <returns>Returns a new instance of a newly constructed type.</returns>
        public delegate TSource ConstructorDelegate<TSource>(params object[] @params) where TSource : new();
        /// <summary>
        /// Delegate used to get the value of a static field.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to get.</typeparam>
        /// <returns>Returns the value of the static field.</returns>
        public delegate TValue StaticFieldGetterDelegate<TValue>();
        /// <summary>
        /// Delegate used to get the value of an instance field.
        /// </summary>
        /// <typeparam name="TSource">Type the field is declared in.</typeparam>
        /// <typeparam name="TValue">Type of the value to get.</typeparam>
        /// <param name="source">Instance of the object to get the field from.</param>
        /// <returns>Returns the value of the instance field on the source object.</returns>
        public delegate TValue FieldGetterDelegate<TSource, TValue>(TSource source);
        /// <summary>
        /// Delegate used to set the value of a static field.
        /// </summary>
        /// <typeparam name="TValue">Type of the field to set.</typeparam>
        /// <param name="value">The value to set the field to.</param>
        public delegate void StaticFieldSetterDelegate<TValue>(TValue value);
        /// <summary>
        /// Delegate used to set the value of an instance field.
        /// </summary>
        /// <typeparam name="TSource">Type the field is declared in.</typeparam>
        /// <typeparam name="TValue">Type of the value to set.</typeparam>
        /// <param name="source">Instance of the object to get the field from.</param>
        /// <param name="value">The value to set the field to.</param>
        public delegate void FieldSetterDelegate<TSource, TValue>(TSource source, TValue value) where TSource : class;
        /// <summary>
        /// Delegate used to get the value of a static property.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to get.</typeparam>
        /// <returns>Returns the value of the static property.</returns>
        public delegate TValue StaticPropertyGetterDelegate<TValue>();
        /// <summary>
        /// Delegate used to get the value of an instance property.
        /// </summary>
        /// <typeparam name="TSource">Type the property is declared in.</typeparam>
        /// <typeparam name="TValue">Type of the value to get.</typeparam>
        /// <param name="source">Instance of the object to get the property from.</param>
        /// <returns>Returns the value of the instance property on the source object.</returns>
        public delegate TValue PropertyGetterDelegate<TSource, TValue>(TSource source);
        /// <summary>
        /// Delegate used to set the value of a static property.
        /// </summary>
        /// <typeparam name="TValue">Type of the property to set.</typeparam>
        /// <param name="value">The value to set the property to.</param>
        public delegate void StaticPropertySetterDelegate<TValue>(TValue value);
        /// <summary>
        /// Delegate used to set the value of an instance property.
        /// </summary>
        /// <typeparam name="TSource">Type the property is declared in.</typeparam>
        /// <typeparam name="TValue">Type of the value to set.</typeparam>
        /// <param name="source">Instance of the object to get the property from.</param>
        /// <param name="value">The value to set the property to.</param>
        public delegate void PropertySetterDelegate<TSource, TValue>(TSource source, TValue value) where TSource : class;
        /// <summary>
        /// Delegate used to set the value of an instance field by reference.
        /// </summary>
        /// <typeparam name="TSource">Type the field is declared in.</typeparam>
        /// <typeparam name="TValue">Type of the value to set.</typeparam>
        /// <param name="source">Instance of the object to get the field from.</param>
        /// <param name="value">The value to set the field to.</param>
        public delegate void FieldSetterDelegateByRef<TSource, TValue>(ref TSource source, TValue value);
        /// <summary>
        /// Delegate used to set the value of an instance property by reference.
        /// </summary>
        /// <typeparam name="TSource">Type the property is declared in.</typeparam>
        /// <typeparam name="TValue">Type of the value to set.</typeparam>
        /// <param name="source">Instance of the object to get the property from.</param>
        /// <param name="value">The value to set the property to.</param>
        public delegate void PropertySetterDelegateByRef<TSource, TValue>(ref TSource source, TValue value);
        /// <summary>
        /// Delegate used to get the value of an instance field by typed reference.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to get.</typeparam>
        /// <param name="source">Typed reference of the instance of the object to get the field from.</param>
        /// <returns>Returns the value of the instance field on the source object.</returns>
        public delegate TValue FieldGetterDelegateByTypedRef<TValue>(TypedReference source);
        /// <summary>
        /// Delegate used to get the value of an instance property by typed reference.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to get.</typeparam>
        /// <param name="source">Typed reference of the instance of the object to get the property from.</param>
        /// <returns>Returns the value of the instance property on the source object.</returns>
        public delegate TValue PropertyGetterDelegateByTypedRef<TValue>(TypedReference source);

        public delegate TValue ArrayValueGetterDelegate<TValue>(Array source, int index);

        #region GENERIC

            /// <summary>
            /// Generates a delegate that casts an object of source type to target type.
            /// </summary>
            /// <typeparam name="TSource">Source type.</typeparam>
            /// <typeparam name="TTarget">Target type.</typeparam>
            /// <returns>Returns the delegate that casts an object from one type to another.</returns>
        public static TypeCastDelegate<TSource, TTarget> GenerateTypeCastDelegate<TSource, TTarget>()
        {
            Type tSource = typeof(TSource);
            Type tTarget = typeof(TTarget);

            singleTypeArray[0] = tSource;
            string dynamicMethodName = useMeaningfulNames ? "Cast" + tSource.Name + "To" + tTarget.Name : GENERATED_NAME;
            DynamicMethod casterMethod = new DynamicMethod(dynamicMethodName, tTarget, singleTypeArray, tSource);
            ILGenerator gen = casterMethod.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            if (tSource != tTarget)
            {
                if (tSource.IsAssignableFromOrTo(tTarget))
                {
                    gen.Emit(OpCodes.Castclass, tTarget);
                }
                else
                {
                    if (tTarget == typeof(sbyte))
                        gen.Emit(OpCodes.Conv_I1);
                    else if (tTarget == typeof(short))
                        gen.Emit(OpCodes.Conv_I2);
                    else if (tTarget == typeof(int))
                        gen.Emit(OpCodes.Conv_I4);
                    else if (tTarget == typeof(long))
                        gen.Emit(OpCodes.Conv_I8);
                    else if (tTarget == typeof(byte))
                        gen.Emit(OpCodes.Conv_U1);
                    else if (tTarget == typeof(ushort) || tTarget == typeof(char))
                        gen.Emit(OpCodes.Conv_U2);
                    else if (tTarget == typeof(uint))
                        gen.Emit(OpCodes.Conv_U4);
                    else if (tTarget == typeof(ulong))
                        gen.Emit(OpCodes.Conv_U8);
                }
            }
            gen.Emit(OpCodes.Ret);

            return (TypeCastDelegate<TSource, TTarget>)casterMethod.CreateDelegate(typeof(TypeCastDelegate<TSource, TTarget>));
        }

        /// <summary>
        /// Generates a delegate that casts an object of source type to target type using expressions.
        /// </summary>
        /// <typeparam name="TSource">Source type.</typeparam>
        /// <typeparam name="TTarget">Target type.</typeparam>
        /// <returns>Returns the delegate that casts an object from one type to another.</returns>
        public static TypeCastDelegate<TSource, TTarget> GenerateTypeCastDelegateExp<TSource, TTarget>()
        {
            Type tSource = typeof(TSource);
            Type tTarget = typeof(TTarget);

            ParameterExpression parameter = Expression.Parameter(tSource, string.Empty);
            UnaryExpression conversion = Expression.Convert(parameter, tTarget);

            return Expression.Lambda<TypeCastDelegate<TSource, TTarget>>(conversion, parameter).Compile();
        }

        /// <summary>
        /// Generates a delegate that constructs an instance of type TSource using the default constructor.
        /// </summary>
        /// <typeparam name="TSource">Declaring type that the constructor belongs to.</typeparam>
        /// <returns>Returns the delegate that invokes the default constructor of a type.</returns>
        public static DefaultConstructorDelegate<TSource> GenerateDefaultConstructorDelegate<TSource>()
            where TSource : new()
        {
            // TODO: Be able construct all primitive types
            Type tSource = typeof(TSource);
            ConstructorInfo defaultConstructorInfo = tSource.GetConstructor(emptyTypeArray);

            if (defaultConstructorInfo == null)
                throw new InvalidTypeException("TSource must have a default constructor");

            string dynamicMethodName = useMeaningfulNames ? tSource.FullName + ".ctor" : GENERATED_NAME;
            DynamicMethod constructorMethod = new DynamicMethod(dynamicMethodName, tSource, emptyTypeArray, tSource);
            ILGenerator gen = constructorMethod.GetILGenerator();

            gen.Emit(OpCodes.Newobj, defaultConstructorInfo);
            gen.Emit(OpCodes.Ret);

            return (DefaultConstructorDelegate<TSource>)constructorMethod.CreateDelegate(typeof(DefaultConstructorDelegate<TSource>));
        }

        /// <summary>
        /// Generates a delegate that constructs an instance of type TSource using a constructor.
        /// </summary>
        /// <typeparam name="TSource">Declaring type that the constructor belongs to.</typeparam>
        /// <param name="paramTypes">Ordered parameters of the chosen constructor.</param>
        /// <returns>Returns the delegate that invokes the constructor of a type.</returns>
        public static ConstructorDelegate<TSource> GenerateConstructorDelegate<TSource>(params Type[] paramTypes)
            where TSource : new()
        {
            Type tSource = typeof(TSource);
            ConstructorInfo constructorInfo = tSource.GetConstructor(paramTypes);

            if (constructorInfo == null)
                throw new InvalidTypeException("TSource must have a constructor with matching parameter types");

            string dynamicMethodName = useMeaningfulNames ? tSource.FullName + ".ctor" : GENERATED_NAME;
            DynamicMethod constructorMethod = new DynamicMethod(dynamicMethodName, tSource, paramTypes, tSource);
            ILGenerator gen = constructorMethod.GetILGenerator();
            if (paramTypes.Length > 0)
            {
                for (byte i = 0; i < paramTypes.Length; i++)
                {
                    gen.Emit(OpCodes.Ldarg_0);
                    gen.Emit(OpCodes.Ldelem, i);
                }
            }
            gen.Emit(OpCodes.Newobj, constructorInfo);
            gen.Emit(OpCodes.Ret);

            return (ConstructorDelegate<TSource>)constructorMethod.CreateDelegate(typeof(ConstructorDelegate<TSource>));
        }

        /// <summary>
        /// Creates a delegate that gets the value of a static field.
        /// </summary>
        /// <typeparam name="TValue">Field type of the field info.</typeparam>
        /// <param name="fieldInfo">Field info to generate getter delegate from.</param>
        /// <returns>Returns the delegate that invokes a generated field getter for the field.</returns>
        public static StaticFieldGetterDelegate<TValue> GenerateStaticFieldGetterDelegate<TValue>(FieldInfo fieldInfo)
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

            return (StaticFieldGetterDelegate<TValue>)getterMethod.CreateDelegate(typeof(StaticFieldGetterDelegate<TValue>));
        }

        /// <summary>
        /// Creates a delegate that gets the value of a field.
        /// </summary>
        /// <typeparam name="TSource">Declaring type that the field belongs to.</typeparam>
        /// <typeparam name="TValue">Field type of the field info.</typeparam>
        /// <param name="fieldInfo">Field info to generate getter delegate from.</param>
        /// <returns>Returns the delegate that invokes a generated field getter for the field.</returns>
        public static FieldGetterDelegate<TSource, TValue> GenerateFieldGetterDelegate<TSource, TValue>(FieldInfo fieldInfo)
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

            return (FieldGetterDelegate<TSource, TValue>)getterMethod.CreateDelegate(typeof(FieldGetterDelegate<TSource, TValue>));
        }

        /// <summary>
        /// Creates a delegate that sets the value of a static field.
        /// </summary>
        /// <typeparam name="TValue">Field type of the field info.</typeparam>
        /// <param name="fieldInfo">Field info to generate setter delegate from.</param>
        /// <returns>Returns the delegate that invokes a generated field setter for the field.</returns>
        public static StaticFieldSetterDelegate<TValue> GenerateStaticFieldSetterDelegate<TValue>(FieldInfo fieldInfo)
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

            return (StaticFieldSetterDelegate<TValue>)setterMethod.CreateDelegate(typeof(StaticFieldSetterDelegate<TValue>));
        }

        /// <summary>
        /// Creates a delegate that sets the value of a field. If TSource is a value type use GenerateFieldSetterDelegateByRef() instead.
        /// </summary>
        /// <typeparam name="TSource">Declaring type that the field belongs to.</typeparam>
        /// <typeparam name="TValue">Field type of the field info.</typeparam>
        /// <param name="fieldInfo">Field info to generate setter delegate from.</param>
        /// <returns>Returns the delegate that invokes a generated field setter for the field.</returns>
        public static FieldSetterDelegate<TSource, TValue> GenerateFieldSetterDelegate<TSource, TValue>(FieldInfo fieldInfo)
            where TSource : class
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

            return (FieldSetterDelegate<TSource, TValue>)setterMethod.CreateDelegate(typeof(FieldSetterDelegate<TSource, TValue>));
        }

        /// <summary>
        /// Creates a delegate that sets the value of a field. If TSource is a reference type use GenerateFieldSetterDelegate() instead.
        /// </summary>
        /// <typeparam name="TSource">Declaring type that the field belongs to.</typeparam>
        /// <typeparam name="TValue">Field type of the field info.</typeparam>
        /// <param name="fieldInfo">Field info to generate setter delegate from.</param>
        /// <returns>Returns the delegate that invokes a generated field setter for the field.</returns>
        public static FieldSetterDelegateByRef<TSource, TValue> GenerateFieldSetterDelegateByRef<TSource, TValue>(FieldInfo fieldInfo)
            where TSource : struct
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

            doubleTypeArray[0] = tSource.MakeByRefType();
            doubleTypeArray[1] = tValue;
            string dynamicMethodName = useMeaningfulNames ? fieldInfo.ReflectedType.FullName + ".set_" + fieldInfo.Name : GENERATED_NAME;
            DynamicMethod setterMethod = new DynamicMethod(dynamicMethodName, null, doubleTypeArray, tSource);

            ILGenerator gen = setterMethod.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Stfld, fieldInfo);
            gen.Emit(OpCodes.Ret);

            return (FieldSetterDelegateByRef<TSource, TValue>)setterMethod.CreateDelegate(typeof(FieldSetterDelegateByRef<TSource, TValue>));
        }

        /// <summary>
        /// Creates a delegate that gets the value of a static property.
        /// </summary>
        /// <typeparam name="TValue">Property type of the property info.</typeparam>
        /// <param name="propertyInfo">Property info to create the delegate from.</param>
        /// <returns>Returns the delegate that invokes the property getter.</returns>
        public static StaticPropertyGetterDelegate<TValue> GenerateStaticPropertyGetterDelegate<TValue>(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            if (!propertyInfo.CanRead)
                throw new InvalidArgumentException("Cannot read from propertyInfo");


            Type tSource = propertyInfo.DeclaringType;
            Type tValue = propertyInfo.PropertyType;

            if (typeof(TValue) != tValue)
                throw new InvalidArgumentException("Type TValue must equal the propertyInfo property type");

            MethodInfo getterMethodInfo = propertyInfo.GetGetMethod(true);

            if (!getterMethodInfo.IsStatic)
                throw new InvalidArgumentException("propertyInfo is not static use GeneratePropertyGetterDelegate() instead");

            string dynamicMethodName = useMeaningfulNames ? propertyInfo.ReflectedType.FullName + ".get_" + propertyInfo.Name : GENERATED_NAME;
            DynamicMethod getterMethod = new DynamicMethod(dynamicMethodName, tValue, emptyTypeArray, tSource);

            ILGenerator gen = getterMethod.GetILGenerator();
            if (getterMethodInfo.IsAbstract || getterMethod.IsVirtual)
                gen.Emit(OpCodes.Callvirt, getterMethodInfo);
            else
                gen.Emit(OpCodes.Call, getterMethodInfo);
            gen.Emit(OpCodes.Ret);

            return (StaticPropertyGetterDelegate<TValue>)getterMethod.CreateDelegate(typeof(StaticPropertyGetterDelegate<TValue>));
        }

        /// <summary>
        /// Creates a delegate that gets the value of a property.
        /// </summary>
        /// <typeparam name="TSource">Declaring type that the property belongs to.</typeparam>
        /// <typeparam name="TValue">Property type of the property info.</typeparam>
        /// <param name="propertyInfo">Property info to create the delegate from.</param>
        /// <returns>Returns the delegate that invokes the property getter.</returns>
        public static PropertyGetterDelegate<TSource, TValue> GeneratePropertyGetterDelegate<TSource, TValue>(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            if (!propertyInfo.CanRead)
                throw new InvalidArgumentException("Cannot read from propertyInfo");

            Type tSource = propertyInfo.DeclaringType;
            Type tValue = propertyInfo.PropertyType;

            if (typeof(TSource) != tSource)
                throw new InvalidArgumentException("Type TSource must equal the propertyInfo declaring type");

            if (typeof(TValue) != tValue)
                throw new InvalidArgumentException("Type TValue must equal the propertyInfo property type");

            MethodInfo getterMethodInfo = propertyInfo.GetGetMethod(true);

            if (getterMethodInfo.IsStatic)
                throw new InvalidArgumentException("propertyInfo is static use GenerateStaticPropertyGetterDelegate() instead");

            singleTypeArray[0] = tSource;
            string dynamicMethodName = useMeaningfulNames ? propertyInfo.ReflectedType.FullName + ".get_" + propertyInfo.Name : GENERATED_NAME;
            DynamicMethod getterMethod = new DynamicMethod(dynamicMethodName, tValue, singleTypeArray, tSource);

            ILGenerator gen = getterMethod.GetILGenerator();
            if (tSource.IsValueType)
                gen.Emit(OpCodes.Ldarga_S, 0x0);
            else
                gen.Emit(OpCodes.Ldarg_0);
            if (getterMethodInfo.IsAbstract || getterMethod.IsVirtual)
                gen.Emit(OpCodes.Callvirt, getterMethodInfo);
            else
                gen.Emit(OpCodes.Call, getterMethodInfo);
            gen.Emit(OpCodes.Ret);

            return (PropertyGetterDelegate<TSource, TValue>)getterMethod.CreateDelegate(typeof(PropertyGetterDelegate<TSource, TValue>));
        }

        /// <summary>
        /// Creates a delegate that sets the value of a static property.
        /// </summary>
        /// <typeparam name="TValue">Property type of the property info.</typeparam>
        /// <param name="propertyInfo">Property info to create the delegate from.</param>
        /// <returns>Returns the delegate that invokes the property setter.</returns>
        public static StaticPropertySetterDelegate<TValue> GenerateStaticPropertySetterDelegate<TValue>(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            if (!propertyInfo.CanWrite)
                throw new InvalidArgumentException("Cannot write to propertyInfo");

            Type tSource = propertyInfo.DeclaringType;
            Type tValue = propertyInfo.PropertyType;

            if (typeof(TValue) != tValue)
                throw new InvalidArgumentException("Type TValue must equal the propertyInfo property type");

            MethodInfo setterMethodInfo = propertyInfo.GetSetMethod(true);

            if (!setterMethodInfo.IsStatic)
                throw new InvalidArgumentException("propertyInfo is not static use GeneratePropertySetterDelegate() instead");

            singleTypeArray[0] = tValue;
            string dynamicMethodName = useMeaningfulNames ? propertyInfo.ReflectedType.FullName + ".set_" + propertyInfo.Name : GENERATED_NAME;
            DynamicMethod setterMethod = new DynamicMethod(dynamicMethodName, null, singleTypeArray, tSource);

            ILGenerator gen = setterMethod.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            if (setterMethodInfo.IsAbstract || setterMethodInfo.IsVirtual)
                gen.Emit(OpCodes.Callvirt, setterMethodInfo);
            else
                gen.Emit(OpCodes.Call, setterMethodInfo);
            gen.Emit(OpCodes.Ret);

            return (StaticPropertySetterDelegate<TValue>)setterMethod.CreateDelegate(typeof(StaticPropertySetterDelegate<TValue>));
        }

        /// <summary>
        /// Creates a delegate that sets the value of a property. If TSource is a value type use GeneratePropertySetterDelegateByRef() instead.
        /// </summary>
        /// <typeparam name="TSource">Declaring type that the property belongs to.</typeparam>
        /// <typeparam name="TValue">Property type of the property info.</typeparam>
        /// <param name="propertyInfo">Property info to create the delegate from.</param>
        /// <returns>Returns the delegate that invokes the property setter.</returns>
        public static PropertySetterDelegate<TSource, TValue> GeneratePropertySetterDelegate<TSource, TValue>(PropertyInfo propertyInfo)
            where TSource : class
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            if (!propertyInfo.CanWrite)
                throw new InvalidArgumentException("Cannot write to propertyInfo");

            Type tSource = propertyInfo.DeclaringType;
            Type tValue = propertyInfo.PropertyType;

            if (typeof(TSource) != tSource)
                throw new InvalidArgumentException("Type TSource must equal the propertyInfo declaring type");

            if (typeof(TValue) != tValue)
                throw new InvalidArgumentException("Type TValue must equal the propertyInfo property type");

            MethodInfo setterMethodInfo = propertyInfo.GetSetMethod(true);

            if (setterMethodInfo.IsStatic)
                throw new InvalidArgumentException("propertyInfo is static use GenerateStaticPropertySetterDelegate() instead");

            doubleTypeArray[0] = tSource;
            doubleTypeArray[1] = tValue;
            string dynamicMethodName = useMeaningfulNames ? propertyInfo.ReflectedType.FullName + ".set_" + propertyInfo.Name : GENERATED_NAME;
            DynamicMethod setterMethod = new DynamicMethod(dynamicMethodName, null, doubleTypeArray, tSource);

            ILGenerator gen = setterMethod.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            if (setterMethodInfo.IsAbstract || setterMethodInfo.IsVirtual)
                gen.Emit(OpCodes.Callvirt, setterMethodInfo);
            else
                gen.Emit(OpCodes.Call, setterMethodInfo);
            gen.Emit(OpCodes.Ret);

            return (PropertySetterDelegate<TSource, TValue>)setterMethod.CreateDelegate(typeof(PropertySetterDelegate<TSource, TValue>));
        }

        /// <summary>
        /// Creates a delegate that sets the value of a property. If TSource is a reference type use GeneratePropertySetterDelegate() instead.
        /// </summary>
        /// <typeparam name="TSource">Declaring type that the property belongs to.</typeparam>
        /// <typeparam name="TValue">Property type of the property info.</typeparam>
        /// <param name="propertyInfo">Property info to create the delegate from.</param>
        /// <returns>Returns the delegate that invokes the property setter.</returns>
        public static PropertySetterDelegateByRef<TSource, TValue> GeneratePropertySetterDelegateByRef<TSource, TValue>(PropertyInfo propertyInfo)
            where TSource : struct
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            if (!propertyInfo.CanWrite)
                throw new InvalidArgumentException("Cannot write to propertyInfo");

            Type tSource = propertyInfo.DeclaringType;
            Type tValue = propertyInfo.PropertyType;

            if (typeof(TSource) != tSource)
                throw new InvalidArgumentException("Type TSource must equal the propertyInfo declaring type");

            if (typeof(TValue) != tValue)
                throw new InvalidArgumentException("Type TValue must equal the propertyInfo property type");

            MethodInfo setterMethodInfo = propertyInfo.GetSetMethod(true);

            if (setterMethodInfo.IsStatic)
                throw new InvalidArgumentException("propertyInfo is static use GenerateStaticPropertySetterDelegate() instead");

            doubleTypeArray[0] = tSource.MakeByRefType();
            doubleTypeArray[1] = tValue;
            string dynamicMethodName = useMeaningfulNames ? propertyInfo.ReflectedType.FullName + ".set_" + propertyInfo.Name : GENERATED_NAME;
            DynamicMethod setterMethod = new DynamicMethod(dynamicMethodName, null, doubleTypeArray, tSource);

            ILGenerator gen = setterMethod.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            if (setterMethodInfo.IsAbstract || setterMethodInfo.IsVirtual)
                gen.Emit(OpCodes.Callvirt, setterMethodInfo);
            else
                gen.Emit(OpCodes.Call, setterMethodInfo);
            gen.Emit(OpCodes.Ret);

            return (PropertySetterDelegateByRef<TSource, TValue>)setterMethod.CreateDelegate(typeof(PropertySetterDelegateByRef<TSource, TValue>));
        }

        public static ArrayValueGetterDelegate<TValue> GenerateArrayValueGetterDelegate<TValue>()
        {
            Type tValue = typeof(TValue);
            Type tValueArray = tValue.MakeArrayType();

            doubleTypeArray[0] = tArray;
            doubleTypeArray[1] = tInt;
            string dynamicMethodName = useMeaningfulNames ? tValue.FullName + ".GetArrayIndex" : GENERATED_NAME;
            DynamicMethod getterMethod = new DynamicMethod(dynamicMethodName, tValue, doubleTypeArray, tValue);

            ILGenerator gen = getterMethod.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Castclass, tValueArray);
            gen.Emit(OpCodes.Ldarg_1);
            if (tValue.IsValueType)
            {
                if (tValue == typeof(sbyte))
                    gen.Emit(OpCodes.Ldelem_I1);
                else if (tValue == typeof(short))
                    gen.Emit(OpCodes.Ldelem_I2);
                else if (tValue == typeof(int))
                    gen.Emit(OpCodes.Ldelem_I4);
                else if (tValue == typeof(long))
                    gen.Emit(OpCodes.Ldelem_I8);
                else if (tValue == typeof(byte))
                    gen.Emit(OpCodes.Ldelem_U1);
                else if (tValue == typeof(ushort))
                    gen.Emit(OpCodes.Ldelem_U2);
                else if (tValue == typeof(uint))
                    gen.Emit(OpCodes.Ldelem_U4);
                else if (tValue == typeof(ulong))
                    gen.Emit(OpCodes.Ldelem_I8);
                else if (tValue == typeof(float))
                    gen.Emit(OpCodes.Ldelem_R4);
                else if (tValue == typeof(double))
                    gen.Emit(OpCodes.Ldelem_R8);
                else
                    gen.Emit(OpCodes.Ldelem);
            }
            else
                gen.Emit(OpCodes.Ldelem_Ref);
            gen.Emit(OpCodes.Ret);

            return (ArrayValueGetterDelegate<TValue>)getterMethod.CreateDelegate(typeof(ArrayValueGetterDelegate<TValue>));
        }

        #endregion

        #region NON_GENERIC

        /// <summary>
        /// Generates a delegate that constructs an instance of type tSource using the default constructor. Slower and less safe than generic overload.
        /// </summary>
        /// <param name="tSource">The type to generate the default constructor invoker from.</param>
        /// <returns>Returns the delegate that invokes the default constructor for a given type.</returns>
        public static DefaultConstructorDelegate<object> GenerateDefaultConstructorDelegate(Type tSource)
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
            else if (tSource != tObject)
                gen.Emit(OpCodes.Castclass, tSource);
            gen.Emit(OpCodes.Ret);

            return (DefaultConstructorDelegate<object>)constructorMethod.CreateDelegate(typeof(DefaultConstructorDelegate<object>));
        }

        /// <summary>
        /// Creates a delegate that gets the value of a field. Slower and less safe than generic overload.
        /// <para>Warning: Boxes/Unboxes value types.</para>
        /// </summary>
        /// <param name="fieldInfo">Field info to generate getter delegate from.</param>
        /// <returns>Returns the delegate that invokes a generated field getter for the field.</returns>
        public static FieldGetterDelegate<object, object> GenerateFieldGetterDelegate(FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
                throw new ArgumentNullException("field");

            if (fieldInfo.IsStatic)
                throw new InvalidArgumentException("fieldInfo is static use GenerateStaticFieldGetterDelegate() instead");

            Type tSource = fieldInfo.DeclaringType;
            Type tValue = fieldInfo.FieldType;

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

            return (FieldGetterDelegate<object, object>)getterMethod.CreateDelegate(typeof(FieldGetterDelegate<object, object>));
        }

        /// <summary>
        /// Creates a delegate that gets the value of a field. Slower and less safe than generic overload.
        /// <para>Warning: Boxes/Unboxes value types.</para>
        /// </summary>
        /// <param name="fieldInfo">Field info to generate getter delegate from.</param>
        /// <returns>Returns the delegate that invokes a generated field getter for the field.</returns>
        public static FieldGetterDelegateByTypedRef<object> GenerateFieldGetterDelegateByTypedRef(FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
                throw new ArgumentNullException("field");

            if (fieldInfo.IsStatic)
                throw new InvalidArgumentException("fieldInfo is static use GenerateStaticFieldGetterDelegate() instead");

            Type tSource = fieldInfo.DeclaringType;
            Type tValue = fieldInfo.FieldType;

            singleTypeArray[0] = tTypedRef;
            string dynamicMethodName = useMeaningfulNames ? fieldInfo.ReflectedType.FullName + ".get_" + fieldInfo.Name : GENERATED_NAME;
            DynamicMethod getterMethod = new DynamicMethod(dynamicMethodName, tObject, singleTypeArray, tSource);

            ILGenerator gen = getterMethod.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Refanyval, tSource);
            gen.Emit(OpCodes.Ldobj, tSource);
            gen.Emit(OpCodes.Ldfld, fieldInfo);
            if (tValue.IsValueType)
                gen.Emit(OpCodes.Box, tValue);
            else if (tValue != tObject)
                gen.Emit(OpCodes.Castclass, tValue);
            gen.Emit(OpCodes.Ret);

            return (FieldGetterDelegateByTypedRef<object>)getterMethod.CreateDelegate(typeof(FieldGetterDelegateByTypedRef<object>));
        }

        /// <summary>
        /// Creates a delegate that gets the value of a static field. Slower and less safe than generic overload.
        /// <para>Warning: Boxes/Unboxes value types.</para>
        /// </summary>
        /// <param name="fieldInfo">Field info to generate getter delegate from.</param>
        /// <returns>Returns the delegate that invokes a generated field getter for the field.</returns>
        public static StaticFieldGetterDelegate<object> GenerateStaticFieldGetterDelegate(FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
                throw new ArgumentNullException("field");

            if (!fieldInfo.IsStatic)
                throw new InvalidArgumentException("fieldInfo is not static use GenerateFieldGetterDelegate() instead");

            Type tSource = fieldInfo.DeclaringType;
            Type tValue = fieldInfo.FieldType;

            string dynamicMethodName = useMeaningfulNames ? fieldInfo.ReflectedType.FullName + ".get_" + fieldInfo.Name : GENERATED_NAME;
            DynamicMethod getterMethod = new DynamicMethod(dynamicMethodName, tObject, emptyTypeArray, tSource);

            ILGenerator gen = getterMethod.GetILGenerator();
            gen.Emit(OpCodes.Ldsfld, fieldInfo);
            if (tValue.IsValueType)
                gen.Emit(OpCodes.Box, tValue);
            else if (tValue != tObject)
                gen.Emit(OpCodes.Castclass, tValue);
            gen.Emit(OpCodes.Ret);

            return (StaticFieldGetterDelegate<object>)getterMethod.CreateDelegate(typeof(StaticFieldGetterDelegate<object>));
        }

        /// <summary>
        /// Creates a delegate that sets the value of a field. Slower and less safe than generic overloads.
        /// <para>Warning: Unboxes value types.</para>
        /// </summary>
        /// <param name="fieldInfo">Field info to generate setter delegate from.</param>
        /// <returns>Returns the delegate that invokes a generated field setter for the field.</returns>
        public static FieldSetterDelegate<object, object> GenerateFieldSetterDelegate(FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
                throw new ArgumentNullException("field");

            if (fieldInfo.IsStatic)
                throw new InvalidArgumentException("fieldInfo is static use GenerateStaticFieldSetterDelegate() instead");

            Type tSource = fieldInfo.DeclaringType;
            Type tValue = fieldInfo.FieldType;

            doubleTypeArray[0] = tObject;
            doubleTypeArray[1] = tObject;
            string dynamicMethodName = useMeaningfulNames ? fieldInfo.ReflectedType.FullName + ".set_" + fieldInfo.Name : GENERATED_NAME;
            DynamicMethod setterMethod = new DynamicMethod(dynamicMethodName, null, doubleTypeArray, tSource);

            ILGenerator gen = setterMethod.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            if (tSource.IsValueType)
                gen.Emit(OpCodes.Unbox, tSource);
            else if (tSource != tObject)
                gen.Emit(OpCodes.Castclass, tSource);
            gen.Emit(OpCodes.Ldarg_1);
            if (tValue.IsValueType)
                gen.Emit(OpCodes.Unbox_Any, tValue);
            else if (tValue != tObject)
                gen.Emit(OpCodes.Castclass, tValue);
            gen.Emit(OpCodes.Stfld, fieldInfo);
            gen.Emit(OpCodes.Ret);

            return (FieldSetterDelegate<object, object>)setterMethod.CreateDelegate(typeof(FieldSetterDelegate<object, object>));
        }

        /// <summary>
        /// Creates a delegate that sets the value of a static field. Slower and less safe than generic overload.
        /// <para>Warning: Unboxes value types.</para>
        /// </summary>
        /// <param name="fieldInfo">Field info to generate setter delegate from.</param>
        /// <returns>Returns the delegate that invokes a generated field setter for the field.</returns>
        public static StaticFieldSetterDelegate<object> GenerateStaticFieldSetterDelegate(FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
                throw new ArgumentNullException("field");

            if (!fieldInfo.IsStatic)
                throw new InvalidArgumentException("fieldInfo is not static use GenerateFieldSetterDelegate() instead");

            Type tSource = fieldInfo.DeclaringType;
            Type tValue = fieldInfo.FieldType;

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

            return (StaticFieldSetterDelegate<object>)setterMethod.CreateDelegate(typeof(StaticFieldSetterDelegate<object>));
        }

        /// <summary>
        /// Creates a delegate that gets the value of a property. Slower and less safe than generic overload.
        /// <para>Warning: Boxes value types.</para>
        /// </summary>
        /// <param name="propertyInfo">Property info to generate getter delegate from.</param>
        /// <returns>Returns the delegate that invokes a generated property getter for the property.</returns>
        public static PropertyGetterDelegate<object, object> GeneratePropertyGetterDelegate(PropertyInfo propertyInfo)
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

            singleTypeArray[0] = tObject;
            string dynamicMethodName = useMeaningfulNames ? propertyInfo.ReflectedType.FullName + ".get_" + propertyInfo.Name : GENERATED_NAME;
            DynamicMethod getterMethod = new DynamicMethod(dynamicMethodName, tObject, singleTypeArray, tSource);

            ILGenerator gen = getterMethod.GetILGenerator();
            if (tSource.IsValueType)
                gen.DeclareLocal(tSource);
            gen.Emit(OpCodes.Ldarg_0);
            if (tSource.IsValueType)
                gen.Emit(OpCodes.Unbox_Any, tSource);
            else if (tSource != tObject)
                gen.Emit(OpCodes.Castclass, tSource);
            if (tSource.IsValueType)
            {
                gen.Emit(OpCodes.Stloc_0);
                gen.Emit(OpCodes.Ldloca_S, 0x0);
            }
            if (getterMethodInfo.IsAbstract || getterMethodInfo.IsVirtual)
                gen.Emit(OpCodes.Callvirt, getterMethodInfo);
            else
                gen.Emit(OpCodes.Call, getterMethodInfo);
            if (tValue.IsValueType)
                gen.Emit(OpCodes.Box, tValue);
            else if (tValue != tObject)
                gen.Emit(OpCodes.Castclass, tValue);
            gen.Emit(OpCodes.Ret);

            return (PropertyGetterDelegate<object, object>)getterMethod.CreateDelegate(typeof(PropertyGetterDelegate<object, object>));
        }

        /// <summary>
        /// Creates a delegate that gets the value of a property. Slower and less safe than generic overload.
        /// <para>Warning: Boxes value types.</para>
        /// </summary>
        /// <param name="propertyInfo">Property info to generate getter delegate from.</param>
        /// <returns>Returns the delegate that invokes a generated property getter for the property.</returns>
        public static PropertyGetterDelegateByTypedRef<object> GeneratePropertySetterDelegateByTypedRef(PropertyInfo propertyInfo)
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

            singleTypeArray[0] = tTypedRef;
            string dynamicMethodName = useMeaningfulNames ? propertyInfo.ReflectedType.FullName + ".get_" + propertyInfo.Name : GENERATED_NAME;
            DynamicMethod getterMethod = new DynamicMethod(dynamicMethodName, tObject, singleTypeArray, tSource);

            ILGenerator gen = getterMethod.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Refanyval, tSource);
            gen.Emit(OpCodes.Ldobj, tSource);
            if (getterMethodInfo.IsVirtual || getterMethodInfo.IsAbstract)
                gen.Emit(OpCodes.Callvirt, getterMethodInfo);
            else
                gen.Emit(OpCodes.Call, getterMethodInfo);
            if (tValue.IsValueType)
                gen.Emit(OpCodes.Box, tValue);
            else if (tValue != tObject)
                gen.Emit(OpCodes.Castclass, tValue);
            gen.Emit(OpCodes.Ret);

            return (PropertyGetterDelegateByTypedRef<object>)getterMethod.CreateDelegate(typeof(PropertyGetterDelegateByTypedRef<object>));
        }

        /// <summary>
        /// Creates a delegate that gets the value of a static property. Slower and less safe than generic overload.
        /// <para>Warning: Boxes/Unboxes value types.</para>
        /// </summary>
        /// <param name="propertyInfo">Property info to generate getter delegate from.</param>
        /// <returns>Returns the delegate that invokes a generated property getter for the property.</returns>
        public static StaticPropertyGetterDelegate<object> GenerateStaticPropertyGetterDelegate(PropertyInfo propertyInfo)
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

            return (StaticPropertyGetterDelegate<object>)getterMethod.CreateDelegate(typeof(StaticPropertyGetterDelegate<object>));
        }

        /// <summary>
        /// Creates a delegate that sets the value of a property. Slower and less safe than generic overloads.
        /// <para>Warning: Unboxes value types.</para>
        /// </summary>
        /// <param name="propertyInfo">Property info to generate setter delegate from.</param>
        /// <returns>Returns the delegate that invokes a generated property setter for the property.</returns>
        public static PropertySetterDelegate<object, object> GeneratePropertySetterDelegate(PropertyInfo propertyInfo)
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

            doubleTypeArray[0] = tObject;
            doubleTypeArray[1] = tObject;
            string dynamicMethodName = useMeaningfulNames ? propertyInfo.ReflectedType.FullName + ".set_" + propertyInfo.Name : GENERATED_NAME;
            DynamicMethod setterMethod = new DynamicMethod(dynamicMethodName, null, doubleTypeArray, tSource);

            ILGenerator gen = setterMethod.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            if (tSource.IsValueType)
                gen.Emit(OpCodes.Unbox, tSource);
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

            return (PropertySetterDelegate<object, object>)setterMethod.CreateDelegate(typeof(PropertySetterDelegate<object, object>));
        }

        /// <summary>
        /// Creates a delegate that sets the value of a static property. Slower and less safe than generic overload.
        /// <para>Warning: Unboxes value types.</para>
        /// </summary>
        /// <param name="propertyInfo">Property info to generate setter delegate from.</param>
        /// <returns>Returns the delegate that invokes a generated property setter for the property.</returns>
        public static StaticPropertySetterDelegate<object> GenerateStaticPropertySetterDelegate(PropertyInfo propertyInfo)
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

            singleTypeArray[0] = tObject;
            string dynamicMethodName = useMeaningfulNames ? propertyInfo.ReflectedType.FullName + ".set_" + propertyInfo.Name : GENERATED_NAME;
            DynamicMethod setterMethod = new DynamicMethod(dynamicMethodName, null, singleTypeArray, tSource);

            ILGenerator gen = setterMethod.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            if (tValue.IsValueType)
                gen.Emit(OpCodes.Unbox_Any, tValue);
            else if (tValue != tObject)
                gen.Emit(OpCodes.Castclass, tValue);
            if (setterMethodInfo.IsAbstract || setterMethodInfo.IsVirtual)
                gen.Emit(OpCodes.Callvirt, setterMethodInfo);
            else
                gen.Emit(OpCodes.Call, setterMethodInfo);
            gen.Emit(OpCodes.Ret);

            return (StaticPropertySetterDelegate<object>)setterMethod.CreateDelegate(typeof(StaticPropertySetterDelegate<object>));
        }

        public static ArrayValueGetterDelegate<object> GenerateArrayValueGetterDelegate(Type tValue)
        {
            Type tValueArray = tValue.MakeArrayType();

            doubleTypeArray[0] = tArray;
            doubleTypeArray[1] = tInt;
            string dynamicMethodName = useMeaningfulNames ? tValue.FullName + ".GetArrayIndex" : GENERATED_NAME;
            DynamicMethod getterMethod = new DynamicMethod(dynamicMethodName, tObject, doubleTypeArray, tValue);

            ILGenerator gen = getterMethod.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Castclass, tValueArray);
            gen.Emit(OpCodes.Ldarg_1);
            if (tValue.IsValueType)
            {
                if (tValue == typeof(sbyte))
                    gen.Emit(OpCodes.Ldelem_I1);
                else if (tValue == typeof(short))
                    gen.Emit(OpCodes.Ldelem_I2);
                else if (tValue == typeof(int))
                    gen.Emit(OpCodes.Ldelem_I4);
                else if (tValue == typeof(long))
                    gen.Emit(OpCodes.Ldelem_I8);
                else if (tValue == typeof(byte))
                    gen.Emit(OpCodes.Ldelem_U1);
                else if (tValue == typeof(ushort))
                    gen.Emit(OpCodes.Ldelem_U2);
                else if (tValue == typeof(uint))
                    gen.Emit(OpCodes.Ldelem_U4);
                else if (tValue == typeof(ulong))
                    gen.Emit(OpCodes.Ldelem_I8);
                else if (tValue == typeof(float))
                    gen.Emit(OpCodes.Ldelem_R4);
                else if (tValue == typeof(double))
                    gen.Emit(OpCodes.Ldelem_R8);
                else
                    gen.Emit(OpCodes.Ldelem);
            }
            else
                gen.Emit(OpCodes.Ldelem_Ref);
            if (tValue.IsValueType)
                gen.Emit(OpCodes.Box, tValue);
            else if (tValue != tObject)
                gen.Emit(OpCodes.Castclass, tObject);
            gen.Emit(OpCodes.Ret);

            return (ArrayValueGetterDelegate<object>)getterMethod.CreateDelegate(typeof(ArrayValueGetterDelegate<object>));
        }

        #endregion

        #region NON_GENERIC_SOURCE

        /// <summary>
        /// Creates a delegate that gets the value of a field. Slower and less safe than generic overload.
        /// <para>Warning: Boxes/Unboxes value types that are the source.</para>
        /// </summary>
        /// <typeparam name="TValue">Field type of the field info.</typeparam>
        /// <param name="fieldInfo">Field info to generate getter delegate from.</param>
        /// <returns>Returns the delegate that invokes a generated field getter for the field.</returns>
        public static FieldGetterDelegate<object, TValue> GenerateFieldGetterDelegate<TValue>(FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
                throw new ArgumentNullException("field");

            if (fieldInfo.IsStatic)
                throw new InvalidArgumentException("fieldInfo is static use GenerateStaticFieldGetterDelegate() instead");

            Type tSource = fieldInfo.DeclaringType;
            Type tValue = typeof(TValue);

            if (tValue != fieldInfo.FieldType)
                throw new InvalidArgumentException("Type TValue must equal the fieldInfo field type");

            singleTypeArray[0] = tObject;
            string dynamicMethodName = useMeaningfulNames ? fieldInfo.ReflectedType.FullName + ".get_" + fieldInfo.Name : GENERATED_NAME;
            DynamicMethod getterMethod = new DynamicMethod(dynamicMethodName, tValue, singleTypeArray, tSource);

            ILGenerator gen = getterMethod.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            if (tSource.IsValueType)
                gen.Emit(OpCodes.Unbox_Any, tSource);
            else if (tSource != tObject)
                gen.Emit(OpCodes.Castclass, tSource);
            gen.Emit(OpCodes.Ldfld, fieldInfo);
            gen.Emit(OpCodes.Ret);

            return (FieldGetterDelegate<object, TValue>)getterMethod.CreateDelegate(typeof(FieldGetterDelegate<object, TValue>));
        }

        /// <summary>
        /// Creates a delegate that gets the value of a field. Slower and less safe than generic overload.
        /// <para>Avoids boxing of value type source by using TypedReferences.</para>
        /// </summary>
        /// <typeparam name="TValue">Field type of the field info.</typeparam>
        /// <param name="fieldInfo">Field info to generate getter delegate from.</param>
        /// <returns>Returns the delegate that invokes a generated field getter for the field.</returns>
        public static FieldGetterDelegateByTypedRef<TValue> GenerateFieldGetterDelegateByTypedRef<TValue>(FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
                throw new ArgumentNullException("field");

            if (fieldInfo.IsStatic)
                throw new InvalidArgumentException("fieldInfo is static use GenerateStaticFieldGetterDelegate() instead");

            Type tSource = fieldInfo.DeclaringType;
            Type tValue = typeof(TValue);

            if (tValue != fieldInfo.FieldType)
                throw new InvalidArgumentException("Type TValue must equal the fieldInfo field type");

            singleTypeArray[0] = tTypedRef;
            string dynamicMethodName = useMeaningfulNames ? fieldInfo.ReflectedType.FullName + ".get_" + fieldInfo.Name : GENERATED_NAME;
            DynamicMethod getterMethod = new DynamicMethod(dynamicMethodName, tValue, singleTypeArray, tSource);

            ILGenerator gen = getterMethod.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Refanyval, tSource);
            gen.Emit(OpCodes.Ldobj, tSource);
            gen.Emit(OpCodes.Ldfld, fieldInfo);
            gen.Emit(OpCodes.Ret);

            return (FieldGetterDelegateByTypedRef<TValue>)getterMethod.CreateDelegate(typeof(FieldGetterDelegateByTypedRef<TValue>));
        }

        /// <summary>
        /// Creates a delegate that sets the value of a field. Slower and less safe than generic overload.
        /// <para>Warning: Unboxes value types that are the source.</para>
        /// </summary>
        /// <typeparam name="TValue">Field type of the field info.</typeparam>
        /// <param name="fieldInfo">Field info to generate setter delegate from.</param>
        /// <returns>Returns the delegate that invokes a generated field setter for the field.</returns>
        public static FieldSetterDelegate<object, TValue> GenerateFieldSetterDelegate<TValue>(FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
                throw new ArgumentNullException("field");

            if (fieldInfo.IsStatic)
                throw new InvalidArgumentException("fieldInfo is static use GenerateStaticFieldSetterDelegate() instead");

            Type tSource = fieldInfo.DeclaringType;
            Type tValue = typeof(TValue);

            if (tValue != fieldInfo.FieldType)
                throw new InvalidArgumentException("Type TValue must equal the fieldInfo field type");

            doubleTypeArray[0] = tObject;
            doubleTypeArray[1] = tValue;
            string dynamicMethodName = useMeaningfulNames ? fieldInfo.ReflectedType.FullName + ".set_" + fieldInfo.Name : GENERATED_NAME;
            DynamicMethod setterMethod = new DynamicMethod(dynamicMethodName, null, doubleTypeArray, tSource);

            ILGenerator gen = setterMethod.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            if (tSource.IsValueType)
                gen.Emit(OpCodes.Unbox, tSource);
            else if (tSource != tObject)
                gen.Emit(OpCodes.Castclass, tSource);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Stfld, fieldInfo);
            gen.Emit(OpCodes.Ret);

            return (FieldSetterDelegate<object, TValue>)setterMethod.CreateDelegate(typeof(FieldSetterDelegate<object, TValue>));
        }

#if NET_4_5
        /// <summary>
        /// Creates a delegate that sets the value of a field using an expression-compiled delegate. Slower than non expression overload.
        /// </summary>
        /// <typeparam name="TValue">Field type of the field info.</typeparam>
        /// <param name="fieldInfo">Field info to generate setter delegate from.</param>
        /// <returns>Returns the delegate that invokes a generated field setter for the field.</returns>
        public static FieldSetterDelegate<object, TValue> GenerateFieldSetterDelegateExp<TValue>(FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
                throw new ArgumentNullException("field");

            if (fieldInfo.IsStatic)
                throw new InvalidArgumentException("fieldInfo is static use GenerateStaticFieldSetterDelegate() instead");

            Type tSource = fieldInfo.DeclaringType;
            Type tValue = typeof(TValue);

            if (tValue != fieldInfo.FieldType)
                throw new InvalidArgumentException("Type TValue must equal the fieldInfo field type");

            var objectExpPara = Expression.Parameter(tObject, string.Empty);
            var valueExpPara = Expression.Parameter(tValue, string.Empty);

            var setterExp = Expression.Assign(Expression.Field(Expression.Unbox(objectExpPara, tSource), fieldInfo), Expression.Convert(valueExpPara, tValue));

            return Expression.Lambda<FieldSetterDelegate<object, TValue>>(setterExp, objectExpPara, valueExpPara).Compile();
        }
#endif

        /// <summary>
        /// Creates a delegate that gets the value of a property. Slower and less safe than generic overload.
        /// <para>Warning: Boxes/Unboxes value types that are the source.</para>
        /// </summary>
        /// <typeparam name="TValue">Property type of the property info.</typeparam>
        /// <param name="propertyInfo">Property info to generate getter delegate from.</param>
        /// <returns>Returns the delegate that invokes a generated property getter for the property.</returns>
        public static PropertyGetterDelegate<object, TValue> GeneratePropertyGetterDelegate<TValue>(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            if (!propertyInfo.CanRead)
                throw new InvalidArgumentException("Cannot read from propertyInfo");

            MethodInfo getterMethodInfo = propertyInfo.GetGetMethod(true);

            if (getterMethodInfo.IsStatic)
                throw new InvalidArgumentException("propertyInfo is static use GenerateStaticPropertyGetterDelegate() instead");

            Type tSource = propertyInfo.DeclaringType;
            Type tValue = typeof(TValue);

            if (tValue != propertyInfo.PropertyType)
                throw new InvalidArgumentException("Type TValue must equal the propertyInfo property type");

            singleTypeArray[0] = tObject;
            string dynamicMethodName = useMeaningfulNames ? propertyInfo.ReflectedType.FullName + ".get_" + propertyInfo.Name : GENERATED_NAME;
            DynamicMethod getterMethod = new DynamicMethod(dynamicMethodName, tValue, singleTypeArray, tSource);

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
            gen.Emit(OpCodes.Ret);

            return (PropertyGetterDelegate<object, TValue>)getterMethod.CreateDelegate(typeof(PropertyGetterDelegate<object, TValue>));
        }

        /// <summary>
        /// Creates a delegate that gets the value of a property. Slower and less safe than generic overload.
        /// <para>Avoids boxing of value type source by using TypedReferences.</para>
        /// </summary>
        /// <typeparam name="TValue">Field type of the field info.</typeparam>
        /// <param name="fieldInfo">Field info to generate getter delegate from.</param>
        /// <returns>Returns the delegate that invokes a generated field getter for the field.</returns>
        public static PropertyGetterDelegateByTypedRef<TValue> GeneratePropertyGetterDelegateByTypedRef<TValue>(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            if (!propertyInfo.CanRead)
                throw new InvalidArgumentException("Cannot read from propertyInfo");

            MethodInfo getterMethodInfo = propertyInfo.GetGetMethod(true);

            if (getterMethodInfo.IsStatic)
                throw new InvalidArgumentException("propertyInfo is static use GenerateStaticPropertyGetterDelegate() instead");

            Type tSource = propertyInfo.DeclaringType;
            Type tValue = typeof(TValue);

            if (tValue != propertyInfo.PropertyType)
                throw new InvalidArgumentException("Type TValue must equal the propertyInfo property type");

            singleTypeArray[0] = tTypedRef;
            string dynamicMethodName = useMeaningfulNames ? propertyInfo.ReflectedType.FullName + ".get_" + propertyInfo.Name : GENERATED_NAME;
            DynamicMethod getterMethod = new DynamicMethod(dynamicMethodName, tValue, singleTypeArray, tSource);

            ILGenerator gen = getterMethod.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Refanyval, tSource);
            gen.Emit(OpCodes.Ldobj, tSource);
            if (getterMethodInfo.IsAbstract || getterMethodInfo.IsVirtual)
                gen.Emit(OpCodes.Callvirt, getterMethodInfo);
            else
                gen.Emit(OpCodes.Call, getterMethodInfo);
            gen.Emit(OpCodes.Ret);

            return (PropertyGetterDelegateByTypedRef<TValue>)getterMethod.CreateDelegate(typeof(PropertyGetterDelegateByTypedRef<TValue>));
        }

        /// <summary>
        /// Creates a delegate that sets the value of a property. Slower and less safe than generic overload.
        /// <para>Warning: Unboxes value types that are the source.</para>
        /// </summary>
        /// <typeparam name="TValue">Property type of the property info.</typeparam>
        /// <param name="propertyInfo">Property info to generate setter delegate from.</param>
        /// <returns>Returns the delegate that invokes a generated property setter for the property.</returns>
        public static PropertySetterDelegate<object, TValue> GeneratePropertySetterDelegate<TValue>(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            if (!propertyInfo.CanWrite)
                throw new InvalidArgumentException("Cannot write to propertyInfo");

            MethodInfo setterMethodInfo = propertyInfo.GetSetMethod(true);

            if (setterMethodInfo.IsStatic)
                throw new InvalidArgumentException("fieldInfo is static use GenerateStaticFieldPropertyDelegate() instead");

            Type tSource = propertyInfo.DeclaringType;
            Type tValue = typeof(TValue);

            if (tValue != propertyInfo.PropertyType)
                throw new InvalidArgumentException("Type TValue must equal the propertyInfo property type");

            doubleTypeArray[0] = tObject;
            doubleTypeArray[1] = tValue;
            string dynamicMethodName = useMeaningfulNames ? propertyInfo.ReflectedType.FullName + ".set_" + propertyInfo.Name : GENERATED_NAME;
            DynamicMethod setterMethod = new DynamicMethod(dynamicMethodName, null, doubleTypeArray, tSource);

            ILGenerator gen = setterMethod.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            if (tSource.IsValueType)
                gen.Emit(OpCodes.Unbox, tSource);
            else if (tSource != tObject)
                gen.Emit(OpCodes.Castclass, tSource);
            gen.Emit(OpCodes.Ldarg_1);
            if (setterMethodInfo.IsAbstract || setterMethodInfo.IsVirtual)
                gen.Emit(OpCodes.Callvirt, setterMethodInfo);
            else
                gen.Emit(OpCodes.Call, setterMethodInfo);
            gen.Emit(OpCodes.Ret);

            return (PropertySetterDelegate<object, TValue>)setterMethod.CreateDelegate(typeof(PropertySetterDelegate<object, TValue>));
        }

#if NET_4_5
        /// <summary>
        /// Creates a delegate that sets the value of a property. Slower than non expression overload.
        /// <para>Warning: Unboxes value types that are the source.</para>
        /// </summary>
        /// <typeparam name="TValue">Property type of the property info.</typeparam>
        /// <param name="propertyInfo">Property info to generate setter delegate from.</param>
        /// <returns>Returns the delegate that invokes a generated property setter for the property.</returns>
        public static PropertySetterDelegate<object, TValue> GeneratePropertySetterDelegateExp<TValue>(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            if (!propertyInfo.CanWrite)
                throw new InvalidArgumentException("Cannot write to propertyInfo");

            MethodInfo setterMethodInfo = propertyInfo.GetSetMethod(true);

            if (setterMethodInfo.IsStatic)
                throw new InvalidArgumentException("fieldInfo is static use GenerateStaticFieldPropertyDelegate() instead");

            Type tSource = propertyInfo.DeclaringType;
            Type tValue = typeof(TValue);

            if (tValue != propertyInfo.PropertyType)
                throw new InvalidArgumentException("Type TValue must equal the propertyInfo property type");

            var objectExpPara = Expression.Parameter(tObject, string.Empty);
            var valueExpPara = Expression.Parameter(tValue, string.Empty);

            var setterExp = Expression.Assign(Expression.Property(Expression.Unbox(objectExpPara, tSource), propertyInfo), Expression.Convert(valueExpPara, tValue));

            return Expression.Lambda<PropertySetterDelegate<object, TValue>>(setterExp, objectExpPara, valueExpPara).Compile();
        }
#endif

        #endregion
    }
}
#endif
