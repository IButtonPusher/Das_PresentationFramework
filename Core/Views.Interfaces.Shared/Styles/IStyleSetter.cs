using System;
using System.Collections.Generic;
using System.Text;

namespace Das.Views.Styles
{
    /// <summary>
    /// Base interface for any property setter or child style
    /// </summary>
    public interface IStyleSetter : IEquatable<IStyleSetter>
    {
        Object? Value { get; }
    }
}
