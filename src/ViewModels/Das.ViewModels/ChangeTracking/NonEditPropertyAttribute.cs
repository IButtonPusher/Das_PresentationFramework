using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Das.ViewModels.ChangeTracking
{
    /// <summary>
    ///     Denotes a property that, when changes, does not affect the
    ///     <see cref="IChangeTracking.IsChanged" /> property of the
    ///     <see cref="IChangeTracking" /> implementing view model
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NonEditPropertyAttribute : Attribute
    {
    }
}
