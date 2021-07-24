using System;
using Das.Views.Core.Enums;
using Das.Views.DataBinding;
using Das.Views.Templates;

namespace Das.Views.Defaults
{
    /// <summary>
    /// Searches for a data template that matches based on the data context's type.
    /// If none is found, builds a label with the text as DataContext.ToString()
    /// </summary>
    public class DefaultContentTemplate : IDataTemplate
    {
        protected readonly IVisualBootstrapper _visualBootstrapper;

        public DefaultContentTemplate(IVisualBootstrapper visualBootstrapper)
        {
            _visualBootstrapper = visualBootstrapper;
            
        }

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
            var txt = new NullTemplateLabel(_visualBootstrapper)
            {
                    HorizontalAlignment = HorizontalAlignments.Center, 
                    VerticalAlignment = VerticalAlignments.Center,
                    Text = dataContext?.ToString() ?? String.Empty
            };

            return txt;
        }

       
        public Type? DataType => null; //typeof(TDataContext);

        IVisualElement IDataTemplate.BuildVisual()
        {
            throw new NotSupportedException();
        }

    }
}
