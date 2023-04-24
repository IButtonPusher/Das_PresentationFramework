using System;
using System.Threading.Tasks;
using Das.Views.ItemsControls;

namespace Das.Views.Defaults;

public class DefaultTabHeaderTemplate : DefaultContentTemplate
{
   public DefaultTabHeaderTemplate(IVisualBootstrapper visualBootstrapper,
                                   ITabControl tabControl)
      : base(visualBootstrapper)
   {
      _itemsControl = tabControl;
   }

   public override IVisualElement? BuildVisual(Object? dataContext)
   {
      return new DefaultTabHeaderPanel(_itemsControl, _visualBootstrapper);
   }


   private readonly ITabControl _itemsControl;
}