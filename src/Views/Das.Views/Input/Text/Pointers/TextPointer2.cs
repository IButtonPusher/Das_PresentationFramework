using System;
using System.Drawing;
using System.Threading.Tasks;
using Das.Views.Collections;
using Das.Views.Core;
using Das.Views.DataBinding;
using Das.Views.Input.Text.Tree;
using Das.Views.Localization;
using Das.Views.Validation;

namespace Das.Views.Input.Text.Pointers
{
    public class TextPointer : ITextPointer
    {
        public TextPointer(TextPointer position,
                           Int32 offset,
                           LogicalDirection direction)
        {
            InitializeOffset(position, offset, direction);
        }

        public TextPointer(TextContainer tree,
                           TextTreeNode node,
                           ElementEdge edge)
        {
            Initialize(tree, node, edge, LogicalDirection.Forward, tree.PositionGeneration, false, false,
                tree.LayoutGeneration);
        }

        public TextPointer(TextPointer position,
                           LogicalDirection direction)
        {
            InitializeOffset(position, 0, direction);
        }

        public TextPointer(TextContainer textContainer,
                           Int32 offset,
                           LogicalDirection direction)
        {
            SplayTreeNode node;
            ElementEdge edge;

            if (offset < 1 || offset > textContainer.publicSymbolCount - 1)
            {
                throw new ArgumentException(SR.Get(SRID.BadDistance));
            }

            textContainer.GetNodeAndEdgeAtOffset(offset, out node, out edge);

            Initialize(textContainer, (TextTreeNode)node, edge, direction, textContainer.PositionGeneration, false,
                false, textContainer.LayoutGeneration);
        }

        public TextPointer(TextContainer tree,
                           TextTreeNode node,
                           ElementEdge edge,
                           LogicalDirection direction)
        {
            Initialize(tree, node, edge, direction, tree.PositionGeneration, false, false, tree.LayoutGeneration);
        }

        public ITextPointer CreatePointer()
        {
            throw new NotImplementedException();
        }

        public StaticTextPointer CreateStaticPointer()
        {
            throw new NotImplementedException();
        }

        public ITextPointer CreatePointer(Int32 offset)
        {
            throw new NotImplementedException();
        }

        public ITextPointer CreatePointer(LogicalDirection gravity)
        {
            throw new NotImplementedException();
        }

        public ITextPointer CreatePointer(Int32 offset,
                                          LogicalDirection gravity)
        {
            throw new NotImplementedException();
        }

        public void SetLogicalDirection(LogicalDirection direction)
        {
            throw new NotImplementedException();
        }

        public Int32 CompareTo(ITextPointer position)
        {
            throw new NotImplementedException();
        }

        public Int32 CompareTo(StaticTextPointer position)
        {
            throw new NotImplementedException();
        }

        public Boolean HasEqualScope(ITextPointer position)
        {
            throw new NotImplementedException();
        }

        public TextPointerContext GetPointerContext(LogicalDirection direction)
        {
            throw new NotImplementedException();
        }

        public Int32 GetOffsetToPosition(ITextPointer position)
        {
            throw new NotImplementedException();
        }

        public Int32 GetTextRunLength(LogicalDirection direction)
        {
            throw new NotImplementedException();
        }

        public String GetTextInRun(LogicalDirection direction)
        {
            throw new NotImplementedException();
        }

        public Int32 GetTextInRun(LogicalDirection direction,
                                  Char[] textBuffer,
                                  Int32 startIndex,
                                  Int32 count)
        {
            throw new NotImplementedException();
        }

        public Object GetAdjacentElement(LogicalDirection direction)
        {
            throw new NotImplementedException();
        }

        public void MoveToPosition(ITextPointer position)
        {
            throw new NotImplementedException();
        }

        public Int32 MoveByOffset(Int32 offset)
        {
            throw new NotImplementedException();
        }

        public Boolean MoveToNextContextPosition(LogicalDirection direction)
        {
            throw new NotImplementedException();
        }

        public ITextPointer GetNextContextPosition(LogicalDirection direction)
        {
            throw new NotImplementedException();
        }

        public Boolean MoveToInsertionPosition(LogicalDirection direction)
        {
            throw new NotImplementedException();
        }

        public ITextPointer GetInsertionPosition(LogicalDirection direction)
        {
            throw new NotImplementedException();
        }

        public ITextPointer GetFormatNormalizedPosition(LogicalDirection direction)
        {
            throw new NotImplementedException();
        }

        public Boolean MoveToNextInsertionPosition(LogicalDirection direction)
        {
            throw new NotImplementedException();
        }

        public ITextPointer GetNextInsertionPosition(LogicalDirection direction)
        {
            throw new NotImplementedException();
        }

        public void MoveToElementEdge(ElementEdge edge)
        {
            throw new NotImplementedException();
        }

        public Int32 MoveToLineBoundary(Int32 count)
        {
            throw new NotImplementedException();
        }

        public Rectangle GetCharacterRect(LogicalDirection direction)
        {
            throw new NotImplementedException();
        }

        public void Freeze()
        {
            throw new NotImplementedException();
        }

        public ITextPointer GetFrozenPointer(LogicalDirection logicalDirection)
        {
            throw new NotImplementedException();
        }

        public void InsertTextInRun(String textData)
        {
            throw new NotImplementedException();
        }

        public void DeleteContentToPosition(ITextPointer limit)
        {
            throw new NotImplementedException();
        }

