using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Das.Serializer;
using Das.Views.Collections;
using Das.Views.DataBinding;
using Das.Views.Rendering;

namespace Das.Views.Panels
{
   public abstract class BasePanel : BindableElement,
                                         IVisualContainer
                                         
    {
        //protected BasePanel(IDataBinding? binding,
        //                    IVisualBootstrapper visualBootstrapper)
        //    : base(binding, visualBootstrapper)
        //{
        //    _children = new VisualCollection();
        //}

        // ReSharper disable once UnusedMember.Global
        protected BasePanel(IVisualBootstrapper visualBootstrapper)
            //: this(null, visualBootstrapper)
            : this(visualBootstrapper, new VisualCollection())
        {
            
        }
        
        protected BasePanel(IVisualBootstrapper visualBootstrapper,
                            IVisualCollection children)
            
            : base(visualBootstrapper)
        {
            _children = children is VisualCollection good ? good : new VisualCollection(children);
        }

        /// <summary>
        /// sealed so inheritors of BasePanel have to override 
        /// </summary>
        protected sealed override void OnDataContextChanged(Object? newValue)
        {
            base.OnDataContextChanged(newValue);
            OnDistributeDataContextToChildren(newValue);
        }

        protected virtual void OnDistributeDataContextToChildren(Object? newValue)
        {
         //   Children.DistributeDataContext(newValue);
        }
        
        protected readonly VisualCollection _children;

        public IVisualCollection Children => _children;

        public void AddChild(IVisualElement element)
        {
            _children.Add(element);
            InvalidateMeasure();
        }

        public Boolean RemoveChild(IVisualElement element)
        {
            var changed = _children.Remove(element);

            if (changed)
                InvalidateMeasure();

            return changed;
        }

        public void AddChildren(IEnumerable<IVisualElement> elements)
        {
            _children.AddRange(elements);
            InvalidateMeasure();
        }

        public virtual void OnChildDeserialized(IVisualElement element, INode node)
        {
        }

        public virtual Boolean Contains(IVisualElement element)
        {
            return _children.IsTrueForAnyChild(element, (child, e) =>
            {
                if (child == element)
                    return true;

                return child is IVisualContainer container &&
                       container.Contains(element);
            });
        }

        //public override IVisualElement DeepCopy()
        //{
        //    var newObject = (BasePanel) base.DeepCopy();
        //    _children.RunOnEachChild(newObject, (p, child) =>
        //    {
        //        var stepChild = child.DeepCopy();
        //        p.AddChild(stepChild);
        //    });

        //    return newObject;
        //}

        public virtual void AcceptChanges()
        {
            _children.RunOnEachChild(child =>
            {
                if (child is IChangeTracking changeTracking)
                    changeTracking.AcceptChanges();
            });

            AcceptChanges(ChangeType.Measure);
            AcceptChanges(ChangeType.Arrange);
        }

        public virtual Boolean IsChanged
        {
            get => _children.IsTrueForAnyChild(child => child is IChangeTracking {IsChanged: true});
        }

        public override void InvalidateMeasure()
        {
            base.InvalidateMeasure();

            _children.RunOnEachChild(child => child.InvalidateMeasure());
        }

        public override void InvalidateArrange()
        {
            base.InvalidateArrange();

            _children.RunOnEachChild(child => child.InvalidateArrange());
        }

        public override Boolean IsRequiresMeasure
        {
            get => base.IsRequiresMeasure || 
                   _children.IsTrueForAnyChild(child => child.IsRequiresMeasure);
            protected set => base.IsRequiresMeasure = value;
        }

        public override Boolean IsRequiresArrange
        {
            get => base.IsRequiresArrange || 
                   _children.IsTrueForAnyChild(child => child.IsRequiresArrange);
            protected set => base.IsRequiresArrange = value;
        }

        //public override void SetBoundValue(Object? value)
        //{
        //    base.SetBoundValue(value);

        //    _children.RunOnEachChild(value, (v, child) =>
        //    {
        //        if (!(child is IBindableElement bindable))
        //            return;

        //        bindable.SetDataContext(v);
        //    });
        //}

        public override void Dispose()
        {
            base.Dispose();
            _children.Dispose();
        }

        //public override async Task SetBoundValueAsync(Object? value)
        //{
        //    await _children.RunOnEachChildAsync(value, async (v, child) =>
        //    {
        //        if (!(child is IBindableElement bindable))
        //            return;

        //        await bindable.SetDataContextAsync(v);
        //    });
        //}


        //private readonly List<IVisualElement> _children;
        
        //private readonly Object _lockChildren;
    }
}