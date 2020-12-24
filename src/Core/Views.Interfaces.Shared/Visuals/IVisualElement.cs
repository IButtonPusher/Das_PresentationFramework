using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Das.Views.Core;
using Das.Views.Core.Drawing;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;
using Das.Views.Templates;

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

        Boolean IsClipsContent { get; set; }

        void OnParentChanging(IVisualElement? newParent);

        event Action<IVisualElement>? Disposed;

        void RaisePropertyChanged(String propertyName,
                                  Object? value);

        Double? Width { get; set; }

        Double? Height { get; set; }

        HorizontalAlignments HorizontalAlignment { get; set; }

        VerticalAlignments VerticalAlignment { get; set; }

        IBrush? Background { get; set; }
        
        Thickness? Margin { get; set; }
        
        ISet<String> StyleClasses { get; }
        
        Double Opacity { get; }
        
        Visibility Visibility { get; }
        
        Boolean IsEnabled { get; set; }

        /// <summary>
        /// Tags in markup that are meant to identify this visual.
        /// </summary>
        /// <example>label = Label, input can be TextBox, Button, CheckBox, etc </example>
        Boolean IsMarkupNameAlias(String markupTag);
        
        Int32 ZIndex { get; }

    }
}