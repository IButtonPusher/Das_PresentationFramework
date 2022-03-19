﻿using System;
using System.Drawing;
using System.Threading.Tasks;
using Das.Views.Core;
using Das.Views.Input.Text.Pointers;

namespace Das.Views.Input.Text
{
    /// <summary>
    ///     The TextSelection class encapsulates selection state for the TextEditor
    ///     class.  It has no public constructor, but is exposed via a public property
    ///     on RichTextBox.
    /// </summary>
    public interface ITextSelection : ITextRange
    {
        //------------------------------------------------------
        //
        //  Internal Methods
        //
        //------------------------------------------------------

        #region Internal Methods

        //......................................................
        //
        //  Extending via movingEnd movements
        //
        //......................................................

        /// <summary>
        ///     Creates empty selection - corresponding to a caret position.
        /// </summary>
        /// <param name="caretPosition">
        ///     Position to which the caret is set.
        /// </param>
        /// <param name="direction">
        ///     Used for normalizing a position, and for its gravity in the following
        ///     behavior when content change happens around.
        ///     The direction can be ignored though if there is a space
        ///     in that direction, or if the position happens to be at line wrapping.
        ///     In case of a space caret chooses a direction opposite to a space;
        ///     in case of line wrapping, Forward direction is taken to set caret
        ///     to the beginning of a next line.
        ///     Both exceptions can be controlled by boolean parameters:
        ///     allowStopAtLineEnd and allowStopNearSpace
        /// </param>
        /// <param name="allowStopAtLineEnd">
        ///     True allows Backward normalization even when the caretPsotion is at
        ///     line wrapping, so that visually it will appear at the end of a first
        ///     of wrapped lines
        /// </param>
        /// <param name="allowStopNearSpace">
        ///     True allows normalization towards a space, event when the caretPosition is
        ///     at formatting switching position.
        /// </param>
        void SetCaretToPosition(ITextPointer caretPosition,
                                LogicalDirection direction,
                                Boolean allowStopAtLineEnd,
                                Boolean allowStopNearSpace);

        /// <summary>
        ///     Selects a content starting from a previous anchorPosition to the given
        ///     textPosition.
        ///     Sets visual orientation of a position to textPosition.LogicalDirection.
        /// </summary>
        void ExtendToPosition(ITextPointer textPosition);

        /// <summary>
        /// </summary>
        Boolean ExtendToNextInsertionPosition(LogicalDirection direction);

        // Returns true if the text matching a pixel position falls within
        // the selection.
        Boolean Contains(Point point);

        //......................................................
        //
        //  Interaction with Selection host
        //
        //......................................................

        // This section must go away when TextEditor merged with TextSelection)

        // Called by TextEditor.OnDetach, when the behavior is shut down.
        void OnDetach();

        // Called by Got/LostKeyboardFocus and LostFocusedElement handlers
        void UpdateCaretAndHighlight();

        // ITextView.Updated event listener.
        // Called by the TextEditor.
        void OnTextViewUpdated();

        // Perform any cleanup necessary when removing the current UiScope
        // from the visual tree (eg, during a template change).
        void DetachFromVisualTree();

        void RefreshCaret();

        void OnInterimSelectionChanged(Boolean interimSelection);

        //......................................................
        //
        //  Selection Heuristics
        //
        //......................................................

        // Moves the selection to the mouse cursor position.
        // Extends the active end if extend == true, otherwise the selection
        // is collapsed to a caret.
        void SetSelectionByMouse(ITextPointer cursorPosition,
                                 Point cursorMousePoint);

        // Extends the selection to the mouse cursor position.
        void ExtendSelectionByMouse(ITextPointer cursorPosition,
                                    Boolean forceWordSelection,
                                    Boolean forceParagraphSelection);

        //......................................................
        //
        //  Table Selection
        //
        //......................................................

        /// <summary>
        ///     Extends table selection by one row in a given direction
        /// </summary>
        /// <param name="direction">
        ///     LogicalDirection.Forward means moving active cell one row down,
        ///     LogicalDirection.Backward - one row up.
        /// </param>
        Boolean ExtendToNextTableRow(LogicalDirection direction);

        // Called after a caret navigation, to signal that the next caret
        // scroll-into-view should include hueristics to include following
        // text.
        void OnCaretNavigation();

        // Forces a synchronous layout validation, up to the selection moving position.
        void ValidateLayout();

        #endregion Internal Methods

        //------------------------------------------------------
        //
        //  Internal Properties
        //
        //------------------------------------------------------

        #region Internal Properties

        ITextEditor TextEditor { get; }

        ITextView TextView { get; }

        // True if the current seleciton is for interim character.
        // Korean Interim character is now invisilbe selection (no highlight) and the controls needs to 
        // have the block caret to indicate the interim character.
        // This should be updated by TextStore.
        Boolean IsInterimSelection { get; }

        /// <summary>
        ///     Static end of a selection.
        ///     LogicalDirection of this pointer is an orientation of a visual caret
        /// </summary>
        /// <returns></returns>
        ITextPointer AnchorPosition { get; }

        /// <summary>
        ///     Active end of a selection.
        ///     LogicalDirection of this pointer is an orientation of a visual caret
        /// </summary>
        /// <returns></returns>
        ITextPointer MovingPosition { get; }

        // Caret associated with this TextSelection.
        ICaretElement CaretElement { get; }

        // Returns true iff there are no additional insertion positions are either
        // end of the selection.
        Boolean CoversEntireContent { get; }

        #endregion Internal Properties
    }
}