        public Type GetElementType(LogicalDirection direction)
        {
            throw new NotImplementedException();
        }

        public Object GetValue(DependencyProperty formattingProperty)
        {
            throw new NotImplementedException();
        }

        public Object ReadLocalValue(DependencyProperty formattingProperty)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the logical parent scoping this TextPointer.
        /// </summary>
        public IBindableElement Parent
        {
            get
            {
                _tree.EmptyDeadPositionList();
                SyncToTreeGeneration();

                return GetLogicalTreeNode();
            }
        }

        // Returns the Logical Tree Node scoping this position.
        public IBindableElement GetLogicalTreeNode()
        {
            DebugAssertGeneration();

            return GetScopingNode().GetLogicalTreeNode();
        }

        public LocalValueEnumerator GetLocalValueEnumerator()
        {
            throw new NotImplementedException();
        }

        public Boolean ValidateLayout()
        {
            throw new NotImplementedException();
        }

        public ITextContainer TextContainer => throw new NotImplementedException();

        public Boolean HasValidLayout => throw new NotImplementedException();

        public Boolean IsAtCaretUnitBoundary => throw new NotImplementedException();

        public LogicalDirection LogicalDirection => throw new NotImplementedException();

        public Type ParentType => throw new NotImplementedException();

        public Boolean IsAtInsertionPosition => throw new NotImplementedException();

        public Boolean IsFrozen => throw new NotImplementedException();

        public Int32 Offset => throw new NotImplementedException();

        public Int32 CharOffset => throw new NotImplementedException();

        public ITextElement? GetAdjacentElementFromOuterPosition(LogicalDirection direction)
        {
            TextTreeTextElementNode? elementNode;

            _tree.EmptyDeadPositionList();
            SyncToTreeGeneration();

            elementNode = GetAdjacentTextElementNodeSibling(direction);
            return elementNode?.TextElement;
        }

        // Returns the sibling node (ie, node in the same scope) in the direction indicated bordering
        // a TextPointer, or null if no such node exists.
        public TextTreeNode GetAdjacentSiblingNode(LogicalDirection direction)
        {
            DebugAssertGeneration();

            return GetAdjacentSiblingNode(_node, Edge, direction);
        }

        // Returns the TextTreeTextNode in the direction indicated bordering
        // a TextPointer, or null if no such node exists.
        public TextTreeTextElementNode? GetAdjacentTextElementNodeSibling(LogicalDirection direction)
        {
            return GetAdjacentSiblingNode(direction) as TextTreeTextElementNode;
        }

        public void DebugAssertGeneration()
        {
            Invariant.Assert(_generation == _tree.PositionGeneration,
                "TextPointer not synchronized to tree generation!");
        }

        public TextTreeNode GetScopingNode()
        {
            return GetScopingNode(_node, Edge);
        }

        public static TextTreeNode GetScopingNode(TextTreeNode node,
                                                  ElementEdge edge)
        {
            TextTreeNode scopingNode;

            switch (edge)
            {
                case ElementEdge.BeforeStart:
                case ElementEdge.AfterEnd:
                    scopingNode = (TextTreeNode)node.GetContainingNode();
                    break;

                case ElementEdge.AfterStart:
                case ElementEdge.BeforeEnd:
                default:
                    scopingNode = node;
                    break;
            }

            return scopingNode;
        }

        public static Int32 GetSymbolOffset(ITextContainer tree,
                                            TextTreeNode node,
                                            ElementEdge edge)
        {
            Int32 offset;

            switch (edge)
            {
                case ElementEdge.BeforeStart:
                    offset = node.GetSymbolOffset(tree.Generation);
                    break;

                case ElementEdge.AfterStart:
                    offset = node.GetSymbolOffset(tree.Generation) + 1;
                    break;

                case ElementEdge.BeforeEnd:
                    offset = node.GetSymbolOffset(tree.Generation) + node.SymbolCount - 1;
                    break;

                case ElementEdge.AfterEnd:
                    offset = node.GetSymbolOffset(tree.Generation) + node.SymbolCount;
                    break;

                default:
                    Invariant.Assert(false, "Unknown value for position edge");
                    offset = 0;
                    break;
            }

            return offset;
        }

        public Int32 GetSymbolOffset()
        {
            DebugAssertGeneration();

            return GetSymbolOffset(_tree, _node, Edge);
        }

