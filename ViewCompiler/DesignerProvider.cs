using System;
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
            var form = new DesignViewWindow(serializer, file);
        
            var renderer = new BitmapRenderer(form, RenderKit.MeasureContext,
                RenderKit.RenderContext);

            var updater = new DesignViewUpdater(form, renderer, RenderKit.MeasureContext,
                RenderKit.RenderContext);
            var _ = new DevInputHandler(form, RenderKit.RenderContext, 
                updater);

            return form;
        }

       
    }
}
