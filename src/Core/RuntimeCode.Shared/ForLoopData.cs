using System.Reflection.Emit;

namespace RuntimeCode.Shared;

public class ForLoopData
{
    public LocalBuilder CurrentValue { get; }

    public LocalBuilder CurrentIndex { get; }

    public LocalBuilder Count { get; }

    public Type ItemType { get; }

    public ForLoopData(LocalBuilder currentValue,
                       LocalBuilder currentIndex,
                       LocalBuilder count,
                       Type itemType)
    {
        CurrentValue = currentValue;
        CurrentIndex = currentIndex;
        Count = count;
        ItemType = itemType;
    }
}
