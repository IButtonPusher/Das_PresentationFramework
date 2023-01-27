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

public class LoopBuilder<T>
{
    public LoopBuilder(ILGenerator il,
                       T member,
                       ITypeManipulator types,
                       LocalBuilder? currentValueLocal = null)
        : this(il, member, BuilderBase<T>.GetPushValueAction(member),
            types, currentValueLocal)
    {
    }

    public LoopBuilder(ILGenerator il,
                       T member,
                       Action<ILGenerator, T> pushMemberToStack,
                       ITypeManipulator types,
                       LocalBuilder? currentValueLocal = null)
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

            case LocalVariable lvar:
                _memberType = lvar.LocalType;
                break;

            case ArgAccessor arga:
                _memberType = arga.ArgType;
                break;


            default:
                throw new ArgumentOutOfRangeException($"{member} is not of a valid type");
        }

        _types = types;

        _elementType = _memberType.IsArray
            ? _memberType.GetElementType() ?? _types.GetGermaneType(_memberType)
            : _types.GetGermaneType(_memberType);

        if (currentValueLocal != null)
            CurrentValueLocal = currentValueLocal;
        else
        {
            CurrentValueLocal = _il.DeclareLocal(_elementType);
        }
    }

    public void ForEach<TData>(TData data,
                               OnIndexedValueReady<TData> action)
    {
        ForEachImpl(null, action, data);
    }

    public void ForEach<TData>(TData data,
                               OnValueReady<TData> action)
    {
        ForEachImpl(action, null, data);
    }

    private void ForLoopSetup(Action<ILGenerator> getLength,
                              out LocalBuilder c,
                              out Label fore,
                              out Label breakLoop,
                              out Type germane,
                              out LocalBuilder arrLength)
    {
       var loadIndex = GetLoadIndexAction(_memberType, out _);
       ForLoopSetup(getLength, loadIndex, false,
          out c, out fore, out breakLoop, out germane, out arrLength);
    }

    private void ForLoopSetup(out LocalBuilder c,
                              out Label fore,
                              out Label breakLoop,
                              out Type germane,
                              out LocalBuilder arrLength)
    {
       var loadIndex = GetLoadIndexAction(_memberType, out var getLength);
       ForLoopSetup(getLength, loadIndex, true,
          out c, out fore, out breakLoop, out germane, out arrLength);

        //germane = _elementType;
       

        //arrLength = _il.DeclareLocal(Const.IntType);
        //_pushMemberToStack(_il, _member);
        //getLength(_il);
        
        //_il.Emit(OpCodes.Stloc, arrLength);

        //c = _il.DeclareLocal(Const.IntType);
        //_il.Emit(OpCodes.Ldc_I4_0);
        //_il.Emit(OpCodes.Stloc, c);

        //ForLoopSetup(arrLength, loadIndex, c,
        //    out fore, out breakLoop, out germane);
    }

    private void ForLoopSetup(Action<ILGenerator> getLength,
                              Action<ILGenerator, LocalBuilder> loadIndex,
                              Boolean pushMemberForLength,
                              out LocalBuilder c,
                              out Label fore,
                              out Label breakLoop,
                              out Type germane,
                              out LocalBuilder arrLength)
    {
       germane = _elementType;
       //var loadIndex = GetLoadIndexAction(_memberType, out var getLength);

       arrLength = _il.DeclareLocal(Const.IntType);
       if (pushMemberForLength)
         _pushMemberToStack(_il, _member);
       getLength(_il);

       _il.Emit(OpCodes.Stloc, arrLength);

       c = _il.DeclareLocal(Const.IntType);
       _il.Emit(OpCodes.Ldc_I4_0);
       _il.Emit(OpCodes.Stloc, c);

       ForLoopSetup(arrLength, loadIndex, c,
          out fore, out breakLoop, out germane);
    }

  

    private void ForLoopSetup(LocalBuilder arrLength,
                              Action<ILGenerator, LocalBuilder> loadIndex,
                              LocalBuilder c,
                              out Label fore,
                              out Label breakLoop,
                              out Type germane)
    {
        germane = _elementType;
        //var loadIndex = GetLoadIndexAction(_memberType, out var getLength);

        //var arrLength = _il.DeclareLocal(Const.IntType);
        //_pushMemberToStack(_il, _member);
        //getLength(_il);
        
        //_il.Emit(OpCodes.Stloc, arrLength);

        //c = _il.DeclareLocal(Const.IntType);
        //_il.Emit(OpCodes.Ldc_I4_0);
        //_il.Emit(OpCodes.Stloc, c);

        // for (var c = 0;
        fore = _il.DefineLabel();
        breakLoop = _il.DefineLabel();

        //c = _il.DeclareLocal(Const.IntType);
        //_il.Emit(OpCodes.Ldc_I4_0);
        //_il.Emit(OpCodes.Stloc, c);
        
        _il.MarkLabel(fore);

        // c < arr.Length
        _il.Emit(OpCodes.Ldloc, c);
        _il.Emit(OpCodes.Ldloc, arrLength);
        _il.Emit(OpCodes.Bge, breakLoop);

        // var current = array[c];
        _pushMemberToStack(_il, _member);
        loadIndex(_il, c);

        _il.Emit(OpCodes.Stloc, CurrentValueLocal);
    }

    private void ForLoopIncrement(LocalBuilder c,
                                  Label fore,
                                  Label breakLoop)
    {
        // c++
        _il.Emit(OpCodes.Ldloc, c);
        _il.Emit(OpCodes.Ldc_I4_1);
        _il.Emit(OpCodes.Add);
        _il.Emit(OpCodes.Stloc, c);
        _il.Emit(OpCodes.Br, fore);

        _il.MarkLabel(breakLoop);
    }

    public void ForLoop<TData>(TData data,
                               OnIndexedValueReady<TData> action)
    {
        ForLoopSetup(out var c, out var fore, out var breakLoop, 
            out var germane, out _);
        
        action(_il, CurrentValueLocal, c, germane, data);

        ForLoopIncrement(c, fore, breakLoop);
    }

    public void ForLoop<TData>(TData data,
                               OnForLoopIteration<TData> action)
    {
        ForLoopSetup(out var c, out var fore, out var breakLoop, 
            out var germane, out var countVar);

        var loopData = new ForLoopData(CurrentValueLocal, c, countVar, germane);
        
        action(_il, loopData, data);

        ForLoopIncrement(c, fore, breakLoop);
    }

    public void ForLoop<TData>(TData data,
                               Action<ILGenerator> getLength,
                               OnForLoopIteration<TData> action)
    {
       ForLoopSetup(getLength, out var c, out var fore, out var breakLoop, 
          out var germane, out var countVar);

       var loopData = new ForLoopData(CurrentValueLocal, c, countVar, germane);
        
       action(_il, loopData, data);

       ForLoopIncrement(c, fore, breakLoop);
    }

    public void ForLoop<TData1, TData2>(TData1 data1,
                                        TData2 data2,
                               OnForLoopIteration<TData1,TData2> action)
    {
        ForLoopSetup(out var c, out var fore, out var breakLoop, 
            out var germane, out var countVar);

        var loopData = new ForLoopData(CurrentValueLocal, c, countVar, germane);

        action(_il, loopData, data1, data2);

        ForLoopIncrement(c, fore, breakLoop);
    }

    //public void ForLoop<TData>(TData data,
    //                           LocalBuilder countVar,
    //                           OnForLoopIteration<TData> action)
    //{
    //    ForLoopSetup(countVar, out var c, out var fore, out var breakLoop, 
    //        out var germane);

    //    var loopData = new ForLoopData(CurrentValueLocal, c, countVar, germane);
        
    //    action(_il, loopData, data);

    //    ForLoopIncrement(c, fore, breakLoop);
    //}

    //public void ForLoop<TData1, TData2>(TData1 data1,
    //                                    TData2 data2,
    //                           LocalBuilder countVar,
    //                           OnForLoopIteration<TData1, TData2> action)
    //{
    //    ForLoopSetup(countVar, out var c, out var fore, out var breakLoop, 
    //        out var germane);

    //    var loopData = new ForLoopData(CurrentValueLocal, c, countVar, germane);
        
    //    action(_il, loopData, data1, data2);

    //    ForLoopIncrement(c, fore, breakLoop);
    //}

    public void ForLoop<TData1, TData2>(TData1 data1,
                                        TData2 data2,
                                        OnIndexedValueReady<TData1, TData2> action)
    {
        ForLoopSetup(out var c, out var fore, out var breakLoop, 
            out var germane, out _);
        
        action(_il, CurrentValueLocal, c, germane, data1, data2);

        ForLoopIncrement(c, fore, breakLoop);
    }

    public void ForLoop<TData1, TData2, TData3>(TData1 data1,
                                        TData2 data2,
                                        TData3 data3,
                                        OnIndexedValueReady<TData1, TData2, TData3> action)
    {
        ForLoopSetup(out var c, out var fore, out var breakLoop,
            out var germane, out _);
        
        action(_il, CurrentValueLocal, c, germane, data1, data2, data3);

        ForLoopIncrement(c, fore, breakLoop);
    }

    public void ForLoop<TData1, TData2, TData3, TData4>(TData1 data1,
                                                TData2 data2,
                                                TData3 data3,
                                                TData4 data4,
                                                OnIndexedValueReady<TData1, TData2, TData3, TData4> action)
    {
        ForLoopSetup(out var c, out var fore, out var breakLoop, 
            out var germane, out _);
        
        action(_il, CurrentValueLocal, c, germane, data1, data2, data3, data4);

        ForLoopIncrement(c, fore, breakLoop);
    }

   
    private static Action<ILGenerator, LocalBuilder> GetLoadIndexAction(Type memberType,
                                                                        out Action<ILGenerator> getLength)
    {
        if (memberType.IsArray)
        {
            getLength = (il) =>
            {
                il.Emit(OpCodes.Ldlen);
            };

            return (il,
                    idx) =>
            {
                il.Emit(OpCodes.Ldloc, idx);
                var opCode = RuntimeCodeHelper.GetLdElem(memberType, out var germane);
                if (opCode == OpCodes.Ldelem)
                    il.Emit(opCode, germane);
                else
                    il.Emit(opCode);
            };
        }

        if (memberType.GetProperty("Item") is { } prop &&
            prop.GetGetMethod() is { } gm && gm.GetParameters() is { } prmArr &&
            prmArr.Length == 1 && prmArr[0].ParameterType == typeof(Int32))
        {
            getLength = il =>
            {
                il.Emit(OpCodes.Callvirt, memberType.GetPropertyGetterOrDie(nameof(IList.Count)));
            };

            return (il,
                    idx) =>
            {
                il.Emit(OpCodes.Ldloc, idx);
                il.Emit(OpCodes.Callvirt, gm);
            };
        }

        throw new InvalidOperationException();
    }

    private void ForEachImpl<TData>(OnValueReady<TData>? action,
                                    OnIndexedValueReady<TData>? indexedAction,
                                    TData data)
    {
        var _getEnumerator = _memberType.GetMethodOrDie(nameof(IEnumerable.GetEnumerator));

        var _enumeratorType = _getEnumerator.ReturnType;

        var _enumeratorDisposeMethod = _enumeratorType.GetMethod(
            nameof(IDisposable.Dispose));

        MethodInfo _enumeratorMoveNext;

        var isExplicit = _enumeratorDisposeMethod == null;
        if (isExplicit && typeof(IDisposable).IsAssignableFrom(_enumeratorType))
        {
            _enumeratorDisposeMethod = typeof(IDisposable).GetMethodOrDie(
                nameof(IDisposable.Dispose));
            _enumeratorMoveNext = typeof(IEnumerator).GetMethodOrDie(
                nameof(IEnumerator.MoveNext));
        }
        else
        {
            _enumeratorMoveNext = _enumeratorType.GetMethodOrDie(
                nameof(IEnumerator.MoveNext));
        }

        var _enumeratorCurrent = _enumeratorType.GetterOrDie(
            nameof(IEnumerator.Current), out _);

        var _enumeratorLocal = _il.DeclareLocal(_enumeratorType);

        //var _enumeratorCurrentType = _enumeratorCurrent.ReturnType;
        OpCode _loadEnumeratorLocal;
        OpCode _callEnumeratorMethod;

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

        if (action == null && indexedAction == null)
            throw new InvalidOperationException();

        var germane = _elementType;
        //var germane = _types.GetGermaneType(_memberType);

        

        LocalBuilder? actionIndex;
        LocalBuilder? enumeratorCurrentValue;

        if (indexedAction != null)
        {
            actionIndex = _il.DeclareLocal(typeof(Int32));
            _il.PushConstant(0);
            _il.StoreLocal(actionIndex);


            enumeratorCurrentValue = _il.DeclareLocal(_enumeratorCurrent.ReturnType);
        }
        else
        {
            actionIndex = default;
            enumeratorCurrentValue = default;
        }


        _pushMemberToStack(_il, _member);

        _il.Emit(OpCodes.Callvirt, _getEnumerator);

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
   

            if (enumeratorCurrentValue is { })
            {
                _il.Emit(_loadEnumeratorLocal, _enumeratorLocal);
                _il.Emit(OpCodes.Callvirt, _enumeratorCurrent);
                _il.Emit(OpCodes.Stloc, enumeratorCurrentValue.LocalIndex);
            }

            /////////////////////////////////////////////////////////////

            if (action is { } notIndexed)
                notIndexed(_il,
                    () => LoadEnumeratorCurrentValue(_enumeratorLocal, _enumeratorCurrent,
                        _loadEnumeratorLocal),
                    germane, data);
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


            /////////////////////////////////////
            // !enumerator.HasNext() -> EXIT LOOP
            /////////////////////////////////////

            _il.MarkLabel(moveNext);
            _il.Emit(_loadEnumeratorLocal, _enumeratorLocal);
            _il.Emit(_callEnumeratorMethod, _enumeratorMoveNext);

            _il.Emit(OpCodes.Brtrue, handleValue);
   
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

        //_il.MarkLabel(allDone);
    }

  

    private void LoadEnumeratorCurrentValue(LocalBuilder _enumeratorLocal,
                                            MethodInfo _enumeratorCurrent,
                                            //Type _enumeratorCurrentType,
                                            OpCode _loadEnumeratorLocal)
    {
        _il.Emit(_loadEnumeratorLocal, _enumeratorLocal);
        _il.Emit(OpCodes.Callvirt, _enumeratorCurrent);

        //if (_enumeratorCurrentType.IsValueType)
        //{
        //    //_il.Emit(OpCodes.Stloc, _enumeratorCurrentValue);
        //    //_il.Emit(OpCodes.Ldloca, _enumeratorCurrentValue);
        //}
    }

    public LocalBuilder CurrentValueLocal { get; }

    public Type ElementType => _elementType;

    private readonly ILGenerator _il;

    

    private readonly T _member;
    private readonly Type _memberType;
    private readonly Action<ILGenerator, T> _pushMemberToStack;

    
    protected readonly ITypeManipulator _types;
    private readonly Type _elementType;
}

