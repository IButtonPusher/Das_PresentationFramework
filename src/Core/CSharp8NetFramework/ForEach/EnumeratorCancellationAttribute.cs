using System;
using System.Threading.Tasks;

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class EnumeratorCancellationAttribute : Attribute
    {
    }
}