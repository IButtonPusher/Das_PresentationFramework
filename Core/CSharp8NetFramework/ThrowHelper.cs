using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace System
{
    internal enum ExceptionArgument
    {
        task,
        source,
        state
    }

    internal static class ThrowHelper
    {
        internal static void ThrowArgumentNullException(ExceptionArgument argument)
        {
            throw GetArgumentNullException(argument);
        }

        internal static void ThrowArgumentOutOfRangeException(ExceptionArgument argument)
        {
            throw GetArgumentOutOfRangeException(argument);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static String GetArgumentName(ExceptionArgument argument)
        {
            return argument.ToString();
        }

        private static ArgumentNullException GetArgumentNullException(
            ExceptionArgument argument)
        {
            return new ArgumentNullException(GetArgumentName(argument));
        }

        private static ArgumentOutOfRangeException GetArgumentOutOfRangeException(
            ExceptionArgument argument)
        {
            return new ArgumentOutOfRangeException(GetArgumentName(argument));
        }
    }
}