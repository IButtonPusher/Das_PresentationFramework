using System;

namespace Das.Views.Styles.Selectors
{
    public class AllStyleSelector : SelectorBase
    {
        public static readonly AllStyleSelector Instance = new AllStyleSelector();
        
        private AllStyleSelector() : base(0)
        {
            
        }

        public sealed override Boolean Equals(IStyleSelector other)
        {
            return other is AllStyleSelector;
        }

        public override String ToString()
        {
            return "Select *";
        }

       
    }
}
