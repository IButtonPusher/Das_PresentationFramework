using System;
using System.Threading.Tasks;
using Das.Views.Collections;
using Das.Views.Core;
using Das.Views.DataBinding;
using Das.Views.Input.Text.Events;
using Das.Views.Input.Text.Pointers;
using Das.Views.Input.Text.Tree;
using Das.Views.Layout;
using Das.Views.Localization;
using Das.Views.Text;
using Das.Views.Undo;
using Das.Views.Validation;

namespace Das.Views.Input.Text
{
    public class TextContainer : ITextContainer
    {
        public TextContainer(IBindableElement parent,
                             Boolean plainTextOnly)
        {
            _parent = parent;
            SetFlags(plainTextOnly, Flags.PlainTextOnly);
        }

        void ITextContainer.BeginChange()
        {
            BeginChange();
        }

        void ITextContainer.BeginChangeNoUndo()
        {
            BeginChangeNoUndo();
        }

        void ITextContainer.EndChange()
        {
            EndChange(false);
        }

        void ITextContainer.EndChange(Boolean skipEvents)
        {
            EndChange(skipEvents);
        }

        ITextPointer ITextContainer.CreatePointerAtOffset(Int32 offset,
                                                          LogicalDirection direction)
        {
            return CreatePointerAtOffset(offset, direction);
        }

        ITextPointer ITextContainer.CreatePointerAtCharOffset(Int32 charOffset,
                                                              LogicalDirection direction)
        {
            return CreatePointerAtCharOffset(charOffset, direction);
        }

        ITextPointer ITextContainer.CreateDynamicTextPointer(StaticTextPointer position,
                                                             LogicalDirection direction)
        {
            return CreatePointerAtOffset(GetpublicOffset(position) - 1, direction);
        }

        StaticTextPointer ITextContainer.CreateStaticPointerAtOffset(Int32 offset)
        {
            return CreateStaticPointerAtOffset(offset);
        }

        TextPointerContext ITextContainer.GetPointerContext(StaticTextPointer pointer,
                                                            LogicalDirection direction)
        {
            var handle0 = (TextTreeNode)pointer.Handle0;
            var handle1 = pointer.Handle1;
            TextPointerContext pointerContext;
            if (handle0 is TextTreeTextNode && handle1 > 0 && handle1 < handle0.SymbolCount)
                pointerContext = TextPointerContext.Text;
            else if (direction == LogicalDirection.Forward)
            {
                var edgeFromOffset = handle0.GetEdgeFromOffset(handle1, LogicalDirection.Forward);
                pointerContext = TextPointer.GetPointerContextForward(handle0, edgeFromOffset);
            }
            else
            {
                var edgeFromOffset = handle0.GetEdgeFromOffset(handle1, LogicalDirection.Backward);
                pointerContext = TextPointer.GetPointerContextBackward(handle0, edgeFromOffset);
            }

            return pointerContext;
        }

        Int32 ITextContainer.GetOffsetToPosition(StaticTextPointer position1,
                                                 StaticTextPointer position2)
        {
            return GetpublicOffset(position2) - GetpublicOffset(position1);
        }

        Int32 ITextContainer.GetTextInRun(StaticTextPointer position,
                                          LogicalDirection direction,
                                          Char[] textBuffer,
                                          Int32 startIndex,
                                          Int32 count)
        {
            var handle0 = (TextTreeNode)position.Handle0;
            var nodeOffset = position.Handle1;
            if (!(handle0 is TextTreeTextNode textNode) || nodeOffset == 0 || nodeOffset == handle0.SymbolCount)
            {
                textNode = TextPointer.GetAdjacentTextNodeSibling(handle0, handle0.GetEdgeFromOffsetNoBias(nodeOffset),
                    direction);
                nodeOffset = -1;
            }

            return textNode != null
                ? TextPointer.GetTextInRun(this, textNode.GetSymbolOffset(Generation), textNode, nodeOffset, direction,
                    textBuffer, startIndex, count)
                : 0;
        }

        Object ITextContainer.GetAdjacentElement(StaticTextPointer position,
                                                 LogicalDirection direction)
        {
            var handle0 = (TextTreeNode)position.Handle0;
            var handle1 = position.Handle1;
            return !(handle0 is TextTreeTextNode) || handle1 <= 0 || handle1 >= handle0.SymbolCount
                ? TextPointer.GetAdjacentElement(handle0, handle0.GetEdgeFromOffset(handle1, direction), direction)
                : (Object)null;
        }

        IBindableElement ITextContainer.GetParent(StaticTextPointer position)
        {
            return GetScopingNode(position).GetLogicalTreeNode();
        }

        StaticTextPointer ITextContainer.CreatePointer(StaticTextPointer position,
                                                       Int32 offset)
        {
            return ((ITextContainer)this).CreateStaticPointerAtOffset(GetpublicOffset(position) - 1 + offset);
        }

        StaticTextPointer ITextContainer.GetNextContextPosition(StaticTextPointer position,
                                                                LogicalDirection direction)
        {
            var node = (TextTreeNode)position.Handle0;
            var handle1 = position.Handle1;
            ElementEdge edge;
            Boolean flag;
            if (node is TextTreeTextNode && handle1 > 0 && handle1 < node.SymbolCount)
            {
                if (PlainTextOnly)
                {
                    node = (TextTreeNode)node.GetContainingNode();
                    edge = direction == LogicalDirection.Backward ? ElementEdge.AfterStart : ElementEdge.BeforeEnd;
                }
                else
                {
                    while ((direction == LogicalDirection.Forward ? node.GetNextNode() : node.GetPreviousNode()) is
                           TextTreeTextNode textTreeTextNode)
                        node = textTreeTextNode;
                    edge = direction == LogicalDirection.Backward ? ElementEdge.BeforeStart : ElementEdge.AfterEnd;
                }

                flag = true;
            }
            else if (direction == LogicalDirection.Forward)
            {
                edge = node.GetEdgeFromOffset(handle1, LogicalDirection.Forward);
                flag = TextPointer.GetNextNodeAndEdge(node, edge, PlainTextOnly, out node, out edge);
            }
            else
            {
                edge = node.GetEdgeFromOffset(handle1, LogicalDirection.Backward);
                flag = TextPointer.GetPreviousNodeAndEdge(node, edge, PlainTextOnly, out node, out edge);
            }

            return !flag ? StaticTextPointer.Null : new StaticTextPointer(this, node, node.GetOffsetFromEdge(edge));
        }

        Int32 ITextContainer.CompareTo(StaticTextPointer position1,
                                       StaticTextPointer position2)
        {
            var publicOffset1 = GetpublicOffset(position1);
            var publicOffset2 = GetpublicOffset(position2);
            return publicOffset1 >= publicOffset2 ? publicOffset1 <= publicOffset2 ? 0 : 1 : -1;
        }

        Int32 ITextContainer.CompareTo(StaticTextPointer position1,
                                       ITextPointer position2)
        {
            var publicOffset = GetpublicOffset(position1);
            var num = position2.Offset + 1;
            return publicOffset >= num ? publicOffset <= num ? 0 : 1 : -1;
        }

        Object ITextContainer.GetValue(StaticTextPointer position,
                                       DependencyProperty formattingProperty)
        {
            var dependencyParent = GetScopingNode(position).GetDependencyParent();
            return dependencyParent != null
                ? dependencyParent.GetValue(formattingProperty)
                : DependencyProperty.UnsetValue;
        }

        Boolean ITextContainer.IsReadOnly => CheckFlags(Flags.ReadOnly);

        ITextPointer ITextContainer.Start => Start;

        ITextPointer ITextContainer.End => End;

        UInt32 ITextContainer.Generation => Generation;

        Highlights ITextContainer.Highlights => Highlights;

        IBindableElement ITextContainer.Parent => Parent;

        ITextSelection ITextContainer.TextSelection
        {
            get => TextSelection;
            set => _textSelection = value;
        }

        UndoManager ITextContainer.UndoManager => UndoManager;

        ITextView ITextContainer.TextView
        {
            get => TextView;
            set => TextView = value;
        }

        Int32 ITextContainer.SymbolCount => SymbolCount;

        Int32 ITextContainer.IMECharCount => IMECharCount;

        event EventHandler ITextContainer.Changing
        {
            add => Changing += value;
            remove => Changing -= value;
        }

        event TextContainerChangeEventHandler ITextContainer.Change
        {
            add => Change += value;
            remove => Change -= value;
        }

        event TextContainerChangedEventHandler ITextContainer.Changed
        {
            add => Changed += value;
            remove => Changed -= value;
        }


        public void InsertText(TextPointer position,
                               Object text)
        {
            Invariant.Assert(text is String || text is Char[], "Unexpected type for 'text' parameter!");
            var textLength = GetTextLength(text);
            if (textLength == 0)
                return;
            DemandCreateText();
            position.SyncToTreeGeneration();
            if (Invariant.Strict && position.Node.SymbolCount == 0)
            {
                Invariant.Assert(position.Node is TextTreeTextNode);
                Invariant.Assert(
                    position.Edge == ElementEdge.AfterEnd && position.Node.GetPreviousNode() is TextTreeTextNode &&
                    position.Node.GetPreviousNode().SymbolCount > 0 || position.Edge == ElementEdge.BeforeStart &&
                    position.Node.GetNextNode() is TextTreeTextNode && position.Node.GetNextNode().SymbolCount > 0);
            }

            BeforeAddChange();
            var startPosition = HasListeners ? new TextPointer(position, LogicalDirection.Backward) : null;
            var direction = position.Edge == ElementEdge.BeforeStart || position.Edge == ElementEdge.BeforeEnd
                ? LogicalDirection.Backward
                : LogicalDirection.Forward;
            var textTreeTextNode = position.GetAdjacentTextNodeSibling(direction);
            if (textTreeTextNode != null &&
                (direction == LogicalDirection.Backward && textTreeTextNode.AfterEndReferenceCount ||
                 direction == LogicalDirection.Forward && textTreeTextNode.BeforeStartReferenceCount))
                textTreeTextNode = null;
            SplayTreeNode containingNode;
            if (textTreeTextNode == null)
            {
                textTreeTextNode = new TextTreeTextNode();
                textTreeTextNode.InsertAtPosition(position);
                containingNode = textTreeTextNode.GetContainingNode();
            }
            else
            {
                textTreeTextNode.Splay();
                containingNode = textTreeTextNode.ParentNode;
            }

            textTreeTextNode.SymbolCount += textLength;
            UpdateContainerSymbolCount(containingNode, textLength, textLength);
            var symbolOffset = textTreeTextNode.GetSymbolOffset(Generation);
            TextTreeText.InsertText(_rootNode.RootTextBlock, symbolOffset, text);
            TextTreeUndo.CreateInsertUndoUnit(this, symbolOffset, textLength);
            NextGeneration(false);
            AddChange(startPosition, textLength, textLength, PrecursorTextChangeType.ContentAdded);
            if (!(position.Parent is ITextElement parent))
                return;
            parent.OnTextUpdated();
        }

