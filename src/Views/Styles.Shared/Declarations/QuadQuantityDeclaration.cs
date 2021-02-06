using Das.Views.Core.Geometry;
using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Das.Views.Styles.Declarations
{
    public abstract class QuadQuantityDeclaration : ValueDeclaration<QuantifiedThickness>
    {
        protected QuadQuantityDeclaration(String value,
            IStyleVariableAccessor variableAccessor, 
                                          DeclarationProperty property) 
            : base(QuantifiedThickness.Parse(value), variableAccessor, property)
        {
        }
    }
}
