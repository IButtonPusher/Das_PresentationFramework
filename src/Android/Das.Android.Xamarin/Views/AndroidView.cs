﻿using System;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Views;
using Das.Container;
using Das.Views;
using Das.Views.Colors;
using Das.Views.Controls;
using Das.Views.Core.Geometry;
using Das.Views.Input;
using Das.Views.Rendering;
using Das.Views.Styles;
using Das.Xamarin.Android.Controls;
using Das.Xamarin.Android.Images;
using Das.Xamarin.Android.Input;
using Java.Lang;
using Boolean = System.Boolean;
using Double = System.Double;
using Math = System.Math;

namespace Das.Xamarin.Android;

/// <summary>
///    ViewGroup that contains the AndroidPaintView for 'normal' visuals as well as surrogated visuals,
///    when applicable
/// </summary>
/// <see cref="AndroidPaintView" />
public class AndroidView : ViewGroup
{
   // ReSharper disable once UnusedMember.Global
   public AndroidView(IVisualElement view,
                      Context context,
                      IWindowManager windowManager,
                      IUiProvider uiProvider,
                      IThemeProvider? themeProvider,
                      IResolver? resolver)
      : this(view, context,
         BuildRenderKit(context, windowManager, uiProvider,
            themeProvider ?? BaselineThemeProvider.Instance,
            resolver ?? new BaseResolver()),
         uiProvider)
   {
   }

   public AndroidView(IVisualElement view,
                      Context context,
                      AndroidRenderKit renderKit,
                      IUiProvider uiProvider)
      : base(context)
   {
      //System.Diagnostics.Debug.WriteLine($"********* AndroidView ctor on {System.Threading.Thread.CurrentThread.ManagedThreadId}");

      UILogger.LogLevel = LogLevel.Level2;

      _loopHandler = new Handler(Looper.MainLooper!);

      _targetRect = ValueRectangle.Empty;
      _view = view;
      _surrogates = new AndroidSurrogateProvider(renderKit, uiProvider, this);

      RenderKit = renderKit;
      _viewState = renderKit.ViewState;
      //_layoutQueue = RenderKit.VisualBootstrapper.LayoutQueue;


      ZoomLevel = renderKit.DisplayMetrics.ScaledDensity;

      _paintView = new AndroidPaintView(context, renderKit, view);
      AddView(_paintView);

      var inputHandler = new BaseInputHandler(RenderKit.RenderContext);
      _inputContext = new AndroidInputContext(this, context, inputHandler, renderKit.ViewState);

      _paintView.PaintViewReady += OnPaintViewReady;
   }


   public sealed override void AddView(View? child)
   {
      base.AddView(child);
   }

   public override Boolean OnGenericMotionEvent(MotionEvent? e)
   {
      _inputContext.OnGenericMotionEvent(e);
      return base.OnGenericMotionEvent(e);
   }

   public override Boolean OnInterceptTouchEvent(MotionEvent? ev)
   {
      _inputContext.OnTouchEvent(ev);
      return false;
   }


   public void OnPause()
   {
   }

   public void OnResume()
   {
      _view.InvalidateMeasure();
   }

   protected override void OnLayout(Boolean changed,
                                    Int32 left,
                                    Int32 top,
                                    Int32 right,
                                    Int32 bottom)
   {
      var count = ChildCount;
      var hasSurrogates = count > 1;

      for (var c = 0; c < count; c++)
      {
         var current = GetChildAt(c);

         if (current == null || current.Visibility != ViewStates.Visible)
         {
            continue;
         }

         if (hasSurrogates && current is IVisualSurrogate surrogate)
         {
            var wants = surrogate.ArrangedBounds;

            if (!wants.IsEmpty)
            {
               left = Convert.ToInt32(wants.Left * ZoomLevel);
               top = Convert.ToInt32(wants.Top * ZoomLevel);
               right = Convert.ToInt32(wants.Right * ZoomLevel);
               bottom = Convert.ToInt32(wants.Bottom * ZoomLevel);
            }
            else
            {
               continue;
            }
         }

         current.Layout(left, top, right, bottom);
      }

      //WriteLine("END AndroidView->OnLayout surrogates: " + hasSurrogates +
      //          " view needs arrange: " + _view.IsRequiresArrange);
   }


   protected override void Dispose(Boolean disposing)
   {
      base.Dispose(disposing);
      _loopHandler.Dispose();

      _isDisposed = true;
   }

   protected override void OnMeasure(Int32 widthMeasureSpec,
                                     Int32 heightMeasureSpec)
   {
      base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
      var w = MeasuredWidth;
      var h = MeasuredHeight;

      var sz = new ValueRenderSize(w, h);

      _targetRect = new ValueRectangle(
         0, // X
         0, // Y
         sz.Width / ZoomLevel,
         sz.Height / ZoomLevel);

      var count = ChildCount;

      for (var c = 0; c < count; c++)
      {
         var current = GetChildAt(c);

         if (current is IVisualSurrogate)
            current.Measure(Convert.ToInt32(_targetRect.Width),
               Convert.ToInt32(_targetRect.Height));
         else
            current?.Measure(widthMeasureSpec, heightMeasureSpec);
      }

      //_measured = new ValueSize(sz.Width, sz.Height);
      RenderKit.MeasureContext.MeasureMainView(_view, sz, _viewState);

      //if (_refreshLoopCount == 0)
      //{
      //   _refreshLoopCount++;
      //   var _ = Task.Factory.StartNew(RefreshLoop3,
      //      TaskCreationOptions.LongRunning);
      //}
   }

