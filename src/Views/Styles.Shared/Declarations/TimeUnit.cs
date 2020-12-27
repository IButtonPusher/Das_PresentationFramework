using System;
using System.ComponentModel;

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