        public static Int32 GetTextInRun(TextContainer textContainer,
                                         Int32 symbolOffset,
                                         TextTreeTextNode textNode,
                                         Int32 nodeOffset,
                                         LogicalDirection direction,
                                         Char[] textBuffer,
                                         Int32 startIndex,
                                         Int32 count)
        {
            Int32 skipCount;
            Int32 finalCount;

            if (textBuffer == null)
            {
                throw new ArgumentNullException("textBuffer");
            }

            if (startIndex < 0)
            {
                throw new ArgumentException(SR.Get(SRID.NegativeValue, "startIndex"));
            }

            if (startIndex > textBuffer.Length)
            {
                throw new ArgumentException(SR.Get(SRID.StartIndexExceedsBufferSize, startIndex, textBuffer.Length));
            }

            if (count < 0)
            {
                throw new ArgumentException(SR.Get(SRID.NegativeValue, "count"));
            }

            if (count > textBuffer.Length - startIndex)
            {
                throw new ArgumentException(SR.Get(SRID.MaxLengthExceedsBufferSize, count, textBuffer.Length,
                    startIndex));
            }

            Invariant.Assert(textNode != null, "textNode is expected to be non-null");

            textContainer.EmptyDeadPositionList();

            if (nodeOffset < 0)
            {
                skipCount = 0;
            }
            else
            {
                skipCount = direction == LogicalDirection.Forward ? nodeOffset : textNode.SymbolCount - nodeOffset;
                symbolOffset += nodeOffset;
            }

            finalCount = 0;

            // Loop and combine adjacent text nodes into a single run.
            // This isn't just a perf optimization.  Because text positions
            // split text nodes, if we just returned a single node's text
            // callers would see strange side effects where position.GetTextLength() !=
            // position.GetText() if another position is moved between the calls.
            while (textNode != null)
            {
                // Never return more textBuffer than the text following this position in the current text node.
                finalCount += Math.Min(count - finalCount, textNode.SymbolCount - skipCount);
                skipCount = 0;
                if (finalCount == count)
                    break;
                textNode =
                    (direction == LogicalDirection.Forward ? textNode.GetNextNode() : textNode.GetPreviousNode()) as
                    TextTreeTextNode;
            }

            // If we're reading backwards, need to fixup symbolOffset to point into the node.
            if (direction == LogicalDirection.Backward)
            {
                symbolOffset -= finalCount;
            }

            if (finalCount > 0) // We may not have allocated textContainer.RootTextBlock if no text was ever inserted.
            {
                TextTreeText.ReadText(textContainer.RootTextBlock, symbolOffset, finalCount, textBuffer, startIndex);
            }

            return finalCount;
        }

        public static IBindableElement GetAdjacentElement(TextTreeNode node,
                                                          ElementEdge edge,
                                                          LogicalDirection direction)
        {
            TextTreeNode adjacentNode;
            IBindableElement element;

            adjacentNode = GetAdjacentNode(node, edge, direction);

            if (adjacentNode is TextTreeObjectNode)
            {
                element = ((TextTreeObjectNode)adjacentNode).EmbeddedElement;
            }
            else if (adjacentNode is TextTreeTextElementNode)
            {
                element = ((TextTreeTextElementNode)adjacentNode).TextElement;
            }
            else
            {
                // We're adjacent to a text node, or have no sibling in the specified direction.
                element = null;
            }

            return element;
        }

        // Returns the node in the direction indicated bordering
        // a TextPointer, or null if no such node exists.
        public TextTreeNode GetAdjacentNode(LogicalDirection direction)
        {
            return GetAdjacentNode(_node, Edge, direction);
        }

        public static TextTreeNode GetAdjacentNode(TextTreeNode node,
                                                   ElementEdge edge,
                                                   LogicalDirection direction)
        {
            TextTreeNode adjacentNode;

            adjacentNode = GetAdjacentSiblingNode(node, edge, direction);

            if (adjacentNode == null)
            {
                // We're the first or last child, try the parent.
                if (edge == ElementEdge.AfterStart || edge == ElementEdge.BeforeEnd)
                {
                    adjacentNode = node;
                }
                else
                {
                    adjacentNode = (TextTreeNode)node.GetContainingNode();
                }
            }

            return adjacentNode;
        }

        public TextTreeTextNode? GetAdjacentTextNodeSibling(LogicalDirection direction)
        {
            return GetAdjacentSiblingNode(direction) as TextTreeTextNode;
        }

        // Returns the TextTreeTextNode in the direction indicated bordering
        // a TextPointer, or null if no such node exists.
        public static TextTreeTextNode? GetAdjacentTextNodeSibling(TextTreeNode node,
                                                                   ElementEdge edge,
                                                                   LogicalDirection direction)
        {
            return GetAdjacentSiblingNode(node, edge, direction) as TextTreeTextNode;
        }

        public static TextTreeNode GetAdjacentSiblingNode(TextTreeNode node,
                                                          ElementEdge edge,
                                                          LogicalDirection direction)
        {
            SplayTreeNode sibling;

            if (direction == LogicalDirection.Forward)
            {
                switch (edge)
                {
                    case ElementEdge.BeforeStart:
                        sibling = node;
                        break;

                    case ElementEdge.AfterStart:
                        sibling = node.GetFirstContainedNode();
                        break;

                    case ElementEdge.BeforeEnd:
                    default:
                        sibling = null;
                        break;

                    case ElementEdge.AfterEnd:
                        sibling = node.GetNextNode();
                        break;
                }
            }
            else // direction == LogicalDirection.Backward
            {
                switch (edge)
                {
                    case ElementEdge.BeforeStart:
                        sibling = node.GetPreviousNode();
                        break;

                    case ElementEdge.AfterStart:
                    default:
                        sibling = null;
                        break;

                    case ElementEdge.BeforeEnd:
                        sibling = node.GetLastContainedNode();
                        break;

                    case ElementEdge.AfterEnd:
                        sibling = node;
                        break;
                }
            }

            return (TextTreeNode)sibling;
        }

