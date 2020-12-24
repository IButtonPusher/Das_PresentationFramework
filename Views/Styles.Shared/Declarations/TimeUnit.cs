using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Das.Views.Styles.Declarations
{
    public enum TimeUnit
    {
        Invalid,
        
        [Description("Seconds")]
        S,
        
        [Description("Milliseconds")]
        Ms
    }
}
