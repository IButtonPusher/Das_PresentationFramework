using System;
using System.Threading.Tasks;

namespace Das.Views.Undo
{
    public interface IParentUndoUnit : IUndoUnit
    {
        void Clear();

        void Open(IParentUndoUnit newUnit);

        void Close(UndoCloseAction closeAction);

        void Close(IParentUndoUnit closingUnit,
                   UndoCloseAction closeAction);

        void Add(IUndoUnit newUnit);

        void OnNextAdd();

        void OnNextDiscard();

        IUndoUnit LastUnit { get; }

        IParentUndoUnit OpenedUnit { get; }

        String Description { get; set; }

        Boolean Locked { get; }

        Object Container { get; set; }
    }
}
