using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Das.Serializer;
using Das.Views.DataBinding;
using Das.Views.Rendering;

//using Das.Serializer;

namespace Das.Views.Panels
{
    public abstract class BasePanel<T> : BindableElement<T>, 
                                         IVisualContainer
    {
        protected BasePanel(IDataBinding<T>? binding,
                            IVisualBootstrapper visualBootstrapper)
            : base(binding, visualBootstrapper)
        {
            //_lockChildren = new Object();
        }

        // ReSharper disable once UnusedMember.Global
        protected BasePanel(IVisualBootstrapper templateResolver) 
            : this(null, templateResolver)
        {
        }

        public IList<IVisualElement> Children
        {
            get
            {
                lock (_lockChildren)
                    return _children;
            }
        }

        //public IEnumerable<IVisualElement> Children
        //{
        //    get
        //    {
        //        lock (_lockChildren)
        //        {
        //            for (var c = 0; c < _children.Count; c++)
        //                yield return _children[c];
        //        }
        //    }
        //}

        public void AddChild(IVisualElement element)
        {
            lock (_lockChildren)
            {
                _children.Add(element);
            }
        }

        public void AddChildren(params IVisualElement[] elements)
        {
            lock (_lockChildren)
            {
                foreach (var element in elements)
                    _children.Add(element);
            }
        }

        public virtual void OnChildDeserialized(IVisualElement element, INode node)
        {
        }

        public virtual Boolean Contains(IVisualElement element)
        {
            foreach (var child in Children)
            {
                if (child == element)
                    return true;

                if (child is IVisualContainer container &&
                    container.Contains(element))
                    return true;
            }

            return false;
        }

        public override IVisualElement DeepCopy()
        {
            var newObject = (BasePanel<T>) base.DeepCopy();
            foreach (var child in Children)
            {
                var stepChild = child.DeepCopy();
                newObject.AddChild(stepChild);
            }

            return newObject;
        }

        public virtual void AcceptChanges()
        {
            foreach (var changeChild in Children.OfType<IChangeTracking>())
                changeChild.AcceptChanges();
        }

        public virtual Boolean IsChanged
        {
            get
            {
                foreach (var changeChild in Children.OfType<IChangeTracking>())
                    if (changeChild.IsChanged)
                        return true;

                return false;
            }
        }

        public override void InvalidateMeasure()
        {
            base.InvalidateMeasure();

            foreach (var child in Children)
            {
                child.InvalidateMeasure();
            }
        }

        public override void InvalidateArrange()
        {
            base.InvalidateArrange();

            foreach (var child in Children)
            {
                child.InvalidateArrange();
            }
        }

        public override Boolean IsRequiresMeasure
        {
            get
            {
                if (base.IsRequiresMeasure)
                    return true;

                foreach (var child in Children)
                {
                    if (child.IsRequiresMeasure)
                        return true;
                }

                return false;
            }
            protected set => base.IsRequiresMeasure = value;
        }

        public override Boolean IsRequiresArrange
        {
            get
            {
                if (base.IsRequiresArrange)
                    return true;

                foreach (var child in Children)
                {
                    if (child.IsRequiresArrange)
                        return true;
                }

                return false;
            }
            protected set => base.IsRequiresArrange = value;
        }

        public override void SetBoundValue(T value)
        {
            base.SetBoundValue(value);

            foreach (var child in Children)
            {
                if (!(child is IBindableElement bindable))
                    continue;

                bindable.SetDataContext(value);
            }
        }

        public override async Task SetBoundValueAsync(T value)
        {
            //await base.SetBoundValueAsync(value);

            foreach (var child in Children)
            {
                if (!(child is IBindableElement bindable))
                    continue;

                await bindable.SetDataContextAsync(value);
            }
        }

        private readonly List<IVisualElement> _children = new List<IVisualElement>();
        private readonly Object _lockChildren = new Object();
    }
}