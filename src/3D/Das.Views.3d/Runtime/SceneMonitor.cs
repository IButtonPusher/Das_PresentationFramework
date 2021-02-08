using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Das.Extensions;
using Das.Views.Core.Geometry;
using Das.Views.DataBinding;
using Das.Views.Rendering;

namespace Das.Views.Extended.Runtime
{
    /// <summary>
    ///     Control that renders the data provided by a Camera that is watching a Scene
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    // ReSharper disable once UnusedType.Global
    public class SceneMonitor : BindableElement, IChangeTracking
    {
        //public SceneMonitor(IDataBinding<ICamera>? binding, 
        //                    IVisualBootstrapper templateResolver) 
        //    : base(binding, templateResolver)
        //{
        //}

        public SceneMonitor(IVisualBootstrapper templateResolver) : base(templateResolver)
        {
        }

        //public override void Dispose()
        //{

        //}

        public void AcceptChanges()
        {
        }

        public Boolean IsChanged => true;

        public override void Arrange<TRenderSize>(TRenderSize availableSpace,
                                                  IRenderContext renderContext)
        {
            if (!TryGetCamera(out var camera))
                return;

            camera.RenderFrame(availableSpace, renderContext);

            //var frame = camera.GetFrame(availableSpace);
            //renderContext.DrawFrame(frame);
        }

        public override ValueSize Measure<TRenderSize>(TRenderSize availableSpace,
                                                       IMeasureContext measureContext)
        {
            if (!TryGetCamera(out var camera))
                return ValueSize.Empty;

            if (availableSpace.Height.AreEqualEnough(0))
                return availableSpace.ToValueSize();

            var aspect = availableSpace.Width / availableSpace.Height;

            if (aspect.AreEqualEnough(camera.AspectRatio))
                return availableSpace.ToValueSize();
            if (aspect > camera.AspectRatio) //available is wider
            {
                var sz = new ValueSize(availableSpace.Height * camera.AspectRatio,
                    availableSpace.Height);
                return sz;
            }
            else //taller
            {
                var sz = new ValueSize(availableSpace.Width / camera.AspectRatio,
                    availableSpace.Width);
                return sz;
            }
        }

        private Boolean TryGetCamera(out ICamera camera)
        {
            camera = (DataContext as ICamera)!;
            return camera != null;

            //if (!(DataContext is { } binding) || !(DataContext is { } dc))
            //    camera = default!;
            //else camera = binding.GetValue(dc);

            //return camera != null;
        }
    }
}
