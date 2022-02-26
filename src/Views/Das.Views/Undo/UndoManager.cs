using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.DataBinding;
using Das.Views.Localization;
using Das.Views.Validation;

namespace Das.Views.Undo
{
    public class UndoManager
    {
        public UndoManager()
        {
            _scope = null;
            _state = UndoState.Normal;
            _isEnabled = false;
            _undoStack = new List<IUndoUnit>(4);
            _redoStack = new Stack(2);
            _topUndoIndex = -1;
            _bottomUndoIndex = 0;
            _undoLimit = 100;
        }

        public static void AttachUndoManager(IBindableElement scope,
                                             UndoManager undoManager)
        {
            if (scope == null)
                throw new ArgumentNullException(nameof(scope));
            if (undoManager == null)
                throw new ArgumentNullException(nameof(undoManager));
            if (undoManager._scope != null)
                throw new InvalidOperationException(SR.Get("UndoManagerAlreadyAttached"));
            DetachUndoManager(scope);
            scope.SetValue(UndoManagerInstanceProperty, (Object)undoManager);

            undoManager._scope = scope;
            undoManager.IsEnabled = true;
        }

        public static void DetachUndoManager(IBindableElement scope)
        {
            if (scope == null)
                throw new ArgumentNullException(nameof(scope));
            if (!(scope.ReadLocalValue(UndoManagerInstanceProperty) is UndoManager undoManager))
                return;
            undoManager.IsEnabled = false;
            scope.ClearValue(UndoManagerInstanceProperty);
            if (undoManager == null)
                return;
            undoManager._scope = null;
        }

        public static UndoManager GetUndoManager(IBindableElement target)
        {
            return target == null ? null : target.GetValue(UndoManagerInstanceProperty);
        }

        public void Open(IParentUndoUnit unit)
        {
            if (!IsEnabled)
                throw new InvalidOperationException(SR.Get("UndoServiceDisabled"));
            if (unit == null)
                throw new ArgumentNullException(nameof(unit));
            var deepestOpenUnit = DeepestOpenUnit;
            if (deepestOpenUnit == unit)
                throw new InvalidOperationException(SR.Get("UndoUnitCantBeOpenedTwice"));
            if (deepestOpenUnit == null)
            {
                if (unit != LastUnit)
                {
                    Add(unit);
                    SetLastUnit(unit);
                }

                SetOpenedUnit(unit);
                unit.Container = this;
            }
            else
            {
                unit.Container = deepestOpenUnit;
                deepestOpenUnit.Open(unit);
            }
        }

        public void Reopen(IParentUndoUnit unit)
        {
            if (!IsEnabled)
                throw new InvalidOperationException(SR.Get("UndoServiceDisabled"));
            if (unit == null)
                throw new ArgumentNullException(nameof(unit));
            if (OpenedUnit != null)
                throw new InvalidOperationException(SR.Get("UndoUnitAlreadyOpen"));
            switch (State)
            {
                case UndoState.Normal:
                case UndoState.Redo:
                    if (UndoCount == 0 || PeekUndoStack() != unit)
                        throw new InvalidOperationException(SR.Get("UndoUnitNotOnTopOfStack"));
                    break;
                case UndoState.Undo:
                    if (RedoStack.Count == 0 || (IParentUndoUnit)RedoStack.Peek() != unit)
                        throw new InvalidOperationException(SR.Get("UndoUnitNotOnTopOfStack"));
                    break;
            }

            if (unit.Locked)
                throw new InvalidOperationException(SR.Get("UndoUnitLocked"));
            Open(unit);
            _lastReopenedUnit = unit;
        }

        public void Close(UndoCloseAction closeAction)
        {
            Close(OpenedUnit, closeAction);
        }

