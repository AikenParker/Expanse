#if (UNITY_EDITOR || UNITY_STANDALONE) && !ENABLE_IL2CPP
#define EMIT_ENABLED
#endif

#if EMIT_ENABLED
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

        public static void Prewarm()
        {
            dynamicModule = dynamicModule ?? GenerateDynamicModule();
        }

        private static Module GenerateDynamicModule()
        {
            AssemblyName dynamicAssemblyName = new AssemblyName("DynamicEmitUtilAssembly");
            AssemblyBuilder dynamicAssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(dynamicAssemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder dynamicModuleBuilder = dynamicAssemblyBuilder.DefineDynamicModule("DynamicEmitUtilModule");
            return dynamicModuleBuilder.Assembly.GetModule("DynamicEmitUtilModule");
        }

        /// <summary>
        /// Returns a dynamically compiled method that calls a default constructor for a type.
        /// </summary>
        public static Func<TSource> GenerateDefaultConstructor<TSource>()
        {
            Type type = typeof(TSource);
            DynamicMethod constructorMethod = new DynamicMethod(useMeaningfulNames ? type.FullName + ".ctor" : GENERATED_NAME, type, emptyTypeArray, DynamicModule);
            ILGenerator gen = constructorMethod.GetILGenerator();

            gen.Emit(OpCodes.Newobj, type.GetConstructor(emptyTypeArray));
            gen.Emit(OpCodes.Ret);

            return (Func<TSource>)constructorMethod.CreateDelegate(typeof(Func<TSource>));
        }

        /// <summary>
        /// Returns a dynamically compiled method that gets the value of a field for a type.
        /// </summary>
        public static Func<TSource, TReturn> GenerateGetter<TSource, TReturn>(FieldInfo field)
        {
            singleTypeArray[0] = typeof(TSource);
            DynamicMethod getterMethod = new DynamicMethod(useMeaningfulNames ? field.ReflectedType.FullName + ".get_" + field.Name : GENERATED_NAME, typeof(TReturn), singleTypeArray, DynamicModule);

            ILGenerator gen = getterMethod.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld, field);
            gen.Emit(OpCodes.Ret);

            return (Func<TSource, TReturn>)getterMethod.CreateDelegate(typeof(Func<TSource, TReturn>));
        }

        /// <summary>
        /// Returns a dynamically compiled method that sets the value of a field for a type.
        /// </summary>
        public static Action<TSource, TValue> GenerateSetter<TSource, TValue>(FieldInfo field)
        {
            doubleTypeArray[0] = typeof(TSource);
            doubleTypeArray[1] = typeof(TValue);
            DynamicMethod setterMethod = new DynamicMethod(useMeaningfulNames ? field.ReflectedType.FullName + ".set_" + field.Name : GENERATED_NAME, null, doubleTypeArray, DynamicModule);

            ILGenerator gen = setterMethod.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Stfld, field);
            gen.Emit(OpCodes.Ret);

            return (Action<TSource, TValue>)setterMethod.CreateDelegate(typeof(Action<TSource, TValue>));
        }
    }
}
#endif