        public void SetValues(ITextPointer position,
                              LocalValueEnumerator values)
        {
            if (position == null)
                throw new ArgumentNullException(nameof(position));
            EmptyDeadPositionList();
            ValidateSetValue(position);
            BeginChange();
            try
            {
                var parent = position.Parent as ITextElement;
                Invariant.Assert(parent != null);
                values.Reset();
                while (values.MoveNext())
                {
                    var current = values.Current;
                    if (current.Property.Name != "CachedSource" && !current.Property.IsReadOnly)
                        //&& current.Property != Run.TextProperty)
                    {
                        //if (current.Value is BindingExpressionBase bindingExpressionBase)
                        //    parent.SetValue(current.Property, bindingExpressionBase.Value);
                        //else
                        parent.SetValue(current.Property, current.Value);
                    }
                }
            }
            finally
            {
                EndChange();
            }
        }

        public void GetNodeAndEdgeAtOffset(Int32 offset,
                                           out SplayTreeNode node,
                                           out ElementEdge edge)
        {
            GetNodeAndEdgeAtOffset(offset, true, out node, out edge);
        }

        public void GetNodeAndEdgeAtOffset(Int32 offset,
                                           Boolean splitNode,
                                           out SplayTreeNode node,
                                           out ElementEdge edge)
        {
            Invariant.Assert(offset >= 1 && offset <= publicSymbolCount - 1, "Bogus symbol offset!");
            var flag = false;
            node = _rootNode;
            var num1 = 0;
            while (true)
            {
                Invariant.Assert(
                    (Int32)node.Generation != (Int32)_rootNode.Generation || node.SymbolOffsetCache == -1 ||
                    node.SymbolOffsetCache == num1, "Bad node offset cache!");
                node.Generation = _rootNode.Generation;
                node.SymbolOffsetCache = num1;
                if (offset != num1)
                {
                    if (node is TextTreeRootNode || node is TextTreeTextElementNode)
                    {
                        if (offset != num1 + 1)
                        {
                            if (offset == num1 + node.SymbolCount - 1)
                                goto label_7;
                        }
                        else
                            goto label_5;
                    }

                    if (offset != num1 + node.SymbolCount)
                    {
                        if (node.ContainedNode != null)
                        {
                            node = node.ContainedNode;
                            var num2 = num1 + 1;
                            Int32 nodeOffset;
                            node = node.GetSiblingAtOffset(offset - num2, out nodeOffset);
                            num1 = num2 + nodeOffset;
                        }
                        else
                            goto label_11;
                    }
                    else
                        goto label_9;
                }
                else
                    break;
            }

            edge = ElementEdge.BeforeStart;
            flag = true;
            goto label_15;
            label_5:
            edge = ElementEdge.AfterStart;
            goto label_15;
            label_7:
            edge = ElementEdge.BeforeEnd;
            goto label_15;
            label_9:
            edge = ElementEdge.AfterEnd;
            flag = true;
            goto label_15;
            label_11:
            Invariant.Assert(node is TextTreeTextNode);
            if (splitNode)
                node = ((TextTreeTextNode)node).Split(offset - num1, ElementEdge.AfterEnd);
            edge = ElementEdge.BeforeStart;
            label_15:
            if (!flag)
                return;
            node = AdjustForZeroWidthNode(node, edge);
        }

        public void EmptyDeadPositionList()
        {
        }

        public Int32 InternalSymbolCount => _rootNode == null ? 2 : _rootNode.SymbolCount;

        public void AssertTree()
        {
        }

        public UInt32 PositionGeneration
        {
            get
            {
                Invariant.Assert(_rootNode != null, "Asking for PositionGeneration before root node create!");
                return _rootNode.PositionGeneration;
            }
        }

        public override String ToString()
        {
            return base.ToString();
        }

        public void EnableUndo(IVisualElement uiScope)
        {
            Invariant.Assert(_undoManager == null, SR.Get("TextContainer_UndoManagerCreatedMoreThanOnce"));
            _undoManager = new UndoManager();
            UndoManager.AttachUndoManager((IBindableElement)uiScope, _undoManager);
        }

        public void DisableUndo(IVisualElement uiScope)
        {
            Invariant.Assert(_undoManager != null, "UndoManager not created.");
            Invariant.Assert(_undoManager == UndoManager.GetUndoManager((IBindableElement)uiScope));
            UndoManager.DetachUndoManager((IBindableElement)uiScope);
            _undoManager = null;
        }

        //public void SetValue(TextPointer position,
        //                     DependencyProperty property,
        //                     Object value)
        //{
        //    if (position == null)
        //        throw new ArgumentNullException(nameof(position));
        //    if (property == null)
        //        throw new ArgumentNullException(nameof(property));
        //    EmptyDeadPositionList();
        //    ValidateSetValue(position);
        //    BeginChange();
        //    try
        //    {
        //        TextElement parent = position.Parent as TextElement;
        //        Invariant.Assert(parent != null);
        //        parent.SetValue(property, value);
        //    }
        //    finally
        //    {
        //        EndChange();
        //    }
        //}


        public void BeginChange()
        {
            BeginChange(true);
        }

        public void BeginChangeNoUndo()
        {
            BeginChange(false);
        }

        public void EndChange()
        {
            EndChange(false);
        }

        public void EndChange(Boolean skipEvents)
        {
            Invariant.Assert(_changeBlockLevel > 0, "Unmatched EndChange call!");
            --_changeBlockLevel;
            if (_changeBlockLevel != 0)
                return;
            try
            {
                //_rootNode.DispatcherProcessingDisabled.Dispose();
                if (_changes == null)
                    return;
                var changes = _changes;
                _changes = null;
                if (ChangedHandler == null || skipEvents)
                    return;
                ChangedHandler(this, changes);
            }
            finally
            {
                if (_changeBlockUndoRecord != null)
                {
                    try
                    {
                        _changeBlockUndoRecord.OnEndChange();
                    }
                    finally
                    {
                        _changeBlockUndoRecord = null;
                    }
                }
            }
        }

        public TextPointer CreatePointerAtOffset(Int32 offset,
                                                 LogicalDirection direction)
        {
            EmptyDeadPositionList();
            DemandCreatePositionState();
            return new TextPointer(this, offset + 1, direction);
        }

        public TextPointer CreatePointerAtCharOffset(Int32 charOffset,
                                                     LogicalDirection direction)
        {
            EmptyDeadPositionList();
            DemandCreatePositionState();
            TextTreeNode node;
            ElementEdge edge;
            GetNodeAndEdgeAtCharOffset(charOffset, out node, out edge);
            return node != null ? new TextPointer(this, node, edge, direction) : null;
        }

        public StaticTextPointer CreateStaticPointerAtOffset(Int32 offset)
        {
            EmptyDeadPositionList();
            DemandCreatePositionState();
            SplayTreeNode node;
            GetNodeAndEdgeAtOffset(offset + 1, false, out node, out var _);
            var handle1 = offset + 1 - node.GetSymbolOffset(Generation);
            return new StaticTextPointer(this, node, handle1);
        }

        public Int32 GetpublicOffset(StaticTextPointer position)
        {
            var handle0 = (TextTreeNode)position.Handle0;
            var handle1 = position.Handle1;
            return !(handle0 is TextTreeTextNode)
                ? TextPointer.GetSymbolOffset(this, handle0, handle0.GetEdgeFromOffsetNoBias(handle1))
                : handle0.GetSymbolOffset(Generation) + handle1;
        }

        public void BeforeAddChange()
        {
            Invariant.Assert(_changeBlockLevel > 0, "All public APIs must call BeginChange!");
            if (!HasListeners)
                return;
            if (ChangingHandler != null)
                ChangingHandler(this, EventArgs.Empty);
            if (_changes != null)
                return;
            _changes = new TextContainerChangedEventArgs();
        }

        public void AddChange(TextPointer startPosition,
                              Int32 symbolCount,
                              Int32 charCount,
                              PrecursorTextChangeType textChange)
        {
            AddChange(startPosition, symbolCount, charCount, textChange, null, false);
        }

        public void AddChange(TextPointer startPosition,
                              Int32 symbolCount,
                              Int32 charCount,
                              PrecursorTextChangeType textChange,
                              DependencyProperty property,
                              Boolean affectsRenderOnly)
        {
            Invariant.Assert(
                textChange != PrecursorTextChangeType.ElementAdded &&
                textChange != PrecursorTextChangeType.ElementExtracted,
                "Need second TextPointer for ElementAdded/Extracted operations!");
            AddChange(startPosition, null, symbolCount, 0, charCount, textChange, property, affectsRenderOnly);
        }

        public void AddChange(TextPointer startPosition,
                              TextPointer endPosition,
                              Int32 symbolCount,
                              Int32 leftEdgeCharCount,
                              Int32 childCharCount,
                              PrecursorTextChangeType textChange,
                              DependencyProperty property,
                              Boolean affectsRenderOnly)
        {
            Invariant.Assert(_changeBlockLevel > 0, "All public APIs must call BeginChange!");
            Invariant.Assert(!CheckFlags(Flags.ReadOnly) || textChange == PrecursorTextChangeType.PropertyModified,
                "Illegal to modify TextContainer structure inside Change event scope!");
            if (!HasListeners)
                return;
            if (_changes == null)
                _changes = new TextContainerChangedEventArgs();
            Invariant.Assert(_changes != null, "Missing call to BeforeAddChange!");
            _changes.AddChange(textChange, startPosition.Offset, symbolCount, CollectTextChanges);
            if (ChangeHandler == null)
                return;
            FireChangeEvent(startPosition, endPosition, symbolCount, leftEdgeCharCount, childCharCount, textChange,
                property, affectsRenderOnly);
        }

        public void AddLocalValueChange()
        {
            Invariant.Assert(_changeBlockLevel > 0, "All public APIs must call BeginChange!");
            _changes.SetLocalPropertyValueChanged();
        }

