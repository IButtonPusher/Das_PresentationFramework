using System;
using System.Collections.Generic;

namespace Das.Views.Styles.Declarations
{
    public class BoxShadowDeclaration : DeclarationBase
    {
        public BoxShadowDeclaration(String value,
            IStyleVariableAccessor variableAccessor) 
            : base(variableAccessor, DeclarationProperty.BoxShadow)
        {
            _shadows = new List<BoxShadow>();
            
            var shadows = GetMultiSplit(value);
            foreach (var shadow in shadows)
            {
                var boxShadow = new BoxShadow(shadow, variableAccessor);
                _shadows.Add(boxShadow);
            }
        }

        private readonly List<BoxShadow> _shadows;

        public IEnumerable<BoxShadow> Shadows => _shadows;
    }
}
