using System;
using System.Drawing;
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
            var point = TypeConverter.GetPoint(input);
            point = PointToClient(point);
            return TypeConverter.GetPoint(point);
        }

        private readonly GdiHostedElement _contents;

        private Boolean _isChanged;

        public Bitmap BackingBitmap
        {
            get => _contents.BackingBitmap!;
            set
            {
                _contents.BackingBitmap = value;
                _isChanged = false;
            }
        }

        public override Boolean IsChanged => base.IsChanged || _isChanged;

        public Bitmap Asset
        {
            get => BackingBitmap;
            set => BackingBitmap = value;
        }

    }
}
