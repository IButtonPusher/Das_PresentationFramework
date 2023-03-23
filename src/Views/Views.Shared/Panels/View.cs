using System;
using System.Threading.Tasks;

namespace Das.Views.Panels
{
   public class View : ContentPanel,
                       IView
   {
      public View(IVisualBootstrapper visualBootstrapper)
         : base(visualBootstrapper)
      {
      }
   }
}
