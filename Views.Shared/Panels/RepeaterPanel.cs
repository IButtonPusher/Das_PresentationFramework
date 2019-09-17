using System;
using System.Collections.Generic;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.DataBinding;
using Das.Views.Rendering;

namespace Das.Views.Panels
{
    public class RepeaterPanel<T> : ContentPanel<IEnumerable<T>>, IRepeaterPanel<T>
    {
        private readonly List<IVisualElement> _controls;
        private readonly ISequentialRenderer _renderer;

        public RepeaterPanel() : this(new SequentialRenderer())
        {
        }

        public RepeaterPanel(ISequentialRenderer renderer)
        {
            _controls = new List<IVisualElement>();
            _renderer = EnsureRenderer(renderer);
        }

        public RepeaterPanel(IDataBinding<IEnumerable<T>> binding,
            ISequentialRenderer renderer = null) : base(binding)
        {
            _controls = new List<IVisualElement>();
            _renderer = EnsureRenderer(renderer);
        }

        private static ISequentialRenderer EnsureRenderer(ISequentialRenderer input)
            => input ?? new SequentialRenderer();

        public override ISize Measure(ISize availableSpace, IMeasureContext measureContext)
            => _renderer.Measure(this, _controls, Orientation, availableSpace, measureContext);

        public override void Arrange(ISize availableSpace, IRenderContext renderContext)
            => _renderer.Arrange(Orientation, availableSpace, renderContext);

        public IList<IVisualElement> Children => _controls;

        public override void SetDataContext(Object dataContext)
        {
            DataContext = dataContext;

            var val = Binding.GetValue(dataContext);
            SetBoundValue(val);
        }

        public override void SetBoundValue(IEnumerable<T> value)
        {
            var content = Content;
            if (content == null)
                return;

            _controls.Clear(); //todo: more efficient

            foreach (var vm in value)
            {
                var ctrl = content.DeepCopy();
                if (ctrl is IBindableElement bindable)
                {
                    bindable.SetBoundValue(vm);
                }

                _controls.Add(ctrl);
            }
        }

        public override void SetBoundValue(Object value)
        {
            if (value is IEnumerable<T> goodOne)
                SetBoundValue(goodOne);
            else throw new NotImplementedException();
        }

        public override Boolean Contains(IVisualElement element)
        {
            foreach (var c in _controls)
            {
                if (c == element)
                    return true;

                if (c is IVisualContainer container &&
                    container.Contains(element))
                    return true;
            }

            return false;
        }

        public Orientations Orientation { get; set; }

        public override IVisualElement Content { get; set; }

        IBindableElement<IEnumerable<T>> IRepeaterPanel<T>.Content =>
            (IBindableElement<IEnumerable<T>>) Content;
    }
}