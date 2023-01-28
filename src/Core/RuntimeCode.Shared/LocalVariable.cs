using System;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace RuntimeCode.Shared;

// ReSharper disable once UnusedType.Global
public class ThrowawayLocalVariable<T> : LocalVariable<T>,
                                         IDisposable
{
    public ThrowawayLocalVariable(LocalBuilder localBuilder,
                                  Dictionary<Type, LocalVariable> buffer)
        : base(localBuilder)
    {
        _buffer = buffer;
    }

    public void Dispose()
    {
        if (!_buffer.ContainsKey(typeof(T)))
            _buffer.Add(typeof(T), this);
    }

    public static implicit operator LocalBuilder(ThrowawayLocalVariable<T>? me)
    {
        return me?._localBuilder!;
    }

    private readonly Dictionary<Type, LocalVariable> _buffer;
}

public class LocalVariable<T> : LocalVariable
{
    public LocalVariable(LocalBuilder localBuilder) 
       : base(localBuilder)
    {
    }

    public static implicit operator LocalBuilder(LocalVariable<T>? me)
    {
        return me?._localBuilder!;
    }
}

public abstract class LocalVariable : IEquatable<LocalBuilder>
{
    public LocalVariable(LocalBuilder localBuilder)
    {
        _localBuilder = localBuilder;

        LocalIndex = localBuilder.LocalIndex;
    }

    public readonly Int32 LocalIndex;

    public static implicit operator LocalBuilder(LocalVariable? me)
    {
        return me?._localBuilder!;
    }

    public Type LocalType => _localBuilder.LocalType ?? throw new NullReferenceException();

    protected readonly LocalBuilder _localBuilder;

    public Boolean Equals(LocalBuilder? other) => ReferenceEquals(_localBuilder, other);
}