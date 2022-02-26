using System;
using System.Threading.Tasks;
using Das.Views.Core;
using Das.Views.DataBinding;
using Das.Views.Input.Text.Pointers;
using Das.Views.Validation;

namespace Das.Views.Input.Text.Tree
{
    public abstract class TextTreeNode : SplayTreeNode
    {
        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        #if INT_EDGE_REF_COUNT
        // Sets the count of TextPositions referencing the node's left
        // edge.
        // Since nodes don't usually have any references, we demand allocate
        // storage when needed.
        protected static void SetBeforeStartReferenceCount(ref EdgeReferenceCounts edgeReferenceCounts, int value)
        {
            if (edgeReferenceCounts != null)
            {
                if (value == 0 &&
                    edgeReferenceCounts.AfterStartReferenceCount == 0 &&
                    edgeReferenceCounts.BeforeEndReferenceCount == 0 &&
                    edgeReferenceCounts.AfterEndReferenceCount == 0)
                {
                    edgeReferenceCounts = null;
                }
                else
                {
                    edgeReferenceCounts.BeforeStartReferenceCount = value;
                }
            }
            else if (value != 0)
            {
                edgeReferenceCounts = new EdgeReferenceCounts();
                edgeReferenceCounts.BeforeStartReferenceCount = value;
            }
        }

        // Sets the count of TextPositions referencing the node's AfterStart edge.
        // Since nodes don't usually have any references, we demand allocate
        // storage when needed.
        protected void SetAfterStartReferenceCount(ref EdgeReferenceCounts edgeReferenceCounts, int value)
        {
            EdgeReferenceCounts originalCounts;

            if (edgeReferenceCounts != null)
            {
                if (value == 0 &&
                    edgeReferenceCounts.BeforeStartReferenceCount == 0 &&
                    edgeReferenceCounts.BeforeEndReferenceCount == 0 &&
                    edgeReferenceCounts.AfterEndReferenceCount == 0)
                {
                    edgeReferenceCounts = null;
                }
                else
                {
                    if (!(edgeReferenceCounts is ElementEdgeReferenceCounts))
                    {
                        // We need a slightly bigger object, which tracks the inner edges.
                        Invariant.Assert(this is TextTreeTextElementNode, "Non-element nodes should never have inner edge references!");
                        originalCounts = edgeReferenceCounts;
                        edgeReferenceCounts = new ElementEdgeReferenceCounts();
                        edgeReferenceCounts.BeforeStartReferenceCount = originalCounts.BeforeStartReferenceCount;
                        edgeReferenceCounts.AfterEndReferenceCount = originalCounts.AfterEndReferenceCount;
                    }
                    edgeReferenceCounts.AfterStartReferenceCount = value;
                }
            }
            else if (value != 0)
            {
                edgeReferenceCounts = new ElementEdgeReferenceCounts();
                edgeReferenceCounts.AfterStartReferenceCount = value;
            }
        }

        // Sets the count of TextPositions referencing the node's BeforeEnd edge.
        // Since nodes don't usually have any references, we demand allocate
        // storage when needed.
        protected void SetBeforeEndReferenceCount(ref EdgeReferenceCounts edgeReferenceCounts, int value)
        {
            EdgeReferenceCounts originalCounts;

            if (edgeReferenceCounts != null)
            {
                if (value == 0 &&
                    edgeReferenceCounts.BeforeStartReferenceCount == 0 &&
                    edgeReferenceCounts.AfterStartReferenceCount == 0 &&
                    edgeReferenceCounts.AfterEndReferenceCount == 0)
                {
                    edgeReferenceCounts = null;
                }
                else
                {
                    if (!(edgeReferenceCounts is ElementEdgeReferenceCounts))
                    {
                        // We need a slightly bigger object, which tracks the inner edges.
                        Invariant.Assert(this is TextTreeTextElementNode, "Non-element nodes should never have inner edge references!");
                        originalCounts = edgeReferenceCounts;
                        edgeReferenceCounts = new ElementEdgeReferenceCounts();
                        edgeReferenceCounts.BeforeStartReferenceCount = originalCounts.BeforeStartReferenceCount;
                        edgeReferenceCounts.AfterEndReferenceCount = originalCounts.AfterEndReferenceCount;
                    }
                    edgeReferenceCounts.BeforeEndReferenceCount = value;
                }
            }
            else if (value != 0)
            {
                edgeReferenceCounts = new ElementEdgeReferenceCounts();
                edgeReferenceCounts.BeforeEndReferenceCount = value;
            }
        }

