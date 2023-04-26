using System;
using System.Collections.Generic;
using Das.Views.Styles;

namespace Das.Views.Construction.Styles;

/// <summary>
/// A Cascading style sheet is made up of a collection of self contained rules.
/// given a single node from a parsed CSS document, this will provide the strongly typed rule
/// </summary>
public interface ICssRuleBuilder
{
   IStyleRule? GetRule(IMarkupNode markupNode);

   IEnumerable<IStyleRule> GetRules(String css);
}