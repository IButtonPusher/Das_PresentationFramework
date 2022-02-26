using System;
using System.Threading.Tasks;
using Das.Views.Core;
using Das.Views.Input.Text.Pointers;
using Das.Views.Input.Text.Validation;
using Das.Views.Validation;

namespace Das.Views.Input.Text
{
    public struct TextSegment
    {
        internal static readonly TextSegment Null;
        private readonly ITextPointer _start;
        private readonly ITextPointer _end;

        internal TextSegment(ITextPointer startPosition,
                             ITextPointer endPosition)
            : this(startPosition, endPosition, false)
        {
        }

        internal TextSegment(ITextPointer startPosition,
                             ITextPointer endPosition,
                             Boolean preserveLogicalDirection)
        {
            ValidationHelper.VerifyPositionPair(startPosition, endPosition);
            if (startPosition.CompareTo(endPosition) == 0)
            {
                _start = startPosition.GetFrozenPointer(startPosition.LogicalDirection);
                _end = _start;
            }
            else
            {
                Invariant.Assert(startPosition.CompareTo(endPosition) < 0);
                _start = startPosition.GetFrozenPointer(preserveLogicalDirection
                    ? startPosition.LogicalDirection
                    : LogicalDirection.Backward);
                _end = endPosition.GetFrozenPointer(preserveLogicalDirection
                    ? endPosition.LogicalDirection
                    : LogicalDirection.Forward);
            }
        }

        internal Boolean Contains(ITextPointer position)
        {
            return !IsNull && _start.CompareTo(position) <= 0 && position.CompareTo(_end) <= 0;
        }

        internal ITextPointer Start => _start;

        internal ITextPointer End => _end;

        internal Boolean IsNull => _start == null || _end == null;
    }
}
