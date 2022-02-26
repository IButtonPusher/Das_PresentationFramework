using System;
using System.Threading.Tasks;

namespace Das.Views.Input.Text.Pointers
{
    /// <summary>
    ///     This identifies the edge of an object where a TextPointer is located
    /// </summary>
    [Flags]
    public enum ElementEdge : byte
    {
        /// <summary>
        ///     Located before the beginning of a DependencyObject
        /// </summary>
        BeforeStart = 1,

        /// <summary>
        ///     Located after the beginning of a DependencyObject
        /// </summary>
        AfterStart = 2,

        /// <summary>
        ///     Located before the end of a DependencyObject
        /// </summary>
        BeforeEnd = 4,

        /// <summary>
        ///     Located after the end of a DependencyObject
        /// </summary>
        AfterEnd = 8
    }
}
