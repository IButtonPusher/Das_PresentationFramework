namespace RuntimeCode.Shared;

public class ArgAccessor
{
    public Int32 Index { get; }

    public Type ArgType { get; }

    public ArgAccessor(Int32 index,
                       Type argType)
    {
        Index = index;
        ArgType = argType;
    }
}
