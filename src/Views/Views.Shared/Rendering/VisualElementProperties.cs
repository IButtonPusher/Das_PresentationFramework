using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.Controls;
using Das.Views.Core;
using Das.Views.Core.Drawing;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
#if !NET40
using TaskEx = System.Threading.Tasks.Task;

#endif
namespace Das.Views
{
    public abstract partial class VisualElement
    {
        public virtual Boolean IsRequiresMeasure
        {
            get => _isRequiresMeasure;
            protected set => SetValue(ref _isRequiresMeasure, value);
        }

        public virtual Boolean IsRequiresArrange
        {
            get => _isRequiresArrange;
            protected set => SetValue(ref _isRequiresArrange, value);
        }
        

        public Double? Width
        {
            get => WidthProperty.GetValue(this);
            set => WidthProperty.SetValue(this, value);
        }

        public Double? Height
        {
            get => HeightProperty.GetValue(this);
            set => HeightProperty.SetValue(this, value);
        }

        public HorizontalAlignments HorizontalAlignment
        {
            get => HorizontalAlignmentProperty.GetValue(this);
            set => HorizontalAlignmentProperty.SetValue(this, value);
        }

        public VerticalAlignments VerticalAlignment
        {
            get => VerticalAlignmentProperty.GetValue(this);
            set => VerticalAlignmentProperty.SetValue(this, value);
        }

        public IBrush? Background
        {
            get => BackgroundProperty.GetValue(this);
            set => BackgroundProperty.SetValue(this, value);
        }

        public Thickness? Margin
        {
            get => MarginProperty.GetValue(this);
            set => MarginProperty.SetValue(this, value);
        }

        public ISet<String> StyleClasses => _styleClasses;

        
        public virtual Int32 Id { get; private set; }

        public virtual Boolean IsClipsContent { get; set; }

        public event Action<IVisualElement>? Disposed;

        public virtual IVisualTemplate? Template
        {
            get => _template;
            // ReSharper disable once UnusedMember.Global
            set => SetValue(ref _template, value);
        }
        
        
        // ReSharper disable once UnusedMember.Global
        //public virtual Boolean IsEnabled
        //{
        //    get => _isEnabled;
        //    set => SetValue(ref _isEnabled, value);
        //}

        public Double Opacity
        {
            get => OpacityProperty.GetValue(this);
            set => OpacityProperty.SetValue(this, value);
        }

        public static readonly DependencyProperty<IVisualElement, Boolean> IsEnabledProperty =
            DependencyProperty<IVisualElement, Boolean>.Register(
                nameof(IsEnabled),
                default);

        public Boolean IsEnabled
        {
            get => IsEnabledProperty.GetValue(this);
            set => IsEnabledProperty.SetValue(this, value);
        }
        
        
        public static readonly DependencyProperty<IVisualElement, Double?> WidthProperty =
            DependencyProperty<IVisualElement, Double?>.Register(nameof(Width), null);

        public static readonly DependencyProperty<IVisualElement, Double?> HeightProperty =
            DependencyProperty<IVisualElement, Double?>.Register(nameof(Height), null);


        public static readonly DependencyProperty<IVisualElement, VerticalAlignments> VerticalAlignmentProperty =
            DependencyProperty<IVisualElement, VerticalAlignments>.Register(nameof(VerticalAlignment),
                VerticalAlignments.Default);

        public static readonly DependencyProperty<IVisualElement, HorizontalAlignments> HorizontalAlignmentProperty =
            DependencyProperty<IVisualElement, HorizontalAlignments>.Register(nameof(HorizontalAlignment),
                HorizontalAlignments.Default);

        public static readonly DependencyProperty<IVisualElement, IBrush?> BackgroundProperty =
            DependencyProperty<IVisualElement, IBrush?>.Register(nameof(Background), default);

        public static readonly DependencyProperty<IVisualElement, Thickness?> MarginProperty =
            DependencyProperty<IVisualElement, Thickness?>.Register(nameof(Margin), null);

        public static readonly DependencyProperty<IVisualElement, Double> OpacityProperty =
            DependencyProperty<IVisualElement, Double>.Register(nameof(Opacity), 1.0);


        public static readonly DependencyProperty<IVisualElement, Visibility> VisibilityProperty =
            DependencyProperty<IVisualElement, Visibility>.Register(
                nameof(Visibility),
                default);

        public Visibility Visibility
        {
            get => VisibilityProperty.GetValue(this);
            set => VisibilityProperty.SetValue(this, value);
        }

        public static readonly DependencyProperty<IVisualElement, Int32> ZIndexProperty =
            DependencyProperty<IVisualElement, Int32>.Register(
                nameof(ZIndex),
                default);

        public Int32 ZIndex
        {
            get => ZIndexProperty.GetValue(this);
            set => ZIndexProperty.SetValue(this, value);
        }
        
        
        //private Boolean _isEnabled;
        private Boolean _isRequiresArrange;
        private Boolean _isRequiresMeasure;
        private IVisualTemplate? _template;
        private readonly HashSet<String> _styleClasses;
    }
}
