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
    public abstract class BasePanel<T> : BindableElement<T>, 
                                         IVisualContainer
    {
        protected BasePanel(IDataBinding<T>? binding,
                            IVisualBootstrapper visualBootstrapper)
            : this(binding, visualBootstrapper, new VisualCollection())
        {

        }

        protected BasePanel(IDataBinding<T>? binding,
                            IVisualBootstrapper visualBootstrapper,
                            IVisualCollection children)
            : base(binding, visualBootstrapper)
        {
            _children = children is VisualCollection good ? good : new VisualCollection(children);
        }

        // ReSharper disable once UnusedMember.Global
        protected BasePanel(IVisualBootstrapper templateResolver) 
            : this(null, templateResolver)
        {
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

        public void AddChildren(params IVisualElement[] elements)
        {
            _children.AddRange(elements);
            InvalidateMeasure();
        }

        public void AddChildren(IEnumerable<IVisualElement> elements)
        {
            _children.AddRange(elements);
            InvalidateMeasure();
        }

        /// <summary>
        /// sealed so inheritors of BasePanel have to override 
        /// </summary>
        protected sealed override void OnDataContextChanged(Object? newValue)
        {
            base.OnDataContextChanged(newValue);
            OnDistributeDataContextToChildren(newValue);
            
            //_children.RunOnEachChild(newValue, (nv, child) =>
            //{
            //    if (child is IBindableElement bindable)
            //        bindable.DataContext = nv;
            //});
        }

        protected abstract void OnDistributeDataContextToChildren(Object? newValue);
        

        public virtual void OnChildDeserialized(IVisualElement element, 
                                                INode node)
        {
        }

        public virtual Boolean Contains(IVisualElement element)
        {
            return _children.Contains(element);

        }

        public override void Dispose()
        {
            base.Dispose();
            _children.Dispose();
        }

        public override IVisualElement DeepCopy()
        {
            var newObject = (BasePanel<T>) base.DeepCopy();

            _children.RunOnEachChild(newObject, (p, child) =>
            {
                var stepChild = child.DeepCopy();
                p.AddChild(stepChild);
            });

           

            return newObject;
        }

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

        public override void AcceptChanges(ChangeType changeType)
        {
            base.AcceptChanges(changeType);
            _children.AcceptChanges(changeType);
        }

        public virtual Boolean IsChanged
        {
            get => _children.IsTrueForAnyChild(child => child is IChangeTracking {IsChanged: true});
        }

        public override void InvalidateMeasure()
        {
            if (_children == null)
                return;
            
            base.InvalidateMeasure();
            _children.InvalidateMeasure();
        }

        public override void InvalidateArrange()
        {
            if (_children == null)
                return;
            
            base.InvalidateArrange();
            _children.InvalidateArrange();
        }

        public override Boolean IsRequiresMeasure
        {
            get => base.IsRequiresMeasure || _children.IsRequiresMeasure;
            protected set => base.IsRequiresMeasure = value;
        }

        public override Boolean IsRequiresArrange
        {
            get => base.IsRequiresArrange || _children.IsRequiresArrange;
            protected set => base.IsRequiresArrange = value;
        }

        public override void SetBoundValue(T value)
        {
            base.SetBoundValue(value);

            _children.RunOnEachChild(value, (v, child) =>
            {
                if (!(child is IBindableElement bindable))
                    return;

                bindable.SetDataContext(v);
            });
        }

        public override async Task SetBoundValueAsync(T value)
        {
            await _children.RunOnEachChildAsync(value, async (v, child) =>
            {
                if (!(child is IBindableElement bindable))
                    return;

                await bindable.SetDataContextAsync(v);
            });
        }
    }
}