using System;
using System.Collections.Generic;
using System.Text;
using Das.Views.Panels;
using Das.Views.Rendering;

namespace Das.Views
{
    public class DataTemplate : IDataTemplate
    {
        private readonly IVisualBootstrapper _visualBootstrapper;

        public DataTemplate(IVisualBootstrapper visualBootstrapper,
                            Type? dataType, 
                            IVisualElement content)
        {
            _visualBootstrapper = visualBootstrapper;
            DataType = dataType;
            Content = content;
        }

        public Type? DataType { get; }

        public IVisualElement? BuildVisual(Object? dataContext)
        {
            return _visualBootstrapper.InstantiateCopy(Content, dataContext);
        }

        public TVisualElement BuildVisual<TVisualElement>(Object? dataContext) 
            where TVisualElement : IVisualElement
        {
            var bilt = BuildVisual(dataContext);
            switch (bilt)
            {
                case TVisualElement good:
                    return good;
            }

            throw new InvalidOperationException();
        }

        public IVisualElement Content { get; }
    }
}
