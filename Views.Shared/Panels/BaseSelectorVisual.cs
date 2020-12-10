using System;
using Das.Views.DataBinding;

namespace Das.Views.Panels
{
    public abstract class SelectorVisual<T> : ItemsControl<T>,
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