        // Returns the symbol type preceding thisPosition.
        public static TextPointerContext GetPointerContextBackward(TextTreeNode node,
                                                                   ElementEdge edge)
        {
            TextPointerContext symbolType;
            TextTreeNode previousNode;
            TextTreeNode lastChildNode;

            switch (edge)
            {
                case ElementEdge.BeforeStart:
                    previousNode = (TextTreeNode)node.GetPreviousNode();
                    if (previousNode != null)
                    {
                        symbolType = previousNode.GetPointerContext(LogicalDirection.Backward);
                    }
                    else
                    {
                        // The root node is special, there's no ElementStart/End, so test for null parent.
                        Invariant.Assert(node.GetContainingNode() != null,
                            "Bad position!"); // Illegal to be at root BeforeStart.
                        symbolType = node.GetContainingNode() is TextTreeRootNode
                            ? TextPointerContext.None
                            : TextPointerContext.ElementStart;
                    }

                    break;

                case ElementEdge.AfterStart:
                    // The root node is special, there's no ElementStart/End, so test for null parent.
                    Invariant.Assert(node.ParentNode != null || node is TextTreeRootNode,
                        "Inconsistent node.ParentNode");
                    symbolType = node.ParentNode != null ? TextPointerContext.ElementStart : TextPointerContext.None;
                    break;

                case ElementEdge.BeforeEnd:
                    lastChildNode = (TextTreeNode)node.GetLastContainedNode();
                    if (lastChildNode != null)
                    {
                        symbolType = lastChildNode.GetPointerContext(LogicalDirection.Backward);
                    }
                    else
                    {
                        goto case ElementEdge.AfterStart;
                    }

                    break;

                case ElementEdge.AfterEnd:
                    symbolType = node.GetPointerContext(LogicalDirection.Backward);
                    break;

                default:
                    Invariant.Assert(false, "Unknown ElementEdge value");
                    symbolType = TextPointerContext.Text;
                    break;
            }

            return symbolType;
        }

        public static TextPointerContext GetPointerContextForward(TextTreeNode node,
                                                                  ElementEdge edge)
        {
            TextTreeNode nextNode;
            TextTreeNode firstContainedNode;
            TextPointerContext symbolType;

            switch (edge)
            {
                case ElementEdge.BeforeStart:
                    symbolType = node.GetPointerContext(LogicalDirection.Forward);
                    break;

                case ElementEdge.AfterStart:
                    if (node.ContainedNode != null)
                    {
                        firstContainedNode = (TextTreeNode)node.GetFirstContainedNode();
                        symbolType = firstContainedNode.GetPointerContext(LogicalDirection.Forward);
                    }
                    else
                    {
                        goto case ElementEdge.BeforeEnd;
                    }

                    break;

                case ElementEdge.BeforeEnd:
                    // The root node is special, there's no ElementStart/End, so test for null parent.
                    Invariant.Assert(node.ParentNode != null || node is TextTreeRootNode,
                        "Inconsistent node.ParentNode");
                    symbolType = node.ParentNode != null ? TextPointerContext.ElementEnd : TextPointerContext.None;
                    break;

                case ElementEdge.AfterEnd:
                    nextNode = (TextTreeNode)node.GetNextNode();
                    if (nextNode != null)
                    {
                        symbolType = nextNode.GetPointerContext(LogicalDirection.Forward);
                    }
                    else
                    {
                        // The root node is special, there's no ElementStart/End, so test for null parent.
                        Invariant.Assert(node.GetContainingNode() != null,
                            "Bad position!"); // Illegal to be at root AfterEnd.
                        symbolType = node.GetContainingNode() is TextTreeRootNode
                            ? TextPointerContext.None
                            : TextPointerContext.ElementEnd;
                    }

                    break;

                default:
                    Invariant.Assert(false, "Unreachable code.");
                    symbolType = TextPointerContext.Text;
                    break;
            }

            return symbolType;
        }

        // Updates the position state if the node referenced by this position has
        // been removed from the TextContainer.  This method must be called before
        // referencing the position's state when a public entry point is called.
        public void SyncToTreeGeneration()
        {
            SplayTreeNode node;
            SplayTreeNode searchNode;
            SplayTreeNode parentNode;
            SplayTreeNode splayNode;
            ElementEdge edge;
            TextTreeFixupNode? fixup = null;

            // If the tree hasn't had any deletions since the last time we
            // checked there's no work to do.
            if (_generation == _tree.PositionGeneration)
                return;

            // Invalidate the caret unit boundary cache -- the surrounding
            // content may have changed.
            IsCaretUnitBoundaryCacheValid = false;

            node = _node;
            edge = Edge;

            // If we can find a fixup node in the ancestor chain, this position
            // needs to be updated.
            //
            // It's possible to have cascading deletes -- some content was
            // deleted, then the nodes pointed to by a fixup node were themselves
            // deleted, and so forth.  So we have to keep checking all the
            // way up to the root.

            while (true)
            {
                searchNode = node;
                splayNode = node;

                while (true)
                {
                    parentNode = searchNode.ParentNode;
                    if (parentNode == null) // The root node is always valid.
                        break;

                    fixup = parentNode as TextTreeFixupNode;
                    if (fixup != null)
                        break;

                    if (searchNode.Role == SplayTreeNodeRole.LocalRoot)
                    {
                        splayNode.Splay();
                        splayNode = parentNode;
                    }

                    searchNode = parentNode;
                }

                if (parentNode == null)
                    break; // Checked all the way to the root, position is valid.

                // If we make it here we've found a fixup node.  Our gravity
                // tells us which direction to follow it.
                if (GetGravitypublic() == LogicalDirection.Forward)
                {
                    if (edge == ElementEdge.BeforeStart && fixup.FirstContainedNode != null)
                    {
                        // We get here if and only if a single TextElementNode was removed.
                        // Because only a single element was removed, we don't have to worry
                        // about whether the position was originally in some contained content.
                        // It originally pointed to the extracted node, so we can always
                        // move to contained content.
                        node = fixup.FirstContainedNode;
                        Invariant.Assert(edge == ElementEdge.BeforeStart, "edge BeforeStart is expected");
                    }
                    else
                    {
                        node = fixup.NextNode;
                        edge = fixup.NextEdge;
                    }
                }
                else
                {
                    if (edge == ElementEdge.AfterEnd && fixup.LastContainedNode != null)
                    {
                        // We get here if and only if a single TextElementNode was removed.
                        // Because only a single element was removed, we don't have to worry
                        // about whether the position was originally in some contained content.
                        // It originally pointed to the extracted node, so we can always
                        // move to contained content.
                        node = fixup.LastContainedNode;
                        Invariant.Assert(edge == ElementEdge.AfterEnd, "edge AfterEnd is expected");
                    }
                    else
                    {
                        node = fixup.PreviousNode;
                        edge = fixup.PreviousEdge;
                    }
                }
            }

            // Note we intentionally don't call AdjustRefCounts here.
            // We already incremented ref counts when the old target
            // node was deleted.
            SetNodeAndEdge((TextTreeNode)node, edge);

            // Update the position generation, so we don't do this work again
            // until the tree changes.
            _generation = _tree.PositionGeneration;

            AssertState();
        }

