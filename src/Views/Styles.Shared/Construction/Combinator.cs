﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Das.Views.Styles.Construction
{
    public enum Combinator
    {
        Invalid,
        None,
        Descendant,
        Child,
        GeneralSibling,
        AdjacentSibling,
        Column
    }
}
