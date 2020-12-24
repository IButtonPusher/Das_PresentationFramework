using System;

namespace Das.Views.Styles.Selectors
{
    public class AllStyleSelector : SelectorBase,
                                    IStyleSelector
    {
        public static readonly AllStyleSelector Instance = new AllStyleSelector();
        
        private AllStyleSelector()
        {
            
        }

        public Boolean IsSelectVisual(IVisualElement visual)
        {
            return true;
        }

        public override String ToString()
        {
            return "Select *";
        }
    }
}
