using System;
using System.Threading.Tasks;
using Das.Views.Core;
using Das.Views.Data;
using Das.Views.Input.Text;
using Das.Views.Input.Text.Pointers;
using Das.Views.Input.Text.Tree;

namespace Das.Views.Undo
{
    public class TextTreeExtractElementUndoUnit : TextTreeUndoUnit
    {
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

        // Creates a new undo unit instance.
        public TextTreeExtractElementUndoUnit(TextContainer tree,
                                              TextTreeTextElementNode elementNode)
            : base(tree, elementNode.GetSymbolOffset(tree.Generation))
        {
            _symbolCount = elementNode.SymbolCount;
            _type = elementNode.TextElement.GetType();
            _localValues = GetPropertyValueArray(elementNode.TextElement);
            //_resources = elementNode.TextElement.Resources;

            // Table requires additional work for storing its Columns collection
            //if (elementNode.TextElement is Table)
            //{
            //    _columns = TextTreeDeleteContentUndoUnit.SaveColumns((Table)elementNode.TextElement);
            //}
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
            ITextElement element;

            VerifyTreeContentHashCode();

            start = new TextPointer(TextContainer, SymbolOffset, LogicalDirection.Forward);
            end = new TextPointer(TextContainer, SymbolOffset + _symbolCount - 2, LogicalDirection.Forward);

            // Insert a new element.
            element = (ITextElement)Activator.CreateInstance(_type);
            element.Reposition(start, end);

            // Restore local resources
            //element.Resources = _resources;

            // Move end into the scope of the new element.
            end.MoveToNextContextPosition(LogicalDirection.Backward);
            // Then restore local property values.
            // Shouldn't we call this with deferLoad=true and call EndDeferLoad after all parameters set?
            TextContainer.SetValues(end, ArrayToLocalValueEnumerator(_localValues));

            //if (element is Table)
            //{
            //    TextTreeDeleteContentUndoUnit.RestoreColumns((Table)element, _columns);
            //}
        }

        #endregion Public Methods

        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        // Count of symbols covered by the extracted TextElement, including its 2 edges.
        private readonly Int32 _symbolCount;

        // Type of the extracted TextElement.
        private readonly Type _type;

        // Collection of all local property values set on the extracted TextElement.
        private readonly PropertyValue[] _localValues;

        // Resources defined locally on the TextElement
        //private readonly ResourceDictionary _resources;

        // TableColumns collection
        //private readonly TableColumn[] _columns;

        #endregion Private Fields
    }
}
