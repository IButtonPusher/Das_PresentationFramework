using System;
using System.Threading.Tasks;

namespace Das.Views.Undo
{
    public enum UndoState
    {
        Normal,
        Undo,
        Redo,
        Rollback,
    }
}
