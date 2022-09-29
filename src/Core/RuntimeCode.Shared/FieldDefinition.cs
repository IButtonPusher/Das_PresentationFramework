using System;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace RuntimeCode.Shared;

public class FieldDefinition<T>
{
    public FieldDefinition(FieldBuilder fieldBuilder)
    {
        _fieldBuilder = fieldBuilder;
    }

    public String Name => _fieldBuilder.Name;

    public Type FieldType => typeof(T);

    public static implicit operator FieldBuilder(FieldDefinition<T> me)
        => me._fieldBuilder;

    private readonly FieldBuilder _fieldBuilder;
}
