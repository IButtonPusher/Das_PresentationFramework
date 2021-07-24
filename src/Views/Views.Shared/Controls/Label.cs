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

          //#if DEBUG

          //lock (_aliveLock)
          //   _alivers.Add(this, 1);

          //#endif

          //var wot = Interlocked.Add(ref _netAlive, 1);

          //if (wot > 1000)
          //{
          //}
       }

       


        #if DEBUG

        //private static Int32 _netAlive;
        //private static readonly Object _aliveLock = new();
        //private static readonly Dictionary<Label, Byte> _alivers = new();

        #endif
      

        public override void Arrange<TRenderSize>(TRenderSize availableSpace,
                                                  IRenderContext renderContext)
        {
            var brush = TextBrush ?? renderContext.ColorPalette.OnBackground;
            renderContext.DrawString(Text, Font, brush, Point2D.Empty);
        }


        public override ValueSize Measure<TRenderSize>(TRenderSize availableSpace,
                                                       IMeasureContext measureContext)
        {
           var size = measureContext.MeasureString(Text, Font);
            return size;
        }


        public override String ToString()
        {
            return !String.IsNullOrEmpty(Text) 
                ? "Label: {" + Text + "}"
                : "Label";
        }

        //public override void Dispose()
        //{
        //   base.Dispose();
           

        //   //#if DEBUG

        //   //Interlocked.Add(ref _netAlive, -1);
        //   //lock (_aliveLock)
        //   //   _alivers.Remove(this);

        //   //#endif
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