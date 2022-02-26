using System;
using System.Threading.Tasks;
using Das.Views.Panels;

namespace Das.Views.Layout
{
    public static class LogicalTreeHelper
    {
        public static void RemoveLogicalChild(IVisualElement parent,
                                              IVisualElement? child)
        {
            if (child == null)
                return;

            switch (parent)
            {
                case IVisualContainer container:
                    container.RemoveChild(child);
                    break;

                case IContentContainer contentContainer
                    when contentContainer.Content?.Equals(child) == true:
                    contentContainer.Content = default;
                    break;
            }
        }

        public static void AddLogicalChild(IVisualElement parent,
                                           IVisualElement? child)
        {
            if (child == null)
                return;

            switch (parent)
            {
                case IVisualContainer container:
                    container.AddChild(child);
                    break;

                case IContentContainer contentContainer:
                    contentContainer.Content = child;
                    break;
            }
        }
    }
}