public delegate void OnValueReady<in TData>(ILGenerator il,
                                            Action loadCurrentValue,
                                            Type itemType,
                                            TData data);

public delegate void OnIndexedValueReady<in TData>(ILGenerator il,
                                                   LocalBuilder currentValue,
                                                   LocalBuilder currentIndex,
                                                   Type itemType,
                                                   TData data);

public delegate void OnForLoopIteration<in TData>(ILGenerator il,
                                                   ForLoopData loopData,
                                                   TData data);

public delegate void OnForLoopIteration<in TData1, in TData2>(ILGenerator il,
                                                              ForLoopData loopData,
                                                              TData1 data1,
                                                              TData2 data2);


public delegate void OnIndexedValueReady<in TData1, in TData2>(ILGenerator il,
                                                   LocalBuilder currentValue,
                                                   LocalBuilder currentIndex,
                                                   Type itemType,
                                                   TData1 data1,
                                                   TData2 data2);

public delegate void OnIndexedValueReady<in TData1, in TData2, in TData3>(ILGenerator il,
                                                               LocalBuilder currentValue,
                                                               LocalBuilder currentIndex,
                                                               Type itemType,
                                                               TData1 data1,
                                                               TData2 data2,
                                                               TData3 data3);

public delegate void OnIndexedValueReady<in TData1, in TData2, in TData3, in TData4>(ILGenerator il,
                                                                          LocalBuilder currentValue,
                                                                          LocalBuilder currentIndex,
                                                                          Type itemType,
                                                                          TData1 data1,
                                                                          TData2 data2,
                                                                          TData3 data3,
                                                                          TData4 data4);
