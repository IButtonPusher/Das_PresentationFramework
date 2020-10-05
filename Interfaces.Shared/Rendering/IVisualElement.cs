using System;
using System.Threading.Tasks;

namespace Das.Views.Rendering
{
    public interface IVisualElement : IVisualRenderer, IDeepCopyable<IVisualElement>, IDisposable
    {
        /// <summary>
        ///     For style lookups. Allows items in repeaters to use the same style assignment
        /// </summary>
        Int32 Id { get; }
    }
}