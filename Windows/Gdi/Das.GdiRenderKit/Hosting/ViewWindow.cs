using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Das.Gdi.Controls;
using Das.Gdi.Core;
using Das.Views;
using Das.Views.Core.Geometry;
using Das.Views.Winforms;

namespace Das.Gdi
{
    public partial class ViewWindow : ViewForm,
                                      IViewHost<Bitmap>
    {
        public ViewWindow(GdiHostedElement element) : base(element)
        {
            _contents = element;

            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw,
                true);
        }

        public override IPoint2D GetOffset(IPoint2D input)
        {
            var point = GdiTypeConverter.GetPoint(input);
            point = PointToClient(point);
            return GdiTypeConverter.GetPoint(point);
        }

        public Bitmap Asset
        {
            get => BackingBitmap;
            set => BackingBitmap = value;
        }

        public Bitmap BackingBitmap
        {
            get => _contents.BackingBitmap!;
            set => _contents.BackingBitmap = value;
        }

        private readonly GdiHostedElement _contents;
    }
}