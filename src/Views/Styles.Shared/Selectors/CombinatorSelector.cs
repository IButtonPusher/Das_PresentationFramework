using System;
using Das.Views.Styles.Construction;

namespace Das.Views.Styles.Selectors
{
    public class CombinatorSelector : SelectorBase
    {
        public Combinator Combinator { get; }

        public CombinatorSelector(Combinator combinator)
            : base((Int32) combinator)
        {
            Combinator = combinator;
        }

        public override Boolean Equals(IStyleSelector other)
        {
            return other is CombinatorSelector combinator &&
                   combinator.Combinator == Combinator;
        }

        public override String ToString()
        {
            switch (Combinator)
            {
                case Combinator.Descendant:
                    return " ";
                
                case Combinator.Child:
                    return ">";

                case Combinator.GeneralSibling:
                    return "~";
                
                case Combinator.AdjacentSibling:
                    return "+";

                case Combinator.Column:
                    return "||";
            }

            return base.ToString();
        }
    }
}
