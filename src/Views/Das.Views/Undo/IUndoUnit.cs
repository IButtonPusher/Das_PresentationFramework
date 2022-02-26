using System;
using System.Threading.Tasks;

namespace Das.Views.Undo
{
    public interface IUndoUnit
    {
        void Do();

        Boolean Merge(IUndoUnit unit);
    }
}
