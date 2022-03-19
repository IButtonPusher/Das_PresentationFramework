﻿using System;
using System.Threading.Tasks;
using Das.Views.Core;
using Das.Views.Input.Text;
using Das.Views.Input.Text.Pointers;
using Das.Views.Validation;

namespace Das.Views.Undo
{
    // Undo unit for TextContainer.InsertElement calls.
    public class TextTreeInsertElementUndoUnit : TextTreeUndoUnit
    {
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

        // Creates a new undo unit instance.
        // symbolOffset should be just before the start edge of the TextElement to remove.
        // If deep is true, this unit will undo not only the scoping element
        // insert, but also all content scoped by the element.
        public TextTreeInsertElementUndoUnit(TextContainer tree,
                                             Int32 symbolOffset,
                                             Boolean deep) : base(tree, symbolOffset)
        {
            _deep = deep;
        }

        #endregion Constructors

        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        // Called by the undo manager.  Restores tree state to its condition
        // when the unit was created.  Assumes the tree state matches conditions
        // just after the unit was created.
        public override void DoCore()
        {
            TextPointer start;
            TextPointer end;
            ITextElement? element;

            VerifyTreeContentHashCode();

            start = new TextPointer(TextContainer, SymbolOffset, LogicalDirection.Forward);

            Invariant.Assert(start.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementStart,
                "TextTree undo unit out of sync with TextTree.");

            element = start.GetAdjacentElementFromOuterPosition(LogicalDirection.Forward);

            if (_deep)
            {
                // Extract the element and its content.
                end = new TextPointer(TextContainer, element.TextElementNode, ElementEdge.AfterEnd);
                TextContainer.DeleteContentpublic(start, end);
            }
            else
            {
                // Just extract the element, not its content.
                TextContainer.ExtractElementInternal(element);
            }
        }

        #endregion Public Methods

        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        // If true, this unit tracks a TextElement and its scoped content.
        // Otherwise, this unit only tracks the TextElement.
        private readonly Boolean _deep;

        #endregion Private Fields
    }
}