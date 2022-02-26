using System;
using System.Threading.Tasks;

namespace Das.Views.Input.Text.Pointers
{
    /// <summary>
    ///     Represents a certain content's position. This position is content
    ///     specific.
    /// </summary>
    public abstract class ContentPosition
    {
        /// <summary>
        ///     Static representation of a non-existent ContentPosition.
        /// </summary>
        public static readonly ContentPosition Missing = new MissingContentPosition();

        #region Missing

        /// <summary>
        ///     Representation of a non-existent ContentPosition.
        /// </summary>
        private class MissingContentPosition : ContentPosition
        {
        }

        #endregion Missing
    }
}
