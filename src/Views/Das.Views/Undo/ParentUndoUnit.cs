using System;
using System.Collections;
using System.Threading.Tasks;
using Das.Views.Localization;
using Das.Views.Validation;

namespace Das.Views.Undo
{
    public class ParentUndoUnit : IParentUndoUnit, IUndoUnit
    {
        public ParentUndoUnit(String description)
        {
            Init(description);
        }

        public virtual void Open(IParentUndoUnit newUnit)
        {
            if (newUnit == null)
                throw new ArgumentNullException(nameof(newUnit));
            var deepestOpenUnit = DeepestOpenUnit;
            if (deepestOpenUnit == null)
            {
                _openedUnit = !IsInParentUnitChain(newUnit)
                    ? newUnit
                    : throw new InvalidOperationException(SR.Get("UndoUnitCantBeOpenedTwice"));
                if (newUnit == null)
                    return;
                newUnit.Container = this;
            }
            else
            {
                if (newUnit != null)
                    newUnit.Container = deepestOpenUnit;
                deepestOpenUnit.Open(newUnit);
            }
        }

        public virtual void Close(UndoCloseAction closeAction)
        {
            Close(OpenedUnit, closeAction);
        }

        public virtual void Close(IParentUndoUnit unit,
                                  UndoCloseAction closeAction)
        {
            if (unit == null)
                throw new ArgumentNullException(nameof(unit));
            if (OpenedUnit == null)
                throw new InvalidOperationException(SR.Get("UndoNoOpenUnit"));
            if (OpenedUnit != unit)
            {
                IParentUndoUnit parentUndoUnit = this;
                while (parentUndoUnit.OpenedUnit != null && parentUndoUnit.OpenedUnit != unit)
                    parentUndoUnit = parentUndoUnit.OpenedUnit;
                if (parentUndoUnit.OpenedUnit == null)
                    throw new ArgumentException(SR.Get("UndoUnitNotFound"), nameof(unit));
                if (parentUndoUnit != this)
                {
                    parentUndoUnit.Close(closeAction);
                    return;
                }
            }

            var topContainer = TopContainer as UndoManager;
            if (closeAction != UndoCloseAction.Commit)
            {
                if (topContainer != null)
                    topContainer.IsEnabled = false;
                if (OpenedUnit.OpenedUnit != null)
                    OpenedUnit.Close(closeAction);
                if (closeAction == UndoCloseAction.Rollback)
                    OpenedUnit.Do();
                _openedUnit = null;
                if (TopContainer is UndoManager)
                    ((UndoManager)TopContainer).OnNextDiscard();
                else
                    ((IParentUndoUnit)TopContainer).OnNextDiscard();
                if (topContainer == null)
                    return;
                topContainer.IsEnabled = true;
            }
            else
            {
                if (OpenedUnit.OpenedUnit != null)
                    OpenedUnit.Close(UndoCloseAction.Commit);
                var openedUnit = OpenedUnit;
                _openedUnit = null;
                Add(openedUnit);
                SetLastUnit(openedUnit);
            }
        }

        public virtual void Add(IUndoUnit unit)
        {
            if (unit == null)
                throw new ArgumentNullException(nameof(unit));
            var deepestOpenUnit = DeepestOpenUnit;
            if (deepestOpenUnit != null)
            {
                deepestOpenUnit.Add(unit);
            }
            else
            {
                if (IsInParentUnitChain(unit))
                    throw new InvalidOperationException(SR.Get("UndoUnitCantBeAddedTwice"));
                if (Locked)
                    throw new InvalidOperationException(SR.Get("UndoUnitLocked"));
                if (Merge(unit))
                    return;
                _units.Push(unit);
                if (LastUnit is IParentUndoUnit)
                    ((IParentUndoUnit)LastUnit).OnNextAdd();
                SetLastUnit(unit);
            }
        }

        public virtual void Clear()
        {
            if (Locked)
                throw new InvalidOperationException(SR.Get("UndoUnitLocked"));
            _units.Clear();
            SetOpenedUnit(null);
            SetLastUnit(null);
        }

        public virtual void OnNextAdd()
        {
            _locked = true;
            foreach (IUndoUnit unit in _units)
            {
                if (unit is IParentUndoUnit)
                    ((IParentUndoUnit)unit).OnNextAdd();
            }
        }

