using System;
using System.Threading.Tasks;
using Das.Views.Core;
using Das.Views.DataBinding;
using Das.Views.Input.Text.Pointers;
using Das.Views.Validation;

namespace Das.Views.Input.Text.Tree
{
    public class TextTreeObjectNode : TextTreeNode
    {
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

        // Creates a new TextTreeObjectNode instance.
        public TextTreeObjectNode(IBindableElement embeddedElement)
        {
            _embeddedElement = embeddedElement;
            _symbolOffsetCache = -1;
        }

        #endregion Constructors

        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        #if DEBUG
        // Debug-only ToString override.
        public override String ToString()
        {
            return "ObjectNode Id=" + DebugId + " Object=" + _embeddedElement;
        }
        #endif // DEBUG

        #endregion Public Methods

        //------------------------------------------------------
        //
        //  public Methods
        //
        //------------------------------------------------------

        #region public Methods

        // Returns a shallow copy of this node.
        public override TextTreeNode Clone()
        {
            TextTreeObjectNode clone;

            clone = new TextTreeObjectNode(_embeddedElement);

            return clone;
        }

        // Returns the TextPointerContext of the node.
        public override TextPointerContext GetPointerContext(LogicalDirection direction)
        {
            return TextPointerContext.EmbeddedElement;
        }

        #endregion public methods

        //------------------------------------------------------
        //
        //  public Properties
        //
        //------------------------------------------------------

        #region public Properties

        // If this node is a local root, then ParentNode contains it.
        // Otherwise, this is the node parenting this node within its tree.
        public override SplayTreeNode ParentNode
        {
            get => _parentNode;

            set => _parentNode = (TextTreeNode)value;
        }

        // TextTreeObjectNode never has contained nodes.
        public override SplayTreeNode ContainedNode
        {
            get => null;

            set => Invariant.Assert(false, "Can't set contained node on a TextTreeObjectNode!");
        }

        // Count of symbols of all siblings preceding this node.
        public override Int32 LeftSymbolCount
        {
            get => _leftSymbolCount;

            set => _leftSymbolCount = value;
        }

        // Count of symbols of all siblings preceding this node.
        public override Int32 LeftCharCount
        {
            get => _leftCharCount;

            set => _leftCharCount = value;
        }

        // Left child node in a sibling tree.
        public override SplayTreeNode LeftChildNode
        {
            get => _leftChildNode;

            set => _leftChildNode = (TextTreeNode)value;
        }

        // Right child node in a sibling tree.
        public override SplayTreeNode RightChildNode
        {
            get => _rightChildNode;

            set => _rightChildNode = (TextTreeNode)value;
        }

        // The TextContainer's generation when SymbolOffsetCache was last updated.
        // If the current generation doesn't match TextContainer.Generation, then
        // SymbolOffsetCache is invalid.
        public override UInt32 Generation
        {
            get => _generation;

            set => _generation = value;
        }

        // Cached symbol offset.
        public override Int32 SymbolOffsetCache
        {
            get => _symbolOffsetCache;

            set => _symbolOffsetCache = value;
        }

        // Count of symbols covered by this node.
        public override Int32 SymbolCount
        {
            get => 1;

            set => Invariant.Assert(false, "Can't set SymbolCount on TextTreeObjectNode!");
        }

        // Count of symbols covered by this node.
        public override Int32 IMECharCount
        {
            get => 1;

            set => Invariant.Assert(false, "Can't set CharCount on TextTreeObjectNode!");
        }

        // Count of TextPositions referencing the node's BeforeStart edge.
        // Since nodes don't usually have any references, we demand allocate
        // storage when needed.
        public override Boolean BeforeStartReferenceCount
        {
            get => (_edgeReferenceCounts & ElementEdge.BeforeStart) != 0;

            set
            {
                Invariant.Assert(value); // Illegal to clear a set ref count.
                _edgeReferenceCounts |= ElementEdge.BeforeStart;
            }
        }

        // Count of TextPositions referencing the node's AfterStart edge.
        // Since object nodes don't have an AfterStart edge, this is always zero.
        public override Boolean AfterStartReferenceCount
        {
            get => false;

            set => Invariant.Assert(false, "Object nodes don't have an AfterStart edge!");
        }

        // Count of TextPositions referencing the node's BeforeEnd edge.
        // Since object nodes don't have an BeforeEnd edge, this is always zero.
        public override Boolean BeforeEndReferenceCount
        {
            get => false;

            set => Invariant.Assert(false, "Object nodes don't have a BeforeEnd edge!");
        }

        // Count of TextPositions referencing the node's right
        // edge.
        // Since nodes don't usually have any references, we demand allocate
        // storage when needed.
        public override Boolean AfterEndReferenceCount
        {
            get => (_edgeReferenceCounts & ElementEdge.AfterEnd) != 0;

            set
            {
                Invariant.Assert(value); // Illegal to clear a set ref count.
                _edgeReferenceCounts |= ElementEdge.AfterEnd;
            }
        }

        // The UIElement or ContentElement linked to this node.
        public IBindableElement EmbeddedElement => _embeddedElement;

        #endregion public Properties

        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        // Count of symbols of all siblings preceding this node.
        private Int32 _leftSymbolCount;

        // Count of chars of all siblings preceding this node.
        private Int32 _leftCharCount;

        // If this node is a local root, then ParentNode contains it.
        // Otherwise, this is the node parenting this node within its tree.
        private TextTreeNode _parentNode;

        // Left child node in a sibling tree.
        private TextTreeNode _leftChildNode;

        // Right child node in a sibling tree.
        private TextTreeNode _rightChildNode;

        // The TextContainer's generation when SymbolOffsetCache was last updated.
        // If the current generation doesn't match TextContainer.Generation, then
        // SymbolOffsetCache is invalid.
        private UInt32 _generation;

        // Cached symbol offset.
        private Int32 _symbolOffsetCache;

        // Reference counts of TextPositions referencing this node.
        // Lazy allocated -- null means no references.
        private ElementEdge _edgeReferenceCounts;

        // The UIElement or ContentElement linked to this node.
        private readonly IBindableElement _embeddedElement;

        #endregion Private Fields
    }
}