        // Sets the count of TextPositions referencing the node's right
        // edge.
        // Since nodes don't usually have any references, we demand allocate
        // storage when needed.
        protected static void SetAfterEndReferenceCount(ref EdgeReferenceCounts edgeReferenceCounts, int value)
        {
            if (edgeReferenceCounts != null)
            {
                if (value == 0 &&
                    edgeReferenceCounts.BeforeStartReferenceCount == 0 &&
                    edgeReferenceCounts.AfterStartReferenceCount == 0 &&
                    edgeReferenceCounts.BeforeEndReferenceCount == 0)
                {
                    edgeReferenceCounts = null;
                }
                else
                {
                    edgeReferenceCounts.AfterEndReferenceCount = value;
                }
            }
            else if (value != 0)
            {
                edgeReferenceCounts = new EdgeReferenceCounts();
                edgeReferenceCounts.AfterEndReferenceCount = value;
            }
        }
        #endif // INT_EDGE_REF_COUNT

        #endregion Protected Methods

        //------------------------------------------------------
        //
        //  public Methods
        //
        //------------------------------------------------------

        #region public Methods

        // Returns a shallow copy of this node.
        // The clone is a local root with no children.
        public abstract TextTreeNode Clone();

        // Returns the TextContainer containing this node.
        public TextContainer GetTextTree()
        {
            SplayTreeNode node;
            SplayTreeNode containingNode;

            node = this;

            while (true)
            {
                containingNode = node.GetContainingNode();
                if (containingNode == null)
                    break;
                node = containingNode;
            }

            return ((TextTreeRootNode)node).TextContainer;
        }

        // Returns the closest IBindableElement scoping a given node.
        // This includes the node itself and TextContainer.Parent.
        public IBindableElement GetDependencyParent()
        {
            SplayTreeNode node;
            IBindableElement parent;
            SplayTreeNode containingNode;
            TextTreeTextElementNode elementNode;

            node = this;

            while (true)
            {
                elementNode = node as TextTreeTextElementNode;
                if (elementNode != null)
                {
                    parent = elementNode.TextElement;
                    Invariant.Assert(parent != null, "TextElementNode has null TextElement!");
                    break;
                }

                containingNode = node.GetContainingNode();
                if (containingNode == null)
                {
                    parent = ((TextTreeRootNode)node).TextContainer.Parent; // This may be null.
                    break;
                }

                node = containingNode;
            }

            return parent;
        }

        // Returns the closest Logical Tree Node to a given node, including the
        // node itself and TextContainer.Parent.
        public IBindableElement GetLogicalTreeNode()
        {
            TextTreeObjectNode objectNode;
            TextTreeTextElementNode textElementNode;
            SplayTreeNode node;
            SplayTreeNode containingNode;
            IBindableElement logicalTreeNode;

            objectNode = this as TextTreeObjectNode;
            if (objectNode != null)
            {
                if (objectNode.EmbeddedElement is { } bindable)
                {
                    return bindable;
                }
            }

            node = this;

            while (true)
            {
                textElementNode = node as TextTreeTextElementNode;
                if (textElementNode != null)
                {
                    logicalTreeNode = textElementNode.TextElement;
                    break;
                }

                containingNode = node.GetContainingNode();
                if (containingNode == null)
                {
                    logicalTreeNode = ((TextTreeRootNode)node).TextContainer.Parent;
                    break;
                }

                node = containingNode;
            }

            return logicalTreeNode;
        }

        // Returns the TextPointerContext of the node.
        // If node is TextTreeTextElementNode, this method returns ElementStart
        // if direction == Forward, otherwise ElementEnd if direction == Backward.
        public abstract TextPointerContext GetPointerContext(LogicalDirection direction);

        // Increments the reference count of TextPositions referencing a
        // particular edge of this node.
        // 
        // If this node is a TextTreeTextNode, the increment may split the node
        // and the return value is guaranteed to be the node containing the referenced
        // edge (which may be a new node).  Otherwise this method always returns
        // the original node.
        public TextTreeNode IncrementReferenceCount(ElementEdge edge)
        {
            return IncrementReferenceCount(edge, +1);
        }

        public virtual TextTreeNode IncrementReferenceCount(ElementEdge edge,
                                                            Boolean delta)
        {
            return IncrementReferenceCount(edge, delta ? 1 : 0);
        }

