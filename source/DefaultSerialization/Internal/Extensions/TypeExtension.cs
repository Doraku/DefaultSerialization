﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace System
{
    internal static class TypeExtension
    {
        public static bool IsUnmanaged(this Type type) => type.GetTypeInfo().IsUnmanaged();

        public static bool IsUnmanaged(this TypeInfo typeInfo) => typeInfo.IsEnum || (typeInfo.IsValueType && (typeInfo.IsPrimitive || typeInfo.DeclaredFields.Where(f => !f.IsStatic).All(f => f.FieldType.IsUnmanaged())));

        public static bool IsList(this Type type) => type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(List<>);

        public static bool IsDictionary(this Type type) => type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>);

        public static bool IsEnum(this Type type) => type.GetTypeInfo().IsEnum;

        public static bool IsAbstract(this Type type) => type.GetTypeInfo().IsAbstract;
    }
}
