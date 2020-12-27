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
            TypeSelector = typeSelector;
        }

        public Boolean TryGetClassName(out String className)
        {
            return TypeSelector.TryGetClassName(out className);
        }

        public Boolean TryGetContentAppendType(out ContentAppendType appendType)
        {
            appendType = AppendType;
            return true;
        }

        public Boolean IsFilteringOnVisualState()
        {
            return TypeSelector.IsFilteringOnVisualState();
        }

        public Boolean Equals(IStyleSelector other)
        {
            return other is ContentAppenderSelector contenty &&
                   contenty.AppendType == AppendType &&
                   contenty.TypeSelector.Equals(TypeSelector);
        }

        public override String ToString()
        {
            return TypeSelector + "::" + AppendType;
        }


        public ContentAppendType AppendType { get; }

        public IStyleSelector TypeSelector { get; }
    }
}