        //public void SetValues(ITextPointer position,
        //                      LocalValueEnumerator values)
        //{
        //    if (position == null)
        //        throw new ArgumentNullException(nameof (position));
        //    this.EmptyDeadPositionList();
        //    this.ValidateSetValue(position);
        //    this.BeginChange();
        //    try
        //    {
        //        ITextElement parent = position.Parent as ITextElement;
        //        Invariant.Assert(parent != null);
        //        values.Reset();
        //        while (values.MoveNext())
        //        {
        //            LocalValueEntry current = values.Current;
        //            if (!(current.Property.Name == "CachedSource") && !current.Property.IsReadOnly )
        //                                                           //&& current.Property != Run.TextProperty)
        //            {
        //                if (current.Value is BindingExpressionBase bindingExpressionBase)
        //                    parent.SetValue(current.Property, bindingExpressionBase.Value);
        //                else
        //                    parent.SetValue(current.Property, current.Value);
        //            }
        //        }
        //    }
        //    finally
        //    {
        //        this.EndChange();
        //    }
        //}

        //public void InsertElementpublic(TextPointer startPosition,
        //                                TextPointer endPosition,
        //                                TextElement element)
        //{
        //    Invariant.Assert(!PlainTextOnly);
        //    Invariant.Assert(startPosition.TextContainer == this);
        //    Invariant.Assert(endPosition.TextContainer == this);
        //    DemandCreateText();
        //    startPosition.SyncToTreeGeneration();
        //    endPosition.SyncToTreeGeneration();
        //    var flag1 = (UInt32)startPosition.CompareTo(endPosition) > 0U;
        //    BeforeAddChange();
        //    ExtractChangeEventArgs extractChangeEventArgs;
        //    Char[] text;
        //    TextTreeTextElementNode treeTextElementNode;
        //    Int32 num1;
        //    Boolean flag2;
        //    if (element.TextElementNode != null)
        //    {
        //        Boolean flag3 = this == element.TextContainer;
        //        if (!flag3)
        //            element.TextContainer.BeginChange();
        //        var flag4 = true;
        //        try
        //        {
        //            text = element.TextContainer.ExtractElementpublic(element, true, out extractChangeEventArgs);
        //            flag4 = false;
        //        }
        //        finally
        //        {
        //            if (flag4 && !flag3)
        //                element.TextContainer.EndChange();
        //        }

        //        treeTextElementNode = element.TextElementNode;
        //        num1 = extractChangeEventArgs.ChildIMECharCount;
        //        if (flag3)
        //        {
        //            startPosition.SyncToTreeGeneration();
        //            endPosition.SyncToTreeGeneration();
        //            extractChangeEventArgs.AddChange();
        //            extractChangeEventArgs = null;
        //        }

        //        flag2 = false;
        //    }
        //    else
        //    {
        //        text = null;
        //        treeTextElementNode = new TextTreeTextElementNode();
        //        num1 = 0;
        //        flag2 = true;
        //        extractChangeEventArgs = null;
        //    }

        //    var logicalTreeNode = startPosition.GetLogicalTreeNode();
        //    TextElementCollectionHelper.MarkDirty(logicalTreeNode);
        //    if (flag2)
        //    {
        //        treeTextElementNode.TextElement = element;
        //        element.TextElementNode = treeTextElementNode;
        //    }

        //    TextTreeTextElementNode node1 = null;
        //    var num2 = 0;
        //    if (flag1)
        //    {
        //        node1 = TextPointer.GetAdjacentTextElementNodeSibling(LogicalDirection.Forward);
        //        if (node1 != null)
        //        {
        //            num2 = -node1.IMELeftEdgeCharCount;
        //            node1.IMECharCount += num2;
        //        }
        //    }

        //    var siblingTree = InsertElementToSiblingTree(startPosition, endPosition, treeTextElementNode);
        //    var charCount = num1 + treeTextElementNode.IMELeftEdgeCharCount;
        //    TextTreeTextElementNode node2 = null;
        //    var num3 = 0;
        //    if (element.IsFirstIMEVisibleSibling && !flag1)
        //    {
        //        node2 = (TextTreeTextElementNode)treeTextElementNode.GetNextNode();
        //        if (node2 != null)
        //        {
        //            num3 = node2.IMELeftEdgeCharCount;
        //            node2.IMECharCount += num3;
        //        }
        //    }

        //    UpdateContainerSymbolCount(treeTextElementNode.GetContainingNode(), text == null ? 2 : text.Length,
        //        charCount + num3 + num2);
        //    var symbolOffset = treeTextElementNode.GetSymbolOffset(Generation);
        //    if (flag2)
        //        TextTreeText.InsertElementEdges(_rootNode.RootTextBlock, symbolOffset, siblingTree);
        //    else
        //        TextTreeText.InsertText(_rootNode.RootTextBlock, symbolOffset, (Object)text);
        //    NextGeneration(false);
        //    TextTreeUndo.CreateInsertElementUndoUnit(this, symbolOffset, text != null);
        //    if (extractChangeEventArgs != null)
        //    {
        //        extractChangeEventArgs.AddChange();
        //        extractChangeEventArgs.TextContainer.EndChange();
        //    }

        //    if (HasListeners)
        //    {
        //        var startPosition1 = new TextPointer(this, treeTextElementNode, ElementEdge.BeforeStart);
        //        if (siblingTree == 0 || text != null)
        //        {
        //            AddChange(startPosition1, text == null ? 2 : text.Length, charCount,
        //                PrecursorTextChangeType.ContentAdded);
        //        }
        //        else
        //        {
        //            var endPosition1 = new TextPointer(this, treeTextElementNode, ElementEdge.BeforeEnd);
        //            AddChange(startPosition1, endPosition1, treeTextElementNode.SymbolCount,
        //                treeTextElementNode.IMELeftEdgeCharCount,
        //                treeTextElementNode.IMECharCount - treeTextElementNode.IMELeftEdgeCharCount,
        //                PrecursorTextChangeType.ElementAdded, null, false);
        //        }

        //        if (num3 != 0)
        //            RaiseEventForFormerFirstIMEVisibleNode((TextTreeNode)node2);
        //        if (num2 != 0)
        //            RaiseEventForNewFirstIMEVisibleNode((TextTreeNode)node1);
        //    }

        //    element.BeforeLogicalTreeChange();
        //    try
        //    {
        //        LogicalTreeHelper.AddLogicalChild(logicalTreeNode, (Object)element);
        //    }
        //    finally
        //    {
        //        element.AfterLogicalTreeChange();
        //    }

        //    if (flag2)
        //        ReparentLogicalChildren(treeTextElementNode, (IBindableElement)treeTextElementNode.TextElement,
        //            logicalTreeNode);
        //    if (!flag1)
        //        return;
        //    element.OnTextUpdated();
        //}

        //public void InsertEmbeddedObjectpublic(TextPointer position,
        //                                       IBindableElement embeddedObject)
        //{
        //    Invariant.Assert(!PlainTextOnly);
        //    DemandCreateText();
        //    position.SyncToTreeGeneration();
        //    BeforeAddChange();
        //    var logicalTreeNode = position.GetLogicalTreeNode();
        //    TextTreeNode node = new TextTreeObjectNode(embeddedObject);
        //    node.InsertAtPosition(position);
        //    UpdateContainerSymbolCount(node.GetContainingNode(), node.SymbolCount, node.IMECharCount);
        //    var symbolOffset = node.GetSymbolOffset(Generation);
        //    TextTreeText.InsertObject(_rootNode.RootTextBlock, symbolOffset);
        //    NextGeneration(false);
        //    TextTreeUndo.CreateInsertUndoUnit(this, symbolOffset, 1);
        //    var child = embeddedObject;
        //    LogicalTreeHelper.AddLogicalChild(logicalTreeNode, (Object)child);
        //    if (!HasListeners)
        //        return;
        //    AddChange(new TextPointer(this, node, ElementEdge.BeforeStart), 1, 1, PrecursorTextChangeType.ContentAdded);
        //}

        public void DeleteContentpublic(TextPointer startPosition,
                                        TextPointer endPosition)
        {
            startPosition.SyncToTreeGeneration();
            endPosition.SyncToTreeGeneration();
            if (startPosition.CompareTo(endPosition) == 0)
                return;
            BeforeAddChange();
            TextTreeUndoUnit deleteContentUndoUnit =
                TextTreeUndo.CreateDeleteContentUndoUnit(this, startPosition, endPosition);
            var scopingNode = startPosition.GetScopingNode();
            TextElementCollectionHelper.MarkDirty(scopingNode.GetLogicalTreeNode());
            var num = 0;
            var nextImeVisibleNode = GetNextIMEVisibleNode(startPosition, endPosition);
            if (nextImeVisibleNode != null)
            {
                num = -nextImeVisibleNode.IMELeftEdgeCharCount;
                nextImeVisibleNode.IMECharCount += num;
            }

            Int32 charCount1;
            Int32 charCount2;
            var symbolCount = CutTopLevelLogicalNodes(scopingNode, startPosition, endPosition, out charCount1) +
                              DeleteContentFromSiblingTree(scopingNode, startPosition, endPosition, (UInt32)num > 0U,
                                  out charCount2);
            var charCount3 = charCount1 + charCount2;
            Invariant.Assert(symbolCount > 0);
            deleteContentUndoUnit?.SetTreeHashCode();
            AddChange(new TextPointer(startPosition, LogicalDirection.Forward), symbolCount, charCount3,
                PrecursorTextChangeType.ContentRemoved);
            if (num == 0)
                return;
            RaiseEventForNewFirstIMEVisibleNode((TextTreeNode)nextImeVisibleNode);
        }

