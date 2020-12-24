using System;
using System.Threading.Tasks;
using Das.Views.Collections;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.DataBinding;
using Das.Views.Rendering;

namespace Das.Views.Panels
{
    /// <summary>
    ///     Renders a collection of visuals in order - with either Vertical or Horizontal orientation
    /// </summary>
    public abstract class BaseSequentialPanel : BasePanel,
                                                             ISequentialPanel
    {
        protected BaseSequentialPanel(IVisualBootstrapper visualBootstrapper)
            : this(visualBootstrapper, new VisualCollection())
        {
        }

        //protected BaseSequentialPanel(IDataBinding<T>? binding,
        //                              IVisualBootstrapper visualBootstrapper)
        //    : this(binding, visualBootstrapper, new VisualCollection())
        //{
        //}

        //protected BaseSequentialPanel(IDataBinding<T>? binding,
        //                              IVisualBootstrapper visualBootstrapper,
        //                              IVisualCollection children,
        //                              ISequentialRenderer? renderer = null)
        //    : base(binding, visualBootstrapper, children)
        //{
        //    _renderer = EnsureRenderer(renderer);

        //    VerticalAlignment = VerticalAlignments.Default;
        //    HorizontalAlignment = HorizontalAlignments.Default;
        //}

        protected BaseSequentialPanel(IVisualBootstrapper templateResolver,
                                      IVisualCollection children,
                                      ISequentialRenderer? renderer = null)
            //: this(null, templateResolver, children, renderer)
        : base(templateResolver, children)
        {
            _renderer = EnsureRenderer(renderer);

            VerticalAlignment = VerticalAlignments.Default;
            HorizontalAlignment = HorizontalAlignments.Default;
        }

        IVisualCollection ISequentialPanel.Children => _children;

        //public IVisualCollection Children => _children;

        public Orientations Orientation
        {
            get => OrientationProperty.GetValue(this);
            set => OrientationProperty.SetValue(this, value);
        }

        public override ValueSize Measure(IRenderSize availableSpace,
                                          IMeasureContext measureContext)
        {
            return _renderer.Measure(this, //GetChildrenToRender(), 
                Orientation,
                availableSpace, measureContext);
        }

        public override void Arrange(IRenderSize availableSpace,
                                     IRenderContext renderContext)
        {
            _renderer.Arrange(Orientation, availableSpace.ToFullRectangle(), renderContext);
        }

        private ISequentialRenderer EnsureRenderer(ISequentialRenderer? input)
        {
            return input ?? new SequentialRenderer(_children);
        }

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
        
        protected override void OnDistributeDataContextToChildren(Object? newValue)
        {
            _children.RunOnEachChild(newValue, (nv, child) =>
            {
                if (child is IBindableElement bindable)
                {
                    //if (bindable.TryGetDataContextBinding(out var dcBinding))
                    //{
                    //    dcBinding.UpdateDataContext(nv);
                    //}
                    //else
                        bindable.DataContext = nv;
                }
            });
        }

        public static readonly DependencyProperty<ISequentialPanel, Orientations> OrientationProperty =
            DependencyProperty<ISequentialPanel, Orientations>.Register(nameof(Orientation),
                Orientations.Vertical, OnOrientationChanged);

        //protected abstract IList<IVisualElement> GetChildrenToRender();

        private readonly ISequentialRenderer _renderer;
    }
}