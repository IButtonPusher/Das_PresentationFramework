using System;
using System.Drawing;
using System.Windows.Forms;
using Das.Views;
using Das.Views.Panels;
using Das.Views.Styles;
using WinForms.Shared;

namespace Das.Gdi.Controls
{
    // ReSharper disable once UnusedMember.Global
    public class GdiHostedElement : HostedViewControl, 
        IViewHost<Bitmap>
    {
        public GdiHostedElement(IView view, IStyleContext styleContext)
            : base(view, styleContext)
        {
            View = view;
            _lockBmp = new object();
        }

        public GdiHostedElement(IStyleContext styleContext) : base(styleContext)
        {
            _lockBmp = new object();
        }
        
        public override bool IsLoaded => true;
        
        private readonly Object _lockBmp;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            lock (_lockBmp)
            {
                if (BackingBitmap == null)
                    return;

                _isChanged = false;

                AvailableSize.Width = Width;
                AvailableSize.Height = Height;

                e.Graphics.DrawImage(BackingBitmap, Point.Empty);
            }
        }
       
        private Bitmap _backingBitmap;
        private bool _isChanged;

        public Bitmap BackingBitmap
        {
            get => _backingBitmap;
            set
            {
                lock (_lockBmp)
                {
                    _backingBitmap?.Dispose();
                    _backingBitmap = value;
                }

                Invalidate();
            }
        }

        public Bitmap Asset
        {
            get => BackingBitmap;
            set => BackingBitmap = value;
        }

        //todo: base on IView after changing element to IView type _
        public override bool IsChanged => View != null && (_isChanged || View.IsChanged 
            || base.IsChanged);
    }
}
