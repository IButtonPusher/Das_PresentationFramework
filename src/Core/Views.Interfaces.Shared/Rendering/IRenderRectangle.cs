using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;

namespace Das.Views.Rendering
{
    public interface IRenderRectangle : IRectangle,
                                        IRenderSize
    {
        new IRenderSize Size { get; }

        TRectangle Reduce<TRectangle>(Double left,
                                      Double top,
                                      Double right,
                                      Double bottom)
            where TRectangle : IRenderRectangle;
    }

    //public interface IRenderRectangle : IRenderRectangle<IRenderRectangle>,
    //                                    IRenderSize
    //{
    //    new IRenderSize Size { get; }
    //}

    //public interface IRenderRectangle<out T> : IRectangle,
    //                                           IRenderSize
    //    where T : IRenderRectangle<T>
    //{
    //    T Reduce(Double left,
    //             Double top,
    //             Double right,
    //             Double bottom);
    //}
}