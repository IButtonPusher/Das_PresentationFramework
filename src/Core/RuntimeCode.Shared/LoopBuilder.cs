using System;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Das.Extensions;
using Das.Serializer;
using Reflection.Common;
// ReSharper disable UnusedMember.Global

namespace RuntimeCode.Shared;

public class LoopBuilder<T> //where T : MemberInfo //<TState> where TState : IDynamicState
{
    public LoopBuilder(ILGenerator il,
                       T member,
                       Action<ILGenerator, T> pushMemberToStack,
                       ITypeManipulator types)
    {
        _il = il;
        _member = member;
        _pushMemberToStack = pushMemberToStack;

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

            default:
                throw new ArgumentOutOfRangeException($"{member} is not of a valid type");
        }

        _types = types;
        //_actionProvider = actionProvider;
        var getEnumeratorMethod = _memberType.GetMethodOrDie(nameof(IEnumerable.GetEnumerator));
        _enumeratorDisposeMethod = getEnumeratorMethod.ReturnType.GetMethod(
            nameof(IDisposable.Dispose));

        _enumeratorMoveNext = typeof(IEnumerator).GetMethodOrDie(
            nameof(IEnumerator.MoveNext));

        var isExplicit = _enumeratorDisposeMethod == null;
        if (isExplicit && typeof(IDisposable).IsAssignableFrom(getEnumeratorMethod.ReturnType))
            _enumeratorDisposeMethod = typeof(IDisposable).GetMethodOrDie(
                nameof(IDisposable.Dispose));
        else
            _enumeratorMoveNext = getEnumeratorMethod.ReturnType.GetMethodOrDie(
                nameof(IEnumerator.MoveNext));

        _enumeratorCurrent = getEnumeratorMethod.ReturnType.GetterOrDie(
            nameof(IEnumerator.Current), out _);

        _enumeratorLocal = _il.DeclareLocal(getEnumeratorMethod.ReturnType);

        var enumeratorType = _enumeratorLocal.LocalType ?? throw new InvalidOperationException();
        //_enumeratorCurrentValue = _il.DeclareLocal(_enumeratorCurrent.ReturnType);

