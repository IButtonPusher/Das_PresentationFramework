using System;
using System.Threading.Tasks;
using Das.Views.Core;
using Das.Views.Input.Text;
using Das.Views.Input.Text.Pointers;
using Das.Views.Validation;

namespace Das.Views.Undo
{
    // Undo unit for TextContainer.InsertText and InsertEmbeddedObject calls.
    public class TextTreeInsertUndoUnit : TextTreeUndoUnit
    {
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

        // Create a new undo unit instance.
        // symbolOffset and symbolCount track the offset of the inserted content
        // and its symbol count, respectively.
        public TextTreeInsertUndoUnit(TextContainer tree,
                                      Int32 symbolOffset,
                                      Int32 symbolCount) : base(tree, symbolOffset)
        {
            Invariant.Assert(symbolCount > 0, "Creating no-op insert undo unit!");

            _symbolCount = symbolCount;
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

            VerifyTreeContentHashCode();

            start = new TextPointer(TextContainer, SymbolOffset, LogicalDirection.Forward);
            end = new TextPointer(TextContainer, SymbolOffset + _symbolCount, LogicalDirection.Forward);

            TextContainer.DeleteContentpublic(start, end);
        }

        #endregion Public Methods

        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        // Count of symbols to remove.
        private readonly Int32 _symbolCount;

        #endregion Private Fields
    }
}
