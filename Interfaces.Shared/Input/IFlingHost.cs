using System;
using Das.Views.Core.Geometry;

namespace Das.Views.Input
{
    public interface IFlingHost
    {
        Boolean CanFlingVertical { get; }

        Boolean CanFlingHorizontal { get; }

        ValueMinMax GetVerticalMinMaxFling();
        
        ValueMinMax GetHorizontalMinMaxFling();


        void OnFlingStarting(Double totalHorizontalChange,
                             Double totalVerticalChange);

        void OnFlingStep(Double deltaHorizontal,
                         Double deltaVertical);

        void OnFlingEnded(Boolean wasCancelled);
    }
}
