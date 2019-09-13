using System;

namespace Das.Views.Rendering
{
    public interface IVisualElement : IVisualRenderer, IDeepCopyable<IVisualElement>
    {
        /// <summary>
        /// For style lookups. Allows items in repeaters to use the same style assignment
        /// </summary>
        Int32 Id { get; }
    }
}