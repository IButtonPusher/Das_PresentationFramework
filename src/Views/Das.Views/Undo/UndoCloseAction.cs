using System;
using System.Threading.Tasks;

namespace Das.Views.Undo
{
    public enum UndoCloseAction
    {
        Commit,
        Rollback,
        Discard,
    }
}
