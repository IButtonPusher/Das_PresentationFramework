using System;
using Das.Views.Styles;

namespace Das.Views.Construction.Styles;

/// <summary>
/// Generates selectors from markup nodes
/// </summary>
public interface IStyleSelectorBuilder
{
   IStyleSelector GetSelector(IMarkupNode cssNode);
}