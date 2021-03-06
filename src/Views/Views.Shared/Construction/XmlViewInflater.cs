﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Das.Views.Layout;

namespace Das.Views.Construction
{
    public partial class ViewInflater
    {
        public async Task<TVisualElement> InflateXmlAsync<TVisualElement>(String xml,
                                                              IDictionary<String, String> namespaceHints)
            where TVisualElement : IVisualElement
        {
            var visual = await InflateXmlAsync(xml, namespaceHints);
            return (TVisualElement) visual;
        }

        public async Task<TVisualElement> InflateResourceXmlAsync<TVisualElement>(String resourceName)
            where TVisualElement : IVisualElement
        {
            var visual = await InflateResourceXmlAsync(resourceName).ConfigureAwait(false);
            return (TVisualElement) visual;
        }

        public Task<IVisualElement> InflateXmlAsync(String xml)
        {
            return InflateXmlImpl(xml, VisualTypeResolver.DefaultNamespaceSeed);//  _defaultNamespaceSeed);
        }

        public Task<IVisualElement> InflateXmlAsync(String xml,
                                               IDictionary<String, String> namespaceHints)
        {
            return InflateXmlImpl(xml, namespaceHints);
        }
        
        public async Task<TVisualElement> InflateXmlAsync<TVisualElement>(String xml)
            where TVisualElement : IVisualElement
        {
            var visual = await InflateXmlAsync(xml).ConfigureAwait(false);
            return (TVisualElement) visual;
        }

        public async Task<IVisualElement> InflateResourceXmlAsync(String resourceName)
        {
            var xml = await GetStringFromResourceAsync(resourceName).ConfigureAwait(false);
            return await InflateXmlAsync(xml).ConfigureAwait(false);
        }
        
        /// <summary>
        /// All xml roads lead through here
        /// </summary>
        private async Task<IVisualElement> InflateXmlImpl(String xml,
                                              IDictionary<String, String> searchSeed)
        {
            var sw = Stopwatch.StartNew();

            var node = GetRootNode(xml);
            
            var nsAsmSearch = _visualTypeResolver.GetNamespaceAssemblySearch(node, searchSeed);

            var lineage = new VisualLineage();

            var res = await GetVisualAsync(node, null, nsAsmSearch, lineage,
                _appliedStyleBuilder.ApplyVisualStylesAsync).ConfigureAwait(false);

            Debug.WriteLine("Inflated visual in " + sw.ElapsedMilliseconds + " ms");

            
            lineage.AssertPopVisual(res);
            
            if (lineage.Count > 0)
                throw new InvalidOperationException();
            
            return res;
        }
    }
}
