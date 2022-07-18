using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using CFR.Solver.TreeCompiler;
using Reflection.Common;

// ReSharper disable All

namespace RuntimeCode.Shared;

public class DictionaryReader<TKey, TValue, TMember> : BuilderBase<TMember>
    where TKey : IConvertible
    where TValue : notnull
{
    public DictionaryReader(ILGenerator il,
                            TMember member/*,
                            Action<ILGenerator, TMember> pushValueToStack,
                            ITypeManipulator types*/) : base(il, member/*, pushValueToStack, types*/)
    {
        _valueLocal = _il.DeclareLocal<TValue>();

        _tryGetValue = _memberType.GetMethodOrDie(nameof(Dictionary<Byte, Byte>.TryGetValue));
        _getValue = _memberType.GetPropertyGetterOrDie(IlGeneratorBase.Indexer);

        var gargs = _memberType.GetGenericArguments();
        switch (gargs.Length)
        {
            case 0:
                _valueType = _keyType = typeof(Object);
                break;

            case 2:
                _keyType = gargs[0];
                _valueType = gargs[1];
                break;

            default:
                throw new InvalidOperationException();
        }

        
    }

    public LocalVariable<TValue> this[TKey key]
    {
        get
        {
            _pushValueToStack(_il, _member);
            _il.PushConstant(key);
            _il.Emit(OpCodes.Callvirt, _getValue);
            _il.Emit(OpCodes.Stloc, _valueLocal);

            return _valueLocal;
        }
    }

    public void TryGetValue<TData>(TKey key,
                                   TData data,
                            Action<ILGenerator, LocalVariable<TValue>, TData> onFound)
    {
        var notContains = _il.DefineLabel();

        //var currentValue = _il.DeclareLocal(_memberType);

        _pushValueToStack(_il, _member);
        _il.PushConstant(key);

        _il.Emit(OpCodes.Ldloca, _valueLocal);

        _il.Emit(OpCodes.Callvirt, _tryGetValue);

        _il.Emit(OpCodes.Brfalse, notContains);

        onFound(_il, _valueLocal, data);


        _il.MarkLabel(notContains);
    }

    private readonly MethodInfo _getValue;
    private readonly Type _keyType;

    private readonly MethodInfo _tryGetValue;


    private readonly LocalVariable<TValue> _valueLocal;
    private readonly Type _valueType;
}

public class DictionaryBuilder<T> : BuilderBase<T>
{
    public DictionaryBuilder(ILGenerator il,
                             T member,
                             Action<ILGenerator, T> pushValueToStack/*,
                             ITypeManipulator types*/) 
        : base(il, member, pushValueToStack/*, types*/)
    {
        //if (!typeof(IDictionary).IsAssignableFrom(_memberType))
        //    throw new InvalidCastException($"{_memberType} cannot be used as a dictionary");

        _tryGetValue = _memberType.GetMethodOrDie(nameof(Dictionary<Byte, Byte>.TryGetValue));
        _getValue = _memberType.GetPropertyGetterOrDie(IlGeneratorBase.Indexer);

        var gargs = _memberType.GetGenericArguments();
        switch (gargs.Length)
        {
            case 0:
                _valueType = _keyType = typeof(Object);
                break;

            case 2:
                _keyType = gargs[0];
                _valueType = gargs[1];
                break;

            default:
                throw new InvalidOperationException();
        }

        _valueLocal = _il.DeclareLocal(_valueType);
    }


    //public LocalBuilder this[]

    public LocalBuilder GetValue<TValue>(TValue value,
                                         Action<ILGenerator, TValue> loadSearchValue)
    {
        _pushValueToStack(_il, _member);
        loadSearchValue(_il, value);
        _il.Emit(OpCodes.Callvirt, _getValue);
        _il.Emit(OpCodes.Stloc, _valueLocal);

        return _valueLocal;
    }

    public void TryGetValue<TData, TValue>(TValue value,
                                           Action<ILGenerator, TValue> loadSearchValue,
                                           TData data,
                                           OnValueFound<TData> action)
    {
        var notContains = _il.DefineLabel();

        var currentValue = _il.DeclareLocal(_memberType);

        _pushValueToStack(_il, _member);
        loadSearchValue(_il, value);

        _il.Emit(OpCodes.Ldloca, currentValue);

        _il.Emit(OpCodes.Callvirt, _tryGetValue);

        _il.Emit(OpCodes.Brfalse, notContains);

        action(_il, currentValue, data);


        _il.MarkLabel(notContains);
    }

    private readonly MethodInfo _getValue;
    private readonly Type _keyType;

    private readonly MethodInfo _tryGetValue;
    private readonly Type _valueType;
    private readonly LocalBuilder _valueLocal;
}

public delegate void OnValueFound<in TData>(ILGenerator il,
                                            LocalBuilder currentValue,
                                            TData data);
