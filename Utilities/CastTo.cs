#region AOT_ONLY_DEFINE
#define AOT_ONLY
#if (UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID) && !ENABLE_IL2CPP
#undef AOT_ONLY
#endif
#endregion

using System;
using System.Linq.Expressions;

namespace Expanse.Utilities
{
    /// <summary>
    /// Class to cast to type <see cref="TTarget"/>
    /// </summary>
    /// <remarks>
    /// Avoid for reference types.
    /// </remarks>
    /// <see cref="https://stackoverflow.com/questions/1189144/c-sharp-non-boxing-conversion-of-generic-enum-to-int"/>
    /// <typeparam name="TTarget">Target type</typeparam>
    public static class CastTo<TTarget>
    {
        /// <summary>
        /// Casts <see cref="TSource"/> to <see cref="TTarget"/>.
        /// This does not cause boxing for value types.
        /// Useful in generic methods.
        /// </summary>

        /// <typeparam name="TSource">Source type to cast from. Usually a generic type.</typeparam>
        /// <param name="source">Source object to cast.</param>
        /// <param name="emitCaster">If true a casting delegate will be used and cached.</param>
        /// <returns>Returns a casted instance of type <see cref="TTarget"/>.</returns>
        public static TTarget From<TSource>(TSource source, bool emitCaster = false)
        {
#if !AOT_ONLY
            if (emitCaster)
                return Cache<TSource>.caster(source);
            else
#endif
            {
                return (TTarget)(object)source;
            }
        }

#if !AOT_ONLY
        /// <summary>
        /// Responsible for caching the casting delegate from <see cref="TSource"/> to <see cref="TTarget"/>.
        /// </summary>
        /// <typeparam name="TSource">Source type.</typeparam>
        private static class Cache<TSource>
        {
            public static readonly Func<TSource, TTarget> caster = Get();

            private static Func<TSource, TTarget> Get()
            {
                var parameter = Expression.Parameter(typeof(TSource), string.Empty);
                var conversion = Expression.Convert(parameter, typeof(TTarget));

                return Expression.Lambda<Func<TSource, TTarget>>(conversion, parameter).Compile();
            }
        }
#endif
    }
}
