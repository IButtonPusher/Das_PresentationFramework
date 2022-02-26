using System;
using System.Threading.Tasks;
using Das.Views.Core;
using Das.Views.Input.Text.Pointers;
using Das.Views.Validation;

namespace Das.Views.Input.Text.Tree
{
    // Each TextElement inserted though a public API is represented publicly
    // by a TextTreeTextElementNode.
    //
    // TextTreeTextElementNodes may contain trees of child nodes, nodes in
    // a contained tree are scoped by the element node.
    public class TextTreeTextElementNode : TextTreeNode
    {
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

        // Creates a new instance.
        public TextTreeTextElementNode()
        {
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
            return "TextElementNode Id=" + DebugId + " SymbolCount=" + _symbolCount;
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
            TextTreeTextElementNode clone;

            clone = new TextTreeTextElementNode();
            clone._symbolCount = _symbolCount;
            clone._imeCharCount = _imeCharCount;
            clone._textElement = _textElement;

            return clone;
        }

        // Returns the TextPointerContext of the node.
        // Returns ElementStart if direction == Forward, otherwise ElementEnd
        // if direction == Backward.
        public override TextPointerContext GetPointerContext(LogicalDirection direction)
        {
            return direction == LogicalDirection.Forward
                ? TextPointerContext.ElementStart
                : TextPointerContext.ElementEnd;
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

        // Root node of a contained tree, if any.
        public override SplayTreeNode ContainedNode
        {
            get => _containedNode;

            set => _containedNode = (TextTreeNode)value;
        }

        // Count of symbols of all siblings preceding this node.
        public override Int32 LeftSymbolCount
        {
            get => _leftSymbolCount;

            set => _leftSymbolCount = value;
        }

        // Count of chars of all siblings preceding this node.
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

        // Count of symbols covered by this node and any contained nodes.
        // Includes two symbols for this node's start/end edges.
        public override Int32 SymbolCount
        {
            get => _symbolCount;

            set => _symbolCount = value;
        }

        // Count of chars covered by this node and any contained nodes.
        public override Int32 IMECharCount
        {
            get => _imeCharCount;

            set => _imeCharCount = value;
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
        // Since nodes don't usually have any references, we demand allocate
        // storage when needed.
        public override Boolean AfterStartReferenceCount
        {
            get => (_edgeReferenceCounts & ElementEdge.AfterStart) != 0;

            set
            {
                Invariant.Assert(value); // Illegal to clear a set ref count.
                _edgeReferenceCounts |= ElementEdge.AfterStart;
            }
        }

        // Count of TextPositions referencing the node's BeforeEnd edge.
        // Since nodes don't usually have any references, we demand allocate
        // storage when needed.
        public override Boolean BeforeEndReferenceCount
        {
            get => (_edgeReferenceCounts & ElementEdge.BeforeEnd) != 0;

            set
            {
                Invariant.Assert(value); // Illegal to clear a set ref count.
                _edgeReferenceCounts |= ElementEdge.BeforeEnd;
            }
        }

        // Count of TextPositions referencing the node's AfterEnd edge.
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

        // The TextElement associated with this node.
        public ITextElement TextElement
        {
            get => _textElement;

            set => _textElement = value;
        }

        // Plain text character count of this element's start edge.
        // This property depends on the current location of the node!
        public Int32 IMELeftEdgeCharCount => _textElement == null ? -1 : _textElement.IMELeftEdgeCharCount;

        // Returns true if this node is the leftmost sibling of its parent.
        public Boolean IsFirstSibling
        {
            get
            {
                Splay();
                return _leftChildNode == null;
            }
        }

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

        // Root node of a contained tree, if any.
        private TextTreeNode _containedNode;

        // The TextContainer's generation when SymbolOffsetCache was last updated.
        // If the current generation doesn't match TextContainer.Generation, then
        // SymbolOffsetCache is invalid.
        private UInt32 _generation;

        // Cached symbol offset.
        private Int32 _symbolOffsetCache;

        // Count of symbols covered by this node and any contained nodes.
        // Includes two symbols for this node's start/end edges.
        private Int32 _symbolCount;

        // Count of chars covered by this node and any contained nodes.
        private Int32 _imeCharCount;

        // The TextElement associated with this node.
        private ITextElement _textElement;

        // Reference counts of TextPositions referencing this node.
        // Lazy allocated -- null means no references.
        private ElementEdge _edgeReferenceCounts;

        #endregion Private Fields
    }
}
