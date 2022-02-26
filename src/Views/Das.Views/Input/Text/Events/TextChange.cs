using System;
using System.Threading.Tasks;

namespace Das.Views.Input.Text.Events
{
    /// <summary>
    ///     Specifies the changes applied to TextContainer content.
    /// </summary>
    public class TextChange
    {
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

        internal TextChange()
        {
        }

        #endregion Constructors

        //------------------------------------------------------
        //
        //  Public Members
        //
        //------------------------------------------------------

        #region Public Members

        /// <summary>
        ///     0-based character offset for this change
        /// </summary>
        public Int32 Offset
        {
            get => _offset;
            internal set => _offset = value;
        }

        /// <summary>
        ///     Number of characters added
        /// </summary>
        public Int32 AddedLength
        {
            get => _addedLength;
            internal set => _addedLength = value;
        }

        /// <summary>
        ///     Number of characters removed
        /// </summary>
        public Int32 RemovedLength
        {
            get => _removedLength;
            internal set => _removedLength = value;
        }

        #endregion Public Members

        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private Int32 _offset;
        private Int32 _addedLength;
        private Int32 _removedLength;

        #endregion Private Fields
    }
}
