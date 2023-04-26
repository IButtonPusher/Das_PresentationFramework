using System;
using Das.Views.Panels;

namespace Das.Views.Input;

public static class HitTester
{
   public static Boolean HitTest<TArgs>(IVisualElement visual,
                                        Int32 x,
                                        Int32 y)
   {
      if (!visual.ArrangedBounds.Contains(x, y))
         return false;

      var current = visual;

      while (true)
      {
         if (current is IContentVisual cv &&
             cv.Content is { } content &&
             content.ArrangedBounds.Contains(x, y))
         {
            current = cv;
            continue;
         }

         if (current is IVisualContainer container)
         {
            for (var c = container.Children.Count - 1; c >= 0; c--)
            {
               var child = container.Children[c];
               if (HitTest<TArgs>(child, x, y))
                  return true;
            }
         }
      }


      //switch (current)
      //{
      //   case IContentVisual cv:
      //      break;
      //}
   }
}