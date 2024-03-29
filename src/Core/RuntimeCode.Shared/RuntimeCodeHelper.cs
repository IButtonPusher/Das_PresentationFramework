﻿using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
// ReSharper disable UnusedMember.Global

namespace RuntimeCode.Shared;

public static class RuntimeCodeHelper
{
    public static OpCode? TryGetLdConst(this Int32 value)
    {
       TryGetLdConstInt32(value, out var opCode);
       return opCode;
    }

    public static Boolean TryGetLdConstInt32(this Int32 value,
                                             out OpCode? opCode)
    {
       switch (value)
       {
          case 0:
             opCode = OpCodes.Ldc_I4_0;
             return true;

          case 1:
             opCode = OpCodes.Ldc_I4_1;
             return true;

          case 2:
             opCode = OpCodes.Ldc_I4_2;
             return true;

          case 3:
             opCode = OpCodes.Ldc_I4_3;
             return true;

          case 4:
             opCode = OpCodes.Ldc_I4_4;
             return true;

          case 5:
             opCode = OpCodes.Ldc_I4_5;
             return true;

          case 6:
             opCode = OpCodes.Ldc_I4_6;
             return true;

          case 7:
             opCode = OpCodes.Ldc_I4_7;
             return true;

          case 8:
             opCode = OpCodes.Ldc_I4_8;
             return true;

          default:
             opCode = default;
             return false;
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

    public static void EmitLdArg(this ILGenerator il,
                                 Int32 value)
    {
        var opCode = TryGetLdArg(value);
        if (opCode != null)
            il.Emit(opCode.Value);
        else if (value <= 127)
            il.Emit(OpCodes.Ldarg_S, value);
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
    ///     1. ldarg_0
    ///     2. ldfld(field)
    /// </summary>
    public static void LoadThisField(this ILGenerator il,
                                     FieldInfo field)
    {
       if (field.IsStatic)
          il.Emit(OpCodes.Ldsfld, field.MetadataToken);
       else
       {
          il.Emit(OpCodes.Ldarg_0);
          il.Emit(OpCodes.Ldfld, field.MetadataToken);
       }
    }

    public static void StoreLocal<T>(this ILGenerator il,
                                     FieldDefinition<T> field,
                                     LocalVariable<T> local)
    {
        il.LoadThisField(field);
        il.StoreLocal(local);
    }

    public static void StoreConstToField<T>(this ILGenerator il,
                                            T constValue,
                                            FieldDefinition<T> field)
        where T : IConvertible
    {
        il.Emit(OpCodes.Ldarg_0);
        il.PushConstant(constValue);
        il.Emit(OpCodes.Stfld, field);
    }

    public static void StoreConstToLocal<T>(this ILGenerator il,
                                            T constValue,
                                            LocalVariable<T> localVar)
        where T : IConvertible
    {
        il.PushConstant(constValue);
        il.Emit(OpCodes.Stloc, localVar);
    }

    public static void StoreConstToLocal(this ILGenerator il,
                                            Double value,
                                            LocalVariable<Double> localVar)
    {
       il.PushConstant(value);
       il.Emit(OpCodes.Stloc, localVar);
    }


    public static void StoreLocal(this ILGenerator il,
                                  FieldInfo field,
                                  LocalBuilder local)
    {
        il.LoadThisField(field);
        il.StoreLocal(local);
    }

    public static void StoreLocal(this ILGenerator il,
                                  LocalBuilder local) => il.Emit(OpCodes.Stloc, local);

    public static void StoreLocal(this ILGenerator il,
                                  Int32 localIndex) => il.Emit(OpCodes.Stloc, localIndex);

    public static void LoadLocal(this ILGenerator il,
                                 LocalBuilder local) => il.Emit(OpCodes.Ldloc, local);

    public static void LoadLocal(this ILGenerator il,
                                 Int32 localIndex) => il.Emit(OpCodes.Ldloc, localIndex);

    /// <summary>
    ///     ret
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

    public static OpCode GetStElem(Type arrayType,
                                   out Type germane)
    {
        germane = (arrayType.IsArray ? arrayType.GetElementType() : arrayType)!;
        var code = Type.GetTypeCode(germane);

        switch (code)
        {
            case TypeCode.Boolean:
                return OpCodes.Stelem_I4;


            case TypeCode.SByte:
            case TypeCode.Byte:
            case TypeCode.Char:
                return OpCodes.Stelem_I1;

            case TypeCode.Int16:
            case TypeCode.UInt16:
                return OpCodes.Stelem_I2;

            case TypeCode.Int32:
            case TypeCode.UInt32:
                return OpCodes.Stelem_I4;


            case TypeCode.Int64:
                return OpCodes.Stelem_I8;

            case TypeCode.UInt64:
                return OpCodes.Stelem_I8;

            case TypeCode.Single:
                return OpCodes.Stelem_R4;

            case TypeCode.Double:
                return OpCodes.Stelem_R8;

            case TypeCode.Object:
                if (germane.IsValueType)
                    return OpCodes.Stelem;
                return OpCodes.Stelem_Ref;

            case TypeCode.Empty:

            case TypeCode.DBNull:
            case TypeCode.Decimal:
            case TypeCode.DateTime:
            case TypeCode.String:
            default:
                return OpCodes.Stelem_Ref;
        }
    }

    public static OpCode GetLdElem(Type arrayType,
                                   out Type germane)
    {
        germane = (arrayType.IsArray ? arrayType.GetElementType() : arrayType)!;
        var code = Type.GetTypeCode(germane);

        switch (code)
        {
            case TypeCode.Boolean:
                return OpCodes.Ldelem_I4;

            case TypeCode.Char:
                return OpCodes.Ldelem_U1;

            case TypeCode.SByte:
                return OpCodes.Ldelem_I1;

            case TypeCode.Byte:
                return OpCodes.Ldelem_U1;

            case TypeCode.Int16:
                return OpCodes.Ldelem_I2;

            case TypeCode.UInt16:
                return OpCodes.Ldelem_U2;

            case TypeCode.Int32:
                return OpCodes.Ldelem_I4;

            case TypeCode.UInt32:
                return OpCodes.Ldelem_U4;

            case TypeCode.Int64:
                return OpCodes.Ldelem_I8;

            case TypeCode.UInt64:
                return OpCodes.Ldelem_I8;

            case TypeCode.Single:
                return OpCodes.Ldelem_R4;

            case TypeCode.Double:
                return OpCodes.Ldelem_R8;

            case TypeCode.Object:
                if (germane.IsValueType)
                    return OpCodes.Ldelem;
                return OpCodes.Ldelem_Ref;

            case TypeCode.Empty:

            case TypeCode.DBNull:
            case TypeCode.Decimal:
            case TypeCode.DateTime:
            case TypeCode.String:
            default:
                return OpCodes.Ldelem_Ref;
        }
    }

    public static void PushConstant(this ILGenerator il,
                                    Int16 value)
    {
       il.Emit(OpCodes.Ldc_I4, (Int32)value);
    }

    public static void PushConstant(this ILGenerator il,
                                    Int32 value)
    {
        var opCode = TryGetLdConst(value);
        if (opCode != null)
            il.Emit(opCode.Value);
        //else if (value >= 0 && value <= 127)
        //    il.Emit(OpCodes.Ldc_I4_S, value);
        else
            il.Emit(OpCodes.Ldc_I4, value);
    }

    public static void PushConstant(this ILGenerator il,
                                    Double value)
    {
       il.Emit(OpCodes.Ldc_R8, value); //Convert.ToDouble(value));
    }

    public static void PushConstant(this ILGenerator il,
                                    UInt64 value)
    {
       EmitConstUInt64(il, value); //Convert.ToDouble(value));
    }

    public static void PushConstant(this ILGenerator il,
                                    Boolean value)
    {
       il.Emit(value ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
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
               var bVal = value.ToBoolean(default); //Convert.ToBoolean(value);
                il.Emit(bVal ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
                return;

            case TypeCode.Char:
               il.Emit(OpCodes.Ldc_I4, value.ToInt32(default)); //Convert.ToInt32(value));
                return;

            case TypeCode.SByte:
                il.Emit(OpCodes.Ldc_I4, value.ToInt32(default));//Convert.ToInt32(value));
                return;


            case TypeCode.Byte:
                il.Emit(OpCodes.Ldc_I4, value.ToInt32(default));//Convert.ToInt32(value));
                return;

            case TypeCode.Int16:
                il.Emit(OpCodes.Ldc_I4, value.ToInt32(default));//Convert.ToInt32(value));
                return;

            case TypeCode.UInt16:
                break;

            case TypeCode.Int32:

               PushConstant(il, value.ToInt32(default)); //Convert.ToInt32(value));
                return;


            case TypeCode.UInt32:
                il.Emit(OpCodes.Ldc_I4, value.ToUInt32(default));//Convert.ToUInt32(value));
                return;

            case TypeCode.Int64:
                il.Emit(OpCodes.Ldc_I8, value.ToInt64(default));//Convert.ToInt64(value));
                return;

            case TypeCode.UInt64:

               EmitConstUInt64(il, value.ToUInt64(default)); //Convert.ToUInt64(value));
                return;

            case TypeCode.Single:
                il.Emit(OpCodes.Ldc_R4, value.ToSingle(default)); //Convert.ToSingle(value));
                return;

            case TypeCode.Double:
                il.Emit(OpCodes.Ldc_R8, value.ToDouble(default)); //Convert.ToDouble(value));
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
