using System;
using System.Threading.Tasks;
using Das.Views.Core;
using Das.Views.Input.Text.Pointers;
using Das.Views.Validation;

namespace Das.Views.Input.Text.Tree
{
    // TextTreeFixupNodes never actually appear in live trees.  However,
    // whenever nodes are removed from the tree, we parent them to a fixup
    // node whose job it is to serve as a guide for any orphaned TextPositions
    // that might later need to find their way back to the original tree.
    public class TextTreeFixupNode : TextTreeNode
    {
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
            return "FixupNode Id=" + DebugId;
        }
        #endif // DEBUG

        #endregion Public Methods

        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

        // Creates a new TextTreeFixupNode instance.
        // previousNode/Edge should point to the node TextPositions will
        // move to after synchronizing against the deleted content.
        public TextTreeFixupNode(TextTreeNode previousNode,
                                 ElementEdge previousEdge,
                                 TextTreeNode nextNode,
                                 ElementEdge nextEdge) :
            this(previousNode, previousEdge, nextNode, nextEdge, null, null)
        {
        }

        // Creates a new TextTreeFixupNode instance.
        // This ctor should only be called when extracting a single TextTreeTextElementNode.
        // previousNode/Edge should point to the node TextPositions will
        // move to after synchronizing against the deleted content.
        // first/lastContainedNode point to the first and last contained nodes
        // of an extracted element node.  Positions may move into these nodes.
        public TextTreeFixupNode(TextTreeNode previousNode,
                                 ElementEdge previousEdge,
                                 TextTreeNode nextNode,
                                 ElementEdge nextEdge,
                                 TextTreeNode firstContainedNode,
                                 TextTreeNode lastContainedNode)
        {
            _previousNode = previousNode;
            _previousEdge = previousEdge;
            _nextNode = nextNode;
            _nextEdge = nextEdge;
            _firstContainedNode = firstContainedNode;
            _lastContainedNode = lastContainedNode;
        }

        #endregion Constructors

        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        //------------------------------------------------------
        //
        //  Public Events
        //
        //------------------------------------------------------

        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        //------------------------------------------------------
        //
        //  Internal Methods
        //
        //------------------------------------------------------

        #region Internal Methods

        // Returns a shallow copy of this node.
        public override TextTreeNode Clone()
        {
            Invariant.Assert(false, "Unexpected call to TextTreeFixupNode.Clone!");
            return null;
        }

        // Returns the TextPointerContext of the node.
        // Because fixup nodes are never in live trees, we should never get here.
        public override TextPointerContext GetPointerContext(LogicalDirection direction)
        {
            Invariant.Assert(false, "Unexpected call to TextTreeFixupNode.GetPointerContext!");
            return TextPointerContext.None;
        }

        #endregion Internal methods

        //------------------------------------------------------
        //
        //  Internal Properties
        //
        //------------------------------------------------------

        #region Internal Properties

        // Fixup nodes never have parents.
        public override SplayTreeNode ParentNode
        {
            get => null;

            set => Invariant.Assert(false, "FixupNode");
        }

        // Fixup nodes never contain nodes.
        public override SplayTreeNode ContainedNode
        {
            get => null;

            set => Invariant.Assert(false, "FixupNode");
        }

        // Fixup nodes don't have symbol counts.
        public override Int32 LeftSymbolCount
        {
            get => 0;

            set => Invariant.Assert(false, "FixupNode");
        }

        // Fixup nodes don't have char counts.
        public override Int32 LeftCharCount
        {
            get => 0;

            set => Invariant.Assert(false, "FixupNode");
        }

        // Fixup nodes don't have siblings.
        public override SplayTreeNode LeftChildNode
        {
            get => null;

            set => Invariant.Assert(false, "FixupNode");
        }

        // Fixup nodes don't have siblings.
        public override SplayTreeNode RightChildNode
        {
            get => null;

            set => Invariant.Assert(false, "FixupNode");
        }

        // Fixup nodes don't have symbol counts.
        public override UInt32 Generation
        {
            get => 0;

            set => Invariant.Assert(false, "FixupNode");
        }

        // Fixup nodes don't have symbol counts.
        public override Int32 SymbolOffsetCache
        {
            get => -1;

            set => Invariant.Assert(false, "FixupNode");
        }

        // Fixup nodes don't have symbol counts.
        public override Int32 SymbolCount
        {
            get => 0;

            set => Invariant.Assert(false, "FixupNode");
        }

        // Fixup nodes don't have char counts.
        public override Int32 IMECharCount
        {
            get => 0;

            set => Invariant.Assert(false, "FixupNode");
        }

        // Fixup nodes are never referenced by TextPositions.
        public override Boolean BeforeStartReferenceCount
        {
            get => false;

            set => Invariant.Assert(false, "TextTreeFixupNode should never have a position reference!");
        }

        // Fixup nodes are never referenced by TextPositions.
        public override Boolean AfterStartReferenceCount
        {
            get => false;

            set => Invariant.Assert(false, "TextTreeFixupNode should never have a position reference!");
        }

        // Fixup nodes are never referenced by TextPositions.
        public override Boolean BeforeEndReferenceCount
        {
            get => false;

            set => Invariant.Assert(false, "TextTreeFixupNode should never have a position reference!");
        }

        // Fixup nodes are never referenced by TextPositions.
        public override Boolean AfterEndReferenceCount
        {
            get => false;

            set => Invariant.Assert(false, "TextTreeFixupNode should never have a position reference!");
        }

        // The node TextPositions with Backward gravity should move to
        // when leaving the deleted content.
        public TextTreeNode PreviousNode => _previousNode;

        // The edge TextPositions with Backward gravity should move to
        // when leaving the deleted content.
        public ElementEdge PreviousEdge => _previousEdge;

        // The node TextPositions with Forward gravity should move to
        // when leaving the deleted content.
        public TextTreeNode NextNode => _nextNode;

        // The edge TextPositions with Forward gravity should move to
        // when leaving the deleted content.
        public ElementEdge NextEdge => _nextEdge;

        // If this fixup is for a single TextElementNode extraction, this
        // field is the first contained node of the extracted element node.
        public TextTreeNode FirstContainedNode => _firstContainedNode;

        // If this fixup is for a single TextElementNode extraction, this
        // field is the last contained node of the extracted element node.
        public TextTreeNode LastContainedNode => _lastContainedNode;

        #endregion Internal Properties

        //------------------------------------------------------
        //
        //  Internal Events
        //
        //------------------------------------------------------

        //------------------------------------------------------
        //
        //  Private Methods
        //
        //------------------------------------------------------

        //------------------------------------------------------
        //
        //  Private Properties
        //
        //------------------------------------------------------

        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        // The node immediately preceding the deleted content parented by this fixup node.
        private readonly TextTreeNode _previousNode;

        // The edge immediately preceding the deleted content parented by this fixup node.
        private readonly ElementEdge _previousEdge;

        // The node immediately following the deleted content parented by this fixup node.
        private readonly TextTreeNode _nextNode;

        // The edge immediately following the deleted content parented by this fixup node.
        private readonly ElementEdge _nextEdge;

        // If this fixup is for a single TextElementNode extraction, this
        // field is the first contained node of the extracted element node.
        private readonly TextTreeNode _firstContainedNode;

        // If this fixup is for a single TextElementNode extraction, this
        // field is the last contained node of the extracted element node.
        private readonly TextTreeNode _lastContainedNode;

        #endregion Private Fields
    }
}
