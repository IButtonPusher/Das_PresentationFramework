using System;
using System.Threading.Tasks;
using Das.Views.Input.Text.Pointers;
using Das.Views.Validation;

namespace Das.Views.Input.Text.Tree
{
    public class TextTreeRootTextBlock : SplayTreeNode
    {
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

        // Creates a TextTreeRootTextBlock instance.
        public TextTreeRootTextBlock()
        {
            TextTreeTextBlock block;

            // Allocate an initial block with just two characters -- one for
            // each edge of the root node.  The block will grow when/if
            // additional content is added.
            block = new TextTreeTextBlock(2);
            block.InsertAtNode(this, ElementEdge.AfterStart);
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
            return "RootTextBlock Id=" + DebugId;
        }
        #endif // DEBUG

        #endregion Public Methods

        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        // Root node of a contained tree, if any.
        private TextTreeTextBlock _containedNode;

        #endregion Private Fields

        //------------------------------------------------------
        //
        //  public Properties
        //
        //------------------------------------------------------

        #region public Properties

        // The root node never has a parent node.
        public override SplayTreeNode ParentNode
        {
            get => null;

            set => Invariant.Assert(false, "Can't set ParentNode on TextBlock root!");
        }

        // Root node of a contained tree, if any.
        public override SplayTreeNode ContainedNode
        {
            get => _containedNode;

            set => _containedNode = (TextTreeTextBlock)value;
        }

        // The root node never has sibling nodes, so the LeftSymbolCount is a
        // constant zero.
        public override Int32 LeftSymbolCount
        {
            get => 0;

            set => Invariant.Assert(false, "TextContainer root is never a sibling!");
        }

        // Count of unicode chars of all siblings preceding this node.
        // This property is only used by TextTreeNodes.
        public override Int32 LeftCharCount
        {
            get => 0;

            set => Invariant.Assert(value == 0);
        }

        // The root node never has siblings, so it never has child nodes.
        public override SplayTreeNode LeftChildNode
        {
            get => null;

            set => Invariant.Assert(false, "TextBlock root never has sibling nodes!");
        }

        // The root node never has siblings, so it never has child nodes.
        public override SplayTreeNode RightChildNode
        {
            get => null;

            set => Invariant.Assert(false, "TextBlock root never has sibling nodes!");
        }

        // The tree generation.  Not used for TextTreeRootTextBlock.
        public override UInt32 Generation
        {
            get => 0;

            set => Invariant.Assert(false, "TextTreeRootTextBlock does not track Generation!");
        }

        // Cached symbol offset.  The root node is always at offset zero.
        public override Int32 SymbolOffsetCache
        {
            get => 0;

            set => Invariant.Assert(false, "TextTreeRootTextBlock does not track SymbolOffsetCache!");
        }

        // Not used for TextTreeRootTextBlock.
        public override Int32 SymbolCount
        {
            get => -1;

            set => Invariant.Assert(false, "TextTreeRootTextBlock does not track symbol count!");
        }

        // Count of unicode chars covered by this node and any contained nodes.
        // This property is only used by TextTreeNodes.
        public override Int32 IMECharCount
        {
            get => 0;

            set => Invariant.Assert(value == 0);
        }

        #endregion public Properties
    }
}
