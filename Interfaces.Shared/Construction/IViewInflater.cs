using System;
using System.Collections;
using System.Collections.Generic;
using Das.Views.Rendering;

namespace Das.Views.Construction
{
    public interface IViewInflater
    {
        /// <summary>
        /// Loads a visual element of the specific type from the provided xml
        /// </summary>
        TVisualElement InflateXml<TVisualElement>(String xml)
            where TVisualElement : IVisualElement;
        
        
        /// <summary>
        /// Loads a visual element of the specific type from the provided xml.
        /// Performs type resolution from the xml namespaces and the provided hints
        /// </summary>
        /// <typeparam name="TVisualElement">The type of the visual to be inflated</typeparam>
        /// <param name="xml">the markup text</param>
        /// <param name="namespaceHints">A dictionary where keys are namespaces and values are
        /// assemblies in which they are located</param>
        /// <returns></returns>
        TVisualElement InflateXml<TVisualElement>(String xml,
                                                  IDictionary<String, String> namespaceHints)
            where TVisualElement : IVisualElement;

        TVisualElement InflateResourceXml<TVisualElement>(String resourceName)
            where TVisualElement : IVisualElement;

        //IVisualElement InflateResourceXml(String resourceName);

        IVisualElement InflateXml(String xml);
        
        IVisualElement InflateXml(String xml,
                                  IDictionary<String, String> namespaceHints);
    }
}