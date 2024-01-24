using System.Runtime.CompilerServices;

namespace DefaultSerialization.Internal
{
    internal static class ObjectInitializer<T>
    {
        public static T Create() => (T)RuntimeHelpers.GetUninitializedObject(typeof(T));
    }
}