        // Finds the next run, returned as a node/edge pair.
        // Returns false if there is no following run, in which case node/edge will match the input position.
        // The returned node/edge pair respects the input position's gravity.
        public static Boolean GetNextNodeAndEdge(TextTreeNode sourceNode,
                                                 ElementEdge sourceEdge,
                                                 Boolean plainTextOnly,
                                                 out TextTreeNode node,
                                                 out ElementEdge edge)
        {
            SplayTreeNode currentNode;
            SplayTreeNode newNode;
            SplayTreeNode nextNode;
            SplayTreeNode containingNode;
            Boolean startedAdjacentToTextNode;
            Boolean endedAdjacentToTextNode;

            node = sourceNode;
            edge = sourceEdge;

            newNode = node;
            currentNode = node;

            // If we started next to a TextTreeTextNode, and the next node
            // is also a TextTreeTextNode, then skip past the second node
            // as well -- multiple text nodes count as a single Move run.
            do
            {
                startedAdjacentToTextNode = false;
                endedAdjacentToTextNode = false;

                switch (edge)
                {
                    case ElementEdge.BeforeStart:
                        newNode = currentNode.GetFirstContainedNode();
                        if (newNode != null)
                        {
                            // Move to inner edge/first child.
                        }
                        else if (currentNode is TextTreeTextElementNode)
                        {
                            // Move to inner edge.
                            newNode = currentNode;
                            edge = ElementEdge.BeforeEnd;
                        }
                        else
                        {
                            // Move to next node.
                            startedAdjacentToTextNode = currentNode is TextTreeTextNode;
                            edge = ElementEdge.BeforeEnd;
                            goto case ElementEdge.BeforeEnd;
                        }

                        break;

                    case ElementEdge.AfterStart:
                        newNode = currentNode.GetFirstContainedNode();
                        if (newNode != null)
                        {
                            // Move to first child/second child or first child/first child child
                            if (newNode is TextTreeTextElementNode)
                            {
                                edge = ElementEdge.AfterStart;
                            }
                            else
                            {
                                startedAdjacentToTextNode = newNode is TextTreeTextNode;
                                endedAdjacentToTextNode = newNode.GetNextNode() is TextTreeTextNode;
                                edge = ElementEdge.AfterEnd;
                            }
                        }
                        else if (currentNode is TextTreeTextElementNode)
                        {
                            // Move to next node.
                            newNode = currentNode;
                            edge = ElementEdge.AfterEnd;
                        }
                        else
                        {
                            Invariant.Assert(currentNode is TextTreeRootNode,
                                "currentNode is expected to be TextTreeRootNode");
                            // This is the root node, leave newNode null.
                        }

                        break;

                    case ElementEdge.BeforeEnd:
                        newNode = currentNode.GetNextNode();
                        if (newNode != null)
                        {
                            // Move to next node;
                            endedAdjacentToTextNode = newNode is TextTreeTextNode;
                            edge = ElementEdge.BeforeStart;
                        }
                        else
                        {
                            // Move to inner edge of parent.
                            newNode = currentNode.GetContainingNode();
                        }

                        break;

                    case ElementEdge.AfterEnd:
                        nextNode = currentNode.GetNextNode();
                        startedAdjacentToTextNode = nextNode is TextTreeTextNode;

                        newNode = nextNode;
                        if (newNode != null)
                        {
                            // Move to next node/first child;
                            if (newNode is TextTreeTextElementNode)
                            {
                                edge = ElementEdge.AfterStart;
                            }
                            else
                            {
                                // Move to next node/next next node.
                                endedAdjacentToTextNode = newNode.GetNextNode() is TextTreeTextNode;
                            }
                        }
                        else
                        {
                            containingNode = currentNode.GetContainingNode();

                            if (!(containingNode is TextTreeRootNode))
                            {
                                // Move to parent.
                                newNode = containingNode;
                            }
                        }

                        break;

                    default:
                        Invariant.Assert(false, "Unknown ElementEdge value");
                        break;
                }

                currentNode = newNode;

                // Multiple text nodes count as a single Move run.
                // Instead of iterating through N text nodes, exploit
                // the fact (when we can) that text nodes are only ever contained in
                // runs with no other content.  Jump straight to the end.
                if (startedAdjacentToTextNode && endedAdjacentToTextNode && plainTextOnly)
                {
                    newNode = newNode.GetContainingNode();
                    Invariant.Assert(newNode is TextTreeRootNode);

                    if (edge == ElementEdge.BeforeStart)
                    {
                        edge = ElementEdge.BeforeEnd;
                    }
                    else
                    {
                        newNode = newNode.GetLastContainedNode();
                        Invariant.Assert(newNode != null);
                        Invariant.Assert(edge == ElementEdge.AfterEnd);
                    }

                    break;
                }
            } while (startedAdjacentToTextNode && endedAdjacentToTextNode);

            if (newNode != null)
            {
                node = (TextTreeNode)newNode;
            }

            return newNode != null;
        }