   private void OnPaintViewReady()
   {
      if (_refreshLoopCount == 0)
      {
         _refreshLoopCount++;
         //var _ = Task.Run(RefreshLoop4).ConfigureAwait(false);
         var _ = Task.Factory.StartNew(RefreshLoop3,
            TaskCreationOptions.LongRunning);
      }
   }


   private static AndroidRenderKit BuildRenderKit(Context context,
                                                  IWindowManager windowManager,
                                                  IUiProvider uiProvider,
                                                  IThemeProvider themeProvider,
                                                  IResolver resolver)
   {
      var displayMetrics = context.Resources?.DisplayMetrics ?? throw new NullReferenceException();


      var viewState = new AndroidViewState(displayMetrics, themeProvider);

      var fontProvider = new AndroidFontProvider(displayMetrics);
      return new AndroidRenderKit(new BasePerspective(), viewState,
         fontProvider, windowManager, uiProvider, themeProvider, displayMetrics,
         resolver, new AndroidImageProvider(displayMetrics)); //, viewState));
   }


   private void RefreshLoop3()
   {
      while (!_isDisposed && !_view.IsDisposed)
      {
         var willInvalidate = false;
         //var willInvalidate2 = _layoutQueue.HasVisualsNeedingLayout;

         if (_view.IsRequiresMeasure)
         {
            //RenderKit.MeasureContext.MeasureMainView(_view,
            //   new ValueRenderSize(_measured), _viewState);

            willInvalidate = true;
         }
         else if (_view.IsRequiresArrange)
            willInvalidate = true;
         else if (_inputContext.IsInteracting)
         {
            System.Diagnostics.Debug.WriteLine("willInvalidate due to currently interacting");
            willInvalidate = true;
         }

         //if (willInvalidate != willInvalidate2)
         //{
         //   //System.Diagnostics.Debug.WriteLine($"mismatch of willinvalidates {willInvalidate} / {willInvalidate2}");
         //}


         if (willInvalidate)
         {
            //_paintView.Layout(0, 0, (Int32)(_targetRect.Right  * ZoomLevel),
            //   (Int32)(_targetRect.Bottom  * ZoomLevel));


            _paintView.Refresh();

            if (ChildCount > 1)
               PostInvalidate();

            //_paintView.Draw();

            //_paintView.PostInvalidate();

            _inputContext.SleepTime = 0;
         }
         else
         {
            //if (_inputContext.SleepTime == 0)
            //    WriteLine("frame skipped!");

            _inputContext.SleepTime = Math.Min(++_inputContext.SleepTime, 50);

            Thread.Sleep(_inputContext.SleepTime);

            //await Task.Delay(_inputContext.SleepTime).ConfigureAwait(false);
         }
      }

      // ReSharper disable once FunctionNeverReturns
   }

   //private async Task RefreshLoop4()
   //{
   //   while (!_isDisposed && !_view.IsDisposed)
   //   {
   //      var willInvalidate = false;
   //      var willInvalidate2 = _layoutQueue.HasVisualsNeedingLayout;

   //      if (_view.IsRequiresMeasure)
   //      {
   //         //RenderKit.MeasureContext.MeasureMainView(_view,
   //         //   new ValueRenderSize(_measured), _viewState);

   //         willInvalidate = true;
   //      }
   //      else if (_view.IsRequiresArrange || _inputContext.IsInteracting)
   //         willInvalidate = true;

   //      if (willInvalidate != willInvalidate2)
   //      {
   //      }


   //      if (willInvalidate)
   //      {
   //         //_paintView.Layout(0, 0, (Int32)(_targetRect.Right  * ZoomLevel),
   //         //   (Int32)(_targetRect.Bottom  * ZoomLevel));


   //         _paintView.Refresh();

   //         if (ChildCount > 1)
   //            PostInvalidate();

   //         //_paintView.Draw();

   //         //_paintView.PostInvalidate();

   //         _inputContext.SleepTime = 0;
   //      }
   //      else
   //      {
   //         //if (_inputContext.SleepTime == 0)
   //         //    WriteLine("frame skipped!");

   //         _inputContext.SleepTime = Math.Min(++_inputContext.SleepTime, 50);

   //         //Thread.Sleep(_inputContext.SleepTime);

   //         await Task.Delay(_inputContext.SleepTime).ConfigureAwait(false);
   //      }
   //   }

   //   // ReSharper disable once FunctionNeverReturns
   //}

   public Double ZoomLevel { get; }

   public AndroidRenderKit RenderKit { get; }


   private readonly AndroidInputContext _inputContext;
   //private readonly ILayoutQueue _layoutQueue;

   private readonly Handler _loopHandler;
   private readonly AndroidPaintView _paintView;

   // ReSharper disable once NotAccessedField.Local
   private readonly AndroidSurrogateProvider _surrogates;

   private readonly IVisualElement _view;
   private readonly IViewState _viewState;


   //private Size _measured;
   private Boolean _isDisposed;
   private Int32 _refreshLoopCount;
   private ValueRectangle _targetRect;
}
