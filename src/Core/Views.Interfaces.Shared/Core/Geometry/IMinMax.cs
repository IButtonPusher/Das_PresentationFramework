using System;
using System.Threading.Tasks;

namespace Das.Views.Core.Geometry
{
    public interface IMinMax<T> : IEquatable<IMinMax<T>>
        where T : IConvertible//, IEquatable<T>
    {
        T Max { get; }

        T Min { get; }

        //IMinMax<IConvertible> ToConvertible();

        Boolean IsEmpty { get; }

        IMinMax<T> Empty { get; }

        Boolean Overlaps(IMinMax<T> mm);

        Boolean Contains(T item);

        Boolean Contains(IMinMax<T> item);

        /// <summary>
        /// Requires item's max to equal Max or item's min to equal Min!
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        IMinMax<T> Minus(IMinMax<T> item);
    }
}
