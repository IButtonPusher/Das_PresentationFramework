using System;
using System.Threading.Tasks;
using Das.Container;
using Das.Views.Controls;
using Das.Views.Rendering;

namespace Das.Views
{
    public interface IRenderKit
    {
        IMeasureContext MeasureContext { get; }

        IRenderContext RenderContext { get; }

        IResolver Resolver { get; }

        //TInterface Resolve<TObject, TInterface>() 
        //    where TObject : TInterface;

        //void ResolveTo<TInterface, TObject>(TObject obj) 
        //    where TObject : class, TInterface;

        void RegisterSurrogate<T>(Func<IVisualElement, IVisualSurrogate> builder)
            where T : IVisualElement;

        //T Resolve<T>();
    }
}