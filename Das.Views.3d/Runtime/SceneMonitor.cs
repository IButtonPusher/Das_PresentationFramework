
using System;
using System.ComponentModel;
using Das.Extensions;
using Das.Views.Core.Geometry;
using Das.Views.DataBinding;
using Das.Views.Rendering;

namespace Das.Views.Extended.Runtime
{
    /// <summary>
    /// Control that renders the data provided by a Camera that is watching a Scene
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    // ReSharper disable once UnusedType.Global
    public class SceneMonitor : BindableElement<ICamera>, IChangeTracking
    {
        public override ISize Measure(ISize availableSpace, 
                                      IMeasureContext measureContext)
        {
            if (!TryGetCamera(out var camera))
                return Size.Empty;

            if (availableSpace.Height.AreEqualEnough(0))
                return availableSpace;

            var aspect = availableSpace.Width / availableSpace.Height;
            
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

        private Boolean TryGetCamera(out ICamera camera)
        {
            if (!(Binding is {} binding) || !(DataContext is {} dc))
                camera = default!;
            else camera = binding.GetValue(dc);

            return camera != null;
        }

        public override void Arrange(ISize availableSpace, IRenderContext renderContext)
        {
            if (!TryGetCamera(out var camera))
                return;

            camera.RenderFrame(availableSpace, renderContext);

            //var frame = camera.GetFrame(availableSpace);
            //renderContext.DrawFrame(frame);
        }

        public override void Dispose()
        {
            
        }

        public void AcceptChanges()
        {
            
        }

        public Boolean IsChanged => true;
    }
}