        // Increments the reference count of TextPositions referencing a
        // particular edge of this node.
        // 
        // If this node is a TextTreeTextNode, the increment may split the node
        // and the return value is guaranteed to be the node containing the referenced
        // edge (which may be a new node).  Otherwise this method always returns
        // the original node.
        public virtual TextTreeNode IncrementReferenceCount(ElementEdge edge,
                                                            Int32 delta)
        {
            Invariant.Assert(delta >= 0);

            if (delta > 0)
            {
                switch (edge)
                {
                    case ElementEdge.BeforeStart:
                        BeforeStartReferenceCount = true;
                        break;

                    case ElementEdge.AfterStart:
                        AfterStartReferenceCount = true;
                        break;

                    case ElementEdge.BeforeEnd:
                        BeforeEndReferenceCount = true;
                        break;

                    case ElementEdge.AfterEnd:
                        AfterEndReferenceCount = true;
                        break;

                    default:
                        Invariant.Assert(false, "Bad ElementEdge value!");
                        break;
                }
            }

            return this;
        }

        // Decrements the reference count of TextPositions referencing this
        // node.
        //
        // Be careful!  If this node is a TextTreeTextNode, the decrement may
        // cause a merge, and this node may be removed from the tree.
        public virtual void DecrementReferenceCount(ElementEdge edge)
        {
            #if INT_EDGE_REF_COUNT
            switch (edge)
            {
                case ElementEdge.BeforeStart:
                    this.BeforeStartReferenceCount--;
                    Invariant.Assert(this.BeforeStartReferenceCount >= 0, "Bad BeforeStart ref count!");
                    break;

                case ElementEdge.AfterStart:
                    this.AfterStartReferenceCount--;
                    Invariant.Assert(this.AfterStartReferenceCount >= 0, "Bad AfterStart ref count!");
                    break;

                case ElementEdge.BeforeEnd:
                    this.BeforeEndReferenceCount--;
                    Invariant.Assert(this.BeforeEndReferenceCount >= 0, "Bad BeforeEnd ref count!");
                    break;

                case ElementEdge.AfterEnd:
                    this.AfterEndReferenceCount--;
                    Invariant.Assert(this.AfterEndReferenceCount >= 0, "Bad AfterEnd ref count!");
                    break;

                default:
                    Invariant.Assert(false, "Bad ElementEdge value!");
                    break;
            }
            #endif // INT_EDGE_REF_COUNT
        }

        // Inserts a node at a specified position.      
        public void InsertAtPosition(TextPointer position)
        {
            InsertAtNode(position.Node, position.Edge);
        }

        public ElementEdge GetEdgeFromOffsetNoBias(Int32 nodeOffset)
        {
            return GetEdgeFromOffset(nodeOffset, LogicalDirection.Forward);
        }

        public ElementEdge GetEdgeFromOffset(Int32 nodeOffset,
                                             LogicalDirection bias)
        {
            ElementEdge edge;

            if (SymbolCount == 0)
            {
                // If we're pointing at a zero-width TextTreeTextNode, we need to make
                // sure we get the right edge -- nodeOffset doesn't convey enough information
                // for GetEdgeFromOffset to compute a correct value.
                edge = bias == LogicalDirection.Forward ? ElementEdge.AfterEnd : ElementEdge.BeforeStart;
            }
            else if (nodeOffset == 0)
            {
                edge = ElementEdge.BeforeStart;
            }
            else if (nodeOffset == SymbolCount)
            {
                edge = ElementEdge.AfterEnd;
            }
            else if (nodeOffset == 1)
            {
                edge = ElementEdge.AfterStart;
            }
            else
            {
                Invariant.Assert(nodeOffset == SymbolCount - 1);
                edge = ElementEdge.BeforeEnd;
            }

            return edge;
        }

        public Int32 GetOffsetFromEdge(ElementEdge edge)
        {
            Int32 offset;

            switch (edge)
            {
                case ElementEdge.BeforeStart:
                    offset = 0;
                    break;

                case ElementEdge.AfterStart:
                    offset = 1;
                    break;

                case ElementEdge.BeforeEnd:
                    offset = SymbolCount - 1;
                    break;

                case ElementEdge.AfterEnd:
                    offset = SymbolCount;
                    break;

                default:
                    offset = 0;
                    Invariant.Assert(false, "Bad ElementEdge value!");
                    break;
            }

            return offset;
        }

        #endregion public methods

        //------------------------------------------------------
        //
        //  public Properties
        //
        //------------------------------------------------------

        #region public Properties

        // Count of TextPositions referencing the node's BeforeStart edge.
        public abstract Boolean BeforeStartReferenceCount { get; set; }

        // Count of TextPositions referencing the node's AfterStart edge.
        public abstract Boolean AfterStartReferenceCount { get; set; }

        // Count of TextPositions referencing the node's BeforeEnd edge.
        public abstract Boolean BeforeEndReferenceCount { get; set; }

        // Count of TextPositions referencing the node's AfterEnd edge.
        public abstract Boolean AfterEndReferenceCount { get; set; }

        #endregion public Properties
    }
}
