using System;

namespace Das.Views.Core.Geometry
{
    public interface IPositionOffseter
    {
        IPoint GetOffset(IPoint input);
    }
}