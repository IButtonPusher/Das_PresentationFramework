using System;
using System.Threading.Tasks;
using Das.Views.Input.Text.Pointers;
using Das.Views.Undo;

namespace Das.Views.Input.Text.Tree
{
    public static class TextTreeUndo
    {
        // Adds a TextTreeInsertElementUndoUnit to the open parent undo unit, if any.
        // Called from TextContainer.InsertElement.
        public static void CreateInsertElementUndoUnit(TextContainer tree,
                                                       Int32 symbolOffset,
                                                       Boolean deep)
        {
            UndoManager undoManager;

            undoManager = GetOrClearUndoManager(tree);
            if (undoManager == null)
                return;

            undoManager.Add(new TextTreeInsertElementUndoUnit(tree, symbolOffset, deep));
        }

        public static TextTreeDeleteContentUndoUnit CreateDeleteContentUndoUnit(TextContainer tree, TextPointer start, TextPointer end)
        {
            UndoManager undoManager;
            TextTreeDeleteContentUndoUnit undoUnit;

            if (start.CompareTo(end) == 0)
                return null;

            undoManager = GetOrClearUndoManager(tree);
            if (undoManager == null)
                return null;

            undoUnit = new TextTreeDeleteContentUndoUnit(tree, start, end);

            undoManager.Add(undoUnit);

            return undoUnit;
        }
        
        // Adds a TextTreeInsertUndoUnit to the open parent undo unit, if any.
        // Called from TextContainer.InsertText and TextContainer.InsertEmbeddedObject.
        public static void CreateInsertUndoUnit(TextContainer tree, int symbolOffset, int symbolCount)
        {
            UndoManager undoManager;

            undoManager = GetOrClearUndoManager(tree);
            if (undoManager == null)
                return;

            undoManager.Add(new TextTreeInsertUndoUnit(tree, symbolOffset, symbolCount));
        }

        // Returns the local UndoManager.
        // Returns null if there is no undo service or if the service exists
        // but is disabled or if there is no open parent undo unit.
        public static UndoManager GetOrClearUndoManager(ITextContainer textContainer)
        {
            UndoManager undoManager;

            undoManager = textContainer.UndoManager;
            if (undoManager == null)
                return null;

            if (!undoManager.IsEnabled)
                return null;

            if (undoManager.OpenedUnit == null)
            {
                // There's no parent undo unit, so we can't open a child.
                //
                // Clear the undo stack -- since we depend on symbol offsets
                // matching the original document state when an undo unit is
                // executed, any of our units currently in the stack will be
                // corrupted after we finished the operation in progress.
                undoManager.Clear();
                return null;
            }

            return undoManager;
        }

        public static TextTreeExtractElementUndoUnit CreateExtractElementUndoUnit(TextContainer tree, TextTreeTextElementNode elementNode)
        {
            UndoManager undoManager;
            TextTreeExtractElementUndoUnit undoUnit;

            undoManager = GetOrClearUndoManager(tree);
            if (undoManager == null)
                return null;

            undoUnit = new TextTreeExtractElementUndoUnit(tree, elementNode);

            undoManager.Add(undoUnit);

            return undoUnit;
        }

    }
}