        public void GetNodeAndEdgeAtCharOffset(Int32 charOffset,
                                               out TextTreeNode node,
                                               out ElementEdge edge)
        {
            Invariant.Assert(charOffset >= 0 && charOffset <= IMECharCount, "Bogus char offset!");
            if (IMECharCount == 0)
            {
                node = null;
                edge = ElementEdge.BeforeStart;
            }
            else
            {
                var flag = false;
                node = _rootNode;
                var num1 = 0;
                while (true)
                {
                    var num2 = 0;
                    if (node is TextTreeTextElementNode treeTextElementNode)
                    {
                        num2 = treeTextElementNode.IMELeftEdgeCharCount;
                        if (num2 > 0)
                        {
                            if (charOffset != num1)
                            {
                                if (charOffset == num1 + num2)
                                    goto label_8;
                            }
                            else
                                break;
                        }
                    }
                    else if (node is TextTreeTextNode || node is TextTreeObjectNode)
                    {
                        if (charOffset != num1)
                        {
                            if (charOffset == num1 + node.IMECharCount)
                                goto label_13;
                        }
                        else
                            goto label_11;
                    }

                    if (node.ContainedNode != null)
                    {
                        node = (TextTreeNode)node.ContainedNode;
                        var num3 = num1 + num2;
                        Int32 nodeCharOffset;
                        node = (TextTreeNode)node.GetSiblingAtCharOffset(charOffset - num3, out nodeCharOffset);
                        num1 = num3 + nodeCharOffset;
                    }
                    else
                        goto label_15;
                }

                edge = ElementEdge.BeforeStart;
                goto label_17;
                label_8:
                edge = ElementEdge.AfterStart;
                goto label_17;
                label_11:
                edge = ElementEdge.BeforeStart;
                flag = true;
                goto label_17;
                label_13:
                edge = ElementEdge.AfterEnd;
                flag = true;
                goto label_17;
                label_15:
                Invariant.Assert(node is TextTreeTextNode);
                node = ((TextTreeTextNode)node).Split(charOffset - num1, ElementEdge.AfterEnd);
                edge = ElementEdge.BeforeStart;
                label_17:
                if (!flag)
                    return;
                node = (TextTreeNode)AdjustForZeroWidthNode(node, edge);
            }
        }


        //public void SetValues(ITextPointer position,
        //                      LocalValueEnumerator values)
        //{
        //    TODO_IMPLEMENT_ME();
        //}

        public static Int32 GetTextLength(Object text)
        {
            Invariant.Assert(text is String || text is Char[], "Bad text parameter!");
            return !(text is String str) ? ((Char[])text).Length : str.Length;
        }

        public Int32 GetContentHashCode()
        {
            return publicSymbolCount;
        }

        public void NextLayoutGeneration()
        {
            ++_rootNode.LayoutGeneration;
        }

        // Removes a TextElement from the tree.
        // Any TextElement content is left in the tree.
        internal void ExtractElementInternal(ITextElement element)
        {
            ExtractChangeEventArgs extractChangeEventArgs;

            ExtractElementInternal(element, false /* deep */, out extractChangeEventArgs);
        }

        //public void ExtractElementpublic(TextElement element)
        //{
        //    ExtractElementpublic(element, false, out var _);
        //}

        //public Boolean IsAtCaretUnitBoundary(TextPointer position)
        //{
        //    position.DebugAssertGeneration();
        //    Invariant.Assert(position.HasValidLayout);
        //    if (_rootNode.CaretUnitBoundaryCacheOffset != position.GetSymbolOffset())
        //    {
        //        _rootNode.CaretUnitBoundaryCacheOffset = position.GetSymbolOffset();
        //        _rootNode.CaretUnitBoundaryCache = _textview.IsAtCaretUnitBoundary(position);
        //        if (!_rootNode.CaretUnitBoundaryCache && position.LogicalDirection == LogicalDirection.Backward)
        //            _rootNode.CaretUnitBoundaryCache =
        //                _textview.IsAtCaretUnitBoundary(position.GetPositionAtOffset(0, LogicalDirection.Forward));
        //    }

        //    return _rootNode.CaretUnitBoundaryCache;
        //}

        private TextTreeNode GetScopingNode(StaticTextPointer position)
        {
            var handle0 = (TextTreeNode)position.Handle0;
            var handle1 = position.Handle1;
            return !(handle0 is TextTreeTextNode) || handle1 <= 0 || handle1 >= handle0.SymbolCount
                ? TextPointer.GetScopingNode(handle0, handle0.GetEdgeFromOffsetNoBias(handle1))
                : handle0;
        }

        private void ReparentLogicalChildren(SplayTreeNode containerNode,
                                             IBindableElement newParentLogicalNode,
                                             IBindableElement oldParentLogicalNode)
        {
            ReparentLogicalChildren(containerNode.GetFirstContainedNode(), null, newParentLogicalNode,
                oldParentLogicalNode);
        }

        private void ReparentLogicalChildren(SplayTreeNode firstChildNode,
                                             SplayTreeNode lastChildNode,
                                             IBindableElement newParentLogicalNode,
                                             IBindableElement oldParentLogicalNode)
        {
            SplayTreeNode node;
            IBindableElement logicalTreeNode;
            TextTreeTextElementNode elementNode;
            TextTreeObjectNode uiElementNode;

            Invariant.Assert(!(newParentLogicalNode == null && oldParentLogicalNode == null),
                "Both new and old parents should not be null");

            for (node = firstChildNode; node != null; node = node.GetNextNode())
            {
                logicalTreeNode = null;

                elementNode = node as TextTreeTextElementNode;
                if (elementNode != null)
                {
                    logicalTreeNode = elementNode.TextElement;
                }
                else
                {
                    uiElementNode = node as TextTreeObjectNode;
                    if (uiElementNode != null)
                    {
                        logicalTreeNode = uiElementNode.EmbeddedElement;
                    }
                }

                var textElement = logicalTreeNode as ITextElement;
                if (textElement != null)
                {
                    textElement.BeforeLogicalTreeChange();
                }

                try
                {
                    if (oldParentLogicalNode != null)
                    {
                        LogicalTreeHelper.RemoveLogicalChild(oldParentLogicalNode, logicalTreeNode);
                    }

                    if (newParentLogicalNode != null)
                    {
                        LogicalTreeHelper.AddLogicalChild(newParentLogicalNode, logicalTreeNode);
                    }
                }
                finally
                {
                    if (textElement != null)
                    {
                        textElement.AfterLogicalTreeChange();
                    }
                }

                if (node == lastChildNode)
                    break;
            }

            //Invariant.Assert(newParentLogicalNode != null || oldParentLogicalNode != null,
            //    "Both new and old parents should not be null");
            //for (var splayTreeNode = firstChildNode; splayTreeNode != null; splayTreeNode = splayTreeNode.GetNextNode())
            //{
            //    IBindableElement child = null;
            //    if (splayTreeNode is TextTreeTextElementNode treeTextElementNode)
            //        child = (IBindableElement)treeTextElementNode.TextElement;
            //    else if (splayTreeNode is TextTreeObjectNode textTreeObjectNode)
            //        child = textTreeObjectNode.EmbeddedElement;
            //    if (child is ITextElement textElement)
            //        textElement.BeforeLogicalTreeChange();
            //    try
            //    {
            //        if (oldParentLogicalNode != null)
            //            LogicalTreeHelper.RemoveLogicalChild(oldParentLogicalNode, (Object)child);
            //        if (newParentLogicalNode != null)
            //            LogicalTreeHelper.AddLogicalChild(newParentLogicalNode, (Object)child);
            //    }
            //    finally
            //    {
            //        textElement?.AfterLogicalTreeChange();
            //    }

            //    if (splayTreeNode == lastChildNode)
            //        break;
            //}
        }

        private SplayTreeNode AdjustForZeroWidthNode(SplayTreeNode node,
                                                     ElementEdge edge)
        {
            if (!(node is TextTreeTextNode textTreeTextNode))
            {
                Invariant.Assert(node.SymbolCount > 0, "Only TextTreeTextNodes may have zero symbol counts!");
                return node;
            }

            if (textTreeTextNode.SymbolCount == 0)
            {
                var nextNode = textTreeTextNode.GetNextNode();
                if (nextNode != null)
                {
                    if (Invariant.Strict && nextNode.SymbolCount == 0)
                    {
                        Invariant.Assert(nextNode is TextTreeTextNode);
                        Invariant.Assert(!textTreeTextNode.BeforeStartReferenceCount);
                        Invariant.Assert(!((TextTreeNode)nextNode).AfterEndReferenceCount);
                        Invariant.Assert(
                            textTreeTextNode.GetPreviousNode() == null ||
                            textTreeTextNode.GetPreviousNode().SymbolCount > 0,
                            "Found three consecutive zero-width text nodes! (1)");
                        Invariant.Assert(nextNode.GetNextNode() == null || nextNode.GetNextNode().SymbolCount > 0,
                            "Found three consecutive zero-width text nodes! (2)");
                    }

                    if (!textTreeTextNode.BeforeStartReferenceCount)
                        node = nextNode;
                }
            }
            else if (edge == ElementEdge.BeforeStart)
            {
                if (textTreeTextNode.AfterEndReferenceCount)
                {
                    var previousNode = textTreeTextNode.GetPreviousNode();
                    if (previousNode != null && previousNode.SymbolCount == 0 &&
                        !((TextTreeNode)previousNode).AfterEndReferenceCount)
                    {
                        Invariant.Assert(previousNode is TextTreeTextNode);
                        node = previousNode;
                    }
                }
            }
            else if (textTreeTextNode.BeforeStartReferenceCount)
            {
                var nextNode = textTreeTextNode.GetNextNode();
                if (nextNode != null && nextNode.SymbolCount == 0 &&
                    !((TextTreeNode)nextNode).BeforeStartReferenceCount)
                {
                    Invariant.Assert(nextNode is TextTreeTextNode);
                    node = nextNode;
                }
            }

            return node;
        }

        private Int32 InsertElementToSiblingTree(TextPointer startPosition,
                                                 TextPointer endPosition,
                                                 TextTreeTextElementNode elementNode)
        {
            var siblingTree = 0;
            var childCharCount = 0;
            if (startPosition.CompareTo(endPosition) == 0)
            {
                var num = elementNode.IMECharCount - elementNode.IMELeftEdgeCharCount;
                elementNode.InsertAtPosition(startPosition);
                if (elementNode.ContainedNode != null)
                {
                    siblingTree = elementNode.SymbolCount - 2;
                    childCharCount = num;
                }
            }
            else
                siblingTree =
                    InsertElementToSiblingTreeComplex(startPosition, endPosition, elementNode, out childCharCount);

            elementNode.SymbolCount = siblingTree + 2;
            elementNode.IMECharCount = childCharCount + elementNode.IMELeftEdgeCharCount;
            return siblingTree;
        }

