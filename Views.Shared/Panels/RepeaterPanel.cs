using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.DataBinding;
using Das.Views.Rendering;

namespace Das.Views.Panels
{
    public class RepeaterPanel<T> : ContentPanel<IEnumerable<T>>, IRepeaterPanel<T>
    {
        // ReSharper disable once UnusedMember.Global
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

        public override void SetBoundValue(Object value)
        {
            if (value is IEnumerable<T> goodOne)
                SetBoundValue(goodOne);
            else throw new NotImplementedException();
        }

        private IVisualElement? ClearBeforeSet()
        {
            var content = Content;
            if (content == null)
                return default;

            for (var c = 0; c < _controls.Count; c++)
                _controls[c].Dispose();

            _controls.Clear(); //todo: more efficient

            return content;
        }

        public override void SetBoundValue(IEnumerable<T> value)
        {
            if (!(ClearBeforeSet() is {} content))
                return;

            //var content = Content;
            //if (content == null)
            //    return;

            //for (var c = 0; c < _controls.Count; c++)
            //    _controls[c].Dispose();

            //_controls.Clear(); //todo: more efficient

            foreach (var vm in value)
            {
                var ctrl = content.DeepCopy();
                if (ctrl is IBindableElement bindable) bindable.SetBoundValue(vm!);

                _controls.Add(ctrl);
            }
        }

        public override async Task SetBoundValueAsync(IEnumerable<T> value)
        {
            if (!(ClearBeforeSet() is {} content))
                return;

            foreach (var vm in value)
            {
                var ctrl = content.DeepCopy();
                if (ctrl is IBindableElement bindable) 
                    await bindable.SetBoundValueAsync(vm!);

                _controls.Add(ctrl);
            }
        }

        private static ISequentialRenderer EnsureRenderer(ISequentialRenderer input)
        {
            return input ?? new SequentialRenderer();
        }

        public override ISize Measure(ISize availableSpace, IMeasureContext measureContext)
        {
            return _renderer.Measure(this, _controls, Orientation, availableSpace, measureContext);
        }

        public override void Arrange(ISize availableSpace, IRenderContext renderContext)
        {
            _renderer.Arrange(Orientation, availableSpace, renderContext);
        }

        public override void Dispose()
        {
            for (var c = 0; c < _controls.Count; c++)
                _controls[c].Dispose();

            _controls.Clear();
        }

        public IList<IVisualElement> Children => _controls;

        public override void SetDataContext(Object dataContext)
        {
            DataContext = dataContext;

            var val = Binding.GetValue(dataContext);
            SetBoundValue(val);
        }

        public override async Task SetDataContextAsync(Object dataContext)
        {
            DataContext = dataContext;

            var val = await Binding.GetValueAsync(dataContext);
            await SetBoundValueAsync(val);
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

        private readonly List<IVisualElement> _controls;
        private readonly ISequentialRenderer _renderer;
    }
}