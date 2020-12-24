using System;
using System.Drawing;
using Das.Views.Rendering;

namespace Das.Views.Winforms
{
    public class HostedVisualControl : HostedControl<Bitmap>
    {
        public HostedVisualControl(IVisualRenderer visual) : base(visual)
        {
     
        }
    }
}
