using System;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;
using Das.Views.Rendering.Geometry;

namespace Das.Views.Panels
{
    // ReSharper disable once UnusedType.Global
    public class Canvas: BasePanel
    {
        public Canvas(IVisualBootstrapper visualBootstrapper) 
            : base(visualBootstrapper)
        {
        }

        public override ValueSize Measure(IRenderSize availableSpace, 
                                          IMeasureContext measureContext)
        {
            if (Children.Count == 0)
                return ValueSize.Empty;

            var useWidth = 0.0;
            var useHeight = 0.0;

            foreach (var child in Children.GetAllChildren())
            {
                var childMeasures = measureContext.MeasureElement(child, availableSpace);
                
                useWidth = Math.Max(useWidth, childMeasures.Width);
                useHeight = Math.Max(useHeight, childMeasures.Height);
            }

            return new ValueSize(useWidth, useHeight);
        }

        public override Boolean IsRequiresArrange
        {
            get => base.IsRequiresArrange || Children.IsRequiresArrange;
            protected set => base.IsRequiresArrange = value;

        }

        public override void Arrange(IRenderSize availableSpace, 
                                     IRenderContext renderContext)
        {
            System.Diagnostics.Debug.WriteLine(">> arranging canvas");

            foreach (var child in Children.GetAllChildren())
            {
                var childWants = renderContext.GetLastMeasure(child);
                if (childWants.IsEmpty)
                    continue;

                var left = child.Left?.GetQuantity(availableSpace.Width) ?? 0;
                var top = child.Top?.GetQuantity(availableSpace.Height) ?? 0;
                var width = Math.Min(childWants.Width, availableSpace.Width);
                var height = Math.Min(childWants.Height, availableSpace.Height);

                var drawMe = new RenderRectangle(left, top, width, height, availableSpace.Offset);

                renderContext.DrawElement(child,drawMe);
            }

            System.Diagnostics.Debug.WriteLine("<< arranged canvas");
        }

        protected override void OnDistributeDataContextToChildren(Object? newValue)
        {
            _children.DistributeDataContext(newValue);
            base.OnDistributeDataContextToChildren(newValue);
        }
    }
}
