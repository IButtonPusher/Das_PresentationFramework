using System.IO;
using Das.Gdi;
using Das.Views.DevKit;

namespace ViewCompiler
{
    public class DesignerProvider : ViewProvider
    {
        public DesignViewWindow Design(FileInfo file)
        {
            var serializer = GetViewDeserializer();
            var form = new DesignViewWindow(serializer, file, RenderKit.VisualBootstrapper);
        
            var renderer = new BitmapRenderer(form, RenderKit.MeasureContext,
                RenderKit.RenderContext);

            var updater = new DesignViewUpdater(form, renderer, RenderKit.MeasureContext,
                RenderKit.RenderContext);
            var devInputHandler = new DevInputHandler(RenderKit.RenderContext, updater);
            var _ = new DevInputProvider(form, RenderKit.RenderContext, 
                updater, devInputHandler);

            return form;
        }

       
    }
}
