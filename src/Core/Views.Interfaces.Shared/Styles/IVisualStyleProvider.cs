using System;
using System.Threading.Tasks;
using Das.Views.Construction;

namespace Das.Views.Styles;

/// <summary>
/// Loads/caches styles by visual types and style/class names
/// </summary>
public interface IVisualStyleProvider
{
   /// <summary>
   /// Finds the style for partially instantiated (constructed bindings, properties, templates etc)
   /// not yet set.  Combines styling for tye visual's type with a style resolved by a class or
   /// style name via the provided attribute dictionary, if applicable.
   /// </summary>
   Task<IStyleSheet?> GetStyleForVisualAsync(IVisualElement visual,
                                             IAttributeDictionary attributeDictionary);

   IStyleSheet? GetCoreStyleForVisual(IVisualElement visual);
}