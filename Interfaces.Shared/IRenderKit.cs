using System;
using System.Threading.Tasks;
using Das.Views.Rendering;

namespace Das.Views
{
    public interface IRenderKit
    {
        IMeasureContext MeasureContext { get; }

        IRenderContext RenderContext { get; }

        TInterface Resolve<TObject, TInterface>() 
            where TObject : TInterface;

        void ResolveTo<TInterface, TObject>(TObject obj) 
            where TObject : class, TInterface;

        T Resolve<T>();
    }
}