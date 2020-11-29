using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.DataBinding;
using Das.Views.Rendering;

namespace Das.Views.Panels
{
    /// <summary>
    /// Renders a collection of visuals in order - with either Vertical or Horizontal orientation
    /// </summary>
    public abstract class BaseSequentialPanel<T> : BasePanel<T>, 
                                                   ISequentialPanel
    {
        protected BaseSequentialPanel(IDataBinding<T>? binding,
                                      IVisualBootstrapper visualBootstrapper,
                                      ISequentialRenderer? renderer = null)
            : base(binding, visualBootstrapper)
        {
            _renderer = EnsureRenderer(renderer);

            VerticalAlignment = VerticalAlignments.Default;
            HorizontalAlignment = HorizontalAlignments.Default;
        }

        protected BaseSequentialPanel(IVisualBootstrapper templateResolver, 
                                      ISequentialRenderer? renderer = null)
            : this(null, templateResolver, renderer)
        {
        }

        public static readonly DependencyProperty<ISequentialPanel, Orientations> OrientationProperty =
            DependencyProperty<ISequentialPanel, Orientations>.Register(nameof(Orientation),
                Orientations.Vertical, OnOrientationChanged);

        private static void OnOrientationChanged(ISequentialPanel panel, 
                                                 Orientations oldValue, 
                                                 Orientations newValue)
        {
            switch (newValue)
            {
                case Orientations.Horizontal:
                    panel.HorizontalAlignment = HorizontalAlignments.Stretch;
                    break;

                case Orientations.Vertical:
                    panel.VerticalAlignment = VerticalAlignments.Stretch;
                    break;
            }
        }

        public Orientations Orientation
        {
            get => OrientationProperty.GetValue(this);
            set => OrientationProperty.SetValue(this, value);
        }

        public override ValueSize Measure(IRenderSize availableSpace, 
                                          IMeasureContext measureContext)
        {
            return _renderer.Measure(this, GetChildrenToRender(), Orientation,
                availableSpace, measureContext);
        }

        public override void Arrange(IRenderSize availableSpace, 
                                     IRenderContext renderContext)
        {
            _renderer.Arrange(Orientation, availableSpace.ToFullRectangle(), renderContext);
        }

        private static ISequentialRenderer EnsureRenderer(ISequentialRenderer? input)
        {
            return input ?? new SequentialRenderer();
        }

        protected abstract IList<IVisualElement> GetChildrenToRender();

        private readonly ISequentialRenderer _renderer;
    }
}