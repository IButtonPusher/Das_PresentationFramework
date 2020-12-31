using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Das.Views.Controls;
using Das.Views.Core;
using Das.Views.Core.Drawing;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;
using Das.Views.Styles.Application;
using Das.Views.Styles.Declarations;
using Das.Views.Templates;
using Das.Views.Transforms;

namespace Das.Views
{
    public interface IVisualElement : IVisualRenderer,
                                      IDisposable,
                                      INotifyPropertyChanged,
                                      ITemplatableVisual,
                                      IEquatable<IVisualElement>

    {
        /// <summary>
        ///     For style lookups. Allows items in repeaters to use the same style assignment
        /// </summary>
        Int32 Id { get; }
        
        /// <summary>
        /// Class name for style resolution
        /// </summary>
        String? Class { get; }

        IAppliedStyle? Style { get; }

        Boolean IsClipsContent { get; set; }

        void OnParentChanging(IVisualElement? newParent);

        event Action<IVisualElement>? Disposed;

        void RaisePropertyChanged(String propertyName,
                                  Object? value);

        QuantifiedDouble? Width { get; set; }

        QuantifiedDouble? Height { get; set; }

        QuantifiedDouble? Left { get; set; }

        QuantifiedDouble? Right { get; set; }

        QuantifiedDouble? Top { get; set; }

        QuantifiedDouble? Bottom { get; set; }

        HorizontalAlignments HorizontalAlignment { get; set; }

        VerticalAlignments VerticalAlignment { get; set; }

        IBrush? Background { get; set; }
        
        QuantifiedThickness Margin { get; set; }

        Double Opacity { get; }
        
        Visibility Visibility { get; set; }
        
        QuantifiedThickness BorderRadius { get; set; }
        
        Boolean IsEnabled { get; set; }

        ITransform Transform { get; set; }

        Boolean TryGetDependencyProperty(DeclarationProperty declarationProperty,
                                         out IDependencyProperty dependencyProperty);

        ILabel? BeforeLabel { get; set; }
        
        ILabel? AfterLabel { get; set; }

        /// <summary>
        /// Tags in markup that are meant to identify this visual.
        /// </summary>
        /// <example>label = Label, input can be TextBox, Button, CheckBox, etc </example>
        Boolean IsMarkupNameAlias(String markupTag);
        
        Int32 ZIndex { get; }
    }
}