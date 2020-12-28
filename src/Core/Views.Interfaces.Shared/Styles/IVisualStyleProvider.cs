using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.Construction;

namespace Das.Views.Styles
{
    /// <summary>
    /// Loads/caches styles by name
    /// </summary>
    public interface IVisualStyleProvider
    {
        IAsyncEnumerable<IStyleRule> GetStylesByClassNameAsync(String className);

        /// <summary>
        /// Finds styles in xml resource files
        /// </summary>
        Task<IStyleSheet?> GetStyleByNameAsync(String name);

        Task<IStyleSheet?> GetStyleForVisualAsync(IVisualElement visual,
                                                  IAttributeDictionary attributeDictionary);
    }
}
