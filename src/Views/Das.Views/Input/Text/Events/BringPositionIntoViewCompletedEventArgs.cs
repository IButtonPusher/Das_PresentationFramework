using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Das.Views.Input.Text
{
    public class BringPositionIntoViewCompletedEventArgs : AsyncCompletedEventArgs
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="error">Error occurred during an asynchronous operation.</param>
        /// <param name="cancelled">Whether an asynchronous operation has been cancelled.</param>
        /// <param name="userState">Unique identifier for the asynchronous task.</param>
        public BringPositionIntoViewCompletedEventArgs(Exception error,
                                                       Boolean cancelled,
                                                       Object userState)
            : base(error, cancelled, userState)
        {
            
        }
    }
}
