using System;
using System.Threading.Tasks;

namespace Das.Views.Input.Text.Events
{
    /// <summary>
    ///     Specifies the type of change applied to TextContainer content.
    /// </summary>
    public enum TextChangeType
    {
        /// <summary>
        ///     New content was inserted.
        /// </summary>
        ContentAdded,

        /// <summary>
        ///     Content was deleted.
        /// </summary>
        ContentRemoved,

        /// <summary>
        ///     A local DependencyProperty value changed.
        /// </summary>
        PropertyModified,
    }
}
