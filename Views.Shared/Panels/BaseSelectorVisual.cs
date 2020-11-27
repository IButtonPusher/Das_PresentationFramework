using System;
using System.Collections.Generic;
using System.Text;
using Das.Views.Rendering;

namespace Das.Views.Panels
{
    public abstract class SelectorVisual : BasePanel,
                                               ISelector
    {
        public SelectorVisual(IVisualBootStrapper visualBootStrapper) 
            : base(visualBootStrapper)
        {
        }

        public static readonly DependencyProperty<ISelector, Object?> SelectedItemProperty =
            DependencyProperty<ISelector, Object?>.Register(nameof(SelectedItem), null);

        

        public Object? SelectedItem
        {
            get => SelectedItemProperty.GetValue(this);
            set => SelectedItemProperty.SetValue(this, value, OnSelectedItemChanging, 
                OnSelectedItemChanged);
        }

        public abstract IVisualElement? SelectedVisual { get; set; }


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


        protected virtual void OnSelectedItemChanged(ISelector selector,
                                                     Object? oldValue,
                                                     Object? newValue)
        {
            InvalidateMeasure();
        }

        protected virtual Boolean OnSelectedItemChanging(ISelector selector,
                                                         Object? oldValue,
                                               Object? newValue)
        {

            return true;
        }
    }
}
