using System;
using System.Threading.Tasks;
using Das.Views.Core;
using Das.Views.Input.Text.Pointers;

namespace Das.Views.Input.Text.Events
{
    /// <summary>
    ///     The TextContainerChangeEventArgs defines the event arguments sent when a
    ///     TextContainer is changed.
    /// </summary>
    public class TextContainerChangeEventArgs : EventArgs
    {
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

        public TextContainerChangeEventArgs(ITextPointer textPosition,
                                            Int32 count,
                                            Int32 charCount,
                                            TextChangeType textChange) :
            this(textPosition, count, charCount, textChange, null, false)
        {
        }

        public TextContainerChangeEventArgs(ITextPointer textPosition,
                                            Int32 count,
                                            Int32 charCount,
                                            TextChangeType textChange,
                                            DependencyProperty property,
                                            Boolean affectsRenderOnly)
        {
            _textPosition = textPosition.GetFrozenPointer(LogicalDirection.Forward);
            _count = count;
            _charCount = charCount;
            _textChange = textChange;
            _property = property;
            _affectsRenderOnly = affectsRenderOnly;
        }

        #endregion Constructors

        //------------------------------------------------------
        //
        //  public Properties
        //
        //------------------------------------------------------

        #region public Properties

        // Position of the segment start, expressed as an ITextPointer.
        public ITextPointer ITextPosition => _textPosition;

        // Number of chars covered by this segment.
        public Int32 IMECharCount => _charCount;

        public Boolean AffectsRenderOnly => _affectsRenderOnly;

        /// <summary>
        /// </summary>
        public Int32 Count => _count;

        /// <summary>
        /// </summary>
        public TextChangeType TextChange => _textChange;

        /// <summary>
        /// </summary>
        public DependencyProperty Property => _property;

        #endregion public Properties

        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        // Position of the segment start, expressed as an ITextPointer.
        private readonly ITextPointer _textPosition;

        // Number of symbols covered by this segment.
        private readonly Int32 _count;

        // Number of chars covered by this segment.
        private readonly Int32 _charCount;

        // Type of change.
        private readonly TextChangeType _textChange;

        private readonly DependencyProperty _property;

        private readonly Boolean _affectsRenderOnly;

        #endregion Private Fields
    }
}
