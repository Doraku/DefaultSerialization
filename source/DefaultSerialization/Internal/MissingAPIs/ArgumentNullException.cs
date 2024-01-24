#if !NET5_0_OR_GREATER

global using ArgumentNullException = DefaultSerialization.ArgumentNullException;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace DefaultSerialization
{
    internal static class ArgumentNullException
    {
        public static void ThrowIfNull([NotNull] object? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        {
            if (argument is null)
            {
                throw new System.ArgumentNullException(paramName);
            }
        }
    }
}

#endif
