using System;
using System.Threading.Tasks;

namespace Das.Views.Core.Geometry
{
    public interface IPositionOffseter
    {
        IPoint2D GetOffset(IPoint2D input);
    }
}