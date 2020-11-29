using System;
using Das.Views.DataBinding;
using Das.Views.Mvvm;
using Das.Views.Rendering;

namespace Das.Views.Panels
{
    public abstract class SelectorVisual<T> : BasePanel<T>,
                                               ISelector<T>
    where T : IEquatable<T>
    {
        public SelectorVisual(IVisualBootstrapper visualBootstrapper) 
            : base(visualBootstrapper)
        {
        }

        public static readonly DependencyProperty<ISelector<T>, T> SelectedItemProperty =
            DependencyProperty<ISelector<T>, T>.Register(nameof(SelectedItem), default!);

        

        public T SelectedItem
        {
            get => SelectedItemProperty.GetValue(this);
            set => SelectedItemProperty.SetValue(this, value, OnSelectedItemChanging, 
                OnSelectedItemChanged);
        }

        public abstract IBindableElement<T>? SelectedVisual { get; set; }


        //public override Boolean IsRequiresMeasure
        //{
        //    get => base.IsRequiresMeasure || SelectedVisual?.IsRequiresMeasure == true;
        //    protected set => base.IsRequiresMeasure = value;
        //}

        //public override Boolean IsRequiresArrange
        //{
        //    get => base.IsRequiresArrange || SelectedVisual?.IsRequiresArrange == true;
        //    protected set => base.IsRequiresArrange = value;
        //}


        protected virtual void OnSelectedItemChanged(T oldValue,
                                                     T newValue)
        {
            InvalidateMeasure();
        }

        protected virtual Boolean OnSelectedItemChanging(T oldValue,
                                                         T newValue)
        {

            return true;
        }
    }
}
