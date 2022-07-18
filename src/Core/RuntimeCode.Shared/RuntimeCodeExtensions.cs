using System;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace RuntimeCode.Shared;

public static class RuntimeCodeExtensions
{
    public static LocalVariable<T> DeclareLocal<T>(this ILGenerator il)
    {
        var loco = il.DeclareLocal(typeof(T));
        return new LocalVariable<T>(loco);
    }
}
