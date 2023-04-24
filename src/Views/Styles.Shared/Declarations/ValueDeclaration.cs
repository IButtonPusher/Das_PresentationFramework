using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Das.Views.Styles.Declarations;

public class ValueDeclaration<T> : DeclarationBase,
                                   IStyleValueDeclaration<T>
{
   public ValueDeclaration(T value,
                           //IStyleVariableAccessor variableAccessor,
                           DeclarationProperty property)
      : base(/*variableAccessor, */property)
   {
      Value = value;
   }

   public override String ToString()
   {
      return Property + ": " + Value;
   }

   Object? IStyleValueDeclaration.Value
   {
      [DebuggerNonUserCode]
      [DebuggerStepThrough]
      get => Value;
   }


   public T Value { get; }
}