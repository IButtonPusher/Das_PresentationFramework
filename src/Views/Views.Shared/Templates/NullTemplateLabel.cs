using System;
using System.Threading.Tasks;
using Das.Views.Controls;

namespace Das.Views.Templates
{
   public class NullTemplateLabel : Label
   {
      public NullTemplateLabel(IVisualBootstrapper visualBootstrapper) 
         : base(visualBootstrapper)
      {
      }

      public sealed override void OnParentChanging(IVisualElement? newParent)
      {
         base.OnParentChanging(newParent);

         if (newParent == null)
            Dispose();
      }

      public override void InvalidateMeasure()
      {
         // intentionally left blank
      }

      public override void InvalidateArrange()
      {
         // intentionally left blank
      }
   }
}
