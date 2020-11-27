using System;
using System.Threading.Tasks;
using Das.Views.Controls;
using Das.Views.Core.Drawing;
using Das.Views.Core.Enums;
using Das.Views.Panels;

namespace Das.Views.Rendering
{
    public interface IVisualElement : IVisualRenderer,
                                      IDeepCopyable<IVisualElement>,
                                      IDisposable,
                                      IEquatable<IVisualElement>

    {
        /// <summary>
        ///     For style lookups. Allows items in repeaters to use the same style assignment
        /// </summary>
        Int32 Id { get; }

        Boolean IsClipsContent { get; set; }

        void OnParentChanging(IContainerVisual? newParent);

        event Action<IVisualElement>? Disposed;

        IControlTemplate? Template { get; }

        void AcceptChanges(ChangeType changeType);

        void RaisePropertyChanged(String propertyName);

        Double? Width { get; set; }

        Double? Height { get; set; }

        HorizontalAlignments HorizontalAlignment { get; set; }

        VerticalAlignments VerticalAlignment { get; set; }

        IBrush? Background { get; set; }

    }
}