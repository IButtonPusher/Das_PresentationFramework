using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace RuntimeCode.Shared;

public static class RuntimeCodeHelper
{
    public static OpCode? TryGetLdConst(this Int32 value)
    {
        switch (value)
        {
            case 0:
                return OpCodes.Ldc_I4_0;

            case 1:
                return OpCodes.Ldc_I4_1;


            case 2:
                return OpCodes.Ldc_I4_2;

            case 3:
                return OpCodes.Ldc_I4_3;

            case 4:
                return OpCodes.Ldc_I4_4;

            case 5:
                return OpCodes.Ldc_I4_5;

            case 6:
                return OpCodes.Ldc_I4_6;

            case 7:
                return OpCodes.Ldc_I4_7;

            case 8:
                return OpCodes.Ldc_I4_8;

            default:
                return default;
        }
    }

    public static OpCode? TryGetLdArg(this Int32 value)
    {
        switch (value)
        {
            case 0:
                return OpCodes.Ldarg_0;

            case 1:
                return OpCodes.Ldarg_1;

            case 2:
                return OpCodes.Ldarg_2;

            case 3:
                return OpCodes.Ldarg_3;

            default:
                return default;
        }
    }

    public static void EmitConstInt32(this ILGenerator il,
                                      Int32 value)
    {
        var opCode = TryGetLdConst(value);
        if (opCode != null)
            il.Emit(opCode.Value);
        else
            il.Emit(OpCodes.Ldc_I4, value);
    }

        
    public static void EmitLdArg(this ILGenerator il,
                                 Int32 value)
    {
        var opCode = TryGetLdArg(value);
        if (opCode != null)
            il.Emit(opCode.Value);
        else
            il.Emit(OpCodes.Ldarg, value);
    }

    public static void EmitConstUInt64(this ILGenerator il,
                                       UInt64 value)
    {
        var uKey = unchecked((Int64)value);
        il.Emit(OpCodes.Ldc_I8, uKey);
    }

    /// <summary>
    /// 1. ldarg_0
    /// 2. ldfld(field)
    /// </summary>
    public static void LoadThisField(this ILGenerator il,
                                     FieldInfo field)
    {
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldfld, field);
    }

    public static void LoadField(this ILGenerator il,
                                 FieldInfo field)
    {
        il.Emit(field.IsStatic
            ? OpCodes.Ldsfld
            : OpCodes.Ldfld, field);
    }

    

    public static Type[] GetParameterTypes(MethodBase method)
    {
        var prms = method.GetParameters();
        return GetParameterTypes(prms);
    }

    public static Type[] GetParameterTypes(ParameterInfo[] prms)
    {
        if (prms.Length == 0)
            return Type.EmptyTypes;

        var res = new Type[prms.Length];
        for (var c = 0; c < res.Length; c++)
        {
            res[c] = prms[c].ParameterType;
        }

        return res;
    }
}