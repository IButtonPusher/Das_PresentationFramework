using System;
using Android.Content;
using Android.Views;
using Das.Views.Controls;
using Das.Views.Core.Geometry;
using Das.Views.Panels;
using Das.Views.Rendering;

namespace Das.Xamarin.Android.Views
{
    public abstract class SurrogateView : ViewGroup, 
                                          IVisualSurrogate
    {
        public IVisualElement VisualElement { get; }

        private readonly View _androidView;

        public SurrogateView(Context? context,
                             IVisualElement visualReplacing,
                             View androidView) 
            : base(context)
        {
            VisualElement = visualReplacing;
            _androidView = androidView;
            // ReSharper disable once VirtualMemberCallInConstructor
            AddView(_androidView);
        }

        public ISize Measure(ISize availableSpace, 
                             IMeasureContext measureContext)
        {
            return measureContext.MeasureElement(VisualElement, availableSpace);
        }

        protected override void OnMeasure(Int32 widthMeasureSpec,
                                          Int32 heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            //_androidView.Measure(widthMeasureSpec, heightMeasureSpec);
            var w = MeasuredWidth;
            var h = MeasuredHeight;
        }

        public void Arrange(ISize availableSpace, 
                            IRenderContext renderContext)
        {
            
        }

        protected override void OnLayout(Boolean changed, 
                                         Int32 l, 
                                         Int32 t, 
                                         Int32 r, 
                                         Int32 b)
        {
            //_androidView.Layout(l,t,r,b);
        }

        public IVisualElement DeepCopy()
        {
            throw new NotImplementedException();
        }

        public event Action<IVisualElement>? Disposed;

        public abstract void OnParentChanging(IContentContainer? newParent);
        

        public abstract Type ReplacesType { get; }
    }
}