        private Int32 InsertElementToSiblingTreeComplex(TextPointer startPosition,
                                                        TextPointer endPosition,
                                                        TextTreeTextElementNode elementNode,
                                                        out Int32 childCharCount)
        {
            SplayTreeNode scopingNode = startPosition.GetScopingNode();
            SplayTreeNode leftSubTree;
            SplayTreeNode middleSubTree;
            SplayTreeNode rightSubTree;
            var siblingTreeComplex = CutContent(startPosition, endPosition, out childCharCount, out leftSubTree,
                out middleSubTree, out rightSubTree);
            SplayTreeNode.Join(elementNode, leftSubTree, rightSubTree);
            elementNode.ContainedNode = middleSubTree;
            middleSubTree.ParentNode = elementNode;
            scopingNode.ContainedNode = elementNode;
            elementNode.ParentNode = scopingNode;
            return siblingTreeComplex;
        }

        private Int32 DeleteContentFromSiblingTree(SplayTreeNode containingNode,
                                                   TextPointer startPosition,
                                                   TextPointer endPosition,
                                                   Boolean newFirstIMEVisibleNode,
                                                   out Int32 charCount)
        {
            if (startPosition.CompareTo(endPosition) == 0)
            {
                if (newFirstIMEVisibleNode)
                    UpdateContainerSymbolCount(containingNode, 0, -1);
                charCount = 0;
                return 0;
            }

            var symbolOffset = startPosition.GetSymbolOffset();
            SplayTreeNode leftSubTree;
            SplayTreeNode middleSubTree;
            SplayTreeNode rightSubTree;
            var count = CutContent(startPosition, endPosition, out charCount, out leftSubTree, out middleSubTree,
                out rightSubTree);
            if (middleSubTree != null)
            {
                TextTreeNode previousNode;
                ElementEdge previousEdge;
                if (leftSubTree != null)
                {
                    previousNode = (TextTreeNode)leftSubTree.GetMaxSibling();
                    previousEdge = ElementEdge.AfterEnd;
                }
                else
                {
                    previousNode = (TextTreeNode)containingNode;
                    previousEdge = ElementEdge.AfterStart;
                }

                TextTreeNode nextNode;
                ElementEdge nextEdge;
                if (rightSubTree != null)
                {
                    nextNode = (TextTreeNode)rightSubTree.GetMinSibling();
                    nextEdge = ElementEdge.BeforeStart;
                }
                else
                {
                    nextNode = (TextTreeNode)containingNode;
                    nextEdge = ElementEdge.BeforeEnd;
                }

                AdjustRefCountsForContentDelete(ref previousNode, previousEdge, ref nextNode, nextEdge,
                    (TextTreeNode)middleSubTree);
                leftSubTree?.Splay();
                rightSubTree?.Splay();
                middleSubTree.Splay();
                Invariant.Assert(middleSubTree.ParentNode == null, "Assigning fixup node to parented child!");
                middleSubTree.ParentNode =
                    new TextTreeFixupNode(previousNode, previousEdge, nextNode, nextEdge);
            }

            var splayTreeNode = SplayTreeNode.Join(leftSubTree, rightSubTree);
            containingNode.ContainedNode = splayTreeNode;
            if (splayTreeNode != null)
                splayTreeNode.ParentNode = containingNode;
            if (count > 0)
            {
                var num = 0;
                if (newFirstIMEVisibleNode)
                    num = -1;
                UpdateContainerSymbolCount(containingNode, -count, -charCount + num);
                TextTreeText.RemoveText(_rootNode.RootTextBlock, symbolOffset, count);
                NextGeneration(true);
                Invariant.Assert(startPosition.Parent == endPosition.Parent);
                if (startPosition.Parent is ITextElement parent)
                    parent.OnTextUpdated();
            }

            return count;
        }

        private Int32 CutTopLevelLogicalNodes(TextTreeNode containingNode,
                                              TextPointer startPosition,
                                              TextPointer endPosition,
                                              out Int32 charCount)
        {
            Invariant.Assert(startPosition.GetScopingNode() == endPosition.GetScopingNode(),
                "startPosition/endPosition not in same sibling tree!");
            SplayTreeNode splayTreeNode = startPosition.GetAdjacentSiblingNode(LogicalDirection.Forward);
            SplayTreeNode adjacentSiblingNode = endPosition.GetAdjacentSiblingNode(LogicalDirection.Forward);
            var num = 0;
            charCount = 0;
            var logicalTreeNode = containingNode.GetLogicalTreeNode();
            SplayTreeNode nextNode;
            for (; splayTreeNode != adjacentSiblingNode; splayTreeNode = nextNode)
            {
                IVisualElement? child = null;
                nextNode = splayTreeNode.GetNextNode();
                switch (splayTreeNode)
                {
                    case TextTreeTextElementNode elementNode:
                        var imeCharCount = elementNode.IMECharCount;
                        var text = TextTreeText.CutText(_rootNode.RootTextBlock,
                            elementNode.GetSymbolOffset(Generation), elementNode.SymbolCount);
                        ExtractElementFromSiblingTree(containingNode, elementNode, true);
                        Invariant.Assert(elementNode.TextElement.TextElementNode != elementNode);
                        var textElementNode = elementNode.TextElement.TextElementNode;
                        UpdateContainerSymbolCount(containingNode, -textElementNode.SymbolCount, -imeCharCount);
                        NextGeneration(true);
                        var textContainer = new TextContainer(null, false);
                        var start = textContainer.Start;
                        textContainer.InsertElementToSiblingTree(start, start, textElementNode);
                        Invariant.Assert(text.Length == textElementNode.SymbolCount);
                        textContainer.UpdateContainerSymbolCount(textElementNode.GetContainingNode(),
                            textElementNode.SymbolCount, textElementNode.IMECharCount);
                        textContainer.DemandCreateText();
                        TextTreeText.InsertText(textContainer.RootTextBlock, 1, text);
                        textContainer.NextGeneration(false);
                        child = textElementNode.TextElement;
                        num += textElementNode.SymbolCount;
                        charCount += imeCharCount;
                        break;
                    case TextTreeObjectNode textTreeObjectNode:
                        child = textTreeObjectNode.EmbeddedElement;
                        break;
                }

                LogicalTreeHelper.RemoveLogicalChild(logicalTreeNode, child);
            }

            if (num > 0)
            {
                startPosition.SyncToTreeGeneration();
                endPosition.SyncToTreeGeneration();
            }

            return num;
        }

        private void AdjustRefCountsForContentDelete(ref TextTreeNode previousNode,
                                                     ElementEdge previousEdge,
                                                     ref TextTreeNode nextNode,
                                                     ElementEdge nextEdge,
                                                     TextTreeNode middleSubTree)
        {
            var leftEdgeReferenceCount = false;
            var rightEdgeReferenceCount = false;
            GetReferenceCounts((TextTreeNode)middleSubTree.GetMinSibling(), ref leftEdgeReferenceCount,
                ref rightEdgeReferenceCount);
            previousNode = previousNode.IncrementReferenceCount(previousEdge, rightEdgeReferenceCount);
            nextNode = nextNode.IncrementReferenceCount(nextEdge, leftEdgeReferenceCount);
        }

        private void GetReferenceCounts(TextTreeNode node,
                                        ref Boolean leftEdgeReferenceCount,
                                        ref Boolean rightEdgeReferenceCount)
        {
            do
            {
                leftEdgeReferenceCount = ((leftEdgeReferenceCount ? 1 : 0) |
                                          (node.BeforeStartReferenceCount ? 1 :
                                              node.BeforeEndReferenceCount ? 1 : 0)) != 0;
                rightEdgeReferenceCount = ((rightEdgeReferenceCount ? 1 : 0) |
                                           (node.AfterStartReferenceCount ? 1 : node.AfterEndReferenceCount ? 1 : 0)) !=
                                          0;
                if (node.ContainedNode != null)
                    GetReferenceCounts((TextTreeNode)node.ContainedNode.GetMinSibling(), ref leftEdgeReferenceCount,
                        ref rightEdgeReferenceCount);
                node = (TextTreeNode)node.GetNextNode();
            } while (node != null);
        }

        private void AdjustRefCountsForShallowDelete(ref TextTreeNode previousNode,
                                                     ElementEdge previousEdge,
                                                     ref TextTreeNode nextNode,
                                                     ElementEdge nextEdge,
                                                     ref TextTreeNode firstContainedNode,
                                                     ref TextTreeNode lastContainedNode,
                                                     TextTreeTextElementNode extractedElementNode)
        {
            previousNode =
                previousNode.IncrementReferenceCount(previousEdge, extractedElementNode.AfterStartReferenceCount);
            nextNode = nextNode.IncrementReferenceCount(nextEdge, extractedElementNode.BeforeEndReferenceCount);
            if (firstContainedNode != null)
                firstContainedNode = firstContainedNode.IncrementReferenceCount(ElementEdge.BeforeStart,
                    extractedElementNode.BeforeStartReferenceCount);
            else
                nextNode = nextNode.IncrementReferenceCount(nextEdge, extractedElementNode.BeforeStartReferenceCount);
            if (lastContainedNode != null)
                lastContainedNode = lastContainedNode.IncrementReferenceCount(ElementEdge.AfterEnd,
                    extractedElementNode.AfterEndReferenceCount);
            else
                previousNode =
                    previousNode.IncrementReferenceCount(previousEdge, extractedElementNode.AfterEndReferenceCount);
        }