        public void Close(IParentUndoUnit unit,
                          UndoCloseAction closeAction)
        {
            if (!IsEnabled)
                throw new InvalidOperationException(SR.Get("UndoServiceDisabled"));
            if (unit == null)
                throw new ArgumentNullException(nameof(unit));
            if (OpenedUnit == null)
                throw new InvalidOperationException(SR.Get("UndoNoOpenUnit"));
            if (OpenedUnit != unit)
            {
                var openedUnit = OpenedUnit;
                while (openedUnit.OpenedUnit != null && openedUnit.OpenedUnit != unit)
                    openedUnit = openedUnit.OpenedUnit;
                if (openedUnit.OpenedUnit == null)
                    throw new ArgumentException(SR.Get("UndoUnitNotFound"), nameof(unit));
                openedUnit.Close(closeAction);
            }
            else if (closeAction != UndoCloseAction.Commit)
            {
                SetState(UndoState.Rollback);
                if (unit.OpenedUnit != null)
                    unit.Close(closeAction);
                if (closeAction == UndoCloseAction.Rollback)
                    unit.Do();
                PopUndoStack();
                SetOpenedUnit(null);
                OnNextDiscard();
                SetLastUnit(_topUndoIndex == -1 ? null : PeekUndoStack());
                SetState(UndoState.Normal);
            }
            else
            {
                if (unit.OpenedUnit != null)
                    unit.Close(UndoCloseAction.Commit);
                if (State != UndoState.Redo && State != UndoState.Undo && RedoStack.Count > 0)
                    RedoStack.Clear();
                SetOpenedUnit(null);
            }
        }

        public void Add(IUndoUnit unit)
        {
            if (!IsEnabled)
                throw new InvalidOperationException(SR.Get("UndoServiceDisabled"));
            if (unit == null)
                throw new ArgumentNullException(nameof(unit));
            var deepestOpenUnit = DeepestOpenUnit;
            if (deepestOpenUnit != null)
            {
                deepestOpenUnit.Add(unit);
            }
            else
            {
                if (!(unit is IParentUndoUnit))
                    throw new InvalidOperationException(SR.Get("UndoNoOpenParentUnit"));
                ((IParentUndoUnit)unit).Container = this;
                if (LastUnit is IParentUndoUnit)
                    ((IParentUndoUnit)LastUnit).OnNextAdd();
                SetLastUnit(unit);
                if (State == UndoState.Normal || State == UndoState.Redo)
                {
                    if (++_topUndoIndex == UndoLimit)
                        _topUndoIndex = 0;
                    if ((_topUndoIndex >= UndoStack.Count || PeekUndoStack() != null) &&
                        (UndoLimit == -1 || UndoStack.Count < UndoLimit))
                    {
                        UndoStack.Add(unit);
                    }
                    else
                    {
                        if (PeekUndoStack() != null && ++_bottomUndoIndex == UndoLimit)
                            _bottomUndoIndex = 0;
                        UndoStack[_topUndoIndex] = unit;
                    }
                }
                else if (State == UndoState.Undo)
                {
                    RedoStack.Push(unit);
                }
                else
                {
                    var state = (Int32)State;
                }
            }
        }

        public void Clear()
        {
            if (!IsEnabled)
                throw new InvalidOperationException(SR.Get("UndoServiceDisabled"));
            if (_imeSupportModeEnabled)
                return;
            DoClear();
        }

