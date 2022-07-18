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

    /// <summary>
    /// ret
    /// </summary>
    public static void Return(this ILGenerator il) => il.Emit(OpCodes.Ret);

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

    public static void PushConstant<TConstValue>(this ILGenerator il,
                                                   TConstValue value)
         where TConstValue : IConvertible
      {
         var code = Type.GetTypeCode(value.GetType());

         switch (code)
         {
            case TypeCode.Empty:
            case TypeCode.Object:
            case TypeCode.DBNull:
               break;


            case TypeCode.Boolean:
               var bVal = Convert.ToBoolean(value);
               il.Emit(bVal ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
               return;

            case TypeCode.Char:
               il.Emit(OpCodes.Ldc_I4, Convert.ToInt32(value));
               return;

            case TypeCode.SByte:
               il.Emit(OpCodes.Ldc_I4, Convert.ToInt32(value));
               return;


            case TypeCode.Byte:
               il.Emit(OpCodes.Ldc_I4, Convert.ToInt32(value));
               return;

            case TypeCode.Int16:
               il.Emit(OpCodes.Ldc_I4, Convert.ToInt32(value));
               return;

            case TypeCode.UInt16:
               break;

            case TypeCode.Int32:
               il.Emit(OpCodes.Ldc_I4, Convert.ToInt32(value));
               return;


            case TypeCode.UInt32:
               break;

            case TypeCode.Int64:
               il.Emit(OpCodes.Ldc_I8, Convert.ToInt64(value));
               return;

            case TypeCode.UInt64:
                EmitConstUInt64(il, Convert.ToUInt64(value));
                return;

            case TypeCode.Single:
               il.Emit(OpCodes.Ldc_R4, Convert.ToSingle(value));
               return;

            case TypeCode.Double:
               il.Emit(OpCodes.Ldc_R8, Convert.ToDouble(value));
               return;

            case TypeCode.Decimal:
               break;
            case TypeCode.DateTime:
               break;

            case TypeCode.String:
               il.Emit(OpCodes.Ldstr, value?.ToString() ?? String.Empty);
               return;

            default:
               throw new ArgumentOutOfRangeException();
         }

         throw new NotImplementedException();
      }
}