        private Int32 CutContent(TextPointer startPosition,
                                 TextPointer endPosition,
                                 out Int32 charCount,
                                 out SplayTreeNode leftSubTree,
                                 out SplayTreeNode middleSubTree,
                                 out SplayTreeNode rightSubTree)
        {
            Invariant.Assert(startPosition.GetScopingNode() == endPosition.GetScopingNode(),
                "startPosition/endPosition not in same sibling tree!");
            Invariant.Assert((UInt32)startPosition.CompareTo(endPosition) > 0U,
                "CutContent doesn't expect empty span!");
            switch (startPosition.Edge)
            {
                case ElementEdge.BeforeStart:
                    leftSubTree = startPosition.Node.GetPreviousNode();
                    break;
                case ElementEdge.AfterStart:
                    leftSubTree = null;
                    break;
                case ElementEdge.AfterEnd:
                    leftSubTree = startPosition.Node;
                    break;
                default:
                    Invariant.Assert(false, "Unexpected edge!");
                    leftSubTree = null;
                    break;
            }

            switch (endPosition.Edge)
            {
                case ElementEdge.BeforeStart:
                    rightSubTree = endPosition.Node;
                    break;
                case ElementEdge.BeforeEnd:
                    rightSubTree = null;
                    break;
                case ElementEdge.AfterEnd:
                    rightSubTree = endPosition.Node.GetNextNode();
                    break;
                default:
                    Invariant.Assert(false, "Unexpected edge! (2)");
                    rightSubTree = null;
                    break;
            }

            if (rightSubTree == null)
            {
                middleSubTree = leftSubTree != null
                    ? leftSubTree.GetNextNode()
                    : startPosition.GetScopingNode().ContainedNode;
            }
            else
            {
                middleSubTree = rightSubTree.GetPreviousNode();
                if (middleSubTree == leftSubTree)
                    middleSubTree = null;
            }

            if (leftSubTree != null)
            {
                leftSubTree.Split();
                Invariant.Assert(leftSubTree.Role == SplayTreeNodeRole.LocalRoot);
                leftSubTree.ParentNode.ContainedNode = null;
                leftSubTree.ParentNode = null;
            }

            var num = 0;
            charCount = 0;
            if (middleSubTree != null)
            {
                if (rightSubTree != null)
                    middleSubTree.Split();
                else
                    middleSubTree.Splay();
                Invariant.Assert(middleSubTree.Role == SplayTreeNodeRole.LocalRoot,
                    "middleSubTree is not a local root!");
                if (middleSubTree.ParentNode != null)
                {
                    middleSubTree.ParentNode.ContainedNode = null;
                    middleSubTree.ParentNode = null;
                }

                for (var splayTreeNode = middleSubTree;
                     splayTreeNode != null;
                     splayTreeNode = splayTreeNode.RightChildNode)
                {
                    num += splayTreeNode.LeftSymbolCount + splayTreeNode.SymbolCount;
                    charCount += splayTreeNode.LeftCharCount + splayTreeNode.IMECharCount;
                }
            }

            if (rightSubTree != null)
                rightSubTree.Splay();
            Invariant.Assert(leftSubTree == null || leftSubTree.Role == SplayTreeNodeRole.LocalRoot);
            Invariant.Assert(middleSubTree == null || middleSubTree.Role == SplayTreeNodeRole.LocalRoot);
            Invariant.Assert(rightSubTree == null || rightSubTree.Role == SplayTreeNodeRole.LocalRoot);
            return num;
        }

        private Char[] ExtractElementInternal(ITextElement element,
                                              Boolean deep,
                                              out ExtractChangeEventArgs extractChangeEventArgs)
        {
            TextTreeTextElementNode elementNode;
            SplayTreeNode containingNode;
            TextPointer startPosition;
            TextPointer endPosition;
            Boolean empty;
            Int32 symbolOffset;
            Char[] elementText;
            TextTreeUndoUnit undoUnit;
            SplayTreeNode firstContainedChildNode;
            SplayTreeNode lastContainedChildNode;
            IBindableElement oldLogicalParent;

            BeforeAddChange();

            firstContainedChildNode = null;
            lastContainedChildNode = null;
            extractChangeEventArgs = null;

            elementText = null;
            elementNode = element.TextElementNode;
            containingNode = elementNode.GetContainingNode();
            empty = elementNode.ContainedNode == null;

            startPosition = new TextPointer(this, elementNode, ElementEdge.BeforeStart, LogicalDirection.Backward);
            // We only need the end position if this element originally spanned any content.
            endPosition = null;
            if (!empty)
            {
                endPosition = new TextPointer(this, elementNode, ElementEdge.AfterEnd, LogicalDirection.Backward);
            }

            symbolOffset = elementNode.GetSymbolOffset(Generation);

            // Remember the old parent
            oldLogicalParent = ((TextTreeNode)containingNode).GetLogicalTreeNode();

            // Invalidate any TextElementCollection that depends on the parent.
            // Make sure we do that before raising any public events.
            TextElementCollectionHelper.MarkDirty(oldLogicalParent);


            // Remove the element from the logical tree.
            // NB: we do this even for a deep extract, because we can't wait --
            // during a deep extract/move to new tree, the property system must be
            // notified before the element moves into its new tree.
            element.BeforeLogicalTreeChange();
            try
            {
                LogicalTreeHelper.RemoveLogicalChild(oldLogicalParent, element);
            }
            finally
            {
                element.AfterLogicalTreeChange();
            }

            // Handle undo.
            if (deep && !empty)
            {
                undoUnit = TextTreeUndo.CreateDeleteContentUndoUnit(this, startPosition, endPosition);
            }
            else
            {
                undoUnit = TextTreeUndo.CreateExtractElementUndoUnit(this, elementNode);
            }

            // Save the first/last contained node now -- after the ExtractElementFromSiblingTree
            // call it will be too late to find them.
            if (!deep && !empty)
            {
                firstContainedChildNode = elementNode.GetFirstContainedNode();
                lastContainedChildNode = elementNode.GetLastContainedNode();
            }

            // Record all the IME related char state before the extract.
            var imeCharCount = elementNode.IMECharCount;
            var imeLeftEdgeCharCount = elementNode.IMELeftEdgeCharCount;

            var nextNodeCharDelta = 0;

            // DevDiv.1092668 We care about the next node only if it will become the First IME Visible Sibling 
            // after the extraction. If this is a deep extract we shouldn't care if the element is empty, 
            // since all of its contents are getting extracted as well
            TextTreeTextElementNode nextNode = null;
            if ((deep || empty) && element.IsFirstIMEVisibleSibling)
            {
                nextNode = (TextTreeTextElementNode)elementNode.GetNextNode();

                if (nextNode != null)
                {
                    // The following node is the new first ime visible sibling.
                    // It just moved, and loses an edge character.
                    nextNodeCharDelta = -nextNode.IMELeftEdgeCharCount;
                    nextNode.IMECharCount += nextNodeCharDelta;
                }
            }

            // Rip the element out of its sibling tree.
            // If this is a deep extract element's TextElementNode will be updated
            // with a deep copy of all contained nodes.
            ExtractElementFromSiblingTree(containingNode, elementNode, deep);

            // The first contained node of the extracted node may no longer
            // be a first sibling after the parent extract.  If that's the case,
            // update its char count.
            var containedNodeCharDelta = 0;
            var firstContainedElementNode = firstContainedChildNode as TextTreeTextElementNode;
            if (firstContainedElementNode != null)
            {
                containedNodeCharDelta = firstContainedElementNode.IMELeftEdgeCharCount;
                firstContainedElementNode.IMECharCount += containedNodeCharDelta;
            }

            if (!deep)
            {
                // Unlink the TextElement from the TextElementNode.
                element.TextElementNode = null;

                // Pull out the edge symbols from the text store.            
                TextTreeText.RemoveElementEdges(_rootNode.RootTextBlock, symbolOffset, elementNode.SymbolCount);
            }
            else
            {
                // We leave element.TextElement alone, since for a deep extract we've already
                // stored a copy of the original nodes there that we'll use in a following insert.

                // Cut and return the matching symbols.
                elementText = TextTreeText.CutText(_rootNode.RootTextBlock, symbolOffset, elementNode.SymbolCount);
            }

            // Ancestor nodes lose either the whole node or two element edge symbols, depending
            // on whether or not this is a deep extract.
            if (deep)
            {
                UpdateContainerSymbolCount(containingNode, -elementNode.SymbolCount,
                    -imeCharCount + nextNodeCharDelta + containedNodeCharDelta);
            }
            else
            {
                UpdateContainerSymbolCount(containingNode, /* symbolCount */ -2, /* charCount */
                    -imeLeftEdgeCharCount + nextNodeCharDelta + containedNodeCharDelta);
            }

            NextGeneration(true /* deletedContent */);

            if (undoUnit != null)
            {
                undoUnit.SetTreeHashCode();
            }

            // Raise the public event.
            if (deep)
            {
                extractChangeEventArgs = new ExtractChangeEventArgs(this, startPosition, elementNode,
                    nextNodeCharDelta == 0 ? null : nextNode,
                    containedNodeCharDelta == 0 ? null : firstContainedElementNode, imeCharCount,
                    imeCharCount - imeLeftEdgeCharCount);
            }
            else if (empty)
            {
                AddChange(startPosition, /* symbolCount */ 2, /* charCount */ imeCharCount,
                    PrecursorTextChangeType.ContentRemoved);
            }
            else
            {
                AddChange(startPosition, endPosition, elementNode.SymbolCount,
                    imeLeftEdgeCharCount,
                    imeCharCount - imeLeftEdgeCharCount,
                    PrecursorTextChangeType.ElementExtracted, null, false);
            }

            // Raise events for nodes that just gained or lost an IME char due
            // to changes in their surroundings.
            if (extractChangeEventArgs == null)
            {
                if (nextNodeCharDelta != 0)
                {
                    RaiseEventForNewFirstIMEVisibleNode(nextNode);
                }

                if (containedNodeCharDelta != 0)
                {
                    RaiseEventForFormerFirstIMEVisibleNode(firstContainedElementNode);
                }
            }

            if (!deep && !empty)
            {
                ReparentLogicalChildren(firstContainedChildNode, lastContainedChildNode,
                    oldLogicalParent /* new parent */, element /* old parent */);
            }

            //
            // Remove char count for logical break, since the element is leaving the tree.
            // For more data refer to comments of dev10 bug 703174.
            //
            if (null != element.TextElementNode)
            {
                element.TextElementNode.IMECharCount -= imeLeftEdgeCharCount;
            }

            return elementText;
        }

        //private Char[] ExtractElementInternal(ITextElement element,
        //                                    Boolean deep,
        //                                    out ExtractChangeEventArgs extractChangeEventArgs)
        //{
        //    BeforeAddChange();
        //    SplayTreeNode firstChildNode = null;
        //    SplayTreeNode lastChildNode = null;
        //    extractChangeEventArgs = null;
        //    Char[] elementpublic = null;
        //    TextTreeTextElementNode textElementNode = element.TextElementNode;
        //    var containingNode = textElementNode.GetContainingNode();
        //    var flag = textElementNode.ContainedNode == null;
        //    var textPointer1 =
        //        new TextPointer(this, textElementNode, ElementEdge.BeforeStart, LogicalDirection.Backward);
        //    TextPointer textPointer2 = null;
        //    if (!flag)
        //        textPointer2 = new TextPointer(this, textElementNode, ElementEdge.AfterEnd, LogicalDirection.Backward);
        //    var symbolOffset = textElementNode.GetSymbolOffset(Generation);
        //    var logicalTreeNode = ((TextTreeNode)containingNode).GetLogicalTreeNode();
        //    TextElementCollectionHelper.MarkDirty(logicalTreeNode);
        //    element.BeforeLogicalTreeChange();
        //    try
        //    {
        //        LogicalTreeHelper.RemoveLogicalChild(logicalTreeNode, (Object)element);
        //    }
        //    finally
        //    {
        //        element.AfterLogicalTreeChange();
        //    }

