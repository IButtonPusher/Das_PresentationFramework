using System;
using System.Threading.Tasks;
using Das.Views.Controls;
using Das.Views.Core;
using Das.Views.Core.Drawing;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.Styles.Declarations;
#if !NET40
using TaskEx = System.Threading.Tasks.Task;

#endif
namespace Das.Views
{
    public abstract partial class VisualElement
    {
        public virtual Boolean TryGetDependencyProperty(DeclarationProperty declarationProperty,
                                                        out IDependencyProperty property)
        {
            IDependencyProperty? dependencyProperty = default;


            switch (declarationProperty)
            {
                case DeclarationProperty.BackgroundColor:
                    dependencyProperty = BackgroundProperty;
                    break;
                
                case DeclarationProperty.BorderRadius:
                case DeclarationProperty.BorderRadiusBottom:
                case DeclarationProperty.BorderRadiusLeft:
                case DeclarationProperty.BorderRadiusRight:
                case DeclarationProperty.BorderRadiusTop:
                    dependencyProperty = BorderRadiusProperty;
                    break;

                case DeclarationProperty.Height:
                    dependencyProperty = HeightProperty;
                    break;
                
                case DeclarationProperty.Margin:
                case DeclarationProperty.MarginBottom:
                case DeclarationProperty.MarginLeft:
                case DeclarationProperty.MarginRight:
                case DeclarationProperty.MarginTop:
                    dependencyProperty = MarginProperty;
                    break;
                
                case DeclarationProperty.Width:
                    dependencyProperty = WidthProperty;
                    break;

                case DeclarationProperty.ZIndex:
                    dependencyProperty = ZIndexProperty;
                    break;
                
                case DeclarationProperty.VerticalAlign:
                    dependencyProperty = VerticalAlignmentProperty;
                    break;
                
                case DeclarationProperty.Appearance:
                    dependencyProperty = VisibilityProperty;
                    break;
            }

            property = dependencyProperty!;
            
            return dependencyProperty != null;
        }
        
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
        

        public QuantifiedDouble? Width
        {
            get => WidthProperty.GetValue(this);
            set => WidthProperty.SetValue(this, value);
        }

        public QuantifiedDouble? Height
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

        public QuantifiedThickness Margin
        {
            get => MarginProperty.GetValue(this);
            set => MarginProperty.SetValue(this, value);
        }

        //public ISet<String> StyleClasses => _styleClasses;

        
        public virtual Int32 Id { get; private set; }

        //public String? Class { get; set; }

        public static readonly DependencyProperty<IVisualElement, String?> ClassProperty =
            DependencyProperty<IVisualElement, String?>.Register(
                nameof(Class), default);

        public String? Class
        {
            get => ClassProperty.GetValue(this);
            set => ClassProperty.SetValue(this, value);
        }
        

        public virtual Boolean IsClipsContent { get; set; }

        public event Action<IVisualElement>? Disposed;

        public static readonly DependencyProperty<IVisualElement, IVisualTemplate?> TemplateProperty =
            DependencyProperty<IVisualElement, IVisualTemplate?>.Register(
                nameof(Template),
                default);

        public IVisualTemplate? Template
        {
            get => TemplateProperty.GetValue(this);
            set => TemplateProperty.SetValue(this, value);
        }

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

        public static readonly DependencyProperty<IVisualElement, ILabel?> BeforeLabelProperty =
            DependencyProperty<IVisualElement, ILabel?>.Register(
                nameof(BeforeLabel),
                default);

        public ILabel? BeforeLabel
        {
            get => BeforeLabelProperty.GetValue(this);
            set => BeforeLabelProperty.SetValue(this, value);
        }

        public static readonly DependencyProperty<IVisualElement, ILabel?> AfterLabelProperty =
            DependencyProperty<IVisualElement, ILabel?>.Register(
                nameof(AfterLabel),
                default);

        public ILabel? AfterLabel
        {
            get => AfterLabelProperty.GetValue(this);
            set => AfterLabelProperty.SetValue(this, value);
        }
        
        
        public static readonly DependencyProperty<IVisualElement, QuantifiedDouble?> WidthProperty =
            DependencyProperty<IVisualElement, QuantifiedDouble?>.Register(nameof(Width), null);

        public static readonly DependencyProperty<IVisualElement, QuantifiedDouble?> HeightProperty =
            DependencyProperty<IVisualElement, QuantifiedDouble?>.Register(nameof(Height), null);


        public static readonly DependencyProperty<IVisualElement, VerticalAlignments> VerticalAlignmentProperty =
            DependencyProperty<IVisualElement, VerticalAlignments>.Register(nameof(VerticalAlignment),
                VerticalAlignments.Default);

        public static readonly DependencyProperty<IVisualElement, HorizontalAlignments> HorizontalAlignmentProperty =
            DependencyProperty<IVisualElement, HorizontalAlignments>.Register(nameof(HorizontalAlignment),
                HorizontalAlignments.Default);

        public static readonly DependencyProperty<IVisualElement, IBrush?> BackgroundProperty =
            DependencyProperty<IVisualElement, IBrush?>.Register(nameof(Background), default);

        public static readonly DependencyProperty<IVisualElement, QuantifiedThickness> MarginProperty =
            DependencyProperty<IVisualElement, QuantifiedThickness>.Register(nameof(Margin), 
                QuantifiedThickness.Empty);

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

        public static readonly DependencyProperty<IVisualElement, QuantifiedThickness> BorderRadiusProperty =
            DependencyProperty<IVisualElement, QuantifiedThickness>.Register(
                nameof(BorderRadius), QuantifiedThickness.Empty);

        public QuantifiedThickness BorderRadius
        {
            get => BorderRadiusProperty.GetValue(this);
            set => BorderRadiusProperty.SetValue(this, value);
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

        public static readonly DependencyProperty<IVisualElement, QuantifiedDouble?> LeftProperty =
            DependencyProperty<IVisualElement, QuantifiedDouble?>.Register(
                nameof(Left),
                default);

        public QuantifiedDouble? Left
        {
            get => LeftProperty.GetValue(this);
            set => LeftProperty.SetValue(this, value);
        }

        public static readonly DependencyProperty<IVisualElement, QuantifiedDouble?> TopProperty =
            DependencyProperty<IVisualElement, QuantifiedDouble?>.Register(
                nameof(Top),
                default);

        public QuantifiedDouble? Top
        {
            get => TopProperty.GetValue(this);
            set => TopProperty.SetValue(this, value);
        }


        public static readonly DependencyProperty<IVisualElement, QuantifiedDouble?> RightProperty =
            DependencyProperty<IVisualElement, QuantifiedDouble?>.Register(
                nameof(Right),
                default);

        public QuantifiedDouble? Right
        {
            get => RightProperty.GetValue(this);
            set => RightProperty.SetValue(this, value);
        }

        public static readonly DependencyProperty<IVisualElement, QuantifiedDouble?> BottomProperty =
            DependencyProperty<IVisualElement, QuantifiedDouble?>.Register(
                nameof(Bottom),
                default);

        public QuantifiedDouble? Bottom
        {
            get => BottomProperty.GetValue(this);
            set => BottomProperty.SetValue(this, value);
        }

        private Boolean _isRequiresArrange;
        private Boolean _isRequiresMeasure;
        //private readonly HashSet<String> _styleClasses;
    }
}
