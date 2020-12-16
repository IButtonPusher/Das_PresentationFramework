using System;
using Das.Views.Controls;
using Das.Views.Core.Enums;
using Das.Views.DataBinding;
using Das.Views.Templates;

namespace Das.Views.Defaults
{
    public class DefaultContentTemplate : //DefaultContentTemplate,
                                             IDataTemplate
    {
        protected readonly IVisualBootstrapper _visualBootstrapper;
        //private readonly IBindableElement<TDataContext>? _host;

        public DefaultContentTemplate(IVisualBootstrapper visualBootstrapper)
                                      //IBindableElement<TDataContext>? host) 
            //: base(visualBootstrapper, host)
        {
            _visualBootstrapper = visualBootstrapper;
            //_host = host;
        }

        //protected override IBindableElement<TLabel> GetToStringLabel<TLabel>(TLabel dataContext)
        //{
            
            
        //    //if (!(dataContext is T valid))
        //    //    throw new InvalidCastException(dataContext + " cannot cast to " + typeof(T));
              
        //    //var txt = new Label<T>(
        //    //    new ObjectBinding<T>(valid),
        //    //    _visualBootstrapper)
        //    //{
        //    //    HorizontalAlignment = HorizontalAlignments.Center, 
        //    //    VerticalAlignment = VerticalAlignments.Center
        //    //};

        //    //return txt;
        //}

        public virtual IVisualElement? BuildVisual(Object? dataContext)
        {
            if (dataContext == null)
                return default;

            var dataTemplate = _visualBootstrapper.TryResolveFromContext(dataContext);
            if (dataTemplate != null)
                return dataTemplate.BuildVisual(dataContext);

            return GetToStringLabel(dataContext);
        }
        
        protected virtual IBindableElement GetToStringLabel(Object? dataContext)
        {
            var txt = new Label(_visualBootstrapper)
                //(new ObjectBinding<TLabel>(dataContext))
                {
                    HorizontalAlignment = HorizontalAlignments.Center, 
                    VerticalAlignment = VerticalAlignments.Center,
                    Text = dataContext?.ToString() ?? String.Empty
                };

            return txt;
        }

        //public virtual TVisualElement BuildVisual<TVisualElement>(TDataContext dataContext) 
        //    where TVisualElement : IBindableElement<TDataContext>
        //{
        //    var bilt = BuildVisual(dataContext);
        //    switch (bilt)
        //    {
        //        case TVisualElement good:
        //            return good;
        //    }

        //    throw new InvalidOperationException();
        //}

        public Type? DataType => null; //typeof(TDataContext);

        IVisualElement IDataTemplate.BuildVisual()
        {
            throw new NotSupportedException();
        }

        //public IVisualElement? BuildVisual(Object? dataContext)
        //{
        //    if (dataContext == null)
        //        return default;
            
            

        //    var dataTemplate = _visualBootstrapper.TryResolveFromContext(dataContext);
        //    if (dataTemplate != null)
        //        return dataTemplate.BuildVisual(dataContext);

        //    return GetToStringLabel(dataContext);
        //}
    }
}