        public virtual void OnNextDiscard()
        {
            _locked = false;
            IParentUndoUnit parentUndoUnit = this;
            foreach (IUndoUnit unit in _units)
            {
                if (unit is IParentUndoUnit)
                    parentUndoUnit = unit as IParentUndoUnit;
            }

            if (parentUndoUnit == this)
                return;
            parentUndoUnit.OnNextDiscard();
        }

        /// <summary>
        ///     Implements IUndoUnit::Do().  For IParentUndoUnit, this means iterating through
        ///     all contained units and calling their Do().
        /// </summary>
        public virtual void Do()
        {
            IParentUndoUnit redo;
            UndoManager topContainer;

            // Create the parent redo unit
            redo = CreateParentUndoUnitForSelf();
            topContainer = TopContainer as UndoManager;

            if (topContainer != null)
            {
                if (topContainer.IsEnabled)
                {
                    topContainer.Open(redo);
                }
            }

            while (_units.Count > 0)
            {
                IUndoUnit unit;

                unit = _units.Pop() as IUndoUnit;
                unit.Do();
            }

            if (topContainer != null)
            {
                if (topContainer.IsEnabled)
                {
                    topContainer.Close(redo, UndoCloseAction.Commit);
                }
            }
        }

        public virtual Boolean Merge(IUndoUnit unit)
        {
            Invariant.Assert(unit != null);
            return false;
        }

        public String Description
        {
            get => _description;
            set
            {
                if (value == null)
                    value = String.Empty;
                _description = value;
            }
        }

        public IParentUndoUnit OpenedUnit => _openedUnit;

        public IUndoUnit LastUnit => _lastUnit;

        public virtual Boolean Locked
        {
            get => _locked;
            protected set => _locked = value;
        }

        public Object Container
        {
            get => _container;
            set
            {
                switch (value)
                {
                    case IParentUndoUnit _:
                    case UndoManager _:
                        _container = value;
                        break;
                    default:
                        throw new Exception(SR.Get("UndoContainerTypeMismatch"));
                }
            }
        }

        protected void Init(String description)
        {
            if (description == null)
                description = String.Empty;
            _description = description;
            _locked = false;
            _openedUnit = null;
            _units = new Stack(2);
            _container = null;
        }

        protected void SetOpenedUnit(IParentUndoUnit value)
        {
            _openedUnit = value;
        }

        protected void SetLastUnit(IUndoUnit value)
        {
            _lastUnit = value;
        }

        protected virtual IParentUndoUnit CreateParentUndoUnitForSelf()
        {
            return new ParentUndoUnit(Description);
        }

        /// <summary>
        ///     Walk up the parent undo unit chain and make sure none of the parent units
        ///     in that chain are the same as the given unit.
        /// </summary>
        /// <param name="unit">
        ///     Unit to search for in the parent chain
        /// </param>
        /// <returns>
        ///     true if the unit is already in the parent chain, false otherwise
        /// </returns>
        private Boolean IsInParentUnitChain(IUndoUnit unit)
        {
            if (unit is IParentUndoUnit)
            {
                IParentUndoUnit parent;

                parent = this;
                do
                {
                    if (parent == unit)
                    {
                        return true;
                    }

                    parent = parent.Container as IParentUndoUnit;
                } while (parent != null);
            }

            return false;
        }

        protected IParentUndoUnit DeepestOpenUnit
        {
            get
            {
                var openedUnit = _openedUnit;
                if (openedUnit != null)
                {
                    while (openedUnit.OpenedUnit != null)
                        openedUnit = openedUnit.OpenedUnit;
                }

                return openedUnit;
            }
        }

        protected Object TopContainer
        {
            get
            {
                Object topContainer = this;
                while (topContainer is IParentUndoUnit && ((IParentUndoUnit)topContainer).Container != null)
                    topContainer = ((IParentUndoUnit)topContainer).Container;
                return topContainer;
            }
        }

        protected Stack Units => _units;

        private Object _container;
        private String _description;
        private IUndoUnit _lastUnit;
        private Boolean _locked;
        private IParentUndoUnit _openedUnit;
        private Stack _units;
    }
}