        // Finds the previous run, returned as a node/edge pair.
        // Returns false if there is no preceding run, in which case node/edge will match the input position.
        // The returned node/edge pair respects the input positon's gravity.
        public static Boolean GetPreviousNodeAndEdge(TextTreeNode sourceNode,
                                                     ElementEdge sourceEdge,
                                                     Boolean plainTextOnly,
                                                     out TextTreeNode node,
                                                     out ElementEdge edge)
        {
            SplayTreeNode currentNode;
            SplayTreeNode newNode;
            SplayTreeNode containingNode;
            Boolean startedAdjacentToTextNode;
            Boolean endedAdjacentToTextNode;

            node = sourceNode;
            edge = sourceEdge;

            newNode = node;
            currentNode = node;

            // If we started next to a TextTreeTextNode, and the next node
            // is also a TextTreeTextNode, then skip past the second node
            // as well -- multiple text nodes count as a single Move run.
            do
            {
                startedAdjacentToTextNode = false;
                endedAdjacentToTextNode = false;

                switch (edge)
                {
                    case ElementEdge.BeforeStart:
                        newNode = currentNode.GetPreviousNode();
                        if (newNode != null)
                        {
                            // Move to next node/last child;
                            if (newNode is TextTreeTextElementNode)
                            {
                                // Move to previous node last child/previous node
                                edge = ElementEdge.BeforeEnd;
                            }
                            else
                            {
                                // Move to previous previous node/previous node.
                                startedAdjacentToTextNode = newNode is TextTreeTextNode;
                                endedAdjacentToTextNode = startedAdjacentToTextNode &&
                                                          newNode.GetPreviousNode() is TextTreeTextNode;
                            }
                        }
                        else
                        {
                            containingNode = currentNode.GetContainingNode();

                            if (!(containingNode is TextTreeRootNode))
                            {
                                // Move to parent.
                                newNode = containingNode;
                            }
                        }

                        break;

                    case ElementEdge.AfterStart:
                        newNode = currentNode.GetPreviousNode();
                        if (newNode != null)
                        {
                            endedAdjacentToTextNode = newNode is TextTreeTextNode;

                            // Move to previous node;
                            edge = ElementEdge.AfterEnd;
                        }
                        else
                        {
                            // Move to inner edge of parent.
                            newNode = currentNode.GetContainingNode();
                        }

                        break;

                    case ElementEdge.BeforeEnd:
                        newNode = currentNode.GetLastContainedNode();
                        if (newNode != null)
                        {
                            // Move to penultimate child/last child or inner edge of last child.
                            if (newNode is TextTreeTextElementNode)
                            {
                                edge = ElementEdge.BeforeEnd;
                            }
                            else
                            {
                                startedAdjacentToTextNode = newNode is TextTreeTextNode;
                                endedAdjacentToTextNode = startedAdjacentToTextNode &&
                                                          newNode.GetPreviousNode() is TextTreeTextNode;
                                edge = ElementEdge.BeforeStart;
                            }
                        }
                        else if (currentNode is TextTreeTextElementNode)
                        {
                            // Move to next node.
                            newNode = currentNode;
                            edge = ElementEdge.BeforeStart;
                        }
                        else
                        {
                            Invariant.Assert(currentNode is TextTreeRootNode,
                                "currentNode is expected to be a TextTreeRootNode");
                            // This is the root node, leave newNode null.
                        }

                        break;

                    case ElementEdge.AfterEnd:
                        newNode = currentNode.GetLastContainedNode();
                        if (newNode != null)
                        {
                            // Move to inner edge/last child.
                        }
                        else if (currentNode is TextTreeTextElementNode)
                        {
                            // Move to opposite edge.
                            newNode = currentNode;
                            edge = ElementEdge.AfterStart;
                        }
                        else
                        {
                            // Move to previous node.
                            startedAdjacentToTextNode = currentNode is TextTreeTextNode;
                            edge = ElementEdge.AfterStart;
                            goto case ElementEdge.AfterStart;
                        }

                        break;

                    default:
                        Invariant.Assert(false, "Unknown ElementEdge value");
                        break;
                }

                currentNode = newNode;

                // Multiple text nodes count as a single Move run.
                // Instead of iterating through N text nodes, exploit
                // the fact (when we can) that text nodes are only ever contained in
                // runs with no other content.  Jump straight to the start.
                if (startedAdjacentToTextNode && endedAdjacentToTextNode && plainTextOnly)
                {
                    newNode = newNode.GetContainingNode();
                    Invariant.Assert(newNode is TextTreeRootNode);

                    if (edge == ElementEdge.AfterEnd)
                    {
                        edge = ElementEdge.AfterStart;
                    }
                    else
                    {
                        newNode = newNode.GetFirstContainedNode();
                        Invariant.Assert(newNode != null);
                        Invariant.Assert(edge == ElementEdge.BeforeStart);
                    }

                    break;
                }
            } while (startedAdjacentToTextNode && endedAdjacentToTextNode);

            if (newNode != null)
            {
                node = (TextTreeNode)newNode;
            }

            return newNode != null;
        }

