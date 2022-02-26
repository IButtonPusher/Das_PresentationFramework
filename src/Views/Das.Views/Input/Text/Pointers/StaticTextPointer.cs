using System;
using System.Threading.Tasks;
using Das.Views.Core;
using Das.Views.DataBinding;
using Das.Views.Validation;

namespace Das.Views.Input.Text.Pointers
{
    public struct StaticTextPointer
    {
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

        public StaticTextPointer(ITextContainer textContainer,
                                 Object handle0) : this(textContainer, handle0, 0)
        {
        }

        public StaticTextPointer(ITextContainer textContainer,
                                 Object handle0,
                                 Int32 handle1)
        {
            _textContainer = textContainer;
            _generation = textContainer != null ? textContainer.Generation : 0;
            _handle0 = handle0;
            _handle1 = handle1;
        }

        #endregion Constructors

        //------------------------------------------------------
        //
        //  Internal Methods
        //
        //------------------------------------------------------

        #region Internal Methods

        internal ITextPointer CreateDynamicTextPointer(LogicalDirection direction)
        {
            AssertGeneration();

            return _textContainer.CreateDynamicTextPointer(this, direction);
        }

        internal TextPointerContext GetPointerContext(LogicalDirection direction)
        {
            AssertGeneration();

            return _textContainer.GetPointerContext(this, direction);
        }

        internal Int32 GetOffsetToPosition(StaticTextPointer position)
        {
            AssertGeneration();

            return _textContainer.GetOffsetToPosition(this, position);
        }

        internal Int32 GetTextInRun(LogicalDirection direction,
                                    Char[] textBuffer,
                                    Int32 startIndex,
                                    Int32 count)
        {
            AssertGeneration();

            return _textContainer.GetTextInRun(this, direction, textBuffer, startIndex, count);
        }

        internal Object GetAdjacentElement(LogicalDirection direction)
        {
            AssertGeneration();

            return _textContainer.GetAdjacentElement(this, direction);
        }

        internal StaticTextPointer CreatePointer(Int32 offset)
        {
            AssertGeneration();

            return _textContainer.CreatePointer(this, offset);
        }

        internal StaticTextPointer GetNextContextPosition(LogicalDirection direction)
        {
            AssertGeneration();

            return _textContainer.GetNextContextPosition(this, direction);
        }

        internal Int32 CompareTo(StaticTextPointer position)
        {
            AssertGeneration();

            return _textContainer.CompareTo(this, position);
        }

        internal Int32 CompareTo(ITextPointer position)
        {
            AssertGeneration();

            return _textContainer.CompareTo(this, position);
        }

        internal Object GetValue(DependencyProperty formattingProperty)
        {
            AssertGeneration();

            return _textContainer.GetValue(this, formattingProperty);
        }

        internal static StaticTextPointer Min(StaticTextPointer position1,
                                              StaticTextPointer position2)
        {
            position2.AssertGeneration();

            return position1.CompareTo(position2) <= 0 ? position1 : position2;
        }

        internal static StaticTextPointer Max(StaticTextPointer position1,
                                              StaticTextPointer position2)
        {
            position2.AssertGeneration();

            return position1.CompareTo(position2) >= 0 ? position1 : position2;
        }

        // Asserts this StaticTextPointer is synchronized to the current tree generation.
        internal void AssertGeneration()
        {
            if (_textContainer != null)
            {
                Invariant.Assert(_generation == _textContainer.Generation,
                    "StaticTextPointer not synchronized to tree generation!");
            }
        }

        #endregion Internal Methods

        //------------------------------------------------------
        //
        //  Internal Properties
        //
        //------------------------------------------------------

        #region Internal Properties

        internal ITextContainer TextContainer => _textContainer;

        internal IBindableElement Parent => _textContainer.GetParent(this);

        internal Boolean IsNull => _textContainer == null;

        internal Object Handle0 => _handle0;

        internal Int32 Handle1 => _handle1;

        #endregion Internal Properties

        //------------------------------------------------------
        //
        //  Internal Fields
        //
        //------------------------------------------------------

        #region Internal Fields

        internal static StaticTextPointer Null = new StaticTextPointer(null, null, 0);

        #endregion Internal Fields

        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private readonly ITextContainer _textContainer;
        private readonly UInt32 _generation;

        private readonly Object _handle0;
        private readonly Int32 _handle1;

        #endregion Private Fields
    }
}