        public void Undo(Int32 count)
        {
            if (!IsEnabled)
                throw new InvalidOperationException(SR.Get("UndoServiceDisabled"));
            if (count > UndoCount || count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (State != UndoState.Normal)
                throw new InvalidOperationException(SR.Get("UndoNotInNormalState"));
            if (OpenedUnit != null)
                throw new InvalidOperationException(SR.Get("UndoUnitOpen"));
            Invariant.Assert(UndoCount > _minUndoStackCount);
            SetState(UndoState.Undo);
            var flag = true;
            try
            {
                for (; count > 0; --count)
                    PopUndoStack().Do();
                flag = false;
            }
            finally
            {
                if (flag)
                    Clear();
            }

            SetState(UndoState.Normal);
        }

        public void Redo(Int32 count)
        {
            if (!IsEnabled)
                throw new InvalidOperationException(SR.Get("UndoServiceDisabled"));
            if (count > RedoStack.Count || count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (State != UndoState.Normal)
                throw new InvalidOperationException(SR.Get("UndoNotInNormalState"));
            if (OpenedUnit != null)
                throw new InvalidOperationException(SR.Get("UndoUnitOpen"));
            SetState(UndoState.Redo);
            var flag = true;
            try
            {
                for (; count > 0; --count)
                    ((IUndoUnit)RedoStack.Pop()).Do();
                flag = false;
            }
            finally
            {
                if (flag)
                    Clear();
            }

            SetState(UndoState.Normal);
        }

        public virtual void OnNextDiscard()
        {
            if (UndoCount <= 0)
                return;
            ((IParentUndoUnit)PeekUndoStack()).OnNextDiscard();
        }

        public IUndoUnit PeekUndoStack()
        {
            return _topUndoIndex < 0 || _topUndoIndex == UndoStack.Count ? null : UndoStack[_topUndoIndex];
        }

        public Stack SetRedoStack(Stack value)
        {
            var redoStack = _redoStack;
            if (value == null)
                value = new Stack(2);
            _redoStack = value;
            return redoStack;
        }

        public IUndoUnit GetUndoUnit(Int32 index)
        {
            Invariant.Assert(index < UndoCount);
            Invariant.Assert(index >= 0);
            Invariant.Assert(_bottomUndoIndex == 0);
            Invariant.Assert(_imeSupportModeEnabled);
            return _undoStack[index];
        }

        public void RemoveUndoRange(Int32 index,
                                    Int32 count)
        {
            Invariant.Assert(index >= 0);
            Invariant.Assert(count >= 0);
            Invariant.Assert(count + index <= UndoCount);
            Invariant.Assert(_bottomUndoIndex == 0);
            Invariant.Assert(_imeSupportModeEnabled);
            for (var index1 = index + count; index1 <= _topUndoIndex; ++index1)
                _undoStack[index1 - count] = _undoStack[index1];
            for (var index2 = _topUndoIndex - (count - 1); index2 <= _topUndoIndex; ++index2)
                _undoStack[index2] = null;
            _topUndoIndex -= count;
        }

        protected void SetState(UndoState value)
        {
            _state = value;
        }

        protected void SetOpenedUnit(IParentUndoUnit value)
        {
            _openedUnit = value;
        }

        protected void SetLastUnit(IUndoUnit value)
        {
            _lastUnit = value;
        }

        private void DoClear()
        {
            Invariant.Assert(!_imeSupportModeEnabled);
            if (UndoStack.Count > 0)
            {
                UndoStack.Clear();
                UndoStack.TrimExcess();
            }

            if (RedoStack.Count > 0)
                RedoStack.Clear();
            SetLastUnit(null);
            SetOpenedUnit(null);
            _topUndoIndex = -1;
            _bottomUndoIndex = 0;
        }

        private IUndoUnit PopUndoStack()
        {
            var num = UndoCount - 1;
            var undo = UndoStack[_topUndoIndex];
            UndoStack[_topUndoIndex--] = null;
            if (_topUndoIndex >= 0 || num <= 0)
                return undo;
            Invariant.Assert(UndoLimit > 0);
            _topUndoIndex = UndoLimit - 1;
            return undo;
        }

        public Boolean IsImeSupportModeEnabled
        {
            get => _imeSupportModeEnabled;
            set
            {
                if (value == _imeSupportModeEnabled)
                    return;
                if (value)
                {
                    if (_bottomUndoIndex != 0 && _topUndoIndex >= 0)
                    {
                        var undoUnitList = new List<IUndoUnit>(UndoCount);
                        if (_bottomUndoIndex > _topUndoIndex)
                        {
                            for (var bottomUndoIndex = _bottomUndoIndex; bottomUndoIndex < UndoLimit; ++bottomUndoIndex)
                                undoUnitList.Add(_undoStack[bottomUndoIndex]);
                            _bottomUndoIndex = 0;
                        }

                        for (var bottomUndoIndex = _bottomUndoIndex;
                             bottomUndoIndex <= _topUndoIndex;
                             ++bottomUndoIndex)
                            undoUnitList.Add(_undoStack[bottomUndoIndex]);
                        _undoStack = undoUnitList;
                        _bottomUndoIndex = 0;
                        _topUndoIndex = undoUnitList.Count - 1;
                    }

                    _imeSupportModeEnabled = value;
                }
                else
                {
                    _imeSupportModeEnabled = value;
                    if (!IsEnabled)
                    {
                        DoClear();
                    }
                    else
                    {
                        if (UndoLimit < 0 || _topUndoIndex < UndoLimit)
                            return;
                        var undoUnitList = new List<IUndoUnit>(UndoLimit);
                        for (var index = _topUndoIndex + 1 - UndoLimit; index <= _topUndoIndex; ++index)
                            undoUnitList.Add(_undoStack[index]);
                        _undoStack = undoUnitList;
                        _bottomUndoIndex = 0;
                        _topUndoIndex = UndoLimit - 1;
                    }
                }
            }
        }

        public Int32 UndoLimit
        {
            get => !_imeSupportModeEnabled ? _undoLimit : -1;
            set
            {
                _undoLimit = value;
                if (_imeSupportModeEnabled)
                    return;
                DoClear();
            }
        }

        public UndoState State => _state;

        public Boolean IsEnabled
        {
            get
            {
                if (_imeSupportModeEnabled)
                    return true;
                return _isEnabled && (UInt32)_undoLimit > 0U;
            }
            set => _isEnabled = value;
        }

        public IParentUndoUnit OpenedUnit => _openedUnit;

        public IUndoUnit LastUnit => _lastUnit;

        public IParentUndoUnit LastReopenedUnit => _lastReopenedUnit;

        public Int32 UndoCount => UndoStack.Count == 0 || _topUndoIndex < 0 ? 0 :
            _topUndoIndex != _bottomUndoIndex - 1 || PeekUndoStack() != null ? _topUndoIndex < _bottomUndoIndex
                ? _topUndoIndex + (UndoLimit - _bottomUndoIndex) + 1
                : _topUndoIndex - _bottomUndoIndex + 1 : 0;

        public Int32 RedoCount => RedoStack.Count;

        public static Int32 UndoLimitDefaultValue => 100;

        public Int32 MinUndoStackCount
        {
            get => _minUndoStackCount;
            set => _minUndoStackCount = value;
        }

        protected IParentUndoUnit DeepestOpenUnit
        {
            get
            {
                var openedUnit = OpenedUnit;
                if (openedUnit != null)
                {
                    while (openedUnit.OpenedUnit != null)
                        openedUnit = openedUnit.OpenedUnit;
                }

                return openedUnit;
            }
        }

        protected List<IUndoUnit> UndoStack => _undoStack;

        protected Stack RedoStack => _redoStack;

        private const Int32 _undoLimitDefaultValue = 100;

        private static readonly DependencyProperty<UndoManager> UndoManagerInstanceProperty =
            DependencyProperty<UndoManager>.Register(
                "UndoManagerInstance", null, typeof(UndoManager));

        private Int32 _bottomUndoIndex;
        private Boolean _imeSupportModeEnabled;
        private Boolean _isEnabled;
        private IParentUndoUnit? _lastReopenedUnit;
        private IUndoUnit? _lastUnit;
        private Int32 _minUndoStackCount;
        private IParentUndoUnit? _openedUnit;
        private Stack _redoStack;
        private IBindableElement? _scope;
        private UndoState _state;
        private Int32 _topUndoIndex;
        private Int32 _undoLimit;
        private List<IUndoUnit> _undoStack;
    }
}
