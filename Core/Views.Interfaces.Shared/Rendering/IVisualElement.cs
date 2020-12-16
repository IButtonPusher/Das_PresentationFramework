using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Das.Views.Controls;
using Das.Views.Core.Drawing;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.Panels;
using Das.Views.Rendering;

namespace Das.Views
{
    public interface IVisualElement : IVisualRenderer,
                                      IDisposable,
                                      INotifyPropertyChanged,
                                      IEquatable<IVisualElement>

    {
        /// <summary>
        ///     For style lookups. Allows items in repeaters to use the same style assignment
        /// </summary>
        Int32 Id { get; }

        Boolean IsClipsContent { get; set; }

        void OnParentChanging(IVisualElement? newParent);

        event Action<IVisualElement>? Disposed;

        IControlTemplate? Template { get; }

        void RaisePropertyChanged(String propertyName,
                                  Object? value);

        Double? Width { get; set; }

        Double? Height { get; set; }

        HorizontalAlignments HorizontalAlignment { get; set; }

        VerticalAlignments VerticalAlignment { get; set; }

        IBrush? Background { get; set; }
        
        Thickness? Margin { get; set; }

    }
}