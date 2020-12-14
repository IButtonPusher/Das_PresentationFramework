using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Das.Serializer;
using Das.Views.Collections;
using Das.Views.DataBinding;
using Das.Views.Rendering;

namespace Das.Views.Panels
{
    public abstract class BasePanel<TVisual, TDataContext> : BindableElement<TDataContext>,
                                                             IVisualContainer<TVisual>,
                                                             IPanelElement
        where TVisual : IVisualElement
    {
        protected BasePanel(//IDataBinding<TDataContext>? binding, 
                            IVisualBootstrapper visualBootstrapper,
                            IVisualCollection<TVisual> children) 
            : base(visualBootstrapper)
        {
            _children = children is VisualCollection<TVisual> good 
                ? good 
                : new VisualCollection<TVisual>(children);
        }

        protected BasePanel(//IDataBinding<TDataContext>? binding,
                            IVisualBootstrapper visualBootstrapper)
            //: this(binding, visualBootstrapper, new VisualCollection<TVisual>())
            : this(visualBootstrapper, new VisualCollection<TVisual>())
        {

        }
        
        
        //protected BasePanel(IVisualBootstrapper visualBootstrapper) 
        //    : this(null, visualBootstrapper)
        //{
        //}

        public Boolean Contains(IVisualElement element)
        {
            return element is TVisual tviz && _children.Contains(tviz);
        }

        public void AcceptChanges()
        {
            _children.AcceptChanges(ChangeType.Measure);
            _children.AcceptChanges(ChangeType.Arrange);
        }

        public Boolean IsChanged => _children.IsTrueForAnyChild(child => child is IChangeTracking {IsChanged: true});

        protected readonly VisualCollection<TVisual> _children;

        public IVisualCollection<TVisual> Children => _children;

        void IPanelElement.AddChild(IVisualElement element)
        {
            ((IList)_children).Add(element);
        }

        public void AddChildren(IEnumerable<IVisualElement> elements)
        {
            _children.AddRange(elements);
        }

        public void AddChild(TVisual element)
        {
            _children.Add(element);

            InvalidateMeasure();
        }

        public Boolean RemoveChild(TVisual element)
        {
            var changed = _children.Remove(element);

            if (changed)
                InvalidateMeasure();

            return changed;
        }

        public void AddChildren(IEnumerable<TVisual> elements)
        {
            _children.AddRange(elements);

            InvalidateMeasure();
        }

        //public override IVisualElement DeepCopy()
        //{
        //    var panel = base.DeepCopy() as BasePanel<TVisual, TDataContext>
        //                   ?? throw new Exception(nameof(DeepCopy) + " failed");
            
        //    panel.AddChildren(Children.GetAllChildren());
        //    return panel;
        //}

        public virtual void OnChildDeserialized(TVisual element, 
                                                INode node)
        {
            
        }

        IVisualCollection IPanelElement.Children => _children;
    }
}
