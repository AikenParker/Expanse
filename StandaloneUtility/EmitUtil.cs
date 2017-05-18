#define AOT_ONLY
#if (UNITY_EDITOR || UNITY_STANDALONE) && !ENABLE_IL2CPP
#undef AOT_ONLY
#endif

#if !AOT_ONLY
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Expanse
{
    /// <summary>
    /// A collection of Reflection.Emit related utility functionality.
    /// </summary>
    public static class EmitUtil
    {
        private static bool useMeaningfulNames = true;
        private const string GENERATED_NAME = "GEN";

        public static bool UseMeaningfulNames
        {
            get { return useMeaningfulNames; }
            set { useMeaningfulNames = value; }
        }

        private static Module dynamicModule;
        private static Module DynamicModule
        {
            get
            {
                dynamicModule = dynamicModule ?? GenerateDynamicModule();
                return dynamicModule;
            }
        }

        private static Type[] emptyTypeArray = new Type[0];
        private static Type[] singleTypeArray = new Type[1];
        private static Type[] doubleTypeArray = new Type[2];

        private static Module GenerateDynamicModule()
        {
            AssemblyName dynamicAssemblyName = new AssemblyName("DynamicEmitUtilAssembly");
            AssemblyBuilder dynamicAssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(dynamicAssemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder dynamicModuleBuilder = dynamicAssemblyBuilder.DefineDynamicModule("DynamicEmitUtilModule");
            return dynamicModuleBuilder.Assembly.GetModule("DynamicEmitUtilModule");
        }

        /// <summary>
        /// Generates the dynamic module if there is not one already.
        /// </summary>
        public static void Prewarm()
        {
            dynamicModule = dynamicModule ?? GenerateDynamicModule();
        }

        /// <summary>
        /// Generates a delegate that constructs an instance of type TSource using the default constructor.
        /// </summary>
        /// <typeparam name="TSource">Declaring type that the constructor belongs to.</typeparam>
        public static Func<TSource> GenerateDefaultConstructorDelegate<TSource>()
        {
            Type tSource = typeof(TSource);
            ConstructorInfo defaultConstructorInfo = tSource.GetConstructor(emptyTypeArray);

            if (defaultConstructorInfo == null)
                throw new InvalidTypeException("TSource must have a default constructor");

            string dynamicMethodName = useMeaningfulNames ? tSource.FullName + ".ctor" : GENERATED_NAME;
            DynamicMethod constructorMethod = new DynamicMethod(dynamicMethodName, tSource, emptyTypeArray, DynamicModule);
            ILGenerator gen = constructorMethod.GetILGenerator();

            gen.Emit(OpCodes.Newobj, defaultConstructorInfo);
            gen.Emit(OpCodes.Ret);

            return ReflectionUtil.CreateMethodInvokerDelegate<Func<TSource>>(constructorMethod);
        }

        /// <summary>
        /// Creates a delegate that gets the value of a static field.
        /// </summary>
        /// <typeparam name="TValue">Field type of the field info.</typeparam>
        public static Func<TValue> GenerateStaticFieldGetterDelegate<TValue>(FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
                throw new ArgumentNullException("field");

            if (!fieldInfo.IsStatic)
                throw new InvalidArgumentException("fieldInfo is not static use GenerateFieldGetterDelegate() instead");

            if (!fieldInfo.IsPublic)
                throw new InvalidArgumentException("fieldInfo must be public");

            Type tValue = typeof(TValue);

            if (tValue != fieldInfo.FieldType)
                throw new InvalidArgumentException("Type TValue must equal the fieldInfo field type");

            string dynamicMethodName = useMeaningfulNames ? fieldInfo.ReflectedType.FullName + ".get_" + fieldInfo.Name : GENERATED_NAME;
            DynamicMethod getterMethod = new DynamicMethod(dynamicMethodName, tValue, emptyTypeArray, DynamicModule);

            ILGenerator gen = getterMethod.GetILGenerator();
            gen.Emit(OpCodes.Ldsfld, fieldInfo);
            gen.Emit(OpCodes.Ret);

            return ReflectionUtil.CreateMethodInvokerDelegate<Func<TValue>>(getterMethod);
        }

        /// <summary>
        /// Creates a delegate that gets the value of a field.
        /// </summary>
        /// <typeparam name="TSource">Declaring type that the field belongs to.</typeparam>
        /// <typeparam name="TValue">Field type of the field info.</typeparam>
        public static Func<TSource, TValue> GenerateFieldGetterDelegate<TSource, TValue>(FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
                throw new ArgumentNullException("field");

            if (fieldInfo.IsStatic)
                throw new InvalidArgumentException("fieldInfo is static use GenerateStaticFieldGetterDelegate() instead");

            if (!fieldInfo.IsPublic)
                throw new InvalidArgumentException("fieldInfo must be public");

            Type tSource = typeof(TSource);
            Type tValue = typeof(TValue);

            if (tSource != fieldInfo.DeclaringType)
                throw new InvalidArgumentException("Type TSource must equal the fieldInfo declaring type");

            if (tValue != fieldInfo.FieldType)
                throw new InvalidArgumentException("Type TValue must equal the fieldInfo field type");

            singleTypeArray[0] = tSource;
            string dynamicMethodName = useMeaningfulNames ? fieldInfo.ReflectedType.FullName + ".get_" + fieldInfo.Name : GENERATED_NAME;
            DynamicMethod getterMethod = new DynamicMethod(dynamicMethodName, tValue, singleTypeArray, DynamicModule);

            ILGenerator gen = getterMethod.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld, fieldInfo);
            gen.Emit(OpCodes.Ret);

            return ReflectionUtil.CreateMethodInvokerDelegate<Func<TSource, TValue>>(getterMethod);
        }

        /// <summary>
        /// Creates a delegate that sets the value of a static field.
        /// </summary>
        /// <typeparam name="TValue">Field type of the field info.</typeparam>
        public static Action<TValue> GenerateStaticFieldSetterDelegate<TValue>(FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
                throw new ArgumentNullException("field");

            if (!fieldInfo.IsStatic)
                throw new InvalidArgumentException("fieldInfo is not static use GenerateFieldSetterDelegate() instead");

            if (!fieldInfo.IsPublic)
                throw new InvalidArgumentException("fieldInfo must be public");

            Type tValue = typeof(TValue);

            if (tValue != fieldInfo.FieldType)
                throw new InvalidArgumentException("Type TValue must equal the fieldInfo field type");

            singleTypeArray[0] = tValue;
            string dynamicMethodName = useMeaningfulNames ? fieldInfo.ReflectedType.FullName + ".set_" + fieldInfo.Name : GENERATED_NAME;
            DynamicMethod setterMethod = new DynamicMethod(dynamicMethodName, null, singleTypeArray, DynamicModule);

            ILGenerator gen = setterMethod.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Stsfld, fieldInfo);
            gen.Emit(OpCodes.Ret);

            return ReflectionUtil.CreateMethodInvokerDelegate<Action<TValue>>(setterMethod);
        }

        /// <summary>
        /// Creates a delegate that sets the value of a field.
        /// </summary>
        /// <typeparam name="TSource">Declaring type that the field belongs to.</typeparam>
        /// <typeparam name="TValue">Field type of the field info.</typeparam>
        public static Action<TSource, TValue> GenerateFieldSetterDelegate<TSource, TValue>(FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
                throw new ArgumentNullException("field");

            if (fieldInfo.IsStatic)
                throw new InvalidArgumentException("fieldInfo is static use GenerateStaticFieldSetterDelegate() instead");

            if (!fieldInfo.IsPublic)
                throw new InvalidArgumentException("fieldInfo must be public");

            Type tSource = typeof(TSource);
            Type tValue = typeof(TValue);

            if (tSource != fieldInfo.DeclaringType)
                throw new InvalidArgumentException("Type TSource must equal the fieldInfo declaring type");

            if (tValue != fieldInfo.FieldType)
                throw new InvalidArgumentException("Type TValue must equal the fieldInfo field type");

            doubleTypeArray[0] = tSource;
            doubleTypeArray[1] = tValue;
            string dynamicMethodName = useMeaningfulNames ? fieldInfo.ReflectedType.FullName + ".set_" + fieldInfo.Name : GENERATED_NAME;
            DynamicMethod setterMethod = new DynamicMethod(dynamicMethodName, null, doubleTypeArray, DynamicModule);

            ILGenerator gen = setterMethod.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Stfld, fieldInfo);
            gen.Emit(OpCodes.Ret);

            return ReflectionUtil.CreateMethodInvokerDelegate<Action<TSource, TValue>>(setterMethod);
        }
    }
}
#endif
