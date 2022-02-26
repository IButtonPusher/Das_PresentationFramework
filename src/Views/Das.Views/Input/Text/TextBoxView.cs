using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Das.Views.Core;
using Das.Views.Core.Geometry;
using Das.Views.DataBinding;
using Das.Views.Input.Text;
using Das.Views.Input.Text.Pointers;
using Das.Views.Text;
using Das.Views.Transforms;

namespace Das.Views.Input
{
    public class TextBoxView : BindableElement,
                               ITextView
    {
        protected TextBoxView(IVisualBootstrapper visualBootstrapper) : base(visualBootstrapper)
        {
        }

        public ITextPointer GetTextPositionFromPoint(Point2D point,
                                                     Boolean snapToText)
        {
            throw new NotImplementedException();
            //Invariant.Assert(this.IsLayoutValid);
            //point = this.TransformToDocumentSpace(point);
            //int lineIndexFromPoint = this.GetLineIndexFromPoint(point, snapToText);
            //ITextPointer positionFromPoint;
            //if (lineIndexFromPoint == -1)
            //{
            //    positionFromPoint = (ITextPointer) null;
            //}
            //else
            //{
            //    positionFromPoint = this.GetTextPositionFromDistance(lineIndexFromPoint, point.X);
            //    positionFromPoint.Freeze();
            //}
            //return positionFromPoint;
        }

        public Rectangle GetRectangleFromTextPosition(ITextPointer position)
        {
            throw new NotImplementedException();
        }

        public Rectangle GetRawRectangleFromTextPosition(ITextPointer position,
                                                         out ITransform transform)
        {
            throw new NotImplementedException();
        }

        public IGeometry GetTightBoundingGeometryFromTextPositions(ITextPointer startPosition,
                                                                   ITextPointer endPosition)
        {
            throw new NotImplementedException();
        }

        public ITextPointer GetPositionAtNextLine(ITextPointer position,
                                                  Double suggestedX,
                                                  Int32 count,
                                                  out Double newSuggestedX,
                                                  out Int32 linesMoved)
        {
            throw new NotImplementedException();
        }

        public ITextPointer GetPositionAtNextPage(ITextPointer position,
                                                  Point2D suggestedOffset,
                                                  Int32 count,
                                                  out Point2D newSuggestedOffset,
                                                  out Int32 pagesMoved)
        {
            throw new NotImplementedException();
        }

        public Boolean IsAtCaretUnitBoundary(ITextPointer position)
        {
            throw new NotImplementedException();
        }

        public ITextPointer GetNextCaretUnitPosition(ITextPointer position,
                                                     LogicalDirection direction)
        {
            throw new NotImplementedException();
        }

        public ITextPointer GetBackspaceCaretUnitPosition(ITextPointer position)
        {
            throw new NotImplementedException();
        }

        public TextSegment GetLineRange(ITextPointer position)
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<GlyphRun> GetGlyphRuns(ITextPointer start,
                                                         ITextPointer end)
        {
            throw new NotImplementedException();
        }

        public Boolean Contains(ITextPointer position)
        {
            throw new NotImplementedException();
        }

        public void BringPositionIntoViewAsync(ITextPointer position,
                                               Object userState)
        {
            throw new NotImplementedException();
        }

        public void BringPointIntoViewAsync(Point2D point,
                                            Object userState)
        {
            throw new NotImplementedException();
        }

        public void BringLineIntoViewAsync(ITextPointer position,
                                           Double suggestedX,
                                           Int32 count,
                                           Object userState)
        {
            throw new NotImplementedException();
        }

        public void BringPageIntoViewAsync(ITextPointer position,
                                           Point2D suggestedOffset,
                                           Int32 count,
                                           Object userState)
        {
            throw new NotImplementedException();
        }

        public void CancelAsync(Object userState)
        {
            throw new NotImplementedException();
        }

        public Boolean Validate()
        {
            throw new NotImplementedException();
        }

        public Boolean Validate(Point2D point)
        {
            throw new NotImplementedException();
        }

        public Boolean Validate(ITextPointer position)
        {
            throw new NotImplementedException();
        }

        public void ThrottleBackgroundTasksForUserInput()
        {
            throw new NotImplementedException();
        }

        public IVisualElement RenderScope => throw new NotImplementedException();

        public ITextContainer TextContainer => throw new NotImplementedException();

        public Boolean IsValid => throw new NotImplementedException();

        public Boolean RendersOwnSelection => throw new NotImplementedException();

        public ReadOnlyCollection<TextSegment> TextSegments => throw new NotImplementedException();

        public event BringPositionIntoViewCompletedEventHandler? BringPositionIntoViewCompleted;

        public event BringPointIntoViewCompletedEventHandler? BringPointIntoViewCompleted;

        public event BringLineIntoViewCompletedEventHandler? BringLineIntoViewCompleted;

        public event BringPageIntoViewCompletedEventHandler? BringPageIntoViewCompleted;

        public event EventHandler? Updated;

        private Point2D TransformToDocumentSpace(Point2D point)
        {
            throw new NotImplementedException();
            //if (this._scrollData != null)
            //    point = new Point(point.X + this._scrollData.HorizontalOffset, point.Y + this._scrollData.VerticalOffset);
            //point.X -= this.GetTextAlignmentCorrection(this.CalculatedTextAlignment, this.GetWrappingWidth(this.RenderSize.Width));
            //point.Y -= this.VerticalAlignmentOffset;
            //return point;
        }

        private Boolean IsLayoutValid => !IsRequiresMeasure && !IsRequiresArrange;
    }
}
