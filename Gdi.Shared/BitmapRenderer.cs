﻿using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using Das.Views;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.Hosting;
using Das.Views.Rendering;
using Rectangle = Das.Views.Core.Geometry.Rectangle;

namespace Das.Gdi
{
    public class BitmapRenderer : IRenderer<Bitmap>
    {
        public BitmapRenderer(IViewHost<Bitmap> viewHost,
                              GdiMeasureContext measureContext,
                              GdiRenderContext renderContext,
                              Color backgroundColor)
        {
            _eventLock = new Object();

            _lock = new Object();
            _viewHost = viewHost;
            _visualHost = viewHost;

            _visualHost.AvailableSizeChanged += OnHostSizeChanged;
            
            _measureContext = measureContext;
            _renderContext = renderContext;
            _gdiDevice = new GdiDevice(backgroundColor,
                _viewHost.AvailableSize);
                //Convert.ToInt32(_viewHost.AvailableSize.Width),
                //Convert.ToInt32(_viewHost.AvailableSize.Height));
            _renderRect = new Rectangle(0, 0, 1, 1);

            _hostRect = new ValueRectangle(0, 0, viewHost.AvailableSize);
        }

      

        public BitmapRenderer(IViewHost<Bitmap> viewHost,
                              GdiMeasureContext measureContext,
                              GdiRenderContext renderContext)
            : this(viewHost, measureContext, renderContext,
                Color.White)
        {
        }

        
        private void OnHostSizeChanged(ISize newValue)
        {
            lock (_lock)
            {
                if (newValue.IsEmpty)
                    return;

                _hostRect = new ValueRectangle(0, 0, newValue);
                
                _gdiDevice.UpdateSize(newValue);
                
                var view = _viewHost.View;
                if (view == null)
                    return;
                
                view.InvalidateMeasure();
            }
        }
        
        public Bitmap DoRender()
        {
            lock (_lock)
            {
                var view = _viewHost.View;
                if (view == null)
                    return default!;

                if (_visualHost.AvailableSize.Width == 0 ||
                    _visualHost.AvailableSize.Height == 0)
                {
                    return null!;
                }

                //todo:
                if (view.VerticalAlignment == VerticalAlignments.Stretch &&
                    view.HorizontalAlignment == HorizontalAlignments.Stretch)
                {
                    if (_gdiDevice.UpdateSize(_visualHost.AvailableSize))
                    {
                        view.InvalidateMeasure();
                        //   return null!;
                    }
                }

                var available = new ValueRenderSize(_visualHost.AvailableSize);

                if (view.IsRequiresMeasure)
                {
                    var desired = _measureContext.MeasureMainView(view, available, _viewHost);
                    _renderRect.Size = desired;

                    switch (_viewHost.SizeToContent)
                    {
                        case SizeToContent.Width | SizeToContent.Height:
                            _gdiDevice.UpdateSize(desired.Width, desired.Height);
                            break;
                        
                        case SizeToContent.Width:
                            _gdiDevice.UpdateSize(desired.Width, _gdiDevice.Height);
                            break;
                        
                        case SizeToContent.Height:
                            _gdiDevice.UpdateSize(_gdiDevice.Width, desired.Height);
                            break;
                    }
                    
                    _gdiDevice.Clear();
                    
                    
                    //if (view.VerticalAlignment == VerticalAlignments.Default &&
                    //    view.HorizontalAlignment == HorizontalAlignments.Default)
                    //{
                    //    if (_gdiDevice.UpdateSize(desired))
                    //    {
                    //        //view.InvalidateMeasure();
                    //        //return null!;
                    //    }
                    //}

                    //_gdiDevice.Width = Convert.ToInt32(desired.Width);
                    //_gdiDevice.Height = Convert.ToInt32(desired.Height);
                }

                var bmp = _gdiDevice.Run(view, DoRender);

                //if (bmp != null)
                //{
                //    _dumpIndex++;
                //    bmp.Save("dump\\img_" + _dumpIndex + ".png");
                //}

                return bmp!;
            }
        }

        //private Int32 _dumpIndex;

        public IRenderSize? GetContentSize(IRenderSize available)
        {
            var view = _viewHost.View;
            if (view == null)
                return default;
                
            return new ValueRenderSize(_measureContext.MeasureMainView(view, 
                available, _viewHost));
        }

        private event EventHandler? _rendering;

        private Int32 _eventCounter;

        private readonly Object _eventLock;

        public event EventHandler? Rendering
        {
            add
            {
                lock (_eventLock)
                {
                    _rendering += value;
                    Interlocked.Add(ref _eventCounter, 1);
                }
            }
            remove
            {
                lock (_eventLock)
                {
                    Interlocked.Add(ref _eventCounter, -1);
                    _rendering -= value;
                }

            }
        }

        private Boolean DoRender(Graphics g,
                                 IVisualElement view)
        {
            if (!view.IsRequiresArrange)
                return false;

            var renderTo = new ValueRectangle(0, 0,
                Math.Min(_hostRect.Width, _renderRect.Width),
                Math.Min(_hostRect.Height, _renderRect.Height));
            
            _renderContext.Graphics = g;
            _renderContext.DrawMainElement(view, renderTo, _viewHost);
            //_renderContext.DrawMainElement(view, _renderRect, _viewHost);

            lock (_eventLock)
            {
                if (_eventCounter > 0)
                    _rendering!.Invoke(this, EventArgs.Empty);
            }

            return true;
        }

        private readonly GdiDevice _gdiDevice;
        private readonly Object _lock;
        private readonly GdiMeasureContext _measureContext;
        private readonly GdiRenderContext _renderContext;
        private readonly Rectangle _renderRect;
        private ValueRectangle _hostRect;
        private readonly IViewHost<Bitmap> _viewHost;
        private readonly IVisualHost<Bitmap> _visualHost;
    }
}