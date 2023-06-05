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
                                                   FieldDefinition<T> field,
                                                   Action<ILGenerator> loadObjForField)
    {
       var loco = il.DeclareLocal(typeof(T));

       if (field.IsStatic)
       {
          il.Emit(OpCodes.Ldsfld, field);
       }
       else
       {
          loadObjForField(il);
          il.Emit(OpCodes.Ldfld, field);
       }

       il.Emit(OpCodes.Stloc, loco);
       
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
