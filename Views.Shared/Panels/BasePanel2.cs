using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Das.Serializer;
using Das.Views.DataBinding;
using Das.Views.Rendering;

namespace Das.Views.Panels
{
   public abstract class BasePanel : BindableElement,
                                         IVisualContainer
    {
        protected BasePanel(IDataBinding? binding,
                            IVisualBootstrapper visualBootstrapper)
            : base(binding, visualBootstrapper)
        {
            Children = new VisualCollection();
        }

        // ReSharper disable once UnusedMember.Global
        protected BasePanel(IVisualBootstrapper visualBootstrapper) 
            : this(null, visualBootstrapper)
        {
        }

        //public IList<IVisualElement> Children
        //{
        //    get
        //    {
        //        lock (_lockChildren)
        //            return _children;
        //    }
        //}

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

        public IVisualCollection Children { get; }

        public void AddChild(IVisualElement element)
        {
            Children.Add(element);
            InvalidateMeasure();
        }

        public Boolean RemoveChild(IVisualElement element)
        {
            var changed = Children.Remove(element);

            if (changed)
                InvalidateMeasure();

            return changed;
        }

        public void AddChildren(params IVisualElement[] elements)
        {
            Children.AddRange(elements);
            InvalidateMeasure();
        }

        public virtual void OnChildDeserialized(IVisualElement element, INode node)
        {
        }

        public virtual Boolean Contains(IVisualElement element)
        {
            return Children.IsTrueForAnyChild(element, (child, e) =>
            {
                if (child == element)
                    return true;

                return child is IVisualContainer container &&
                       container.Contains(element);
            });
        }

        public override IVisualElement DeepCopy()
        {
            var newObject = (BasePanel) base.DeepCopy();
            Children.RunOnEachChild(newObject, (p, child) =>
            {
                var stepChild = child.DeepCopy();
                p.AddChild(stepChild);
            });

            return newObject;
        }

        public virtual void AcceptChanges()
        {
            Children.RunOnEachChild(child =>
            {
                if (child is IChangeTracking changeTracking)
                    changeTracking.AcceptChanges();
            });

            AcceptChanges(ChangeType.Measure);
            AcceptChanges(ChangeType.Arrange);
        }

        public virtual Boolean IsChanged
        {
            get => Children.IsTrueForAnyChild(child => child is IChangeTracking {IsChanged: true});
        }

        public override void InvalidateMeasure()
        {
            base.InvalidateMeasure();

            Children.RunOnEachChild(child => child.InvalidateMeasure());
        }

        public override void InvalidateArrange()
        {
            base.InvalidateArrange();

            Children.RunOnEachChild(child => child.InvalidateArrange());
        }

        public override Boolean IsRequiresMeasure
        {
            get => base.IsRequiresMeasure || 
                   Children.IsTrueForAnyChild(child => child.IsRequiresMeasure);
            protected set => base.IsRequiresMeasure = value;
        }

        public override Boolean IsRequiresArrange
        {
            get => base.IsRequiresArrange || 
                   Children.IsTrueForAnyChild(child => child.IsRequiresArrange);
            protected set => base.IsRequiresArrange = value;
        }

        public override void SetBoundValue(Object? value)
        {
            base.SetBoundValue(value);

            Children.RunOnEachChild(value, (v, child) =>
            {
                if (!(child is IBindableElement bindable))
                    return;

                bindable.SetDataContext(v);
            });
        }

        public override void Dispose()
        {
            base.Dispose();
            Children.Dispose();
        }

        public override async Task SetBoundValueAsync(Object? value)
        {
            await Children.RunOnEachChildAsync(value, async (v, child) =>
            {
                if (!(child is IBindableElement bindable))
                    return;

                await bindable.SetDataContextAsync(v);
            });
        }


        //private readonly List<IVisualElement> _children;
        
        //private readonly Object _lockChildren;
    }
}