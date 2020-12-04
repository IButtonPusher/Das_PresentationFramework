using System;
using System.Collections.Generic;
using System.ComponentModel;

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
            : this(binding, visualBootstrapper, new VisualCollection())
        {
            

            //_lockChildren = new Object();
        }

        protected BasePanel(IDataBinding<T>? binding,
                            IVisualBootstrapper visualBootstrapper,
                            IVisualCollection children)
            : base(binding, visualBootstrapper)
        {
            Children = children;

            //_lockChildren = new Object();
        }

        // ReSharper disable once UnusedMember.Global
        protected BasePanel(IVisualBootstrapper templateResolver) 
            : this(null, templateResolver)
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

        //public Boolean IsTrueForAnyChild<TInput>(TInput input,
        //                                         Func<IVisualElement, TInput, Boolean> action)
        //{
        //    lock (_lockChildren)
        //    {
        //        foreach (var visual in _children)
        //        {
        //            if (action(visual, input))
        //                return true;
        //        }

        //        return false;
        //    }
        //}

        //public Boolean IsTrueForAnyChild(Func<IVisualElement, Boolean> action)
        //{
        //    lock (_lockChildren)
        //    {
        //        foreach (var visual in _children)
        //        {
        //            if (action(visual))
        //                return true;
        //        }

        //        return false;
        //    }
        //}

        //public IEnumerable<T1> GetFromEachChild<T1>(Func<IVisualElement, T1> action)
        //{
        //    lock (_lockChildren)
        //    {
        //        foreach (var visual in _children)
        //        {
        //            yield return action(visual);
        //        }
        //    }
        //}

        //public void RunOnEachChild(Action<IVisualElement> action)
        //{
        //    lock (_lockChildren)
        //    {
        //        foreach (var visual in _children)
        //        {
        //            action(visual);
        //        }
        //    }
        //}

        //public void RunOnEachChild<TInput>(TInput input, 
        //                                   Action<TInput, IVisualElement> action)
        //{
        //    lock (_lockChildren)
        //    {
        //        foreach (var visual in _children)
        //        {
        //            action(input, visual);
        //        }
        //    }
        //}

        //public async Task RunOnEachChildAsync<TInput>(TInput input, 
        //                                        Func<TInput, IVisualElement, Task> action)
        //{
        //    var running = new List<Task>();

        //    lock (_lockChildren)
        //    {
        //        foreach (var visual in _children)
        //        {
        //            running.Add(action(input, visual));
        //        }
        //    }

        //    await Task.WhenAll(running);
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

        public virtual void OnChildDeserialized(IVisualElement element, 
                                                INode node)
        {
        }

        public virtual Boolean Contains(IVisualElement element)
        {
            return Children.Contains(element);

            //foreach (var child in Children)
            //{
            //    if (child == element)
            //        return true;

            //    if (child is IVisualContainer container &&
            //        container.Contains(element))
            //        return true;
            //}

            //return false;
        }

        public override void Dispose()
        {
            base.Dispose();
            Children.Dispose();
        }

        public override IVisualElement DeepCopy()
        {
            var newObject = (BasePanel<T>) base.DeepCopy();

            Children.RunOnEachChild(newObject, (p, child) =>
            {
                var stepChild = child.DeepCopy();
                p.AddChild(stepChild);
            });

            //foreach (var child in Children)
            //{
            //    var stepChild = child.DeepCopy();
            //    newObject.AddChild(stepChild);
            //}

            return newObject;
        }

        public virtual void AcceptChanges()
        {
            Children.RunOnEachChild(child =>
            {
                if (child is IChangeTracking changeTracking)
                    changeTracking.AcceptChanges();
            });

            //foreach (var changeChild in Children.OfType<IChangeTracking>())
            //    changeChild.AcceptChanges();

            AcceptChanges(ChangeType.Measure);
            AcceptChanges(ChangeType.Arrange);
        }

        public override void AcceptChanges(ChangeType changeType)
        {
            base.AcceptChanges(changeType);
            Children.AcceptChanges(changeType);
        }

        public virtual Boolean IsChanged
        {
            get => Children.IsTrueForAnyChild(child => child is IChangeTracking {IsChanged: true});
        }

        public override void InvalidateMeasure()
        {
            base.InvalidateMeasure();
            Children.InvalidateMeasure();
        }

        public override void InvalidateArrange()
        {
            base.InvalidateArrange();
            Children.InvalidateArrange();

            //foreach (var child in Children)
            //{
            //    child.InvalidateArrange();
            //}
        }

        public override Boolean IsRequiresMeasure
        {
            get => base.IsRequiresMeasure || Children.IsRequiresMeasure;
            protected set => base.IsRequiresMeasure = value;
        }

        public override Boolean IsRequiresArrange
        {
            get => base.IsRequiresArrange || Children.IsRequiresArrange;
            protected set => base.IsRequiresArrange = value;
        }

        public override void SetBoundValue(T value)
        {
            base.SetBoundValue(value);

            Children.RunOnEachChild(value, (v, child) =>
            {
                if (!(child is IBindableElement bindable))
                    return;

                bindable.SetDataContext(v);
            });

            //foreach (var child in Children)
            //{
            //    if (!(child is IBindableElement bindable))
            //        continue;

            //    bindable.SetDataContext(value);
            //}
        }

        public override async Task SetBoundValueAsync(T value)
        {
            await Children.RunOnEachChildAsync(value, async (v, child) =>
            {
                if (!(child is IBindableElement bindable))
                    return;

                await bindable.SetDataContextAsync(v);
            });

            //foreach (var child in Children)
            //{
            //    if (!(child is IBindableElement bindable))
            //        continue;

            //    await bindable.SetDataContextAsync(value);
            //}
        }

        //private readonly List<IVisualElement> _children = new List<IVisualElement>();
        //private readonly Object _lockChildren = new Object();
    }
}