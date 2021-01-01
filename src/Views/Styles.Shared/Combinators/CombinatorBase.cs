﻿using System;
using Das.Views.Styles.Selectors;

namespace Das.Views.Styles.Combinators
{
    public abstract class CombinatorBase : SelectorBase
    {
        public override Boolean Equals(IStyleSelector other)
        {
            return other.GetType() == GetType();
        }
    }
}