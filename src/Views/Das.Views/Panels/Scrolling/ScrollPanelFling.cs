using System;
using Das.Views.Core.Geometry;
using Das.Views.Input;

namespace Das.Views.Panels;

public partial class ScrollPanel
{
   FlingMode IFlingHost.VerticalFlingMode => IsScrollsVertical ? FlingMode.Default : FlingMode.None;

   FlingMode IFlingHost.HorizontalFlingMode => IsScrollsHorizontal ? FlingMode.Inverted : FlingMode.None;

   Double IFlingHost.CurrentX => HorizontalOffset;

   Double IFlingHost.CurrentY => VerticalOffset;

   public ValueMinMax GetVerticalMinMaxFling() =>
      IsScrollsVertical
         ? new ValueMinMax(0 - VerticalOffset, _maximumYScroll - VerticalOffset)
         : ValueMinMax.Empty;

   public ValueMinMax GetHorizontalMinMaxFling() =>
      IsScrollsHorizontal
         ? new ValueMinMax(0 - HorizontalOffset, _maximumXScroll - HorizontalOffset)
         : ValueMinMax.Empty;


   void IFlingHost.OnFlingStarting(Double totalHorizontalChange,
                                   Double totalVerticalChange)
   {
      OnScrollTransitionStarting?.Invoke(totalHorizontalChange, totalVerticalChange);
   }

   void IFlingHost.OnFlingStep(Double deltaHorizontal,
                               Double deltaVertical)
   {
      //Debug.WriteLine("fling step x: " + deltaHorizontal + " y: " + deltaVertical);

      OnScroll(deltaHorizontal, deltaVertical);

      //Debug.WriteLine("after step x: " + HorizontalOffset + " y: " + VerticalOffset);
   }

   public void OnFlingEnded(Boolean wasCancelled)
   {
      //Debug.WriteLine("***end of fling v-offset: " + VerticalOffset + 
      //                " h: " + HorizontalOffset + "***");

      if (_inputContext is { } inputContext)
         inputContext.TryReleaseMouseCapture(this);

      OnScrollTransitionEnded?.Invoke();
   }
}