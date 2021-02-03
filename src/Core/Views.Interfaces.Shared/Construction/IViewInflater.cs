using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.Rendering;
// ReSharper disable UnusedMemberInSuper.Global
// ReSharper disable UnusedMember.Global

namespace Das.Views.Construction
{
    public interface IViewInflater
    {
        Task<IVisualElement> GetVisualAsync(IMarkupNode node,
                                            Type dataContextType,
                                            IVisualLineage visualLineage,
                                            ApplyVisualStyles applyStyles);

        Task<TVisualElement> InflateResourceXmlAsync<TVisualElement>(String resourceName)
            where TVisualElement : IVisualElement;

        Task<IVisualElement> InflateResourceXmlAsync(String resourceName);

        /// <summary>
        ///     Loads a visual element of the specific type from the provided xml
        /// </summary>
        Task<TVisualElement> InflateXmlAsync<TVisualElement>(String xml)
            where TVisualElement : IVisualElement;


        /// <summary>
        ///     Loads a visual element of the specific type from the provided xml.
        ///     Performs type resolution from the xml namespaces and the provided hints
        /// </summary>
        /// <typeparam name="TVisualElement">The type of the visual to be inflated</typeparam>
        /// <param name="xml">the markup text</param>
        /// <param name="namespaceHints">
        ///     A dictionary where keys are namespaces and values are
        ///     assemblies in which they are located
        /// </param>
        Task<TVisualElement> InflateXmlAsync<TVisualElement>(String xml,
                                                             IDictionary<String, String> namespaceHints)
            where TVisualElement : IVisualElement;

        Task<IVisualElement> InflateXmlAsync(String xml);

        Task<IVisualElement> InflateXmlAsync(String xml,
                                             IDictionary<String, String> namespaceHints);
    }
}