        if (enumeratorType.IsValueType)
        {
            _loadEnumeratorLocal = OpCodes.Ldloca;
            _callEnumeratorMethod = OpCodes.Call;
        }
        else
        {
            _loadEnumeratorLocal = OpCodes.Ldloc;
            _callEnumeratorMethod = OpCodes.Callvirt;
        }
    }

    private static Action<ILGenerator, LocalBuilder> GetLoadIndexAction(Type memberType,
            out MethodInfo getLength)
    {
        if (memberType.IsArray)
        {
            getLength = memberType.GetPropertyGetterOrDie(nameof(Array.Length));

            return ((il,
                     idx) =>
            {
                //il.Emit(OpCodes.Ldarg_0);
                //il.Emit(OpCodes.Ldfld, f);
                il.Emit(OpCodes.Ldloc, idx);
                il.Emit(OpCodes.Ldelem_Ref);
            });
        }

        if (memberType.GetProperty("Item") is { } prop &&
            prop.GetGetMethod() is { } gm && gm.GetParameters() is {} prmArr && 
            prmArr.Length == 1 && prmArr[0].ParameterType == typeof(Int32))
        {
            getLength = memberType.GetPropertyGetterOrDie(nameof(IList.Count));

            return ((il,
                     idx) =>
            {
                //il.Emit(OpCodes.Ldarg_0);
                //il.Emit(OpCodes.Ldfld, f);
                il.Emit(OpCodes.Ldloc, idx);
                il.Emit(OpCodes.Callvirt, gm);
            });
        }

        throw new InvalidOperationException();
    }

    public void ForEach<TData>(OnIndexedValueReady<TData> action,
                               TData data)
    {
        ForEachImpl(null, action, data);
    }

    public void ForEach<TData>(OnValueReady<TData> action,
                               TData data)
    {
        ForEachImpl(action, null, data);
    }

    public void ForLoop<TData>(OnIndexedValueReady<TData> action,
                               TData data)
    {
        //var pv = _buildState.CurrentField;

        var germane = _types.GetGermaneType(_memberType);


        var loadIndex = GetLoadIndexAction(_memberType, out var getLength);
    

        var arrLength = _il.DeclareLocal(Const.IntType);
        _pushMemberToStack(_il, _member);
        _il.Emit(OpCodes.Callvirt, getLength);
        _il.Emit(OpCodes.Stloc, arrLength);

        // for (var c = 0;
        var fore = _il.DefineLabel();
        var breakLoop = _il.DefineLabel();

        var c = _il.DeclareLocal(Const.IntType);
        _il.Emit(OpCodes.Ldc_I4_0);
        _il.Emit(OpCodes.Stloc, c);
        _il.MarkLabel(fore);

        // c < arr.Length
        _il.Emit(OpCodes.Ldloc, c);
        _il.Emit(OpCodes.Ldloc, arrLength);
        _il.Emit(OpCodes.Bge, breakLoop);

        // var current = array[c];

        var current = _il.DeclareLocal(germane);

        _pushMemberToStack(_il, _member);
        loadIndex(_il, c);
        
        _il.Emit(OpCodes.Stloc, current);

        ///////////////////////////////////////////////////////////////
        action(_il, current, c, germane, data);
        ///////////////////////////////////////////////////////////////

        // c++
        _il.Emit(OpCodes.Ldloc, c);
        _il.Emit(OpCodes.Ldc_I4_1);
        _il.Emit(OpCodes.Add);
        _il.Emit(OpCodes.Stloc, c);
        _il.Emit(OpCodes.Br, fore);


        _il.MarkLabel(breakLoop);
    }

    private void ForEachImpl<TData>(OnValueReady<TData>? action,
                             OnIndexedValueReady<TData>? indexedAction,
                             TData data)
    {
        if (action == null && indexedAction == null)
            throw new InvalidOperationException();

        var germane = _types.GetGermaneType(_memberType);

        var allDone = _il.DefineLabel();

        LocalBuilder? actionIndex;
        LocalBuilder? enumeratorCurrentValue;

        if (indexedAction != null)
        {
            actionIndex = _il.DeclareLocal(typeof(Int32));
            enumeratorCurrentValue = _il.DeclareLocal(_enumeratorCurrent.ReturnType);
        }
        else
        {
            actionIndex = default;
            enumeratorCurrentValue = default;
        }


        _pushMemberToStack(_il, _member);

        var getEnumeratorMethod = _memberType.GetMethodOrDie(nameof(IEnumerable.GetEnumerator));

        _il.Emit(OpCodes.Callvirt, getEnumeratorMethod);
        

        _il.Emit(OpCodes.Stloc, _enumeratorLocal);


        /////////////////////////////////////
        // TRY
        /////////////////////////////////////
        if (_enumeratorDisposeMethod != null)
            _il.BeginExceptionBlock();
        {
            var tryNext = _il.DefineLabel();
            _il.MarkLabel(tryNext);

            /////////////////////////////////////
            // !enumerator.HasNext() -> EXIT LOOP
            /////////////////////////////////////
            _il.Emit(_loadEnumeratorLocal, _enumeratorLocal);
            _il.Emit(_callEnumeratorMethod, _enumeratorMoveNext);
            _il.Emit(OpCodes.Brfalse, allDone);

            if (enumeratorCurrentValue is { })
            {
                _il.Emit(_loadEnumeratorLocal, _enumeratorLocal);
                _il.Emit(OpCodes.Callvirt, _enumeratorCurrent);
                _il.Emit(OpCodes.Stloc, enumeratorCurrentValue);
            }

            /////////////////////////////////////////////////////////////

            if (action is { } notIndexed)
                notIndexed(_il, LoadEnumeratorCurrentValue, germane, data);
            else
            {
                indexedAction!(_il, enumeratorCurrentValue!, actionIndex!, 
                    germane, data);
                _il.Emit(OpCodes.Ldloc, actionIndex!);
                _il.Emit(OpCodes.Ldc_I4_1);
                _il.Emit(OpCodes.Add);
                _il.Emit(OpCodes.Stloc, actionIndex!);
            }

            /////////////////////////////////////////////////////////////

            _il.Emit(OpCodes.Br, tryNext);

            _il.MarkLabel(allDone);
        }

        if (_enumeratorDisposeMethod == null)
            return;

        /////////////////////////////////////
        // FINALLY
        /////////////////////////////////////
        _il.BeginFinallyBlock();
        {
            _il.Emit(_loadEnumeratorLocal, _enumeratorLocal);
            _il.Emit(_callEnumeratorMethod, _enumeratorDisposeMethod);
        }
        _il.EndExceptionBlock();
    }

    private void LoadEnumeratorCurrentValue()
    {
        _il.Emit(_loadEnumeratorLocal, _enumeratorLocal);
        _il.Emit(OpCodes.Callvirt, _enumeratorCurrent);
    }

    private readonly MethodInfo _enumeratorCurrent;
    //private readonly LocalBuilder _enumeratorCurrentValue;
    private readonly MethodInfo? _enumeratorDisposeMethod;
    private readonly LocalBuilder _enumeratorLocal;
    private readonly MethodInfo _enumeratorMoveNext;

    private readonly OpCode _loadEnumeratorLocal;
    private readonly OpCode _callEnumeratorMethod;

    private readonly ILGenerator _il;

    private readonly T _member;
    private readonly Type _memberType;
    private readonly Action<ILGenerator, T> _pushMemberToStack;

    //private readonly TState _buildState;
    protected readonly ITypeManipulator _types;
    //private readonly IFieldActionProvider _actionProvider;
}

public delegate void OnValueReady<in TData>(ILGenerator il,
                                            //LocalBuilder enumeratorCurrentValue,
                                            Action loadCurrentValue,
                                            Type itemType,
                                            TData data);

public delegate void OnIndexedValueReady<in TData>(ILGenerator il,
                                                   LocalBuilder currentValue,
                                                   LocalBuilder currentIndex,
                                                   Type itemType,
                                                   TData data);
