using System;
using System.Threading;
using System.Threading.Tasks;
using Das.Views.Controls;
using Das.Views.Core.Geometry;
using Das.Views.Input;
using Das.Views.Mvvm;
using Das.Views.Rendering;
using Das.Views.Rendering.Geometry;
using Das.Views.Styles.Application;

#if !NET40
using TaskEx = System.Threading.Tasks.Task;

#endif

namespace Das.Views
{
   public abstract partial class VisualElement : NotifyPropertyChangedBase,
                                                 IVisualElement
   {
      protected VisualElement(IVisualBootstrapper visualBootstrapper)
      {
         _visualBootstrapper = visualBootstrapper;
         _arrangedBounds = ValueRenderRectangle.Empty;

         _measuredSize = ValueSize.Empty;
         _layoutQueue = visualBootstrapper.LayoutQueue;
         Id = Interlocked.Increment(ref _currentId);
         
         // Keep a reference typed to IVisualElement so that dependency property lookups are faster
         _me = this;
      }

      public virtual ValueSize Measure<TRenderSize>(TRenderSize availableSpace,
                                                    IMeasureContext measureContext)
         where TRenderSize : IRenderSize
      {
         measureContext.TryGetElementSize(this, availableSpace, out _measuredSize);
         return _measuredSize;
      }

      public virtual ValueRenderRectangle ArrangedBounds
      {
         get => _arrangedBounds;
         set => SetValue(ref _arrangedBounds, value);
      }

      public virtual void Arrange<TRenderSize>(TRenderSize availableSpace,
                                               IRenderContext renderContext)
         where TRenderSize : IRenderSize
      {
         //intentionally left blank
      }

      public virtual void InvalidateMeasure()
      {
         if (_isLayoutSuspended)
            return;

         IsRequiresMeasure = true;
         IsRequiresArrange = true;
         _layoutQueue.QueueVisualForMeasure(this);
      }

      public virtual void InvalidateArrange()
      {
         if (_isLayoutSuspended)
            return;

         IsRequiresArrange = true;
         _layoutQueue.QueueVisualForArrange(this);
      }

      public virtual void AcceptChanges(ChangeType changeType)
      {
         switch (changeType)
         {
            case ChangeType.MeasureAndArrange:
               _layoutQueue.RemoveVisualFromMeasureQueue(this);
               IsRequiresMeasure = false;
               goto acceptArrange;

            case ChangeType.Measure:
               _layoutQueue.RemoveVisualFromMeasureQueue(this);
               IsRequiresMeasure = false;
               break;

            case ChangeType.Arrange:
               acceptArrange:
               _layoutQueue.RemoveVisualFromArrangeQueue(this);
               IsRequiresArrange = false;
               break;
         }
      }


      public new void RaisePropertyChanged(String propertyName,
                                           Object? value)
      {
         base.RaisePropertyChanged(propertyName, value);
      }

      public virtual Boolean TryHandleInput<TArgs>(TArgs args,
                                                   Int32 x,
                                                   Int32 y)
         where TArgs : IMouseInputEventArgs<TArgs>
      {
         //if (!ArrangedBounds.Contains(x, y))
         //   return false;

         return TryHandleInput(args);
      }

      /// <summary>
      /// Assumes that hit testing has already passed
      /// </summary>
      protected Boolean TryHandleInput<TArgs>(TArgs args)
         where TArgs : IMouseInputEventArgs<TArgs>
      {
         if (this is not IHandleInput interactive ||
             (interactive.HandlesActions & args.Action) != args.Action ||
             interactive is not IHandleInput<TArgs> inputHandler)
            return false;

         return inputHandler.OnInput(args);
      }

      // ReSharper disable once UnusedAutoPropertyAccessor.Global
      public IAppliedStyle? Style { get; set; }

      public virtual void OnParentChanging(IVisualElement? newParent)
      {
         if (ReferenceEquals(newParent, null))
            _layoutQueue.RemoveVisualFromQueues(this, ChangeType.MeasureAndArrange);
      }

      public override void Dispose()
      {
         _isDisposed = true;
         
         base.Dispose();

         if (BeforeLabel is { } beforeLabel)
         {
            beforeLabel.Dispose();
            BeforeLabel = default;
         }

         if (AfterLabel is { } afterLabel)
         {
            afterLabel.Dispose();
            AfterLabel = default;
         }

         _layoutQueue.RemoveVisualFromQueues(this, ChangeType.MeasureAndArrange);


         #if DEBUG

         _disposed?.Invoke(this);
         _disposed = null;
         _disposeCheck.Clear();

         #else

         Disposed?.Invoke(this);
         Disposed = null;

         #endif


         //Disposed?.Invoke(this);

         //Disposed = null;
      }

      public Boolean Equals(IVisualElement other)
      {
         return ReferenceEquals(this, other);
      }

      public override String ToString()
      {
         return GetType().Name;
      }

      public virtual void SuspendLayout()
      {
         _isLayoutSuspended = true;
      }

      public virtual void ResumeLayout()
      {
         _isLayoutSuspended = false;
      }

      protected virtual void OnTemplateSet(IVisualTemplate? newValue)
      {
      }


      private static Int32 _currentId;


      protected readonly IVisualBootstrapper _visualBootstrapper;


      private ValueRenderRectangle _arrangedBounds;

#pragma warning disable 414
      private Boolean _isDisposed;
#pragma warning restore 414
      private Boolean _isLayoutSuspended;
      private ValueSize _measuredSize;
      private readonly ILayoutQueue _layoutQueue;
      private readonly IVisualElement _me;
   }
}
