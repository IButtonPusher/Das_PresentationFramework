using System;
using System.IO;
using System.Windows.Forms;
using Das.Gdi;
using Das.Views.DevKit;
using Das.Views.Winforms;

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

            form.Shown += OnFormShown;


            return form;
        }

        private void OnFormShown(Object sender, 
                                 EventArgs e)
        {
            if (!(sender is ViewWindow form))
                return;
            
            var renderer = new BitmapRenderer(form, RenderKit.MeasureContext,
                RenderKit.RenderContext);
            
            var updater = new DesignViewUpdater(form, renderer, RenderKit.MeasureContext,
                RenderKit.RenderContext);
            var devInputHandler = new DevInputHandler(RenderKit.RenderContext, updater);
            
            var _ = new DevInputProvider(form, RenderKit.RenderContext, 
                updater, devInputHandler, form.Handle);
        }
    }
}
