using System;

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
