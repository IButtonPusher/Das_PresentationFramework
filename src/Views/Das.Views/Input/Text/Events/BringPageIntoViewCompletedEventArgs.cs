using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using Das.Views.Input.Text.Pointers;

namespace Das.Views.Input.Text
{
    /// <summary>
    ///     Event arguments for the BringPageIntoViewCompleted event.
    /// </summary>
    public class BringPageIntoViewCompletedEventArgs : AsyncCompletedEventArgs
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="position">Initial text position of an object/character.</param>
        /// <param name="count">Number of lines to advance. Negative means move backwards.</param>
        /// <param name="newPosition">ITextPointer matching the given position advanced by the given number of line.</param>
        /// <param name="newSuggestedOffset">The offset at the position moved (useful when moving between columns or pages).</param>
        /// <param name="error">Error occurred during an asynchronous operation.</param>
        /// <param name="cancelled">Whether an asynchronous operation has been cancelled.</param>
        /// <param name="userState">Unique identifier for the asynchronous task.</param>
        public BringPageIntoViewCompletedEventArgs(ITextPointer position,
                                                   Int32 count,
                                                   ITextPointer newPosition,
                                                   Point newSuggestedOffset,
                                                   Exception error,
                                                   Boolean cancelled,
                                                   Object userState)
            : base(error, cancelled, userState)
        {
            _position = position;
            
            _count = count;
            _newPosition = newPosition;
            _newSuggestedOffset = newSuggestedOffset;
            
        }

        /// <summary>
        ///     Initial text position of an object/character.
        /// </summary>
        public ITextPointer Position
        {
            get
            {
                // Raise an exception if the operation failed or was cancelled.
                RaiseExceptionIfNecessary();
                return _position;
            }
        }

        /// <summary>
        ///     Number of lines to advance. Negative means move backwards.
        /// </summary>
        public Int32 Count
        {
            get
            {
                // Raise an exception if the operation failed or was cancelled.
                RaiseExceptionIfNecessary();
                return _count;
            }
        }

        /// <summary>
        ///     ITextPointer matching the given position advanced by the given number of line.
        /// </summary>
        public ITextPointer NewPosition
        {
            get
            {
                // Raise an exception if the operation failed or was cancelled.
                RaiseExceptionIfNecessary();
                return _newPosition;
            }
        }

        /// <summary>
        ///     The offset at the position moved (useful when moving between columns or pages).
        /// </summary>
        public Point NewSuggestedOffset
        {
            get
            {
                // Raise an exception if the operation failed or was cancelled.
                RaiseExceptionIfNecessary();
                return _newSuggestedOffset;
            }
        }

        // The suggestedX parameter is the suggested X offset, in pixels, of 
        // the TextPointer on the destination line.
        //private readonly double _suggestedX;

        /// <summary>
        ///     Number of lines to advance. Negative means move backwards.
        /// </summary>
        private readonly Int32 _count;

        /// <summary>
        ///     ITextPointer matching the given position advanced by the given number of line.
        /// </summary>
        private readonly ITextPointer _newPosition;

        /// <summary>
        ///     The offset at the position moved (useful when moving between columns or pages).
        /// </summary>
        private readonly Point _newSuggestedOffset;

        /// <summary>
        ///     Initial text position of an object/character.
        /// </summary>
        private readonly ITextPointer _position;
    }
}
