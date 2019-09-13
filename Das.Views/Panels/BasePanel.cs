using System;
using System.Collections.Generic;
using Das.Serializer;
using Das.Views.Core.Geometry;
using Das.Views.DataBinding;
using Das.Views.Rendering;

namespace Das.Views.Panels
{
    public abstract class BasePanel<T> : BindableElement<T>, IVisualContainer
    {
        protected BasePanel(IDataBinding<T> binding) : base(binding)
        {
            ElementsRendered = new Dictionary<VisualElement, Rectangle>();
            _children = new List<IVisualElement>();
        }

        // ReSharper disable once UnusedMember.Global
        protected BasePanel() : this(null)
        {
        }

        public IList<IVisualElement> Children => _children;


        private readonly List<IVisualElement> _children;

        public void AddChild(IVisualElement element)
        {
            _children.Add(element);
        }

        public void AddChildren(params IVisualElement[] elements)
        {
            foreach (var element in elements)
                AddChild(element);
        }

        public virtual void OnChildDeserialized(IVisualElement element, INode node)
        {
        }

        public virtual bool Contains(IVisualElement element)
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

        protected readonly Dictionary<VisualElement, Rectangle> ElementsRendered;
    }
}