using System;
using System.Collections.Generic;
using Das.Views.Panels;
using Das.Views.Rendering;

namespace Das.Views
{
    public class MultiDataTemplate : IMultiDataTemplate
    {
        public MultiDataTemplate(IVisualBootstrapper visualBootstrapper, 
                                 Type? dataType,
                                 IEnumerable<IVisualElement> visualElements)
        {
            _visualBootstrapper = visualBootstrapper;
            _visualElements = new List<IVisualElement>(visualElements);
            DataType = dataType;
        }
        
        public Type? DataType { get; }

        IVisualElement? IDataTemplate.BuildVisual(Object? dataContext)
        {
            var sp = new StackPanel<Object>(_visualBootstrapper);
            
            foreach (var vis in BuildVisuals(dataContext))
                sp.AddChild(vis);

            return sp;
        }

        public TVisualElement BuildVisual<TVisualElement>(Object? dataContext) 
            where TVisualElement : IVisualElement
        {
            foreach (var visual in _visualElements)
            {
                if (visual is TVisualElement)
                {
                    var res = _visualBootstrapper.InstantiateCopy(visual, dataContext);
                    
                    switch (res)
                    {
                        case TVisualElement good:
                            return good;
                    }
                }
            }
            
            
            throw new InvalidOperationException();
        }

        public IEnumerable<IVisualElement> BuildVisuals(Object? dataContext)
        {
            foreach (var visual in _visualElements)
            {
                yield return _visualBootstrapper.InstantiateCopy(visual, dataContext);
            }
        }
        
        private readonly IVisualBootstrapper _visualBootstrapper;
        private readonly List<IVisualElement> _visualElements;
    }
}
