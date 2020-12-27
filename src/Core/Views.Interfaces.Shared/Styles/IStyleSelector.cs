using System;

namespace Das.Views.Styles
{
    public interface IStyleSelector : IEquatable<IStyleSelector>
    {
        Boolean TryGetClassName(out String className);

        Boolean TryGetContentAppendType(out ContentAppendType appendType);

        /// <summary>
        /// Returns true if this selector should only apply to a visual in certain states
        /// </summary>
        Boolean IsFilteringOnVisualState();
    }
}