        //    TextTreeUndoUnit textTreeUndoUnit = !deep || flag
        //        ? (TextTreeUndoUnit)TextTreeUndo.CreateExtractElementUndoUnit(this, textElementNode)
        //        : (TextTreeUndoUnit)TextTreeUndo.CreateDeleteContentUndoUnit(this, textPointer1, textPointer2);
        //    if (!deep && !flag)
        //    {
        //        firstChildNode = textElementNode.GetFirstContainedNode();
        //        lastChildNode = textElementNode.GetLastContainedNode();
        //    }

        //    var imeCharCount = textElementNode.IMECharCount;
        //    var leftEdgeCharCount = textElementNode.IMELeftEdgeCharCount;
        //    var num1 = 0;
        //    TextTreeTextElementNode node1 = null;
        //    if (deep | flag && element.IsFirstIMEVisibleSibling)
        //    {
        //        node1 = (TextTreeTextElementNode)textElementNode.GetNextNode();
        //        if (node1 != null)
        //        {
        //            num1 = -node1.IMELeftEdgeCharCount;
        //            node1.IMECharCount += num1;
        //        }
        //    }

        //    ExtractElementFromSiblingTree(containingNode, textElementNode, deep);
        //    var num2 = 0;
        //    if (firstChildNode is TextTreeTextElementNode node2)
        //    {
        //        num2 = node2.IMELeftEdgeCharCount;
        //        node2.IMECharCount += num2;
        //    }

        //    if (!deep)
        //    {
        //        element.TextElementNode = (TextTreeTextElementNode)null;
        //        TextTreeText.RemoveElementEdges(_rootNode.RootTextBlock, symbolOffset, textElementNode.SymbolCount);
        //    }
        //    else
        //        elementpublic =
        //            TextTreeText.CutText(_rootNode.RootTextBlock, symbolOffset, textElementNode.SymbolCount);

        //    if (deep)
        //        UpdateContainerSymbolCount(containingNode, -textElementNode.SymbolCount, -imeCharCount + num1 + num2);
        //    else
        //        UpdateContainerSymbolCount(containingNode, -2, -leftEdgeCharCount + num1 + num2);
        //    NextGeneration(true);
        //    textTreeUndoUnit?.SetTreeHashCode();
        //    if (deep)
        //        extractChangeEventArgs = new ExtractChangeEventArgs(this, textPointer1, textElementNode,
        //            num1 == 0 ? null : node1, num2 == 0 ? null : node2, imeCharCount, imeCharCount - leftEdgeCharCount);
        //    else if (flag)
        //        AddChange(textPointer1, 2, imeCharCount, PrecursorTextChangeType.ContentRemoved);
        //    else
        //        AddChange(textPointer1, textPointer2, textElementNode.SymbolCount, leftEdgeCharCount,
        //            imeCharCount - leftEdgeCharCount, PrecursorTextChangeType.ElementExtracted, null, false);
        //    if (extractChangeEventArgs == null)
        //    {
        //        if (num1 != 0)
        //            RaiseEventForNewFirstIMEVisibleNode((TextTreeNode)node1);
        //        if (num2 != 0)
        //            RaiseEventForFormerFirstIMEVisibleNode(node2);
        //    }

        //    if (!deep && !flag)
        //        ReparentLogicalChildren(firstChildNode, lastChildNode, logicalTreeNode, (IBindableElement)element);
        //    if (element.TextElementNode != null)
        //        element.TextElementNode.IMECharCount -= leftEdgeCharCount;
        //    return elementpublic;
        //}

        private void ExtractElementFromSiblingTree(SplayTreeNode containingNode,
                                                   TextTreeTextElementNode elementNode,
                                                   Boolean deep)
        {
            var previousNode = (TextTreeNode)elementNode.GetPreviousNode();
            var previousEdge = ElementEdge.AfterEnd;
            if (previousNode == null)
            {
                previousNode = (TextTreeNode)containingNode;
                previousEdge = ElementEdge.AfterStart;
            }

            var nextNode = (TextTreeNode)elementNode.GetNextNode();
            var nextEdge = ElementEdge.BeforeStart;
            if (nextNode == null)
            {
                nextNode = (TextTreeNode)containingNode;
                nextEdge = ElementEdge.BeforeEnd;
            }

            elementNode.Remove();
            Invariant.Assert(elementNode.Role == SplayTreeNodeRole.LocalRoot);
            if (deep)
            {
                AdjustRefCountsForContentDelete(ref previousNode, previousEdge, ref nextNode, nextEdge, elementNode);
                elementNode.ParentNode =
                    new TextTreeFixupNode(previousNode, previousEdge, nextNode, nextEdge);
                DeepCopy(elementNode);
            }
            else
            {
                var containedNode = elementNode.ContainedNode;
                elementNode.ContainedNode = null;
                TextTreeNode firstContainedNode;
                TextTreeNode lastContainedNode;
                if (containedNode != null)
                {
                    containedNode.ParentNode = null;
                    firstContainedNode = (TextTreeNode)containedNode.GetMinSibling();
                    lastContainedNode = (TextTreeNode)containedNode.GetMaxSibling();
                }
                else
                {
                    firstContainedNode = null;
                    lastContainedNode = null;
                }

                AdjustRefCountsForShallowDelete(ref previousNode, previousEdge, ref nextNode, nextEdge,
                    ref firstContainedNode, ref lastContainedNode, elementNode);
                elementNode.ParentNode = new TextTreeFixupNode(previousNode, previousEdge, nextNode,
                    nextEdge, firstContainedNode, lastContainedNode);
                if (containedNode == null)
                    return;
                containedNode.Splay();
                var splayTreeNode = containedNode;
                if (previousNode != containingNode)
                {
                    previousNode.Split();
                    Invariant.Assert(previousNode.Role == SplayTreeNodeRole.LocalRoot);
                    Invariant.Assert(previousNode.RightChildNode == null);
                    var minSibling = containedNode.GetMinSibling();
                    minSibling.Splay();
                    previousNode.RightChildNode = minSibling;
                    minSibling.ParentNode = previousNode;
                    splayTreeNode = previousNode;
                }

                if (nextNode != containingNode)
                {
                    nextNode.Splay();
                    Invariant.Assert(nextNode.Role == SplayTreeNodeRole.LocalRoot);
                    Invariant.Assert(nextNode.LeftChildNode == null);
                    var maxSibling = containedNode.GetMaxSibling();
                    maxSibling.Splay();
                    nextNode.LeftChildNode = maxSibling;
                    nextNode.LeftSymbolCount += maxSibling.LeftSymbolCount + maxSibling.SymbolCount;
                    nextNode.LeftCharCount += maxSibling.LeftCharCount + maxSibling.IMECharCount;
                    maxSibling.ParentNode = nextNode;
                    splayTreeNode = nextNode;
                }

                containingNode.ContainedNode = splayTreeNode;
                if (splayTreeNode == null)
                    return;
                splayTreeNode.ParentNode = containingNode;
            }
        }

        private TextTreeTextElementNode DeepCopy(TextTreeTextElementNode elementNode)
        {
            var treeTextElementNode = (TextTreeTextElementNode)elementNode.Clone();
            elementNode.TextElement.TextElementNode = treeTextElementNode;
            if (elementNode.ContainedNode != null)
            {
                treeTextElementNode.ContainedNode =
                    DeepCopyContainedNodes((TextTreeNode)elementNode.ContainedNode.GetMinSibling());
                treeTextElementNode.ContainedNode.ParentNode = treeTextElementNode;
            }

            return treeTextElementNode;
        }

        private TextTreeNode DeepCopyContainedNodes(TextTreeNode node)
        {
            TextTreeNode textTreeNode1 = null;
            TextTreeNode textTreeNode2 = null;
            do
            {
                var textTreeNode3 = !(node is TextTreeTextElementNode elementNode)
                    ? node.Clone()
                    : DeepCopy(elementNode);
                Invariant.Assert(textTreeNode3 != null || node is TextTreeTextNode && node.SymbolCount == 0);
                if (textTreeNode3 != null)
                {
                    textTreeNode3.ParentNode = (SplayTreeNode)textTreeNode2;
                    if (textTreeNode2 != null)
                    {
                        textTreeNode2.RightChildNode = textTreeNode3;
                    }
                    else
                    {
                        Invariant.Assert(textTreeNode3.Role == SplayTreeNodeRole.LocalRoot);
                        textTreeNode1 = textTreeNode3;
                    }

                    textTreeNode2 = textTreeNode3;
                }

                node = (TextTreeNode)node.GetNextNode();
            } while (node != null);

            return textTreeNode1;
        }

        private void DemandCreatePositionState()
        {
            if (_rootNode != null)
                return;
            _rootNode = new TextTreeRootNode(this);
        }

        private void DemandCreateText()
        {
            Invariant.Assert(_rootNode != null, "Unexpected DemandCreateText call before position allocation.");
            if (_rootNode.RootTextBlock != null)
                return;
            _rootNode.RootTextBlock = new TextTreeRootTextBlock();
            TextTreeText.InsertElementEdges(_rootNode.RootTextBlock, 0, 0);
        }

        private void UpdateContainerSymbolCount(SplayTreeNode containingNode,
                                                Int32 symbolCount,
                                                Int32 charCount)
        {
            do
            {
                containingNode.Splay();
                containingNode.SymbolCount += symbolCount;
                containingNode.IMECharCount += charCount;
                containingNode = containingNode.ParentNode;
            } while (containingNode != null);
        }

        private void NextGeneration(Boolean deletedContent)
        {
            AssertTree();
            AssertTreeAndTextSize();
            ++_rootNode.Generation;
            if (deletedContent)
                ++_rootNode.PositionGeneration;
            NextLayoutGeneration();
        }

        private IDependencyProperty[] LocalValueEnumeratorToArray(LocalValueEnumerator valuesEnumerator)
        {
            var array = new IDependencyProperty[valuesEnumerator.Count];
            var num = 0;
            valuesEnumerator.Reset();
            while (valuesEnumerator.MoveNext())
                array[num++] = valuesEnumerator.Current.Property;
            return array;
        }

        private void ValidateSetValue(ITextPointer position)
        {
            if (position.TextContainer != this)
                throw new InvalidOperationException(SR.Get("NotInThisTree", nameof(position)));
            position.SyncToTreeGeneration();
            if (!(position.Parent is ITextElement))
                throw new InvalidOperationException(SR.Get("NoElement"));
        }

