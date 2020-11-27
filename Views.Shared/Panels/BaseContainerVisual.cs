using System;
using System.ComponentModel;
using Das.Views.DataBinding;
using Das.Views.Rendering;

namespace Das.Views.Panels
{
    public abstract class BaseContainerVisual : BindableElement,
                                              IContainerVisual
    {
        public BaseContainerVisual(IVisualBootStrapper visualBootStrapper) 
            : base(visualBootStrapper)
        {
        }

        public virtual Boolean IsChanged
        {
            get => IsRequiresMeasure || IsRequiresArrange;
            //protected set => SetValue(ref _isChanged, value);
        }

        //private Boolean _isChanged;

        public virtual void AcceptChanges()
        {
            AcceptChanges(ChangeType.Measure);
            AcceptChanges(ChangeType.Arrange);
            //IsChanged = false;
        }

        //public override void InvalidateMeasure()
        //{
        //    base.InvalidateMeasure();
        //    IsChanged = true;
        //}

        protected void OnChildPropertyChanged(Object sender,
                                              PropertyChangedEventArgs e)
        {
            //switch (e.PropertyName)
            //{
            //    case nameof(IsChanged) when sender is IChangeTracking content && content.IsChanged:
            //        IsChanged = true;
            //        break;

            //    case nameof(IsRequiresMeasure) when sender is IVisualRenderer renderer &&
            //                                        renderer.IsRequiresMeasure:
            //        IsRequiresMeasure = true;
            //        IsChanged = true;
            //        break;

            //    case nameof(IsRequiresArrange) when sender is IVisualRenderer renderer &&
            //                                        renderer.IsRequiresArrange:
            //        IsRequiresArrange = true;
            //        IsChanged = true;
            //        break;
            //}
        }

    }
}
