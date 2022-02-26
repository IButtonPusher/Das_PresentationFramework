using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using Das.Views.Input.Text.Pointers;

namespace Das.Views.Input.Text
{
    /// <summary>
    ///     Event arguments for the BringPointIntoViewCompleted event.
    /// </summary>
    public class BringPointIntoViewCompletedEventArgs : AsyncCompletedEventArgs
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="point">Point in pixel coordinates.</param>
        /// <param name="position">A text position and its orientation matching or closest to the point.</param>
        /// <param name="succeeded">Whether operation was successful.</param>
        /// <param name="error">Error occurred during an asynchronous operation.</param>
        /// <param name="cancelled">Whether an asynchronous operation has been cancelled.</param>
        /// <param name="userState">Unique identifier for the asynchronous task.</param>
        public BringPointIntoViewCompletedEventArgs(Point point,
                                                    ITextPointer position,
                                                    Boolean succeeded,
                                                    Exception error,
                                                    Boolean cancelled,
                                                    Object userState)
            : base(error, cancelled, userState)
        {
            _point = point;
            _position = position;
            //_succeeded = succeeded;
        }

        /// <summary>
        ///     Point in pixel coordinates.
        /// </summary>
        public Point Point
        {
            get
            {
                // Raise an exception if the operation failed or was cancelled.
                RaiseExceptionIfNecessary();
                return _point;
            }
        }

        /// <summary>
        ///     A text position and its orientation matching or closest to the point.
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
        ///     Point in pixel coordinates.
        /// </summary>
        private readonly Point _point;

        /// <summary>
        ///     A text position and its orientation matching or closest to the point.
        /// </summary>
        private readonly ITextPointer _position;

        // Whether operation was successful.
        //private readonly bool _succeeded;
    }
}
