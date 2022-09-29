using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace RuntimeCode.Shared;

public abstract class BuilderBase<T>
{
    public BuilderBase(ILGenerator il,
                       T member/*,
                       ITypeManipulator types*/)
    : this(il, member, GetPushValueAction(member)/*, types*/)
    {

    }

    public BuilderBase(ILGenerator il,
                       T member,
                       Action<ILGenerator, T> pushValueToStack/*,
                       ITypeManipulator types*/)
    {
        _il = il;
        _member = member;
        _pushValueToStack = pushValueToStack;
        //_types = types;

        switch (member)
        {
            case PropertyInfo p:
                _memberType = p.PropertyType;
                break;

            case FieldInfo f:
                _memberType = f.FieldType;
                break;

            case MethodInfo m:
                _memberType = m.ReturnType;
                break;

            case LocalBuilder l:
                _memberType = l.LocalType;
                break;

            case LocalVariable lvar:
                _memberType = lvar.LocalType;
                break;

            case ArgAccessor arga:
                _memberType = arga.ArgType;
                break;

            default:
                throw new ArgumentOutOfRangeException($"{member} is not of a valid type");
        }

    }

    public static Action<ILGenerator, T> GetPushValueAction(T member)
    {
        switch (member)
        {
            case PropertyInfo _ when _pushProperty is Action<ILGenerator, T> good:
                return good;

            case FieldInfo when _pushField is Action<ILGenerator, T> good:
                return good;

            case MethodInfo when _pushMethod is Action<ILGenerator, T> good:
                return good;
                

            case LocalBuilder when _pushLocal is Action<ILGenerator, T> good:
                return good;

            case LocalVariable when _pushLocal2 is Action<ILGenerator, T> good:
                return good;

            case ArgAccessor when _pushArg is Action<ILGenerator, T> good:
                return good;

            default:
                throw new ArgumentOutOfRangeException($"{member} is not of a valid type");
        }
    }

    private static readonly Action<ILGenerator, PropertyInfo> _pushProperty = PushProperty;
    private static readonly Action<ILGenerator, FieldInfo> _pushField = PushField;
    private static readonly Action<ILGenerator, MethodInfo> _pushMethod = PushMethod;
    private static readonly Action<ILGenerator, LocalBuilder> _pushLocal = PushLocal;
    private static readonly Action<ILGenerator, LocalVariable> _pushLocal2 = PushLocal2;
    private static readonly Action<ILGenerator, ArgAccessor> _pushArg = PushArg;

    private static void PushProperty(ILGenerator il,
                                     PropertyInfo prop)
    {
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Callvirt, prop.GetMethod!);
    }

    private static void PushField(ILGenerator il,
                                     FieldInfo field)
    {
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldfld, field);
    }

    private static void PushMethod(ILGenerator il,
                                  MethodInfo m)
    {
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Callvirt, m);
    }

    private static void PushLocal(ILGenerator il,
                                   LocalBuilder local)
    {
        il.Emit(OpCodes.Ldloc, local);
    }

    private static void PushLocal2(ILGenerator il,
                                   LocalVariable local)
    {
        il.Emit(OpCodes.Ldloc, local);
    }

    private static void PushArg(ILGenerator il,
                                ArgAccessor argAccessor)
    {
        il.EmitLdArg(argAccessor.Index);
    }

    protected readonly ILGenerator _il;
    protected readonly T _member;
    protected readonly Type _memberType;
    protected readonly Action<ILGenerator, T> _pushValueToStack;
    //private readonly ITypeManipulator _types;
  
}
