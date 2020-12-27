using System;
using System.Threading.Tasks;

namespace Das.Views.Styles.Declarations
{
    public class ScalarDeclaration<T> : ValueDeclaration<T>
                                        
        where T : IConvertible
    {
        public ScalarDeclaration(String value,
                                 IStyleVariableAccessor variableAccessor,
                                 DeclarationProperty property)
            : this((T) Convert.ChangeType(value, typeof(T)), variableAccessor, property)
        {

        }

        public ScalarDeclaration(T value,
                                 IStyleVariableAccessor variableAccessor,
                                 DeclarationProperty property)
            : base(value, variableAccessor, property)
        {
         
        }
    }
}