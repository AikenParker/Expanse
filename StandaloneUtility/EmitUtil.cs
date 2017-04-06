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
        /// <summary>
        /// Returns a dynamically compiled method that calls a default constructor for a type.
        /// </summary>
        public static Func<TSource> GenerateDefaultConstructor<TSource>()
        {
            Type type = typeof(TSource);
            string methodName = type.ReflectedType.FullName + ".ctor";
            DynamicMethod constructorMethod = new DynamicMethod(methodName, type, new Type[0], true);
            ILGenerator gen = constructorMethod.GetILGenerator();

            gen.Emit(OpCodes.Newobj, type.GetConstructor(new Type[0]));
            gen.Emit(OpCodes.Ret);

            return (Func<TSource>)constructorMethod.CreateDelegate(typeof(Func<TSource>));
        }

        /// <summary>
        /// Returns a dynamically compiled method that gets the value of a field for a type.
        /// </summary>
        public static Func<TSource, TReturn> GenerateGetter<TSource, TReturn>(FieldInfo field)
        {
            string methodName = field.ReflectedType.FullName + ".get_" + field.Name;
            DynamicMethod getterMethod = new DynamicMethod(methodName, typeof(TReturn), new Type[1] { typeof(TSource) }, true);
            ILGenerator gen = getterMethod.GetILGenerator();

            if (field.IsStatic)
            {
                gen.Emit(OpCodes.Ldsfld, field);
            }
            else
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldfld, field);
            }

            gen.Emit(OpCodes.Ret);
            return (Func<TSource, TReturn>)getterMethod.CreateDelegate(typeof(Func<TSource, TReturn>));
        }

        /// <summary>
        /// Returns a dynamically compiled method that sets the value of a field for a type.
        /// </summary>
        public static Action<TSource, TValue> GenerateSetter<TSource, TValue>(FieldInfo field)
        {
            string methodName = field.ReflectedType.FullName + ".set_" + field.Name;
            DynamicMethod setterMethod = new DynamicMethod(methodName, null, new Type[2] { typeof(TSource), typeof(TValue) }, true);
            ILGenerator gen = setterMethod.GetILGenerator();

            if (field.IsStatic)
            {
                gen.Emit(OpCodes.Ldarg_1);
                gen.Emit(OpCodes.Stsfld, field);
            }
            else
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldarg_1);
                gen.Emit(OpCodes.Stfld, field);
            }

            gen.Emit(OpCodes.Ret);
            return (Action<TSource, TValue>)setterMethod.CreateDelegate(typeof(Action<TSource, TValue>));
        }
    }
}