        private void InitializeOffset(TextPointer position,
                                      Int32 distance,
                                      LogicalDirection direction)
        {
            SplayTreeNode node;
            ElementEdge edge;
            Int32 offset;
            Boolean isCaretUnitBoundaryCacheValid;

            // We MUST sync to the current tree, otherwise we could addref
            // an orphaned node, resulting in a future unmatched release...
            // Ref counts on orphaned nodes are only considered at the time
            // of removal, not afterwards.
            position.SyncToTreeGeneration();

            if (distance != 0)
            {
                offset = position.GetSymbolOffset() + distance;
                if (offset < 1 || offset > position.TextContainer.InternalSymbolCount - 1)
                {
                    throw new ArgumentException(SR.Get(SRID.BadDistance));
                }

                position.TextContainer.GetNodeAndEdgeAtOffset(offset, out node, out edge);

                isCaretUnitBoundaryCacheValid = false;
            }
            else
            {
                node = position.Node;
                edge = position.Edge;
                isCaretUnitBoundaryCacheValid = position.IsCaretUnitBoundaryCacheValid;
            }

            Initialize(position.TextContainer, (TextTreeNode)node, edge, direction,
                position.TextContainer.PositionGeneration,
                position.CaretUnitBoundaryCache, isCaretUnitBoundaryCacheValid, position._layoutGeneration);
        }

        private void Initialize(ITextContainer tree,
                                TextTreeNode node,
                                ElementEdge edge,
                                LogicalDirection gravity,
                                UInt32 generation,
                                Boolean caretUnitBoundaryCache,
                                Boolean isCaretUnitBoundaryCacheValid,
                                UInt32 layoutGeneration)
        {
            _tree = tree;

            // Fixup of the target node based on gravity.
            // Positions always cling to a node edge that matches their gravity,
            // so that insert ops never affect the position.
            RepositionForGravity(ref node, ref edge, gravity);

            SetNodeAndEdge(node.IncrementReferenceCount(edge), edge);
            _generation = generation;

            CaretUnitBoundaryCache = caretUnitBoundaryCache;
            IsCaretUnitBoundaryCacheValid = isCaretUnitBoundaryCacheValid;
            _layoutGeneration = layoutGeneration;

            VerifyFlags();
            tree.AssertTree();
            AssertState();
        }

        private static void RepositionForGravity(ref TextTreeNode node,
                                                 ref ElementEdge edge,
                                                 LogicalDirection gravity)
        {
            SplayTreeNode newNode;
            ElementEdge newEdge;

            newNode = node;
            newEdge = edge;

            switch (edge)
            {
                case ElementEdge.BeforeStart:
                    if (gravity == LogicalDirection.Backward)
                    {
                        newNode = node.GetPreviousNode();
                        newEdge = ElementEdge.AfterEnd;
                        if (newNode == null)
                        {
                            newNode = node.GetContainingNode();
                            newEdge = ElementEdge.AfterStart;
                        }
                    }

                    break;

                case ElementEdge.AfterStart:
                    if (gravity == LogicalDirection.Forward)
                    {
                        newNode = node.GetFirstContainedNode();
                        newEdge = ElementEdge.BeforeStart;
                        if (newNode == null)
                        {
                            newNode = node;
                            newEdge = ElementEdge.BeforeEnd;
                        }
                    }

                    break;

                case ElementEdge.BeforeEnd:
                    if (gravity == LogicalDirection.Backward)
                    {
                        newNode = node.GetLastContainedNode();
                        newEdge = ElementEdge.AfterEnd;
                        if (newNode == null)
                        {
                            newNode = node;
                            newEdge = ElementEdge.AfterStart;
                        }
                    }

                    break;

                case ElementEdge.AfterEnd:
                    if (gravity == LogicalDirection.Forward)
                    {
                        newNode = node.GetNextNode();
                        newEdge = ElementEdge.BeforeStart;
                        if (newNode == null)
                        {
                            newNode = node.GetContainingNode();
                            newEdge = ElementEdge.BeforeEnd;
                        }
                    }

                    break;
            }

            node = (TextTreeNode)newNode;
            edge = newEdge;
        }

