using Das.Views.Core.Geometry;
using Das.Views.DataBinding;
using Das.Views.Extended.Core;
using Das.Views.Rendering;

namespace Das.Views.Extended.Runtime
{
    /// <summary>
    /// Control that renders the data provided by a Camera that is watching a Scene
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public class SceneMonitor : BindableElement<ICamera>, IBindableElement<ICamera>
    {
        public override ISize Measure(ISize availableSpace, IMeasureContext measureContext)
        {
            var h = availableSpace.Height;
            var camera = Binding.GetValue(DataContext);
            if (camera == null)
                return Size.Empty;

            if (h.AreEqualEnough(0))
                return availableSpace;

            var aspect = availableSpace.Width / h;
            
            if (aspect.AreEqualEnough(camera.AspectRatio))
            {
                return availableSpace;
            }
            if (aspect > camera.AspectRatio) //available is wider
            {
                var sz = new Size(availableSpace.Height * camera.AspectRatio,
                    availableSpace.Height);
                return sz;
            }
            else //taller
            {
                var sz = new Size(availableSpace.Width / camera.AspectRatio,
                    availableSpace.Width);
                return sz;
            }
        }

        public override void Arrange(ISize availableSpace, IRenderContext renderContext)
        {
            var camera = Binding.GetValue(DataContext);
            if (camera == null)
                return;

            var frame = camera.GetFrame(availableSpace);
            renderContext.DrawFrame(frame);
        }
    }
}
