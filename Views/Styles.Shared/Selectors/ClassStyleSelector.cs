using System;

namespace Das.Views.Styles.Selectors
{
    public class ClassStyleSelector : IStyleSelector
    {
        private readonly String _className;

        public ClassStyleSelector(String className)
        {
            if (!className.StartsWith(".", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException();
            
            _className = className.Substring(1);
        }

        public override String ToString()
        {
            return "Select class: " + _className;
        }

        public Boolean TryGetClassName(out String className)
        {
            className = _className;
            return true;
        }

        public Boolean IsSelectVisual(IVisualElement visual)
        {
            return visual.StyleClasses.Contains(_className);
        }
    }
}
