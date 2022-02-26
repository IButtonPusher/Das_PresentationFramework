using System;
using System.Threading.Tasks;
using Das.Views.Core;
using Das.Views.Input.Text.Pointers;
using Das.Views.Text.Highlight;

namespace Das.Views.Text
{
    /// <summary>
    ///     A group of non-overlapping text highlights with a single owner.
    ///     Conceptually, this object is a collection of ranges and a list
    ///     of property/values pairs highlighting content under each range.
    /// </summary>
    public abstract class HighlightLayer
    {
        //------------------------------------------------------
        //
        //  Internal Properties
        //
        //------------------------------------------------------

        #region Internal Properties

        /// <summary>
        ///     Type identifying the owner of this layer for Highlights.GetHighlightValue calls.
        /// </summary>
        internal abstract Type OwnerType { get; }

        #endregion Internal Properties

        //------------------------------------------------------
        //
        //  Internal Events
        //
        //------------------------------------------------------

        #region Internal Events

        /// <summary>
        ///     Event raised when a highlight is inserted, removed, moved, or
        ///     has a local property value change.
        /// </summary>
        internal abstract event HighlightChangedEventHandler Changed;

        #endregion Internal Events

        //------------------------------------------------------
        //
        //  Internal Methods
        //
        //------------------------------------------------------

        #region Internal Methods

        /// <summary>
        ///     Returns the value of a property stored on scoping highlight, if any.
        /// </summary>
        /// <param name="textPosition">
        ///     Position to query.
        /// </param>
        /// <param name="direction">
        ///     Direction of content to query.
        /// </param>
        /// <returns>
        ///     The property value if set on any scoping highlight.  If no property
        ///     value is set, returns DependencyProperty.UnsetValue.
        /// </returns>
        internal abstract Object GetHighlightValue(StaticTextPointer textPosition,
                                                   LogicalDirection direction);

        /// <summary>
        ///     Returns true iff the indicated content has scoping highlights.
        /// </summary>
        /// <param name="textPosition">
        ///     Position to query.
        /// </param>
        /// <param name="direction">
        ///     Direction of content to query.
        /// </param>
        internal abstract Boolean IsContentHighlighted(StaticTextPointer textPosition,
                                                       LogicalDirection direction);

        /// <summary>
        ///     Returns the position of the next highlight start or end in an
        ///     indicated direction, or null if there is no such position.
        /// </summary>
        /// <param name="textPosition">
        ///     Position to query.
        /// </param>
        /// <param name="direction">
        ///     Direction of content to query.
        /// </param>
        internal abstract StaticTextPointer GetNextChangePosition(StaticTextPointer textPosition,
                                                                  LogicalDirection direction);

        #endregion Internal Methods
    }
}
