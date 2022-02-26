using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Das.Views.Core;
using Das.Views.Core.Geometry;
using Das.Views.Input.Text.Pointers;
using Das.Views.Text;
using Das.Views.Transforms;

namespace Das.Views.Input.Text
{
    public interface ITextView
    {
        ITextPointer GetTextPositionFromPoint(Point2D point,
                                              Boolean snapToText);

        Rectangle GetRectangleFromTextPosition(ITextPointer position);

        Rectangle GetRawRectangleFromTextPosition(ITextPointer position,
                                                  out ITransform transform);

        IGeometry GetTightBoundingGeometryFromTextPositions(ITextPointer startPosition,
                                                            ITextPointer endPosition);

        ITextPointer GetPositionAtNextLine(ITextPointer position,
                                           Double suggestedX,
                                           Int32 count,
                                           out Double newSuggestedX,
                                           out Int32 linesMoved);

        ITextPointer GetPositionAtNextPage(ITextPointer position,
                                           Point2D suggestedOffset,
                                           Int32 count,
                                           out Point2D newSuggestedOffset,
                                           out Int32 pagesMoved);

        Boolean IsAtCaretUnitBoundary(ITextPointer position);

        ITextPointer GetNextCaretUnitPosition(ITextPointer position,
                                              LogicalDirection direction);

        ITextPointer GetBackspaceCaretUnitPosition(ITextPointer position);

        TextSegment GetLineRange(ITextPointer position);

        ReadOnlyCollection<GlyphRun> GetGlyphRuns(ITextPointer start,
                                                  ITextPointer end);

        Boolean Contains(ITextPointer position);

        void BringPositionIntoViewAsync(ITextPointer position,
                                        Object userState);

        void BringPointIntoViewAsync(Point2D point,
                                     Object userState);

        void BringLineIntoViewAsync(ITextPointer position,
                                    Double suggestedX,
                                    Int32 count,
                                    Object userState);

        void BringPageIntoViewAsync(ITextPointer position,
                                    Point2D suggestedOffset,
                                    Int32 count,
                                    Object userState);

        void CancelAsync(Object userState);

        Boolean Validate();

        Boolean Validate(Point2D point);

        Boolean Validate(ITextPointer position);

        void ThrottleBackgroundTasksForUserInput();

        IVisualElement RenderScope { get; }

        ITextContainer TextContainer { get; }

        Boolean IsValid { get; }

        Boolean RendersOwnSelection { get; }

        ReadOnlyCollection<TextSegment> TextSegments { get; }

        event BringPositionIntoViewCompletedEventHandler BringPositionIntoViewCompleted;

        event BringPointIntoViewCompletedEventHandler BringPointIntoViewCompleted;

        event BringLineIntoViewCompletedEventHandler BringLineIntoViewCompleted;

        event BringPageIntoViewCompletedEventHandler BringPageIntoViewCompleted;

        event EventHandler Updated;
    }
}