        private void AssertState()
        {
            if (Invariant.Strict)
            {
                // Positions must never have a null tree pointer.
                Invariant.Assert(_node != null, "Null position node!");

                if (GetGravitypublic() == LogicalDirection.Forward)
                {
                    // Positions with forward gravity must stay at left edges, otherwise inserts could displace them.
                    Invariant.Assert(Edge == ElementEdge.BeforeStart || Edge == ElementEdge.BeforeEnd,
                        "Bad position edge/gravity pair! (1)");
                }
                else
                {
                    // Positions with backward gravity must stay at right edges, otherwise inserts could displace them.
                    Invariant.Assert(Edge == ElementEdge.AfterStart || Edge == ElementEdge.AfterEnd,
                        "Bad position edge/gravity pair! (2)");
                }

                if (_node is TextTreeRootNode)
                {
                    // Positions may never be at the outer edge of the root node, since you can't insert content there.
                    Invariant.Assert(Edge != ElementEdge.BeforeStart && Edge != ElementEdge.AfterEnd,
                        "Position at outer edge of root!");
                }
                else if (_node is TextTreeTextNode || _node is TextTreeObjectNode)
                {
                    // Text and object nodes have no inner edges/chilren, so you can't put a position there.
                    Invariant.Assert(Edge != ElementEdge.AfterStart && Edge != ElementEdge.BeforeEnd,
                        "Position at inner leaf node edge!");
                }
                else
                {
                    // Add new asserts for new node types here.
                    Invariant.Assert(_node is TextTreeTextElementNode, "Unknown node type!");
                }

                Invariant.Assert(_tree != null, "Position has no tree!");

                #if DEBUG_SLOW
                // This test is so slow we can't afford to run it even with Invariant.Strict.
                // It grinds execution to a halt.

                int count;

                if (_tree.RootTextBlock == null)
                {
                    count = 2; // Empty tree has two implicit edge symbols.
                }
                else
                {
                    count = 0;
                    for (TextTreeTextBlock textBlock =
 (TextTreeTextBlock)_tree.RootTextBlock.ContainedNode.GetMinSibling();
                         textBlock != null;
                         textBlock = (TextTreeTextBlock)textBlock.GetNextNode())
                    {
                        Invariant.Assert(textBlock.Count > 0, "Empty TextBlock!");
                        count += textBlock.Count;
                    }
                }
                Invariant.Assert(_tree.publicSymbolCount == count, "Bad root symbol count!");

                Invariant.Assert((_tree.RootNode == null && count == 2) || count == GetNodeSymbolCountSlow(_tree.RootNode), "TextNode symbol count not in synch with tree!");

                if (_tree.RootNode != null)
                {
                    DebugWalkTree(_tree.RootNode.GetMinSibling());
                }
                #endif // DEBUG_SLOW
            }
        }

        // Worker for GetGravity.  No parameter validation.
        private LogicalDirection GetGravitypublic()
        {
            return Edge == ElementEdge.BeforeStart || Edge == ElementEdge.BeforeEnd
                ? LogicalDirection.Forward
                : LogicalDirection.Backward;
        }

        // Repositions the TextPointer and clears any relevant caches.
        private void SetNodeAndEdge(TextTreeNode node,
                                    ElementEdge edge)
        {
            Invariant.Assert(edge == ElementEdge.BeforeStart ||
                             edge == ElementEdge.AfterStart ||
                             edge == ElementEdge.BeforeEnd ||
                             edge == ElementEdge.AfterEnd);

            _node = node;
            _flags = (_flags & ~(UInt32)Flags.EdgeMask) | (UInt32)edge;
            VerifyFlags();

            // Always clear the caret unit boundary cache when we move to a new position.
            IsCaretUnitBoundaryCacheValid = false;
        }

        // Ensure we have a valid _flags field.
        // See bug 1249258.
        private void VerifyFlags()
        {
            var edge = (ElementEdge)(_flags & (UInt32)Flags.EdgeMask);

            Invariant.Assert(edge == ElementEdge.BeforeStart ||
                             edge == ElementEdge.AfterStart ||
                             edge == ElementEdge.BeforeEnd ||
                             edge == ElementEdge.AfterEnd);
        }

        private Boolean CaretUnitBoundaryCache
        {
            get => (_flags & (UInt32)Flags.CaretUnitBoundaryCache) == (UInt32)Flags.CaretUnitBoundaryCache;

            set
            {
                _flags = (_flags & ~(UInt32)Flags.CaretUnitBoundaryCache) |
                         (value ? (UInt32)Flags.CaretUnitBoundaryCache : 0);
                VerifyFlags();
            }
        }

        // The node referenced by this position.
        public TextTreeNode Node => _node;

        // The edge referenced by this position.
        public ElementEdge Edge => (ElementEdge)(_flags & (UInt32)Flags.EdgeMask);

        // True when the CaretUnitBoundaryCache is ready for use.
        // If false the cache is not reliable.
        private Boolean IsCaretUnitBoundaryCacheValid
        {
            get => (_flags & (UInt32)Flags.IsCaretUnitBoundaryCacheValid) ==
                   (UInt32)Flags.IsCaretUnitBoundaryCacheValid;

            set
            {
                _flags = (_flags & ~(UInt32)Flags.IsCaretUnitBoundaryCacheValid) |
                         (value ? (UInt32)Flags.IsCaretUnitBoundaryCacheValid : 0);
                VerifyFlags();
            }
        }

        private UInt32 _flags;

        private UInt32 _generation;

        private UInt32 _layoutGeneration;
        private TextTreeNode _node;

        private ITextContainer _tree;

        // Enum used for the _flags bitfield.
        [Flags]
        private enum Flags
        {
            EdgeMask = 15, // 4 low-order bis are an ElementEdge.
            IsFrozen = 16,
            IsCaretUnitBoundaryCacheValid = 32,
            CaretUnitBoundaryCache = 64,
        }
    }
}
