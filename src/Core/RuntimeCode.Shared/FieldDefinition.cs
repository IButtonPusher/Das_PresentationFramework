using System;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace RuntimeCode.Shared;

public class FieldDefinition<T>
{
    public FieldDefinition(//TypeBuilder declaringType,
                           FieldBuilder fieldBuilder)
    {
        //_declaringType = declaringType;
        _fieldBuilder = fieldBuilder;
    }

    public String Name => _fieldBuilder.Name;

    public Type FieldType => typeof(T);// _fieldBuilder.FieldType;

    public static implicit operator FieldBuilder(FieldDefinition<T> me)
        => me._fieldBuilder;

    //private readonly TypeBuilder _declaringType;
    private readonly FieldBuilder _fieldBuilder;
}
