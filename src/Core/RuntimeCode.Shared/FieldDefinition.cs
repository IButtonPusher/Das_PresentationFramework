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

    public Boolean IsStatic => _fieldBuilder.IsStatic;

    public String Name => _fieldBuilder.Name;

    public Int32 MetadataToken => _fieldBuilder.MetadataToken;

    public Type FieldType => typeof(T);

    public static implicit operator FieldBuilder(FieldDefinition<T> me)
        => me._fieldBuilder;

    private readonly FieldBuilder _fieldBuilder;
}
