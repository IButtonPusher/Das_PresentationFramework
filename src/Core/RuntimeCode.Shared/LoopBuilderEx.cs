using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using Das.Extensions;
using Reflection.Common;

namespace RuntimeCode.Shared;

public class LoopBuilderEx<TEnumerable, TGermane> 
    where TEnumerable : IEnumerable<TGermane>
{
    private readonly ILGenerator _il;
    //private readonly LocalVariable<TGermane> _currentValue;
    //private readonly LocalVariable<IEnumerator<TGermane>> _enumerator;
    //private readonly LocalVariable<IEnumerable<TGermane>> _enumerable;
    private readonly MethodInfo? _enumeratorDisposeMethod;

    private readonly OpCode _loadEnumeratorLocal;
    private readonly OpCode _callEnumeratorMethod;

    //private static readonly MethodInfo GetEnumerator =
    //    typeof(IEnumerable<TGermane>).GetMethodOrDie(nameof(IEnumerable<TGermane>.GetEnumerator));

    private readonly Type _enumeratorType;
    private readonly MethodInfo _enumeratorMoveNext;
    private readonly LocalBuilder _enumeratorLocal;
    private readonly MethodInfo _enumeratorGetCurrent;
    private readonly LocalVariable<TGermane> _enumeratorCurrentValue;
    private readonly Type _enumeratorCurrentType;
    private readonly MethodInfo _getEnumeratorMethod;

    public LoopBuilderEx(ILGenerator il)
    {
        _il = il;

        var memberType = typeof(TEnumerable);
        _getEnumeratorMethod = memberType.GetMethodOrDie(nameof(IEnumerable.GetEnumerator));

        _enumeratorType = _getEnumeratorMethod.ReturnType;


        if (_enumeratorType.IsValueType)
        {
            _loadEnumeratorLocal = OpCodes.Ldloca;
            _callEnumeratorMethod = OpCodes.Call;
        }
        else
        {
            _loadEnumeratorLocal = OpCodes.Ldloc;
            _callEnumeratorMethod = OpCodes.Callvirt;
        }

        _enumeratorDisposeMethod = _enumeratorType.GetMethod(
            nameof(IDisposable.Dispose));

        _enumeratorMoveNext = _enumeratorType.GetMethodOrDie(
            nameof(IEnumerator.MoveNext));

        var isExplicit = _enumeratorDisposeMethod == null;
        if (isExplicit && typeof(IDisposable).IsAssignableFrom(_enumeratorType))
            _enumeratorDisposeMethod = typeof(IDisposable).GetMethodOrDie(
                nameof(IDisposable.Dispose));
        else
            _enumeratorMoveNext = _enumeratorType.GetMethodOrDie(
                nameof(IEnumerator.MoveNext));

        _enumeratorGetCurrent = _enumeratorType.GetterOrDie(
            nameof(IEnumerator.Current), out _);

        _enumeratorCurrentType = _enumeratorGetCurrent.ReturnType;

        _enumeratorCurrentValue = _il.DeclareLocal<TGermane>();//(_enumeratorGetCurrent.ReturnType);

        _enumeratorLocal = _il.DeclareLocal(_enumeratorType);

    }

    private void LoadEnumeratorCurrentValue()
    {
        _il.Emit(_loadEnumeratorLocal, _enumeratorLocal);
        _il.Emit(OpCodes.Callvirt, _enumeratorGetCurrent);

        if (_enumeratorCurrentType.IsValueType)
        {
            _il.Emit(OpCodes.Stloc, _enumeratorCurrentValue);
            _il.Emit(OpCodes.Ldloca, _enumeratorCurrentValue);
        }
        //else
        //{
        //    _il.Emit(OpCodes.Ldloc, _enumeratorCurrentValue);
        //}

    }

    public void ForEach(Action<ILGenerator> loadIenum,
                        Action<Action> runWithValue)
    {
        loadIenum(_il);
        _il.Emit(OpCodes.Callvirt, _getEnumeratorMethod);
        _il.Emit(OpCodes.Stloc, _enumeratorLocal);

        var allDone = _il.DefineLabel();

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

            //if (enumeratorCurrentValue is { })
            //{
            //    _il.Emit(_loadEnumeratorLocal, _enumeratorLocal);
            //    _il.Emit(OpCodes.Callvirt, _enumeratorCurrent);
            //    _il.Emit(OpCodes.Stloc, enumeratorCurrentValue);
            //}

            /////////////////////////////////////////////////////////////

            runWithValue(LoadEnumeratorCurrentValue);

            //if (action is { } notIndexed)
            //    notIndexed(_il, LoadEnumeratorCurrentValue, germane, data);
            //else
            //{
            //    indexedAction!(_il, enumeratorCurrentValue!, actionIndex!, 
            //        germane, data);
            //    _il.Emit(OpCodes.Ldloc, actionIndex!);
            //    _il.Emit(OpCodes.Ldc_I4_1);
            //    _il.Emit(OpCodes.Add);
            //    _il.Emit(OpCodes.Stloc, actionIndex!);
            //}

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

    public void ForEach2(Action<ILGenerator> loadIenum,
                        Action<Action> runWithValue)
    {
        loadIenum(_il);
        _il.Emit(OpCodes.Callvirt, _getEnumeratorMethod);
        _il.Emit(OpCodes.Stloc, _enumeratorLocal);

       /////////////////////////////////////
        // TRY
        /////////////////////////////////////
        if (_enumeratorDisposeMethod != null)
            _il.BeginExceptionBlock();
        {
            var moveNext = _il.DefineLabel();
            var handleValue = _il.DefineLabel();

            _il.Emit(OpCodes.Br, moveNext);


            _il.MarkLabel(handleValue);


            //_il.Emit(_loadEnumeratorLocal, _enumeratorLocal);
            //_il.Emit(_callEnumeratorMethod, _enumeratorMoveNext);
            //_il.Emit(OpCodes.Brfalse, allDone);

            //if (enumeratorCurrentValue is { })
            //{
            //    _il.Emit(_loadEnumeratorLocal, _enumeratorLocal);
            //    _il.Emit(OpCodes.Callvirt, _enumeratorCurrent);
            //    _il.Emit(OpCodes.Stloc, enumeratorCurrentValue);
            //}

            /////////////////////////////////////////////////////////////

            //LoadEnumeratorCurrentValue();

            runWithValue(LoadEnumeratorCurrentValue);

            //if (action is { } notIndexed)
            //    notIndexed(_il, LoadEnumeratorCurrentValue, germane, data);
            //else
            //{
            //    indexedAction!(_il, enumeratorCurrentValue!, actionIndex!, 
            //        germane, data);
            //    _il.Emit(OpCodes.Ldloc, actionIndex!);
            //    _il.Emit(OpCodes.Ldc_I4_1);
            //    _il.Emit(OpCodes.Add);
            //    _il.Emit(OpCodes.Stloc, actionIndex!);
            //}

            /////////////////////////////////////////////////////////////

           

            /////////////////////////////////////
            // !enumerator.HasNext() -> EXIT LOOP
            /////////////////////////////////////

            _il.MarkLabel(moveNext);
            _il.Emit(_loadEnumeratorLocal, _enumeratorLocal);
            _il.Emit(_callEnumeratorMethod, _enumeratorMoveNext);

            _il.Emit(OpCodes.Brtrue, handleValue);

            //_il.Emit(OpCodes.Leave, allDone);

            /////////////////////////////////////////////////////////////

            
        }

        if (_enumeratorDisposeMethod == null)
            return;

        /////////////////////////////////////
        // FINALLY
        /////////////////////////////////////
        _il.BeginFinallyBlock();
        {
            var endfinally = _il.DefineLabel();

            if (!_enumeratorType.IsValueType)
            {
                _il.Emit(_loadEnumeratorLocal, _enumeratorLocal);
                _il.Emit(OpCodes.Brfalse, endfinally);
            }

            _il.Emit(_loadEnumeratorLocal, _enumeratorLocal);
            _il.Emit(_callEnumeratorMethod, _enumeratorDisposeMethod);

            _il.MarkLabel(endfinally);
        }
        _il.EndExceptionBlock();

    }
}
