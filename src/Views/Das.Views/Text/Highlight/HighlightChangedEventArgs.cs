using System;
using System.Collections;
using System.Threading.Tasks;

namespace Das.Views.Text.Highlight
{
    /// <summary>
    ///     HighlightLayer.Changed event argument.
    /// </summary>
    public abstract class HighlightChangedEventArgs
    {
        /// <summary>
        ///     Sorted, non-overlapping, readonly collection of TextSegments
        ///     affected by a highlight change.
        /// </summary>
        public abstract IList Ranges { get; }

        /// <summary>
        ///     Type identifying the owner of the changed layer.
        /// </summary>
        public abstract Type OwnerType { get; }
    }
}
