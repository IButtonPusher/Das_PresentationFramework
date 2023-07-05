using System.Reflection.Emit;

namespace RuntimeCode.Shared;

public static class ConstForLoop
{
    public static void Build<TData1, TData2>(ILGenerator il,
                                    Int32 fromInclusive,
                                    Int32 maxExclusive,
                                    TData1 data1,
                                    TData2 data2,
                                    ForLoopDelegate<TData1, TData2> action)
    {
        var index = il.DeclareLocal<Int32>();
        il.PushConstant(fromInclusive);
        il.StoreLocal(index);

        var endOfLoop = il.DefineLabel();
        var nextIteration = il.DefineLabel();

        il.MarkLabel(nextIteration);

        il.LoadLocal(index);
        il.PushConstant(maxExclusive);
        il.Emit(OpCodes.Bge, endOfLoop);

        //////

        action(il, index, data1, data2);


        //////

        il.LoadLocal(index);
        il.PushConstant(1);
        il.Emit(OpCodes.Add);
        il.StoreLocal(index);

        il.Emit(OpCodes.Br, nextIteration);


        il.MarkLabel(endOfLoop);
    }

    /// <summary>
    /// indexer should already be set to the min value
    /// </summary>
    public static void BuildCounter<TData1, TData2, TIndexer>(ILGenerator il,
                                                              LocalVariable<TIndexer> indexer,
                                                              TData1 data1,
                                                              TData2 data2,
                                                              LocalVariable<TIndexer> maxExclusive,
                                                              Action<ILGenerator, LocalVariable<TIndexer>> incrementor,
                                                              ForLIndexeroopDelegate<TData1, TData2, TIndexer> action)
    
    {
       var endOfLoop = il.DefineLabel();
       var nextIteration = il.DefineLabel();

       il.MarkLabel(nextIteration);

       //////

       action(il, indexer, data1, data2);


       //////

       incrementor(il, indexer);

       il.LoadLocal(indexer);
       il.LoadLocal(maxExclusive);
       il.Emit(OpCodes.Bgt, endOfLoop);

       il.Emit(OpCodes.Br, nextIteration);


       il.MarkLabel(endOfLoop);
    }

    public static void Build<TData>(ILGenerator il,
                                    Int32 fromInclusive,
                                    Int32 toExclusive,
                                    TData data,
                                    ForLoopDelegate<TData> action)
    {
        void ConvertAction(ILGenerator g,
                  LocalVariable<Int32> idx,
                  TData d1,
                  Int32 d2) =>
            LoopBodyConverter(action, g, idx, d1);

        Build(il, fromInclusive, toExclusive, data, 0, ConvertAction);
    }

    private static void LoopBodyConverter<TData1>(ForLoopDelegate<TData1> action,
                                                  ILGenerator il,
                                                  LocalVariable<Int32> currentIndex,
                                                  TData1 data1)
    {
        action(il, currentIndex, data1);
    }
}

public delegate void ForLoopDelegate<in TData>(ILGenerator il,
                                               LocalVariable<Int32> currentIndex,
                                               TData data);

public delegate void ForLoopDelegate<in TData1, in TData2>(ILGenerator il,
                                                           LocalVariable<Int32> currentIndex,
                                                           TData1 data1,
                                                           TData2 data2);

public delegate void ForLIndexeroopDelegate<in TData1, in TData2, TLocal>(ILGenerator il,
                                                           LocalVariable<TLocal> currentIndex,
                                                           TData1 data1,
                                                           TData2 data2);
