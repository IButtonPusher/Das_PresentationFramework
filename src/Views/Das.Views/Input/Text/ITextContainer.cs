using System;
using System.Threading.Tasks;
using Das.Views.Collections;
using Das.Views.Core;
using Das.Views.DataBinding;
using Das.Views.Input.Text.Events;
using Das.Views.Input.Text.Pointers;
using Das.Views.Input.Text.Tree;
using Das.Views.Text;
using Das.Views.Undo;

namespace Das.Views.Input.Text
{
    public interface ITextContainer
    {
        void BeginChange();

        // Like BeginChange, but does not ever create an undo unit.
        // This method is called before UndoManager.Undo, and can't have
        // an open undo unit while running Undo.
        void BeginChangeNoUndo();

        void EndChange();

        void EndChange(Boolean skipEvents);

        // Allocate a new ITextPointer at the specified offset.
        // Equivalent to this.Start.CreatePointer(offset), but does not
        // necessarily allocate this.Start.
        ITextPointer CreatePointerAtOffset(Int32 offset,
                                           LogicalDirection direction);

        // Allocate a new ITextPointer at a specificed offset in unicode chars within the document.
        //  this should probably be refactored out of ITextContainer
        // since only TextStore supports it.
        ITextPointer CreatePointerAtCharOffset(Int32 charOffset,
                                               LogicalDirection direction);

        ITextPointer CreateDynamicTextPointer(StaticTextPointer position,
                                              LogicalDirection direction);

        StaticTextPointer CreateStaticPointerAtOffset(Int32 offset);

        TextPointerContext GetPointerContext(StaticTextPointer pointer,
                                             LogicalDirection direction);

        Int32 GetOffsetToPosition(StaticTextPointer position1,
                                  StaticTextPointer position2);

        Int32 GetTextInRun(StaticTextPointer position,
                           LogicalDirection direction,
                           Char[] textBuffer,
                           Int32 startIndex,
                           Int32 count);

        Object GetAdjacentElement(StaticTextPointer position,
                                  LogicalDirection direction);

        IBindableElement GetParent(StaticTextPointer position);

        StaticTextPointer CreatePointer(StaticTextPointer position,
                                        Int32 offset);

        StaticTextPointer GetNextContextPosition(StaticTextPointer position,
                                                 LogicalDirection direction);

        Int32 CompareTo(StaticTextPointer position1,
                        StaticTextPointer position2);

        Int32 CompareTo(StaticTextPointer position1,
                        ITextPointer position2);

        Object GetValue(StaticTextPointer position,
                        DependencyProperty formattingProperty);

        void GetNodeAndEdgeAtOffset(Int32 offset,
                                    Boolean splitNode,
                                    out SplayTreeNode node,
                                    out ElementEdge edge);

        void GetNodeAndEdgeAtOffset(Int32 offset,
                                    out SplayTreeNode node,
                                    out ElementEdge edge);

        void AssertTree();

        void EmptyDeadPositionList();

        void InsertText(TextPointer position,
                        Object text);

        void SetValues(ITextPointer position,
                       LocalValueEnumerator values);

        UInt32 PositionGeneration { get; }

        Int32 InternalSymbolCount { get; }


        //------------------------------------------------------
        //
        //  Internal Properties
        //
        //------------------------------------------------------


        /// <summary>
        ///     Specifies whether or not the content of this TextContainer may be
        ///     modified.
        /// </summary>
        /// <value>
        ///     True if content may be modified, false otherwise.
        /// </value>
        /// <remarks>
        ///     Methods that modify the TextContainer, such as InsertText or
        ///     DeleteContent, will throw InvalidOperationExceptions if this
        ///     property returns true.
        /// </remarks>
        Boolean IsReadOnly { get; }

        /// <summary>
        ///     A position preceding the first symbol of this TextContainer.
        /// </summary>
        /// <remarks>
        ///     The returned ITextPointer has LogicalDirection.Backward gravity.
        /// </remarks>
        ITextPointer Start { get; }

        /// <summary>
        ///     A position following the last symbol of this TextContainer.
        /// </summary>
        /// <remarks>
        ///     The returned ITextPointer has LogicalDirection.Forward gravity.
        /// </remarks>
        ITextPointer End { get; }

        /// <summary>
        ///     The object containing this TextContainer, from which property
        ///     values are inherited.
        /// </summary>
        /// <remarks>
        ///     May be null.
        /// </remarks>
        IBindableElement Parent { get; }

        /// <summary>
        ///     Collection of highlights applied to TextContainer content.
        /// </summary>
        Highlights Highlights { get; }

        // Optional text selection, may be null if there's no TextEditor
        // associated with an ITextContainer.
        // TextEditor owns setting and clearing this property inside its
        // ctor/OnDetach methods.
        //
        // 9/29/05: It may be possible to remove this property
        // by relying on a tree walk up from the Parent property looking
        // for an attached TextEditor (which maps 1-to-1 with TextSelection).
        ITextSelection TextSelection { get; set; }

        // Optional undo manager, may be null.
        UndoManager UndoManager { get; }

        // Optional TextView, may be null.
        // When several views are nested, this view is always the "top-level"
        // view, the one used by the TextEditor.
        ITextView TextView { get; set; }

        // Count of symbols in this tree, equivalent to this.Start.GetOffsetToPosition(this.End),
        // but doesn't necessarily allocate anything.
        Int32 SymbolCount { get; }

        // Count of unicode characters in the tree.
        //  this should probably be refactored out of ITextContainer
        // since only TextStore supports it.
        Int32 IMECharCount { get; }

        // Autoincremented counter of content changes in this TextContainer
        UInt32 Generation { get; }


        //------------------------------------------------------
        //
        //  Internal Events
        //
        //------------------------------------------------------


        // Fired before each edit to the ITextContainer.
        // This event is useful to flag reentrant calls to the listener while the
        // container is being modified (via logical tree events) before the
        // matching Change event is fired.
        // Listener has READ-ONLY access to the ITextContainer inside the scope
        // of the callback.
        event EventHandler Changing;

        // Fired on each edit.
        // Listener has READ-ONLY access to the ITextContainer inside the scope
        // of the callback.
        event TextContainerChangeEventHandler Change;

        // Fired once as a change block exits -- after one or more edits.
        // It is legal to modify the ITextContainer inside this event.
        event TextContainerChangedEventHandler Changed;
    }
}
