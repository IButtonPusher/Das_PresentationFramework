﻿using System;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Das.Views;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;

namespace Das.Xamarin.Android;

/// <summary>
///    Android view that measures and arranges DPF visuals that are not delegated to surrogates
/// </summary>
public class AndroidPaintView : SurfaceView,
                                ISurfaceHolderCallback
{
   public AndroidPaintView(Context? context,
                           AndroidRenderKit renderKit,
                           IVisualElement view)
      : base(context)
   {
      _surfaceHolder = Holder;
      _surfaceHolder.AddCallback(this);
      _renderContext = renderKit.RenderContext;
      _measureContext = renderKit.MeasureContext;
      _targetRect = ValueRectangle.Empty;
      _renderKit = renderKit;
      _view = view;


      //_totalRun = new Stopwatch();
      //_currentRender = new Stopwatch();

      //_refreshStopwatch = new Stopwatch();
      //_sinceLastRender= new Stopwatch();

      _viewState = renderKit.RenderContext.ViewState ?? throw new NullReferenceException();
   }


   //private readonly Stopwatch _totalRun;
   //private readonly Stopwatch _currentRender;
   //private readonly Stopwatch _sinceLastRender;

   public void SurfaceChanged(ISurfaceHolder? holder,
                              Format format,
                              Int32 width,
                              Int32 height)
   {
      _targetRect = new ValueRectangle(0, 0,
         width / _viewState.ZoomLevel,
         height / _viewState.ZoomLevel);
   }

   public void SurfaceCreated(ISurfaceHolder? holder)
   {
      //Debug.WriteLine($"********* Surface created on thread {System.Threading.Thread.CurrentThread.ManagedThreadId}");
      //_totalRun.Restart();
      //_sinceLastRender.Restart();
      _isSurfaceReady = true;
      PaintViewReady?.Invoke();
   }

   public void SurfaceDestroyed(ISurfaceHolder? holder)
   {
      //Debug.WriteLine($"********* Surface destroyed on thread {System.Threading.Thread.CurrentThread.ManagedThreadId}");

      _isSurfaceReady = false;
   }


   //public override void PostInvalidate()
   //{
   //   //Interlocked.Add(ref _timesInvalidated, 1);
   //   base.PostInvalidate();
   //}


   public void Refresh()
   {
      if (!_isSurfaceReady)
         return;

      //var since = _sinceLastRender.ElapsedMilliseconds;
      //System.Diagnostics.Debug.WriteLine($"refresh apv - ms since: {since}");

      //var sw = Stopwatch.StartNew();
      //_surfaceHolder.
      var cnv = _surfaceHolder.LockCanvas();

      //var t1 = sw.ElapsedMilliseconds;
      //Int64 t2 = 0, t3;

      try
      {
         DrawImpl(cnv!);

         //t2 = sw.ElapsedMilliseconds - t1;
      }
      finally
      {
         _surfaceHolder.UnlockCanvasAndPost(cnv);

         //_sinceLastRender.Restart();

         //t3 = sw.ElapsedMilliseconds - t2;

         //_sum1 += t1;
         //_sum2 += t2;
         //_sum3 += t3;

         //if ((++_counter % 100) == 0)
         //{
         //   Debug.WriteLine($"t1: {_sum1} t2: {_sum2} t3: {_sum3} counter: {_counter}");
         //}
      }
   }

   //private Int64 _sum1;
   //private Int64 _sum2;
   //private Int64 _sum3;

   //private Int32 _counter;

   protected override void OnDraw(Canvas? canvas)
   {
      if (canvas == null)
         return;

      DrawImpl(canvas);
   }

   protected override void OnMeasure(Int32 widthMeasureSpec,
                                     Int32 heightMeasureSpec)
   {
      //_timeStarted = DateTime.Now;

      base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
      var w = MeasuredWidth;
      var h = MeasuredHeight;

      var sz = new ValueRenderSize(w, h);

      _measureContext.MeasureMainView(_view, sz, _viewState);

      _targetRect = new ValueRectangle(
         0, // X
         0, // Y
         sz.Width / _viewState.ZoomLevel,
         sz.Height / _viewState.ZoomLevel);
   }

   private void DrawImpl(Canvas canvas)
   {
      //UILogger.Log("BEGIN AndroidPaintView->OnDraw.  view needs arrange: " +
      //                _view.IsRequiresArrange, LogLevel.Level1);

      //_currentRender.Restart();

      if (_view.IsRequiresMeasure)
      {
         _renderKit.MeasureContext.MeasureMainView(_view,
            new ValueRenderSize(_targetRect), _viewState);
      }

      _renderContext.Canvas = canvas;
      _renderContext.DrawMainElement(_view,
         _targetRect, _viewState);


      //_totalLayoutMs += _currentRender.ElapsedMilliseconds;
      //_totalLayouts++;
      ////if (Interlocked.Add(ref _totalLayouts, 1) % 10 == 0)
      //if (_totalRun.ElapsedMilliseconds >= 5000)
      //{
      //   var ts = DateTime.Now - _timeStarted;
      //   var layoutsPerSecond = _totalLayouts / ts.TotalSeconds;
      //   var avgLayout = _totalLayoutMs / (Double) _totalLayouts;

      //   UILogger.Log("********layouts per second: " + layoutsPerSecond +
      //                         " avg layout time: " + avgLayout +
      //                         " invalidate requests: " + _timesInvalidated, LogLevel.Level2);

      //   _totalRun.Restart();
      //   _totalLayoutMs = 0;
      //   _totalLayouts = 0;
      //}
   }

   public sealed override ISurfaceHolder Holder => base.Holder ?? throw new NullReferenceException(nameof(Holder));
   //private Int32 _timesInvalidated;

   //private DateTime _timeStarted;
   //private Int64 _totalLayoutMs;
   //private Int32 _totalLayouts;

   //private readonly Stopwatch _refreshStopwatch;
   //private Int64 _totalRefreshMs;
   //private Int32 _totalRefreshes;

   public event Action? PaintViewReady;

   private readonly AndroidMeasureKit _measureContext;
   private readonly AndroidRenderContext _renderContext;


   private readonly AndroidRenderKit _renderKit;

   private readonly ISurfaceHolder _surfaceHolder;
   private readonly IVisualElement _view;
   private readonly IViewState _viewState;
   private Boolean _isSurfaceReady;

   private ValueRectangle _targetRect;
}
