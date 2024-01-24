#if !NETSTANDARD2_1_OR_GREATER && !NET5_0_OR_GREATER

global using RuntimeHelpers = DefaultSerialization.RuntimeHelpers;

using System;
using System.Reflection;

namespace DefaultSerialization
{
    internal static class RuntimeHelpers
    {
#if !NETSTANDARD2_0
        private static readonly Func<Type, object> _initializer;

        static RuntimeHelpers()
        {
            MethodInfo method = typeof(System.Runtime.CompilerServices.RuntimeHelpers).GetRuntimeMethod(nameof(GetUninitializedObject), [typeof(Type)]);

            _initializer = method != null ? (Func<Type, object>)method.CreateDelegate(typeof(Func<Type, object>)) : Activator.CreateInstance;
        }
#endif

        public static object GetUninitializedObject(Type type)
        {
#if NETSTANDARD2_0
            return System.Runtime.Serialization.FormatterServices.GetUninitializedObject(type);
#else
            return _initializer(type);
#endif
        }
    }
}

#endif
