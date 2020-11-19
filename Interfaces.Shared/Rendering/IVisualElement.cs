using System;
using System.Threading.Tasks;
using Das.Views.Controls;
using Das.Views.Panels;

namespace Das.Views.Rendering
{
    public interface IVisualElement : IVisualRenderer,
                                      IDeepCopyable<IVisualElement>,
                                      IDisposable
    {
        /// <summary>
        ///     For style lookups. Allows items in repeaters to use the same style assignment
        /// </summary>
        Int32 Id { get; }

        void OnParentChanging(IContainerVisual? newParent);

        event Action<IVisualElement>? Disposed;

        IControlTemplate? Template { get; }

        void AcceptChanges(ChangeType changeType);
    }
}