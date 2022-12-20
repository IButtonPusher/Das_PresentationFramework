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

    public static LocalVariable<T> DeclareLocal<T>(this ILGenerator il,
                                                   T defaultValue)
       where T : IConvertible
    {
       var localVar = DeclareLocal<T>(il);

       il.PushConstant(defaultValue);
       il.StoreLocal(localVar);

       return localVar;
    }
}
