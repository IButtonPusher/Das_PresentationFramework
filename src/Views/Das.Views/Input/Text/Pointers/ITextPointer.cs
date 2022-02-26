using System;
using System.Drawing;
using System.Threading.Tasks;
using Das.Views.Collections;
using Das.Views.Core;
using Das.Views.DataBinding;

namespace Das.Views.Input.Text.Pointers
{
    public interface ITextPointer
    {
        //------------------------------------------------------
        //
        //  Internal Methods
        //
        //------------------------------------------------------

        

        // Constructor.
        ITextPointer CreatePointer();

        // Constructor.
        StaticTextPointer CreateStaticPointer();

        // Constructor.
        ITextPointer CreatePointer(Int32 offset);

        // Constructor.
        ITextPointer CreatePointer(LogicalDirection gravity);

        // Constructor.
        ITextPointer CreatePointer(Int32 offset,
                                   LogicalDirection gravity);

        // Property accessor.
        void SetLogicalDirection(LogicalDirection direction);

        // Returns
        //  -1 if this ITextPointer is positioned before position.
        //   0 if this ITextPointer is positioned at position.
        //  +1 if this ITextPointer is positioned after position.
        Int32 CompareTo(ITextPointer position);

        Int32 CompareTo(StaticTextPointer position);

        // Returns true if this ITextPointer has the same logical parent has position.
        Boolean HasEqualScope(ITextPointer position);

        // <see cref="TextPointer.GetPointerContext"/>
        TextPointerContext GetPointerContext(LogicalDirection direction);

        // <see cref="TextPointer.GetOffsetToPosition"/>
        Int32 GetOffsetToPosition(ITextPointer position);

        // <see cref="TextPointer.GetTextRunLength"/>
        Int32 GetTextRunLength(LogicalDirection direction);

        // <see cref="TextPointer.GetTextInRun"/>
        String GetTextInRun(LogicalDirection direction);

        // <see cref="TextPointer.GetTextInRun"/>
        Int32 GetTextInRun(LogicalDirection direction,
                           Char[] textBuffer,
                           Int32 startIndex,
                           Int32 count);

        // <see cref="TextPointer.GetAdjacentElement"/>
        //  this should return DependencyObject (which is
        // either ContentElement or UIElement) for consistency with TextPointer.GetAdjacentElement.
        // Blocking issue: DocumentSequenceTextPointer returns an object to break
        // pages.
        Object GetAdjacentElement(LogicalDirection direction);

        // <see cref="TextPointer.MoveToPosition"/>
        void MoveToPosition(ITextPointer position);

        // <see cref="TextPointer.MoveByOffset"/>
        Int32 MoveByOffset(Int32 offset);

        // <see cref="TextPointer.MoveToNextContextPosition"/>
        Boolean MoveToNextContextPosition(LogicalDirection direction);

        // <see cref="TextPointer.GetNextContextPosition"/>
        ITextPointer GetNextContextPosition(LogicalDirection direction);

        // <see cref="TextPointer.MoveToInsertionPosition"/>
        Boolean MoveToInsertionPosition(LogicalDirection direction);

        // <see cref="TextPointer.GetInsertionPosition"/>
        ITextPointer GetInsertionPosition(LogicalDirection direction);

        // <see cref="TextPointer.GetFormatNormalizedPosition"/>
        ITextPointer GetFormatNormalizedPosition(LogicalDirection direction);

        // <see cref="TextPointer.MoveToNextInsertionPosition"/>
        Boolean MoveToNextInsertionPosition(LogicalDirection direction);

        // <see cref="TextPointer.GetNextInsertionPosition"/>
        ITextPointer GetNextInsertionPosition(LogicalDirection direction);

        // Moves this ITextPointer to the specified edge of the parent text element.
        void MoveToElementEdge(ElementEdge edge);

        // <see cref="TextPointer.MoveToLineStart"/>
        Int32 MoveToLineBoundary(Int32 count);

        // <see cref="TextPointer.GetCharacterRect"/>
        Rectangle GetCharacterRect(LogicalDirection direction);

        // <see cref="TextPointer.Freeze"/>
        void Freeze();

        // <see cref="TextPointer.GetFrozenPointer"/>
        ITextPointer GetFrozenPointer(LogicalDirection logicalDirection);

        // <see cref="TextPointer.InsertText"/>
        void InsertTextInRun(String textData);

        // <see cref="TextPointer.DeleteContentToPosition"/>
        void DeleteContentToPosition(ITextPointer limit);

        // <see cref="TextPointer.GetTextElement"/>
        // rename this method to match eventual TextPointer equivalent.
        Type GetElementType(LogicalDirection direction);

        // Returns a DP value on this ITextPointer's logical parent.
        Object GetValue(DependencyProperty formattingProperty);

        // Returns a local DP value on this ITextPointer's logical parent.
        Object ReadLocalValue(DependencyProperty formattingProperty);

        // Returns all local values on this ITextPointer's logical parent.
        LocalValueEnumerator GetLocalValueEnumerator();

        /// <summary>
        ///     Ensures layout information is available at this position.
        /// </summary>
        /// <returns>
        ///     True if the position is validated, false otherwise.
        /// </returns>
        /// <remarks>
        ///     Use this method before calling GetCharacterRect, MoveToLineBoundary,
        ///     IsAtLineStartPosition.
        ///     This method can be very expensive.  To detect an invalid layout
        ///     without actually doing any work, use the HasValidLayout property.
        /// </remarks>
        Boolean ValidateLayout();

        void SyncToTreeGeneration();

        //------------------------------------------------------
        //
        //  Internal Properties
        //
        //------------------------------------------------------

        IBindableElement Parent { get; }

        // Associated TextContainer.
        ITextContainer TextContainer { get; }

        // <see cref="TextPointer.HasValidLayout"/>
        Boolean HasValidLayout { get; }

        // Returns true if this pointer is at a caret unit boundary.
        // Logically equivalent to this.TextContainer.TextView.IsAtCaretUnitBoundary(this),
        // but some implementations may have better performance.
        //
        // This method must not be called unless HasValidLayout == true.
        Boolean IsAtCaretUnitBoundary { get; }

        // <see cref="TextPointer.LogicalDirection"/>
        LogicalDirection LogicalDirection { get; }

        // <see cref="TextPointer.Parent"/>
        Type ParentType { get; }

        // <see cref="TextPointer.ParentContentStart"/>
        //ITextPointer ParentContentStart { get; }

        // <see cref="TextPointer.ParentContentEnd"/>
        //ITextPointer ParentContentEnd { get; }

        // <see cref="TextPointer.ContainerContentStart"/>
        //ITextPointer ContainerContentStart { get; }

        // <see cref="TextPointer.ContainerContentEnd"/>
        //ITextPointer ContainerContentEnd { get; }

        // <see cref="TextPointer.IsAtInsertionPosition"/>
        Boolean IsAtInsertionPosition { get; }

        // <see cref="TextPointer.IsFrozen"/>
        Boolean IsFrozen { get; }

        // Offset from the TextContainer.Start position.
        // Equivalent to this.TextContainer.Start.GetOffsetToPosition(this),
        // but doesn't necessarily allocate anything.
        Int32 Offset { get; }

        // Offset in unicode chars within the document.
        //  this should probably be refactored out of ITextPointer
        // since only TextStore supports it.
        Int32 CharOffset { get; }

        
    }
}
