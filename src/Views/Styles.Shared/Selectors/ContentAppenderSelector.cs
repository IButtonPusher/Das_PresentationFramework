using System;
using System.Threading.Tasks;

namespace Das.Views.Styles.Selectors
{
    public class ContentAppenderSelector : IStyleSelector
    {
        public ContentAppenderSelector(IStyleSelector typeSelector,
                                       ContentAppendType appendType)
        {
            AppendType = appendType;
            _typeSelector = typeSelector;
        }

        public Boolean TryGetClassName(out String className)
        {
            return _typeSelector.TryGetClassName(out className);
        }

        public Boolean IsSelectVisual(IVisualElement visual)
        {
            return _typeSelector.IsSelectVisual(visual);
        }

        public override String ToString()
        {
            return _typeSelector + "::" + AppendType;
        }


        public ContentAppendType AppendType { get; }

        private readonly IStyleSelector _typeSelector;
    }
}