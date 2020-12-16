using System;
using Das.Views.DataBinding;

namespace Das.Views.Panels
{
    public abstract class SelectorVisual : ItemsControl,
                                              ISelector
    {
        public SelectorVisual(IVisualBootstrapper visualBootstrapper) 
            : base(visualBootstrapper)
        {
        }

        public static readonly DependencyProperty<ISelector, Object?> SelectedItemProperty =
            DependencyProperty<ISelector, Object?>.Register(nameof(SelectedItem), default);

        

        public Object? SelectedItem
        {
            get => SelectedItemProperty.GetValue(this);
            set => SelectedItemProperty.SetValue(this, value, OnSelectedItemChanging, 
                OnSelectedItemChanged);
        }

        public abstract IVisualElement? SelectedVisual { get; set; }


        protected virtual void OnSelectedItemChanged(Object? oldValue,
                                                     Object? newValue)
        {
            InvalidateMeasure();
        }

        protected virtual Boolean OnSelectedItemChanging(Object? oldValue,
                                                         Object? newValue)
        {

            return true;
        }
    }
}