        private void AssertTreeAndTextSize()
        {
            if (!Invariant.Strict || _rootNode.RootTextBlock == null)
                return;
            var num = 0;
            for (var textTreeTextBlock = (TextTreeTextBlock)_rootNode.RootTextBlock.ContainedNode.GetMinSibling();
                 textTreeTextBlock != null;
                 textTreeTextBlock = (TextTreeTextBlock)textTreeTextBlock.GetNextNode())
            {
                Invariant.Assert(textTreeTextBlock.Count > 0, "Empty TextBlock!");
                num += textTreeTextBlock.Count;
            }

            Invariant.Assert(num == publicSymbolCount, "TextContainer.SymbolCount does not match TextTreeText size!");
        }

        private void BeginChange(Boolean undo)
        {
            if (undo && _changeBlockUndoRecord == null && _changeBlockLevel == 0)
            {
                Invariant.Assert(_changeBlockLevel == 0);
                _changeBlockUndoRecord = new ChangeBlockUndoRecord(this, String.Empty);
            }

            if (_changeBlockLevel == 0)
            {
                DemandCreatePositionState();
                //if (Dispatcher != null)
                //    _rootNode.DispatcherProcessingDisabled = Dispatcher.DisableProcessing();
            }

            ++_changeBlockLevel;
        }

        private void FireChangeEvent(TextPointer startPosition,
                                     TextPointer endPosition,
                                     Int32 symbolCount,
                                     Int32 leftEdgeCharCount,
                                     Int32 childCharCount,
                                     PrecursorTextChangeType precursorTextChange,
                                     DependencyProperty property,
                                     Boolean affectsRenderOnly)
        {
            Invariant.Assert(ChangeHandler != null);
            SetFlags(true, Flags.ReadOnly);
            try
            {
                if (precursorTextChange == PrecursorTextChangeType.ElementAdded)
                {
                    Invariant.Assert(symbolCount > 2,
                        "ElementAdded must span at least two element edges and one content symbol!");
                    var e1 = new TextContainerChangeEventArgs(startPosition, 1, leftEdgeCharCount,
                        TextChangeType.ContentAdded);
                    var e2 = new TextContainerChangeEventArgs(endPosition, 1, 0, TextChangeType.ContentAdded);
                    ChangeHandler(this, e1);
                    ChangeHandler(this, e2);
                }
                else if (precursorTextChange == PrecursorTextChangeType.ElementExtracted)
                {
                    Invariant.Assert(symbolCount > 2,
                        "ElementExtracted must span at least two element edges and one content symbol!");
                    var e3 = new TextContainerChangeEventArgs(startPosition, 1, leftEdgeCharCount,
                        TextChangeType.ContentRemoved);
                    var e4 = new TextContainerChangeEventArgs(endPosition, 1, 0, TextChangeType.ContentRemoved);
                    ChangeHandler(this, e3);
                    ChangeHandler(this, e4);
                }
                else
                    ChangeHandler(this,
                        new TextContainerChangeEventArgs(startPosition, symbolCount, leftEdgeCharCount + childCharCount,
                            ConvertSimplePrecursorChangeToTextChange(precursorTextChange), property,
                            affectsRenderOnly));
            }
            finally
            {
                SetFlags(false, Flags.ReadOnly);
            }
        }

        private TextChangeType ConvertSimplePrecursorChangeToTextChange(PrecursorTextChangeType precursorTextChange)
        {
            Invariant.Assert(precursorTextChange != PrecursorTextChangeType.ElementAdded &&
                             precursorTextChange != PrecursorTextChangeType.ElementExtracted);
            return (TextChangeType)precursorTextChange;
        }

        private TextTreeTextElementNode GetNextIMEVisibleNode(TextPointer startPosition,
                                                              TextPointer endPosition)
        {
            TextTreeTextElementNode nextImeVisibleNode = null;
            if (startPosition.GetAdjacentElement(LogicalDirection.Forward) is ITextElement adjacentElement &&
                adjacentElement.IsFirstIMEVisibleSibling)
                nextImeVisibleNode =
                    (TextTreeTextElementNode)endPosition.GetAdjacentSiblingNode(LogicalDirection.Forward);
            return nextImeVisibleNode;
        }

        private void RaiseEventForFormerFirstIMEVisibleNode(TextTreeNode node)
        {
            AddChange(new TextPointer(this, node, ElementEdge.BeforeStart), 0, 1, PrecursorTextChangeType.ContentAdded);
        }

        private void RaiseEventForNewFirstIMEVisibleNode(TextTreeNode node)
        {
            AddChange(new TextPointer(this, node, ElementEdge.BeforeStart), 0, 1,
                PrecursorTextChangeType.ContentRemoved);
        }

        private void SetFlags(Boolean value,
                              Flags flags)
        {
            _flags = value ? _flags | flags : _flags & ~flags;
        }

        private Boolean CheckFlags(Flags flags)
        {
            return (_flags & flags) == flags;
        }

        public TextPointer Start
        {
            get
            {
                EmptyDeadPositionList();
                DemandCreatePositionState();
                var start = new TextPointer(this, _rootNode, ElementEdge.AfterStart, LogicalDirection.Backward);
                start.Freeze();
                return start;
            }
        }

        public TextPointer End
        {
            get
            {
                EmptyDeadPositionList();
                DemandCreatePositionState();
                var end = new TextPointer(this, _rootNode, ElementEdge.BeforeEnd, LogicalDirection.Forward);
                end.Freeze();
                return end;
            }
        }

        public IBindableElement? Parent => _parent;

        public ITextView TextView
        {
            get => _textview;
            set => _textview = value;
        }

        public Int32 SymbolCount => publicSymbolCount - 2;

        public Int32 publicSymbolCount => _rootNode != null ? _rootNode.SymbolCount : 2;

        public Int32 IMECharCount => _rootNode != null ? _rootNode.IMECharCount : 0;

        public TextTreeRootTextBlock RootTextBlock
        {
            get
            {
                Invariant.Assert(_rootNode != null, "Asking for TextBlocks before root node create!");
                return _rootNode.RootTextBlock;
            }
        }

        public UInt32 Generation
        {
            get
            {
                Invariant.Assert(_rootNode != null, "Asking for Generation before root node create!");
                return _rootNode.Generation;
            }
        }

        public UInt32 LayoutGeneration
        {
            get
            {
                Invariant.Assert(_rootNode != null, "Asking for LayoutGeneration before root node create!");
                return _rootNode.LayoutGeneration;
            }
        }

        public Highlights Highlights
        {
            get
            {
                if (_highlights == null)
                    _highlights = new Highlights(this);
                return _highlights;
            }
        }

        public TextTreeRootNode RootNode => _rootNode;

        public TextTreeNode FirstContainedNode =>
            _rootNode != null ? (TextTreeNode)_rootNode.GetFirstContainedNode() : null;

        public TextTreeNode LastContainedNode =>
            _rootNode != null ? (TextTreeNode)_rootNode.GetLastContainedNode() : null;

        public UndoManager UndoManager => _undoManager;

        public ITextSelection TextSelection => _textSelection;

        public Boolean HasListeners => ChangingHandler != null || ChangeHandler != null || ChangedHandler != null;

        public Boolean PlainTextOnly => CheckFlags(Flags.PlainTextOnly);

        public Boolean CollectTextChanges
        {
            get => CheckFlags(Flags.CollectTextChanges);
            set => SetFlags(value, Flags.CollectTextChanges);
        }

        //private Dispatcher Dispatcher => Parent == null ? (Dispatcher)null : ((DispatcherObject)Parent).Dispatcher;

        public event EventHandler Changing
        {
            add => ChangingHandler += value;
            remove => ChangingHandler -= value;
        }

        public event TextContainerChangeEventHandler? Change
        {
            add => ChangeHandler += value;
            remove => ChangeHandler -= value;
        }

        public event TextContainerChangedEventHandler? Changed
        {
            add => ChangedHandler += value;
            remove => ChangedHandler -= value;
        }

        private readonly IBindableElement? _parent;
        private Int32 _changeBlockLevel;
        private ChangeBlockUndoRecord _changeBlockUndoRecord;
        private TextContainerChangedEventArgs _changes;
        private Flags _flags;
        private Highlights _highlights;
        private TextTreeRootNode _rootNode;
        private ITextSelection _textSelection;
        private ITextView _textview;
        private UndoManager _undoManager;
        private TextContainerChangedEventHandler? ChangedHandler;
        private TextContainerChangeEventHandler? ChangeHandler;
        private EventHandler? ChangingHandler;

        private class ExtractChangeEventArgs
        {
            public ExtractChangeEventArgs(TextContainer textTree,
                                          TextPointer startPosition,
                                          TextTreeTextElementNode node,
                                          TextTreeTextElementNode newFirstIMEVisibleNode,
                                          TextTreeTextElementNode formerFirstIMEVisibleNode,
                                          Int32 charCount,
                                          Int32 childCharCount)
            {
                _textTree = textTree;
                _startPosition = startPosition;
                _symbolCount = node.SymbolCount;
                _charCount = charCount;
                _childCharCount = childCharCount;
                _newFirstIMEVisibleNode = newFirstIMEVisibleNode;
                _formerFirstIMEVisibleNode = formerFirstIMEVisibleNode;
            }

            public void AddChange()
            {
                _textTree.AddChange(_startPosition, _symbolCount, _charCount, PrecursorTextChangeType.ContentRemoved);
                if (_newFirstIMEVisibleNode != null)
                    _textTree.RaiseEventForNewFirstIMEVisibleNode(_newFirstIMEVisibleNode);
                if (_formerFirstIMEVisibleNode == null)
                    return;
                _textTree.RaiseEventForFormerFirstIMEVisibleNode(_formerFirstIMEVisibleNode);
            }

            public TextContainer TextContainer => _textTree;

            public Int32 ChildIMECharCount => _childCharCount;

            private readonly Int32 _charCount;
            private readonly Int32 _childCharCount;
            private readonly TextTreeTextElementNode _formerFirstIMEVisibleNode;
            private readonly TextTreeTextElementNode _newFirstIMEVisibleNode;
            private readonly TextPointer _startPosition;
            private readonly Int32 _symbolCount;
            private readonly TextContainer _textTree;
        }

        [Flags]
        private enum Flags
        {
            ReadOnly = 1,
            PlainTextOnly = 2,
            CollectTextChanges = 4,
        }
    }
}
