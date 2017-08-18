#region AOT_ONLY_DEFINE
#define AOT_ONLY
#if (UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID) && !ENABLE_IL2CPP
#undef AOT_ONLY
#endif
#endregion

#if !AOT_ONLY

using System;

namespace Expanse.Utilities
{
    /// <summary>
    /// Helper utility class that caches emitted constructor and casting delegates.
    /// </summary>
    /// <typeparam name="TTarget">Target type.</typeparam>
    public static class EmitHelper<TTarget>
    {
        private static Type targetType;

        static EmitHelper()
        {
            targetType = typeof(TTarget);
        }

        /// <summary>
        /// Creates a new instance of <see cref="TTarget"/> using an emitted default constructor delegate.
        /// </summary>
        /// <typeparam name="TSource">Non-abstract type of <see cref="TTarget"/> to create an instance of.</typeparam>
        /// <returns>Returns an new instance of <see cref="TSource"/></returns>
        public static TSource CreateInstance<TSource>()
            where TSource : TTarget, new()
        {
            return ConstructorCache<TSource>.constructor();
        }

        /// <summary>
        /// Casts <see cref="TSource"/> to <see cref="TTarget"/>.
        /// This does not cause boxing for value types making it useful in generic methods.
        /// </summary>
        /// <see cref="https://stackoverflow.com/questions/1189144/c-sharp-non-boxing-conversion-of-generic-enum-to-int"/>
        /// <typeparam name="TSource">Source type to cast from. Usually a generic type.</typeparam>
        /// <param name="source">Source object to cast.</param>
        /// <returns>Returns a casted instance of type <see cref="TTarget"/>.</returns>
        public static TTarget CastFrom<TSource>(TSource source)
        {
            return CastCache<TSource>.caster(source);
        }

        /// <summary>
        /// Responsible for caching and emitting a default constructor delegate for type <see cref="TSource"/>.
        /// </summary>
        /// <typeparam name="TSource">Type to create a default constructor of.</typeparam>
        private static class ConstructorCache<TSource> where TSource : new()
        {
            public static readonly EmitUtil.DefaultConstructorDelegate<TSource> constructor = GetConstructor();

            private static EmitUtil.DefaultConstructorDelegate<TSource> GetConstructor()
            {
                return EmitUtil.GenerateDefaultConstructorDelegate<TSource>();
            }
        }

        /// <summary>
        /// Responsible for caching the casting delegate from <see cref="TSource"/> to <see cref="TTarget"/>.
        /// </summary>
        /// <typeparam name="TSource">Source type.</typeparam>
        private static class CastCache<TSource>
        {
            public static readonly EmitUtil.TypeCastDelegate<TSource, TTarget> caster = GetCaster();

            private static EmitUtil.TypeCastDelegate<TSource, TTarget> GetCaster()
            {
                return EmitUtil.GenerateTypeCastDelegate<TSource, TTarget>();
            }
        }
    }
}
#endif
