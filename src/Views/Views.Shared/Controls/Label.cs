using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;
using Das.Views.Primitives;
using Das.Views.Rendering;
using Das.Views.Styles.Declarations;
#if !NET40
using TaskEx = System.Threading.Tasks.Task;

#endif

namespace Das.Views.Controls
{
    [ContentProperty(nameof(Text))]
    public class Label : TextBase,
                         ILabel
    {
        public Label(IVisualBootstrapper visualBootstrapper)
            : base(visualBootstrapper)
        {
        }

      

        public override void Arrange<TRenderSize>(TRenderSize availableSpace,
                                                  IRenderContext renderContext)
        {
            var brush = TextBrush ?? renderContext.ColorPalette.OnBackground;
            renderContext.DrawString(Text, Font, brush, Point2D.Empty);
        }


        public override ValueSize Measure<TRenderSize>(TRenderSize availableSpace,
                                                      IMeasureContext measureContext)
        {
            //var font = GetFont(measureContext);
            var size = measureContext.MeasureString(Text, Font);
            return size;
        }


        public override String ToString()
        {
            return !String.IsNullOrEmpty(Text) 
                ? Text
                : "Label";
        }

       

        //private static void OnTextChanged(Label sender,
        //                                  String oldValue, String newValue)
        //{
        //    sender.InvalidateMeasure();
        //}

        public override Boolean TryGetDependencyProperty(DeclarationProperty declarationProperty, 
                                                         out IDependencyProperty property)
        {
            switch (declarationProperty)
            {
                case DeclarationProperty.Color:
                    property = TextBrushProperty;
                    return true;
                
                default:
                    return base.TryGetDependencyProperty(declarationProperty, out property);
            }
            
            
        }

        


        
    }
}