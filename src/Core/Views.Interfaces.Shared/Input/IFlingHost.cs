using System;
using Das.Views.Core.Geometry;

namespace Das.Views.Input;

public interface IFlingHost
{
   FlingMode VerticalFlingMode { get; }

   FlingMode HorizontalFlingMode { get; }

   //Boolean CanFlingVertical { get; }

   //Boolean CanFlingHorizontal { get; }
        
   Double CurrentX { get; }
        
   Double CurrentY { get; }

   ValueMinMax GetVerticalMinMaxFling();
        
   ValueMinMax GetHorizontalMinMaxFling();


   void OnFlingStarting(Double totalHorizontalChange,
                        Double totalVerticalChange);

   void OnFlingStep(Double deltaHorizontal,
                    Double deltaVertical);

   void OnFlingEnded(Boolean wasCancelled);
}