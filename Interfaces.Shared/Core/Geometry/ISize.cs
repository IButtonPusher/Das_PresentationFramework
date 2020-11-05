using System;
using System.Threading.Tasks;

namespace Das.Views.Core.Geometry
{
    public interface ISize : IEquatable<ISize>,
                             IDeepCopyable<ISize>
    {
        Double Height { get; }

        Boolean IsEmpty { get; }

        Double Width { get; }

        ISize Minus(ISize subtract);

        ISize Reduce(Thickness padding);

        ///// <summary>
        ///// Gives the Y value needed to show an item centered in this size
        ///// </summary>
        ///// <example>This has a height of 100.  <param name="item" /> has a size of 20,
        ///// return value would be 40</example>
        //Double CenterY(ISize item);
    }
}